# SimpleMailApplication
### `Simple Mail Application combines ASP .Net Core Web API with Blazor WebAssembly App`
## Short description
API uses `MailKit` NuGet package to provide logic for sending emails to specific mail host. 
WebAssembly uses `Blazor Radzen` and `Blazor Toast Notification` to provide user friendly interface.
[`Ethereal Email`](https://ethereal.email/) service is used as email host.

Sollution schema in `MS Visual Studio` (the most important elements are marked):

![0 Schema](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/06bf734a-6c09-4263-bc1a-1be954fb65b7)

#### I Creating projects, combining them and installing NuGet packages
1) [`SimpleMail.Lib`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/a60ae285-2c9b-4c3f-a9f1-284eabdb9cfa) is a class library which contains `EmailDto.cs` class used as data transfer object in other parts of sollution.
2) [`SimpleMailApp.Api`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/64adc087-4437-4375-91f2-107d4be45a5f) is `ASP .Net Core Web API` part of sollution. `MailKit` NuGet package was installed in this part.
3) [`SimpleMailApp.WebAssembly`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/38dfb658-8bb3-4fa2-9721-5fb0ce899fb2) is `Blazor WebAssembly App` part of sollution. `Radzen.Blazor` and `Blazor.Toast` NuGet packages was installed in this part.
4) [Combining](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1c294229-df6e-4dab-9780-b343f5be8afd) Api and WebAssembly parts of sollution.
5) Project reference to `SimpleMail.Lib` added in [`SimpleMailApp.Api`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/eb7923c3-7728-4fbe-8aa5-d7c6a8aeab9b) and in [`SimpleMailApp.WebAssembly`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/5a9d7e44-b28d-4e7f-aec7-c0484096c3f3).
#### II `SimpleMailApp.Api` functionalities
1) [`appsettings.json`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/e93272c4-517c-4538-96c1-603d604f4e1d) file configured to use specific mail host from `Ethereal Email` service: `EmailHost`, `EmailUsername` and `EmailPassword` added.
2) [`IEmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/cb4e9f76-b519-4cc2-bb20-9e6e0b6881b3) interface created which takes `EmailDto.cs` from `SimpleMail.Lib` as parameter.
3) [`EmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/4b19c788-e640-49ca-9aeb-8609aabceaec) class which implements `IEmailService` interface created. `IConfiguration` interface injected in class constructor, it allows using configuration from `appsettings.json`. `EmailService` class has only one method called `SendEmail()`. This method takes `EmailDto` as parameter and uses `MailKit` NuGet package features to configure email message, connect with smtp server, send email and disconnect.
4) [`EmailService`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1030bb8a-7b0f-451c-b144-68de676c5b1f) add for dependency injection in `Program.cs` file.
5) [`EmailController.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/2efd75f7-1cc6-48bd-99c3-193e8c43ca73) created. `EmailService` injected in class constructor. `EmailController` has only one method called `SendEmail` which is a `HttpPost` method. This method takes `EmailDto` as parameter, uses `EmailService` to send emails and returns `Status200OK` when everything goes fine.
#### III `SimpleMailApp.WebAssembly`functionalities
