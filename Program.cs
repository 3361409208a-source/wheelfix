using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.Json;

namespace WheelFix;

// ==================== 配置 ====================
class Config
{
    public Keys LockDownKey { get; set; } = Keys.F8;
    public Keys LockUpKey { get; set; } = Keys.F9;
    public Keys NormalKey { get; set; } = Keys.F10;
    public int FilterThresholdMs { get; set; } = 8;

    static string ConfigPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "WheelFix", "config.json");

    static readonly JsonSerializerOptions JsonOpt = new() { WriteIndented = true };

    public void Save()
    {
        var dir = Path.GetDirectoryName(ConfigPath)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var obj = new { LockDownKey = (int)LockDownKey, LockUpKey = (int)LockUpKey,
            NormalKey = (int)NormalKey, FilterThresholdMs };
        File.WriteAllText(ConfigPath, JsonSerializer.Serialize(obj, JsonOpt));
    }

    public static Config Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                return new Config
                {
                    LockDownKey = (Keys)(root.GetProperty("LockDownKey").GetInt32()),
                    LockUpKey = (Keys)(root.GetProperty("LockUpKey").GetInt32()),
                    NormalKey = (Keys)(root.GetProperty("NormalKey").GetInt32()),
                    FilterThresholdMs = root.GetProperty("FilterThresholdMs").GetInt32()
                };
            }
        }
        catch { }
        return new Config();
    }
}

// ==================== 主程序 ====================
static class Program
{
    delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll")]
    static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll")]
    static extern IntPtr GetModuleHandle(string? lpModuleName);
    [DllImport("user32.dll")]
    static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    const int WH_MOUSE_LL = 14;
    const int WM_MOUSEWHEEL = 0x020A;
    const uint MOD_NONE = 0x0000;

    [StructLayout(LayoutKind.Sequential)]
    struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct POINT { public int X, Y; }

    static IntPtr _hookId = IntPtr.Zero;
    static LowLevelMouseProc? _hookProc;
    static int _dominantDir = 0;
    static int _sameDirCount = 0;
    static long _lastTick = 0;

    static volatile bool _lockMode = false;
    static volatile int _lockDirection = -1;
    static NotifyIcon? _trayIcon;
    static MessageWindow? _msgWindow;
    static Config _config = Config.Load();

    public static Config Config => _config;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        _msgWindow = new MessageWindow();
        _msgWindow.HotkeyReceived += OnHotkey;

        ApplyHotkeys();

        _hookProc = HookCallback;
        using var proc = Process.GetCurrentProcess();
        using var mod = proc.MainModule!;
        _hookId = SetWindowsHookEx(WH_MOUSE_LL, _hookProc, GetModuleHandle(mod.ModuleName), 0);

