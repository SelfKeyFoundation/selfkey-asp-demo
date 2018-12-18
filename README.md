# Overview

SelfKey ASP.NET Core Demo is a demo project consisting of a web app (**SelfKey.Login.Client**) and a web API (**SelfKey.Login.Service**) which both use ASP.NET Core MVC to demonstrate how to use SelfKey.NET to verify messages via a RESTful web interface.  Authentication is achieved using MSAL.NET and Azure Active Directory (AAD) v2.0 which uses the OpenID Connect (OIDC) authentication protocol (built on OAuth 2.0).  Users may sign in to the web app using their Work and School accounts and Microsoft Personal accounts.  You will need to register both applications in the [Azure portal](https://portal.azure.com).

This is of course just one method of authentication and it is demonstrated using an ASP.NET Core MVC web app as the user interface.  Other authentication methods may also be used such as OpenIddict or IdentityServer4 (or a commercial offering such as Auth0, Okta, etc.), and these may be within any application type.  This demo project uses Microsoft's latest technologies to keep within the Microsoft ecosystem and technology stack.

## SelfKey.Login.Client

 * This project consists of two controller classes (`AccountController` and `HomeController`) which handle the signing-in process and sending of the JSON data entered on the web page to the web API to be verified (via the `/api/selfkey/verify` route).
 * There are several views represented by Razor pages.  The view connected to `HomeController` is Index.cshtml which allows entry of JSON data.
 * There are also some extension and helper classes to assist with the Azure AD authentication process.  This is generic code.

## SelfKey.Login.Service

 * This project consists of a controller class (`SelfKeyController`) which exposes several routing methods.
 * The `Verify` method takes a JSON data object based on the `User` class in **SelfKey.Login.Data** which is then verified using **SelfKey.Login.Api**.  A status code of 200 (OK) is returned if successful; otherwise 400 (Bad Request) is returned.

# Development

To develop and build locally, open `SelfKey.AspNetCore.Demo.sln` in Visual Studio 2017 15.9.4 or newer.  You will also need [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) or newer installed.  Press F5 to run in the debugger.  It will open your browser at a listening endpoint.

## Usage

Example JSON data ([User.example.json](User.example.json)) is provided in the solution which describes a `User` object containing the nonce, signature and public key which may be submitted for verification using the demo application.