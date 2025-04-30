# RemoveMarkdown.NET

[![NuGet](https://img.shields.io/nuget/v/RemoveMarkdown.NET.svg)](https://www.nuget.org/packages/RemoveMarkdown.NET/)

[English](#introduction) | [中文](#简介)

## Introduction

**RemoveMarkdown.NET** is a .NET library that converts Markdown-formatted text to plain text.

## Use Cases

The primary use case is generating plain text previews or summaries from Markdown content - for example, creating article lists, search result previews, or extracting content for indexing and search purposes.

## Installation

Install via NuGet:

```
dotnet add package RemoveMarkdown.NET
```

Or via the Package Manager Console in Visual Studio:

```
Install-Package RemoveMarkdown.NET
```

## Usage

```csharp
using RemoveMarkdown;

string markdown = "# This is a heading\n\nThis is a paragraph with a [link](http://www.example.com/).";
string plainText = MarkdownRemover.Remove(markdown); // plainText becomes "This is a heading\n\nThis is a paragraph with a link."
```

Options:

```csharp
using RemoveMarkdown;

string plainText = MarkdownRemover.Remove(markdown, new RemoveMarkdownOptions
{
    StripListLeaders = true,      // Remove list leaders (default: true)
    ListUnicodeChar = "",         // Character to replace list leaders with (default: "")
    SupportGitHubFavored = true,  // Support GitHub-Flavored Markdown (default: true)
    UseImgAltText = true,         // Replace images with alt text (default: true)
    RemoveAbbr = false,           // Remove abbreviations (default: false)
    ReplaceLinksWithURL = false,  // Replace links with URLs (default: false)
    HtmlTagsToSkip = new[] { "sub", "sup" },  // HTML tags to preserve (default: empty array)
    ThrowError = false            // Throw exceptions on errors (default: false)
});
```

Setting `StripListLeaders` to false will preserve list markers (`*, -, +, (number).`).

## Features

- Removing headers (ATX and Setext style)
- Removing emphasis (* and _)
- Removing links and images
- Removing code blocks and inline code
- Removing list markers
- Removing horizontal rules
- Removing blockquotes
- Removing HTML tags (with option to preserve specific tags)
- Support for GitHub-Flavored Markdown

## Acknowledgements

Based on the [remove-markdown](https://github.com/zuchka/remove-markdown) Node.js module.

## License

MIT

---

## 简介

**RemoveMarkdown.NET** 是将 Markdown 格式文本转为纯文本的 .NET 库

## 应用场景

主要应用场景是从 Markdown 内容生成纯文本预览或摘要，例如创建文章列表、搜索结果预览，或提取内容用于索引和搜索。

## 安装方法

通过 NuGet 安装：

```
dotnet add package RemoveMarkdown.NET
```

或在 Visual Studio 的包管理器控制台中：

```
Install-Package RemoveMarkdown.NET
```

## 使用方法

```csharp
using RemoveMarkdown;

string markdown = "# 这是标题\n\n这是一个包含 [链接](http://www.example.com/) 的段落。";
string plainText = MarkdownRemover.Remove(markdown); // plainText 变为 "这是标题\n\n这是一个包含 链接 的段落。"
```

配置项：

```csharp
using RemoveMarkdown;

string plainText = MarkdownRemover.Remove(markdown, new RemoveMarkdownOptions
{
    StripListLeaders = true,      // 是否移除列表前导符号 (默认: true)
    ListUnicodeChar = "",         // 替代移除的列表前导符号的字符 (默认: "")
    SupportGitHubFavored = true,  // 是否支持 GitHub 风格的 Markdown (默认: true)
    UseImgAltText = true,         // 是否用图片的 alt 文本替换图片 (默认: true)
    RemoveAbbr = false,           // 是否移除缩略语 (默认: false)
    ReplaceLinksWithURL = false,  // 是否用 URL 替换链接 (默认: false)
    HtmlTagsToSkip = new[] { "sub", "sup" },  // 不移除的 HTML 标签 (默认: 空数组)
    ThrowError = false            // 是否在出错时抛出异常 (默认: false)
});
```

将 `StripListLeaders` 设置为 false 将保留列表标记符号（`*, -, +, (数字).`）。

## 功能特性

- 移除标题（ATX 和 Setext 风格）
- 移除强调（* 和 _）
- 移除链接和图片
- 移除代码块和内联代码
- 移除列表标记
- 移除水平分隔线
- 移除引用块
- 移除 HTML 标签（可选择保留特定标签）
- 支持 GitHub 风格的 Markdown

## 致谢

基于 [remove-markdown](https://github.com/zuchka/remove-markdown) Node.js 模块转写

## 许可证

MIT