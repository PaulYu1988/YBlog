# YBlog 中文博客
## 介绍
基于.NET 8开发的跨平台博客CMS系统。前端使用LayUI风格，支持PC和移动端响应式布局。数据库使用微软的SQL Server。网站可以在Windows和Linux服务器上部署运行。
[Demo](https://www.y617.com/)
## IIS 部署
### 1. 安装.NET 8运行时
ASP.NET 核心运行时使你能够运行现有的 Web/服务器应用程序。在 Windows 上，我们建议安装托管捆绑包，其中包括 .NET 运行时和 IIS 支持。

Microsoft .NET8运行时官网下载地址：[.NET8运行时下载](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0) [IIS托管捆绑包下载](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-8.0.4-windows-hosting-bundle-installer)
### 2. 在IIS里创建一个网站
在IIS里新建一个网站，指定网站目录。将YBlog.zip解压缩到网站根目录。
### 3. 创建数据库并配置连接字符串
创建一个空的数据库。创建完成后，打开网站根目录下的appsettings.json文件，配置数据库连接字符串。
### 4. 初始化网站
在浏览器打开网站，会进去初始化页面。根据页面提示填写网站信息，然后点击一键安装按钮。

弹出安装成功后点 确定 按钮会进入首页。安装完成后无法再打开安装页面。

参考图文：[https://www.y617.com/articles/58](https://www.y617.com/articles/58)
### Linux 部署
参考图文：[https://www.y617.com/articles/18](https://www.y617.com/articles/18)
## appsettings 配置说明
### CompatibilityLevel
如果数据库使用的是SQL Server 2016之前的版本，请将appsettings.json中的CompatibilityLevel配置为120,否则EF Core 8会有兼容性问题导致报错。

```
{
  "ConnectionStrings": {
    "YBlog": ""
  },
  "CompatibilityLevel": 120
}
```

如果数据库是 SQL Server 2016 (13.x) 或更高版本，或者使用的是 Azure SQL，请通过以下命令检查已配置的数据库兼容性级别：

```
SELECT name, compatibility_level FROM sys.databases;
```

如果兼容级别 >= 130，无需添加CompatibilityLevel配置。如果兼容级别低于 130 (SQL Server 2016)，请考虑将兼容级别修改为较新的值。修改方法请参考 [OpenAI](https://learn.microsoft.com/zh-cn/sql/t-sql/statements/alter-database-transact-sql-compatibility-level?view=sql-server-ver16#best-practices-for-upgrading-database-compatibility-leve)。

如果不想修改兼容级别，请将appsettings.json中的CompatibilityLevel配置为120,否则EF Core 8会有兼容性问题导致报错。