        if (_hookId == IntPtr.Zero)
        {
            MessageBox.Show("安装钩子失败，请以管理员身份运行", "WheelFix",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _trayIcon = new NotifyIcon
        {
            Icon = MakeIcon(Color.LimeGreen),
            Text = "WheelFix [正常]",
            Visible = true,
            ContextMenuStrip = BuildMenu()
        };

        Application.Run();

        UnhookWindowsHookEx(_hookId);
        ClearHotkeys();
        _trayIcon.Dispose();
    }

    static void ApplyHotkeys()
    {
        ClearHotkeys();
        RegisterHotKey(_msgWindow!.Handle, 1, MOD_NONE, (uint)_config.LockDownKey);
        RegisterHotKey(_msgWindow.Handle, 2, MOD_NONE, (uint)_config.LockUpKey);
        RegisterHotKey(_msgWindow.Handle, 3, MOD_NONE, (uint)_config.NormalKey);
    }

    static void ClearHotkeys()
    {
        if (_msgWindow == null) return;
        for (int i = 1; i <= 3; i++)
            UnregisterHotKey(_msgWindow.Handle, i);
    }

    public static void ReapplyHotkeys() => ApplyHotkeys();

    static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (int)wParam == WM_MOUSEWHEEL)
        {
            var info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            short delta = (short)((info.mouseData >> 16) & 0xFFFF);
            int dir = delta > 0 ? 1 : -1;
            long now = Stopwatch.GetTimestamp();
            double elapsedMs = (now - _lastTick) * 1000.0 / Stopwatch.Frequency;
            _lastTick = now;

            if (_lockMode)
            {
                return dir == _lockDirection
                    ? CallNextHookEx(_hookId, nCode, wParam, lParam)
                    : (IntPtr)1;
            }

            if (_dominantDir == 0)
            {
                _dominantDir = dir;
                _sameDirCount = 1;
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            if (dir == _dominantDir)
            {
                _sameDirCount = Math.Min(_sameDirCount + 1, 10);
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            if (elapsedMs < _config.FilterThresholdMs)
                return (IntPtr)1;

            _sameDirCount--;
            if (_sameDirCount <= 0)
            {
                _dominantDir = dir;
                _sameDirCount = 1;
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    static void OnHotkey(int id)
    {
        switch (id)
        {
            case 1: SetLock(-1); break;
            case 2: SetLock(1); break;
            case 3: SetNormal(); break;
        }
    }

    static void SetLock(int dir)
    {
        _lockMode = true; _lockDirection = dir;
        _dominantDir = 0; _sameDirCount = 0;
        UpdateTray();
    }

    static void SetNormal()
    {
        _lockMode = false;
        _dominantDir = 0; _sameDirCount = 0;
        UpdateTray();
    }

    static void UpdateTray()
    {
        if (_trayIcon == null) return;
        _trayIcon.Icon = MakeIcon(_lockMode ? Color.OrangeRed : Color.LimeGreen);
        if (_lockMode)
        {
            var dir = _lockDirection > 0 ? "上" : "下";
            _trayIcon.Text = $"WheelFix [锁定{dir}]";
            _trayIcon.ShowBalloonTip(1000, "WheelFix", $"已锁定向{dir}滚动", ToolTipIcon.Info);
        }
        else
        {
            _trayIcon.Text = "WheelFix [正常]";
            _trayIcon.ShowBalloonTip(1000, "WheelFix", "恢复正常过滤模式", ToolTipIcon.Info);
        }
    }

    static ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("正常过滤模式", null, (_, _) => SetNormal());
        menu.Items.Add("锁定向上", null, (_, _) => SetLock(1));
        menu.Items.Add("锁定向下", null, (_, _) => SetLock(-1));
        menu.Items.Add("-");
        menu.Items.Add("测试滚动", null, (_, _) => TestPage.Open());
        menu.Items.Add("设置", null, (_, _) => SettingsForm.Open());
        menu.Items.Add("-");
        menu.Items.Add("退出", null, (_, _) => Application.Exit());
        return menu;
    }

    static Icon MakeIcon(Color color)
    {
        using var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);
        using var brush = new SolidBrush(color);
        g.FillEllipse(brush, 2, 2, 12, 12);
        using var pen = new Pen(Color.White, 1.5f);
        g.DrawLine(pen, 8, 3, 8, 13);
        g.DrawLine(pen, 5, 5, 11, 11);
        return Icon.FromHandle(bmp.GetHicon());
    }
}

// ==================== 消息窗口 ====================
class MessageWindow : Form
{
    public event Action<int>? HotkeyReceived;
    public MessageWindow() { ShowInTaskbar = false; WindowState = FormWindowState.Minimized; }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312) HotkeyReceived?.Invoke(m.WParam.ToInt32());
        base.WndProc(ref m);
    }
}

// ==================== 设置窗口 ====================
class SettingsForm : Form
{
    private static SettingsForm? _instance;
    private KeyBindBox _lockDownBox = null!, _lockUpBox = null!, _normalBox = null!;
    private NumericUpDown _thresholdSpinner = null!;

    public static void Open()
    {
        if (_instance == null || _instance.IsDisposed)
            _instance = new SettingsForm();
        _instance.Show();
        _instance.Activate();
    }

