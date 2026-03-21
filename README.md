# WheelFix

[English](#english) | [中文](#中文)

鼠标滚轮修复工具 / Mouse wheel fix tool

---

## 中文

### 这是什么

WheelFix 是一款鼠标滚轮修复工具。鼠标滚轮用久了会出现"上下抖动"的问题——明明向下滚，却偶尔跳回向上。这是滚轮编码器接触不良导致的硬件问题。

WheelFix 通过软件方式自动过滤滚轮抖动，让失灵的滚轮恢复正常。

### 功能

- **智能过滤** — 自动检测并过滤滚轮抖动，不影响正常操作
- **锁定模式** — 锁定向上或向下，彻底屏蔽反方向滚动
- **自定义快捷键** — 可以按自己的习惯设置热键
- **测试页面** — 内置测试页面，方便验证效果
- **系统托盘** — 后台运行，不占桌面空间

### 使用方法

1. 下载 `WheelFix.exe` 双击运行（可能需要管理员权限）
2. 右下角托盘区会出现绿色图标
3. 正常滚动鼠标，抖动会被自动过滤
4. 如果抖动太严重，按快捷键进入锁定模式

### 默认快捷键

| 快捷键 | 功能 |
|--------|------|
| F8 | 锁定向下滚动 |
| F9 | 锁定向上滚动 |
| F10 | 恢复正常模式 |

快捷键可以在托盘菜单 → 设置 中自定义。

### 托盘菜单

右键托盘图标可以：
- 切换模式（正常 / 锁定向上 / 锁定向下）
- 打开测试页面
- 打开设置
- 退出程序

### 配置

配置文件保存在 `%AppData%\WheelFix\config.json`：

```json
{
  "LockDownKey": 119,
  "LockUpKey": 120,
  "NormalKey": 121,
  "FilterThresholdMs": 8
}
```

`FilterThresholdMs` — 抖动过滤阈值（毫秒），值越小过滤越严格，推荐 5-15

### 编译

需要 .NET 10 SDK。

```bash
dotnet publish -c Release --self-contained false -p:PublishSingleFile=true
```

生成的 exe 在 `bin/Release/net10.0-windows/win-x64/publish/` 下。

### 原理

1. 安装全局低级鼠标钩子（`WH_MOUSE_LL`）
2. 监听 `WM_MOUSEWHEEL` 事件
3. 记录最近几次滚动的方向
4. 如果两次滚动间隔 < 阈值且方向相反，判定为抖动并丢弃
5. 锁定模式下，直接屏蔽非锁定方向的所有滚动

### 系统要求

- Windows 10 / 11
- .NET 10 运行时（或使用自包含发布）

---

## English

### What is this

WheelFix is a mouse wheel fix tool. Over time, mouse wheels develop a "jitter" problem — you scroll down, but it occasionally jumps back up. This is a hardware issue caused by worn encoder contacts.

WheelFix automatically filters out wheel jitter through software, making a broken mouse wheel usable again.

### Features

- **Smart filtering** — Automatically detects and filters wheel jitter without affecting normal use
- **Lock mode** — Lock scrolling to one direction, completely blocking the opposite direction
- **Custom hotkeys** — Set your own keyboard shortcuts
- **Test page** — Built-in test page to verify the fix
- **System tray** — Runs in the background, stays out of the way

### How to use

1. Download `WheelFix.exe` and double-click to run (may require admin rights)
2. A green icon will appear in the system tray (bottom-right corner)
3. Scroll normally — jitter will be automatically filtered
4. If jitter is too severe, press a hotkey to enter lock mode

### Default hotkeys

| Hotkey | Action |
|--------|--------|
| F8 | Lock scroll down |
| F9 | Lock scroll up |
| F10 | Normal mode |

Hotkeys can be customized via tray menu → Settings.

### Tray menu

Right-click the tray icon to:
- Switch mode (Normal / Lock Up / Lock Down)
- Open test page
- Open settings
- Exit

### Configuration

Config file is saved at `%AppData%\WheelFix\config.json`:

```json
{
  "LockDownKey": 119,
  "LockUpKey": 120,
  "NormalKey": 121,
  "FilterThresholdMs": 8
}
```

`FilterThresholdMs` — Jitter filter threshold in milliseconds. Lower = stricter. Recommended: 5-15.

### Build

Requires .NET 10 SDK.

```bash
dotnet publish -c Release --self-contained false -p:PublishSingleFile=true
```

The exe will be in `bin/Release/net10.0-windows/win-x64/publish/`.

### How it works

1. Installs a global low-level mouse hook (`WH_MOUSE_LL`)
2. Listens for `WM_MOUSEWHEEL` events
3. Tracks the direction of recent scroll events
4. If two scrolls are < threshold apart and in opposite directions, it's jitter — discarded
5. In lock mode, all scrolls in the non-locked direction are blocked

### System requirements

- Windows 10 / 11
- .NET 10 runtime (or use self-contained publish)

---

## License

MIT
