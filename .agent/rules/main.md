---
trigger: always_on
---

# 项目开发规范(Agent Rules)

作为本项目的资深全栈工程师,请严格遵循以下开发规范:

## 技术栈
- **后端框架**: .NET 8
- **数据库**: SQLite
- **UI框架**: WPF
- **语言**: 所有对话、文档、代码注释、Git Commit Message 等均严格使用**中文**

## 项目结构
- **SteamHub**: 主程序(WPF UI)
- **SteamHub.Common**: 通用代码(Helper、Enum等)
- **SteamHub.SQLite**: 数据库层(Entities、DbContext)
- **SteamHub.Source**: 数据源解析层(只负责解析Steam本地文件,不涉及数据库操作)
- 所有项目的根命名空间都是 [SteamHub](cci:2://file:///d:/Code/github/xq546247083/SteamHub/src/SteamHub.SQLite/SteamHubDbContext.cs:8:0-69:1)

## 编码规范
- **代码可读性优先**: 清晰的命名、适当的注释
- **思考先行**: 编写复杂逻辑前先阐述设计思路
- **不使用异步**: 项目中不使用 async/await

## 职责分离
- **SteamHub.Source**: 只负责解析Steam本地文件(VDF、ACF等),返回数据源模型
- **SteamHub.SQLite**: 只负责数据库操作
- **SteamHub**: 主程序负责调用解析器获取数据,然后存储到数据库,并展示UI

## 注意事项
- 添加新功能时要适配现有的代码风格