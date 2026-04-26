# README 顶部横幅交付发现

## 项目定位
项目名为 TranslationExtension，是面向 PowerToys Command Palette 的 Windows 翻译扩展。

## README 现状
README 顶部已有居中标题、中文简介、构建状态、许可证、.NET、Windows、星标、议题和派生徽章。

## 功能关键词
项目强调极速响应、PowerToys 集成、中英文自动检测、多翻译服务商配置、WinUI 3 和 Windows 设计风格。

## 当前工作区
README 已有两处未提交文字调整，分别是 PowerToys 集成描述和百度翻译提示文案。后续修改需要保留这些改动。

## 横幅设计决策
横幅采用 SVG 文件，原因是项目名、中文说明、技术标签和翻译示例必须准确可控。位图生成工具在文字渲染上存在不稳定风险，不适合作为包含项目文案的 README 顶部资产。

## 横幅落盘路径
横幅保存为 `docs/readme-banner.svg`，README 使用 `./docs/readme-banner.svg` 相对路径引用，便于 GitHub 渲染。
