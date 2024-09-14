![workflow on aspnetcoredemo branch](https://github.com/Jocoboy/dotnet-demos/actions/workflows/dotnet.yml/badge.svg?branch=aspnetcoredemo)

[:rewind:Back to Home](https://github.com/Jocoboy/dotnet-demos/tree/master)

## ASP.NET Core Web API Demo

A demo for Monolithic Architecture based on ASP.NET Core, which provides web API using HTTP methods (GET and POST only).

### Architecture

The Repository-Service pattern breaks up the business layer of the app into two distinct layers. 
- The lower layer is the Repositories. These classes handle getting data into and out of our data store, with the important caveat that each Repository only works against a single Model class. 
- The upper layer is the Services. These classes will have Repositories injected to them and can query multiple Repository classes and combine their data to form new, more complex business objects. Further, they introduce a layer of abstraction between the web application and the Repositories so that they can change more independently.

The Repository-Service pattern relies on dependency injection to work properly. Classes at each layer of the architecture will have classes they need from the "lower" layers injected into them.

![aspnetcore_layer_deps](https://jocoboy.github.io/Hexo-Blog/2024/08/13/abp-and-ddd/aspnetcore_layer_deps.png)

### Environment

- [C# 12](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-12)
- [.NET 8.0](https://learn.microsoft.com/zh-cn/dotnet/core/whats-new/dotnet-8/overview) 
- [EF Core 8.0](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-8.0/whatsnew)
- [ASP.NET Core 8.0](https://learn.microsoft.com/zh-cn/aspnet/core/release-notes/aspnetcore-8.0?view=aspnetcore-8.0)
- MySQL 8.0.19

### Features

#### Overview 

- Use HTTP GET and POST methods only to get rid of network or client proxy limits.
- Use MySQL database without foreign key constraint to improve database concurrency and prepare for tables migration in the future.

#### Details

- :white_check_mark: Use JSON Web Token (JWT) for Authentication.
- :white_check_mark: Add ServiceExtension to realize DI using reflection without Autofac.
- :white_check_mark: Add RouteConvention to configure global routing prefix.
- :white_check_mark: Add ModelStateValidateExtension to intercept error and catch invalid fields of models directly before entering the method.
- :white_check_mark: Add CustomExceptionFilter to catch expected exceptions.
- :white_check_mark: Add RemarkAttribute to bind error code with corresponding message.