    SettingsForm()
    {
        Text = "WheelFix 设置";
        Size = new Size(400, 260);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var cfg = Program.Config;
        int y = 15;

        AddLabel("锁定向下:", 15, y);
        _lockDownBox = new KeyBindBox { Text = cfg.LockDownKey.ToString(), Tag = cfg.LockDownKey, Location = new Point(150, y - 2), Size = new Size(120, 24) };
        Controls.Add(_lockDownBox);
        y += 35;

        AddLabel("锁定向上:", 15, y);
        _lockUpBox = new KeyBindBox { Text = cfg.LockUpKey.ToString(), Tag = cfg.LockUpKey, Location = new Point(150, y - 2), Size = new Size(120, 24) };
        Controls.Add(_lockUpBox);
        y += 35;

        AddLabel("正常模式:", 15, y);
        _normalBox = new KeyBindBox { Text = cfg.NormalKey.ToString(), Tag = cfg.NormalKey, Location = new Point(150, y - 2), Size = new Size(120, 24) };
        Controls.Add(_normalBox);
        y += 35;

        AddLabel("过滤阈值(ms):", 15, y);
        _thresholdSpinner = new NumericUpDown { Minimum = 1, Maximum = 50, Value = cfg.FilterThresholdMs, Location = new Point(150, y - 2), Size = new Size(80, 24) };
        Controls.Add(_thresholdSpinner);
        y += 45;

        var saveBtn = new Button { Text = "保存", Location = new Point(150, y), Size = new Size(80, 30) };
        saveBtn.Click += (_, _) =>
        {
            cfg.LockDownKey = (Keys)(_lockDownBox.Tag ?? Keys.F8);
            cfg.LockUpKey = (Keys)(_lockUpBox.Tag ?? Keys.F9);
            cfg.NormalKey = (Keys)(_normalBox.Tag ?? Keys.F10);
            cfg.FilterThresholdMs = (int)_thresholdSpinner.Value;
            cfg.Save();
            Program.ReapplyHotkeys();
            MessageBox.Show("已保存，快捷键已生效", "WheelFix", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        Controls.Add(saveBtn);

        var cancelBtn = new Button { Text = "取消", Location = new Point(250, y), Size = new Size(80, 30) };
        cancelBtn.Click += (_, _) => Close();
        Controls.Add(cancelBtn);
    }

    void AddLabel(string text, int x, int y)
    {
        Controls.Add(new Label { Text = text, AutoSize = true, Location = new Point(x, y + 2) });
    }
}

// ==================== 快捷键输入框 ====================
class KeyBindBox : TextBox
{
    public KeyBindBox()
    {
        ReadOnly = true;
        BackColor = Color.White;
        Cursor = Cursors.Hand;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.SuppressKeyPress = true;
        Tag = e.KeyCode;
        Text = e.KeyCode.ToString();
        base.OnKeyDown(e);
    }
}

// ==================== 测试页面 ====================
static class TestPage
{
    public static void Open()
    {
        var path = Path.Combine(Path.GetTempPath(), "wheelfix_test.html");
        var html = """
        <!DOCTYPE html>
        <html lang="zh">
        <head>
        <meta charset="UTF-8">
        <title>WheelFix 滚轮测试</title>
        <style>
            body { background: #1a1a2e; color: #eee; font-family: 'Consolas', monospace; margin: 0; }
            .container { max-width: 600px; margin: 0 auto; padding: 20px; }
            h1 { color: #e94560; text-align: center; }
            .line { padding: 8px 20px; border-bottom: 1px solid #333; font-size: 14px; }
            .line.mark { color: #e94560; font-size: 18px; font-weight: bold; padding: 15px 20px; }
            .hint { text-align: center; color: #888; margin: 20px 0; font-size: 13px; }
        </style>
        </head>
        <body>
        <div class="container">
            <h1>WheelFix 滚轮测试页面</h1>
            <p class="hint">按 F8 锁定向下 / F9 锁定向上 / F10 正常模式<br>然后在这个页面滚动测试</p>
            <div id="list"></div>
        </div>
        <script>
            const list = document.getElementById('list');
            for (let i = 1; i <= 500; i++) {
                const div = document.createElement('div');
                div.className = i % 10 === 0 ? 'line mark' : 'line';
                div.textContent = i % 10 === 0 ? `▼ 第 ${i} 行 ▼` : `第 ${i} 行`;
                list.appendChild(div);
            }
        </script>
        </body>
        </html>
        """;
        File.WriteAllText(path, html);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
}

