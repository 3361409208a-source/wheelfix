<div align="center">

# 🖱️ WheelFix

**鼠标滚轮修复工具 / Mouse wheel jitter fix**

[![GitHub stars](https://img.shields.io/github/stars/3361409208a-source/wheelfix?style=social)](https://github.com/3361409208a-source/wheelfix)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6.svg)](#)
[![Release](https://img.shields.io/github/v/release/3361409208a-source/wheelfix)](https://github.com/3361409208a-source/wheelfix/releases)

[English](#-english)  |  [中文](#-中文)

---

</div>

## 🇨🇳 中文

### 你的鼠标滚轮是不是也这样？

向下滚一下，页面却跳回去了。反复上下抖动，根本没法正常用。

**原因：** 滚轮编码器接触不良，是硬件问题，不是软件问题。
**方案：** WheelFix 用软件方式过滤抖动，让坏掉的滚轮重新好用。

### ✨ 功能

<table>
<tr>
<td width="50%">

🔧 **智能过滤**
自动检测并过滤滚轮抖动
不影响正常操作

🎯 **锁定模式**
锁定向上或向下
彻底屏蔽反方向滚动

</td>
<td width="50%">

⌨️ **自定义快捷键**
按自己的习惯设置热键

🧪 **测试页面**
内置测试页面，验证效果

📌 **系统托盘**
后台运行，不占桌面

</td>
</tr>
</table>

### 📥 下载

**[前往 Releases 页面下载](https://github.com/3361409208a-source/wheelfix/releases)**

下载 `WheelFix.exe`，双击即可运行。无需安装 .NET，无需任何依赖。

### 🎮 使用

| 快捷键 | 功能 |
|:------:|------|
| `F8` | 🔽 锁定向下滚动 |
| `F9` | 🔼 锁定向上滚动 |
| `F10` | 🔄 恢复正常模式 |

> 快捷键可在托盘菜单 → 设置 中自定义

**托盘菜单**（右键图标）：
- 切换模式（正常 / 锁定向上 / 锁定向下）
- 测试滚动
- 设置
- 退出

### ⚙️ 配置

配置保存在 `%AppData%\WheelFix\config.json`：

```json
{
  "LockDownKey": 119,
  "LockUpKey": 120,
  "NormalKey": 121,
  "FilterThresholdMs": 8
}
```

`FilterThresholdMs` — 抖动过滤阈值（毫秒），值越小过滤越严格，推荐 `5-15`

### 🔧 编译

```bash
git clone https://github.com/3361409208a-source/wheelfix.git
cd wheelfix
dotnet publish -c Release
```

需要 [.NET 10 SDK](https://dotnet.microsoft.com/download)。

### 📐 原理

```
鼠标滚轮事件
    │
    ▼
┌─────────────────┐
│  全局鼠标钩子    │  WH_MOUSE_LL
│  监听滚轮事件    │  WM_MOUSEWHEEL
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  方向判断        │  记录最近滚动方向
│  抖动检测        │  <8ms 方向反转 = 抖动
└────────┬────────┘
         │
    ┌────┴────┐
    ▼         ▼
  放行       丢弃
 正常滚动   抖动事件
```

### 💻 系统要求

| 项目 | 要求 |
|------|------|
| 系统 | Windows 10 / 11（64位） |
| 依赖 | 无，下载即用 |
| 权限 | 管理员（安装钩子需要） |
| 大小 | ~50MB |

---

<div align="center">

## 🇺🇸 English

</div>

### Is your mouse wheel doing this?

You scroll down, but the page jumps back up. Jittering up and down, unusable.

**Cause:** The wheel encoder has worn-out contacts. It's a hardware issue.
**Solution:** WheelFix filters out the jitter in software, making your broken wheel work again.

### ✨ Features

<table>
<tr>
<td width="50%">

🔧 **Smart Filtering**
Automatically detects and filters wheel jitter
Doesn't affect normal scrolling

🎯 **Lock Mode**
Lock scrolling to one direction
Completely blocks the opposite direction

</td>
<td width="50%">

⌨️ **Custom Hotkeys**
Set your own keyboard shortcuts

🧪 **Test Page**
Built-in test page to verify the fix

📌 **System Tray**
Runs silently in the background

</td>
</tr>
</table>

### 📥 Download

**[Go to Releases page](https://github.com/3361409208a-source/wheelfix/releases)**

Download `WheelFix.exe` and double-click to run. No .NET installation required.

### 🎮 Usage

| Hotkey | Action |
|:------:|--------|
| `F8` | 🔽 Lock scroll down |
| `F9` | 🔼 Lock scroll up |
| `F10` | 🔄 Normal mode |

> Customize hotkeys in tray menu → Settings

### 💻 Requirements

| | |
|---|---|
| OS | Windows 10 / 11 (64-bit) |
| Dependencies | None |
| Size | ~50MB |

---

## 📄 License

[MIT](LICENSE)
