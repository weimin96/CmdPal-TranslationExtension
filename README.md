# TranslationExtension (PowerToys翻译扩展)

这是一个基于 PowerToys 的翻译扩展工具，旨在为用户提供便捷的文本翻译功能。目前支持百度翻译 API。

## ✨ 主要功能

*   **多源支持**：目前内置支持 **百度翻译**，架构上支持扩展更多翻译源（如 Google 翻译等）。
*   **配置管理**：提供统一的设置页面，方便用户配置 API 密钥和其他选项。
*   **自动检测**：支持中英互译（自动检测源语言）。
*   **Windows 原生体验**：基于 .NET 9 和 Windows App SDK 构建，提供流畅的 Windows 原生应用体验。

## 🛠️ 环境要求

*   Windows 10 version 19041.0 或更高版本
*   [Visual Studio 2022](https://visualstudio.microsoft.com/)
*   .NET 9.0 SDK

## 🚀 快速开始

1.  **克隆项目**
    ```powershell
    git clone https://github.com/yourusername/TranslationExtension.git
    cd TranslationExtension
    ```

2.  **打开项目**
    使用 Visual Studio 打开 `TranslationExtension.sln` 解决方案文件。

3.  **构建与运行**
    *   确保已安装所需的 Windows App SDK workload。
    *   将 `TranslationExtension` 设为启动项目。
    *   按 `F5` 运行或构建项目。

## ⚙️ 配置说明

在使用翻译功能之前，您需要配置相应的翻译服务提供商凭证。

### 百度翻译配置

1.  访问 [百度翻译开放平台](http://api.fanyi.baidu.com/) 并注册账号。
2.  开通通用翻译 API 服务，获取您的 `App ID` 和 `密钥 (Secret Key)`。
3.  运行本扩展程序，进入 **设置 (Settings)** 页面。
4.  在提供商列表中选择 **Baidu**。
5.  在对应的输入框中填入您的 App ID 和 Secret Key。
6.  保存设置即可开始使用。

## 📁 目录结构

*   **TranslationExtension/**: 核心项目代码
    *   **Pages/**: UI 页面 (TranslationExtensionPage, SettingsPage 等)
    *   **Properties/**: 项目属性
    *   **Assets/**: 静态资源文件
    *   **TranslationService.cs**: 翻译服务核心逻辑
    *   **TranslationSettings.cs**: 设置模型定义
    *   **SettingsManager.cs**: 设置存储与管理

## 🤝 贡献

欢迎提交 Issues 和 Pull Requests 来改进这个项目！

## 📄 许可证

本项目采用 MIT 许可证。
