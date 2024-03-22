# SimpleMailApplication
### `Simple Mail Application combines ASP .Net Core Web API with Blazor WebAssembly App`
## Short description
API uses `MailKit` NuGet package to provide logic for sending emails to specific mail host. 
WebAssembly uses `Blazor Radzen` and `Blazor Toast Notification` to provide user friendly interface.
`Gmail` is used as smtp server.

Sollution schema in `MS Visual Studio` (the most important elements are marked):

![0Schema2](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/4a6bc57a-f8fb-4e9b-9bec-518b8a16fe31)


#### I Creating projects, combining them and installing NuGet packages
1) [`SimpleMail.Lib`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/88643c43-90b1-4638-bdb3-30c9cebdc957)
 is a class library which contains `EmailDto.cs` class used as data transfer object in other parts of sollution.
2) [`SimpleMailApp.Api`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/64adc087-4437-4375-91f2-107d4be45a5f) is `ASP .Net Core Web API` part of sollution. `MailKit` NuGet package was installed in this part.
3) [`SimpleMailApp.WebAssembly`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/38dfb658-8bb3-4fa2-9721-5fb0ce899fb2) is `Blazor WebAssembly App` part of sollution. `Radzen.Blazor` and `Blazor.Toast` NuGet packages was installed in this part.
4) [`Combining`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1c294229-df6e-4dab-9780-b343f5be8afd) Api and WebAssembly parts of sollution.
5) Project reference to `SimpleMail.Lib` added in [`SimpleMailApp.Api`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/eb7923c3-7728-4fbe-8aa5-d7c6a8aeab9b) and in [`SimpleMailApp.WebAssembly`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/5a9d7e44-b28d-4e7f-aec7-c0484096c3f3).
#### II `SimpleMailApp.Api` functionalities
1) [`appsettings.json`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/776710cd-cdca-4f85-b8ea-eac083134f50)
file configured to use specific mail host from `Gmail` service: `EmailHost`, `EmailUsername` and `EmailPassword` added.
2) [`IEmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/cb4e9f76-b519-4cc2-bb20-9e6e0b6881b3) interface created which takes `EmailDto.cs` from `SimpleMail.Lib` as parameter.
3) [`EmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1fc1e1dd-aad0-4030-8d23-480043655901)
class which implements `IEmailService` interface created. `IConfiguration` interface injected in class constructor, it allows using configuration from `appsettings.json`. `EmailService` class has only one method called `SendEmail()`. This method takes `EmailDto` as parameter and uses `MailKit` NuGet package features to configure email message, connect with smtp server, send email and disconnect.
4) [`EmailService`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1030bb8a-7b0f-451c-b144-68de676c5b1f) add for dependency injection in `Program.cs` file.
5) [`EmailController.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/2efd75f7-1cc6-48bd-99c3-193e8c43ca73) created. `EmailService` injected in class constructor. `EmailController` has only one method called `SendEmail` which is a `HttpPost` method. This method takes `EmailDto` as parameter, uses `EmailService` to send emails and returns `Status200OK` when everything goes fine.
#### III `SimpleMailApp.WebAssembly`functionalities
1) [`IEmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/ed2258d4-72e5-414d-873a-48e24a94385f) interface which takes `EmailDto.cs` as parameter created.
2) [`EmailService.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/3ecb6171-1009-4557-8714-ff9e6d5db200) class which implements `IEmailService` created. `HttpClient` is injected in class constructor. This class has only one method called `SendEmail()` which takes `EmailDto` as parameter. This method uses `HttpClient` to connect with Api part of the project and sends emails when everything is fine.
3) `EmailService`, `Blazor Toast` and `Blazor Radzen` added for dependency injection in [`Program.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/47ec152b-2f42-4188-9441-cc59f64b6265).
4) `Blazor Toast` and `Blazor Radzen` usings addded in [`_Imports.razor`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/bd108632-6554-40e6-a280-e98a8ca26429).
5) In `wwwroot` direcotry in [`index.html`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/b6a967fc-fefc-4efe-8033-53681ccefc67) file `FontAwesome` added in `<head>` section in `<link>` tags, `Blazor Radzen` added in `<body>` section in `<script>` tag.
6) In [`MainLayout.razor`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/89bc5649-261f-4417-8dcd-70ebfac9d5e4) added `Blazor Radzen` on the bottom of the element, `Blazor Toast` configured using `FontAwesome` in `<BlazoredToast>` tag: notifications show for 2s on the top left corner of the website.
7) [`EmailSenderBase.cs`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/5dd1afae-76f9-483d-975b-d112ab5f5fff)
class which implements `ComponentBase` class created. `EmailService`, Blazor `ToastService` and `NavigationManager` injected. `EmailSenderBase` class has two properties: `popup` which is a boolean value set as `true` and `model` which is `EmailDto` type. The class has only one method called `SendEmail()` which checks if email address porvided by user is correct. When is it correct then it sends email, shows appropriate notification and reloads page, otherwise it only shows error message notification.
8) [`EmailSender.razor`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/c8315f52-0fcc-4f7e-910e-6be6353d4f3c) component created. It inherits from `EmailSenderBase` class and contains `Blazor Radzen` components. It provides user friendly interface and also some logic.
#### IV `SimpleMailApplication` in action
1) [`SimpleMailApp.Api`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/042f7ae4-41f2-4dd2-86c5-f0181bbd2d5c) part which could be desplayed and used in Internet Browser using `Swaagger`.
2) [`Email Sender`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/7e80be35-5001-4f4f-bd0b-c299ac30cdf9) page view: three text fields: email addres, email subject and email message and `Send Email` button.
3) `Blazor Radzen` in action: [`wrong email addres`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/94339253-8d6d-4769-9e51-5d83722d743f), 
[`empty email address`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/de1daa36-ddea-42f1-b1ac-d21a9d1037c7).
4) `Blazor Toast Notification` in action after `Send Email` button click (top left corner): [`empty email addressd`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/559dc39d-2111-43a0-9755-67beb7f3ca18), [`wrong email address format`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/d3564665-e80b-4294-b634-3a99caa6a980), [`correct email address format`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/b1d279aa-643c-4f8c-805c-31917b4903c1).
5) [`Sent message`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/954b329a-bbb1-41e2-858e-8ba1437e34cd), message recipient's inbox view - [`unread message`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/1ecfba0f-df3c-4dd9-8f79-7b10befc8811), [`read message`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/613fa005-e593-4870-b8ab-80f7fa4484b1) and Email Sender [`outbox view`](https://github.com/jpurgat95/SimpleMailApplication/assets/94840984/e4c1e616-4a44-4557-9691-6067891106a8).
