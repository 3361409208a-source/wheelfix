<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,24,12&height=180&section=header&text=WheelFix&fontSize=60&fontColor=fff&animation=fadeIn" />

### 🖱️ 鼠标滚轮修复工具 / Mouse Wheel Jitter Fix

**让坏掉的滚轮重获新生 / Bring your broken wheel back to life**

[![GitHub stars](https://img.shields.io/github/stars/3361409208a-source/wheelfix?style=for-the-badge&logo=github&color=green)](https://github.com/3361409208a-source/wheelfix/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/3361409208a-source/wheelfix?style=for-the-badge&logo=github&color=blue)](https://github.com/3361409208a-source/wheelfix/forks)
[![GitHub release](https://img.shields.io/github/v/release/3361409208a-source/wheelfix?style=for-the-badge&color=orange)](https://github.com/3361409208a-source/wheelfix/releases)
[![License](https://img.shields.io/badge/license-MIT-green?style=for-the-badge&logo=)](LICENSE)
[![Platform](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)](#)

---

[🇺🇸 English](#-english)  ·  [🇨🇳 中文](#-中文)

</div>

---

<!-- ============================================================ -->
<!--                        中文部分                               -->
<!-- ============================================================ -->

<a id="-中文"></a>

<div align="center">

# 🇨🇳 中文

</div>

## 😫 你的鼠标是不是也这样？

```
↓↓↓↓↓↓↓↓↑↓↓↓↓↓↓↓↓↑↓↓↓↓↓↓↓↓↑↓↓↓
  向下滚              跳回去了！
```

> 滚轮用久了，滚动页面会**上下乱跳**
> 本来向下看，突然跳回顶部
> 写文档、看网页、刷代码，**烦到爆**

**根因：** 滚轮编码器磨损，硬件问题，换鼠标要花钱 💸
**妙招：** WheelFix 用软件过滤抖动，**不花一分钱**修好它 ✨

## 🚀 一键修复

<div align="center">

| ![箭头指示](https://img.shields.io/badge/⬇️-切换锁定向下-2ea44f?style=for-the-badge) | ![箭头指示](https://img.shields.io/badge/⬆️-切换锁定向上-2ea44f?style=for-the-badge) | ![箭头指示](https://img.shields.io/badge/🔄-恢复正常模式-8b5cf6?style=for-the-badge) |
|:---:|:---:|:---:|
| 按 `F8` | 按 `F9` | 按 `F10` |

</div>

## ✨ 功能亮点

<div align="center">

| 🔧 智能过滤 | 🎯 锁定模式 | ⚙️ 自定义设置 |
|:---:|:---:|:---:|
| 自动检测抖动，<8ms 方向反转直接过滤 | 锁定一个方向，反方向彻底屏蔽 | 快捷键、阈值全都能改 |
| 不影响正常滚动 | 三种图标颜色区分当前状态 | 配置自动保存，重启不丢失 |

| 🧪 测试页面 | 📌 系统托盘 | 📦 零依赖 |
|:---:|:---:|:---:|
| 500 行滚动页，当场验证效果 | 后台运行，绿色/红色图标一目了然 | 下载即用，不装 .NET |
| 浏览器打开，F8/F9 测试 | 右键菜单一键切换 | 单文件 50MB，任何电脑都能跑 |

</div>

## 📥 下载安装

<div align="center">

### 👇 点这里 👇

[![Download](https://img.shields.io/badge/⬇️_下载_WheelFix_最新版-000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/3361409208a-source/wheelfix/releases/latest)

**下载 → 双击 → 搞定。** 三步结束，就这么简单。

</div>

## 🎮 操作指南

### 快捷键

| 快捷键 | 功能 | 图标 |
|:---:|:---:|:---:|
| `F8` | 🔽 锁定向下 | 🔴 红色圆 + ⬇ |
| `F9` | 🔼 锁定向上 | 🔵 蓝色圆 + ⬆ |
| `F10` | 🔄 正常模式 | 🟢 绿色圆 + ↔ |

> 💡 快捷键可在托盘菜单 → **设置** 中自定义，点击输入框按你想用的键就行

### 托盘菜单

右下角绿色小圆点 → 右键：

```
┌──────────────────┐
│  正常过滤模式    │  ← 解除锁定
│  锁定向上        │  ← 只能往上滚
│  锁定向下        │  ← 只能往下滚
│──────────────────│
│  测试滚动        │  ← 打开测试页
│  设置            │  ← 自定义快捷键
│──────────────────│
│  退出            │
└──────────────────┘
```

### 状态指示

| 图标样式 | 含义 |
|:---:|:---:|
| 🟢 绿色圆 + ↔ 双箭头 | 正常过滤模式 |
| 🔵 蓝色圆 + ⬆ 上箭头 | 锁定向上滚动 |
| 🔴 红色圆 + ⬇ 下箭头 | 锁定向下滚动 |

## 📐 工作原理

```
    🖱️ 鼠标滚轮
         │
         ▼
  ┌──────────────────┐
  │  全局鼠标钩子     │  ← WH_MOUSE_LL
  │  捕获滚轮事件     │     拦截所有滚动
  └────────┬─────────┘
           │
           ▼
  ┌──────────────────┐
  │  方向分析器       │  ← 记录最近 N 次方向
  │  抖动检测器       │  ← <8ms 反转 = 抖动
  └────────┬─────────┘
           │
      ┌────┴────┐
      ▼         ▼
   ✅ 放行    ❌ 丢弃
  正常滚动   抖动事件
  页面滚动    不响应
```

## ⚙️ 配置文件

位置：`%AppData%\WheelFix\config.json`

```json
{
  "LockDownKey": 119,       // 锁定向下热键 (F8)
  "LockUpKey": 120,         // 锁定向上热键 (F9)
  "NormalKey": 121,         // 正常模式热键 (F10)
  "FilterThresholdMs": 8    // 抖动阈值(毫秒)，越小越严格
}
```

## 🔧 从源码编译

```bash
git clone https://github.com/3361409208a-source/wheelfix.git
cd wheelfix
dotnet publish -c Release
```

需要 [.NET 10 SDK](https://dotnet.microsoft.com/download)。输出文件在 `bin/Release/.../publish/` 下。

## 💻 系统要求

<div align="center">

| 系统 | Windows 10 / 11（64 位） |
|:---:|:---:|
| 依赖 | **无**，下载即用 |
| 权限 | 管理员（安装钩子需要） |
| 大小 | ~50MB |

</div>

---

<!-- ============================================================ -->
<!--                        English                                -->
<!-- ============================================================ -->

<a id="-english"></a>

<div align="center">

# 🇺🇸 English

</div>

## 😫 Is your mouse wheel doing this?

> You scroll down, the page jumps back up.
> Jittering up and down, completely unusable.
> Writing, reading, coding — **infuriating.**

**Root cause:** Worn wheel encoder contacts. Hardware failure.
**Solution:** WheelFix filters jitter in software. **Free fix.** ✨

## 🚀 One-Click Fix

<div align="center">

| `F8` | `F9` | `F10` |
|:---:|:---:|:---:|
| 🔽 Lock Down | 🔼 Lock Up | 🔄 Normal |

</div>

## ✨ Features

| Feature | Description |
|---|---|
| 🔧 **Smart Filter** | Auto-detects jitter, filters <8ms direction reversals |
| 🎯 **Lock Mode** | Lock to one direction, block the opposite completely |
| ⌨️ **Custom Hotkeys** | Set any key combination you want |
| 🧪 **Test Page** | Built-in 500-line scroll page to verify the fix |
| 📌 **System Tray** | 3 distinct icons (green/blue/red), right-click menu |
| 📦 **Zero Dependencies** | Single 50MB exe, works on any Windows 10/11 PC |

## 📥 Download

[![Download](https://img.shields.io/badge/⬇️_Download_WheelFix-000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/3361409208a-source/wheelfix/releases/latest)

**Download → Double-click → Done.** That's it.

## 💻 Requirements

| OS | Windows 10 / 11 (64-bit) |
|---|---|
| Dependencies | **None** |
| Size | ~50MB |
| Admin | Yes (for hook installation) |

---

## 📄 License

<div align="center">

[MIT](LICENSE)

---

<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,24,12&height=100&section=footer" />

**如果这个项目帮到了你，点个 ⭐ Star 支持一下！**
**If this project helped you, give it a ⭐ Star!**

</div>
