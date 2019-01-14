# Overview

SelfKey ASP.NET Core Demo is a demo project consisting of a web app (**SelfKey.Login.Client**) and a web API (**SelfKey.Login.Service**) which both use ASP.NET Core MVC to demonstrate how to use SelfKey.NET to verify messages via a RESTful web interface.  Authentication is achieved using MSAL.NET and Azure Active Directory (AAD) v2.0 which uses the OpenID Connect (OIDC) authentication protocol (built on OAuth 2.0).  Users may sign in to the web app using their Work and School accounts and Microsoft Personal accounts.  You will need to register both applications in the [Azure portal](https://portal.azure.com).

This is of course just one method of authentication and it is demonstrated using an ASP.NET Core MVC web app as the user interface.  Other authentication methods may also be used such as OpenIddict or IdentityServer4 (or a commercial offering such as Auth0, Okta, etc.), and these may be within any application type.  This demo project uses Microsoft's latest technologies to keep within the Microsoft ecosystem and technology stack.

## SelfKey.Login.Client

 * This project consists of two controller classes (`AccountController` and `HomeController`) which handle the signing-in process and sending of the JSON data entered on the web page to the web API to be verified (via the `/api/selfkey/verify` route).
 * There are several views represented by Razor pages.  The view connected to `HomeController` is [Index.cshtml](SelfKey.Login.Client/Views/Home/Index.cshtml) which allows entry of JSON data.
 * There are also some extension and helper classes to assist with the Azure AD authentication process.  This is generic code.

## SelfKey.Login.Service

 * This project consists of a controller class (`SelfKeyController`) which exposes several routing methods.
 * The `Verify` method takes a JSON data object based on the `User` class in **SelfKey.Login.Data** which is then verified using **SelfKey.Login.Api**.  A status code of 200 (OK) is returned if successful; otherwise 400 (Bad Request) is returned.

## Running the sample

### Prerequisites

- [.NET Core SDK 2.2](https://dotnet.microsoft.com/download) or newer
- [Visual Studio 2017](https://aka.ms/vsdownload) or newer
- [Azure AD tenant](https://docs.microsoft.com/en-gb/azure/active-directory/fundamentals/active-directory-access-create-new-tenant)
- [Azure AD user](https://docs.microsoft.com/en-gb/azure/active-directory/fundamentals/add-users-azure-active-directory) (this sample will not work with a Microsoft account)

### Step 1: Register the sample with the Azure AD tenant

#### Choose the Azure AD tenant where you want to create your applications

1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account or a personal Microsoft account.
1. If your account gives you access to more than one tenant, select your account in the top right corner, and set your portal session to the desired Azure AD tenant (using **Switch Directory**).

There are two projects in this sample.  Each needs to be registered in your Azure AD tenant.

#### Register the **SelfKey.Login.Service** web API

1. In the left-hand navigation pane, select **Azure Active Directory** service, then select **App registrations (Preview)**.
1. In the **App registrations (Preview)** page, select **New registration**.
1. In the **Register an application** page, enter your application's registration information:
   - For **Name**, enter `SelfKey.Login.Service`.
   - For **Supported account types**, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.
   - In the Redirect URI (optional) section, select **Web** and enter the base URL for the sample which is `https://localhost:44305`.
   - Select **Register** to create the application.
1. On the app **Overview** page, find the **Application (client) ID** value and note it for use later.  You'll need it to configure the Visual Studio configuration file for this project.

#### Register the **SelfKey.Login.Client** web application

1. Return to the list of app registrations using the **App registrations (Preview)** breadcrumb at the top.
1. In the **App registrations (Preview)** page, select **New registration**.
1. In the **Register an application** page, enter your application's registration information:
   - For **Name**, enter `SelfKey.Login.Client`.
   - For **Supported account types**, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.
   - For **Redirect URI (optional)**, select **Web** and enter the base URL for the sample which is `https://localhost:44378/signin-oidc`.
   - Select **Register** to create the application.
1. On the app **Overview** page, find the **Application (client) ID** value and note it for use later.  You'll need it to configure the Visual Studio configuration file for this project.
1. On the app **Authentication** page, enter `https://localhost:44378/Account/SignOut` for **Logout URL**, then click **Save**.
1. On the app **Certifications and secrets** page, click **New client secret**, select your desired expiry (personal preference), then click **Add**.  Note the displayed secret value for use later as it will not be displayed again.  Should it be lost, a new one must be created.
1. On the app **Expose an API** page, click **Add a scope**, then enter the following information:
   - For **Scope name**, enter `access_as_user`.  Note the scope URI (starts with `api://`) for use later.
   - For **Admin consent display name**, enter `Access SelfKey.Login.Service as an admin`.
   - For **Admin consent description**, enter `Accesses the SelfKey.Login.Service Web API as an admin`.
   - For **User consent display name**, enter `Access SelfKey.Login.Service as a user`.
   - For **User consent description**, enter `Accesses the SelfKey.Login.Service Web API as a user`.
   - Ensure **State** is enabled, then click **Save**.

### Step 2: Configure the sample to use the Azure AD tenant

> [!WARNING]
> Follow these steps carefully. The **Application ID** property of both app registrations are used below.

#### Configure the **SelfKey.Login.Service** project

1. In the **SelfKey.Login.Service** project, open the [appsettings.json](SelfKey.Login.Service/appsettings.json) file.
1. Replace the `ClientId` value with the **Application ID** of the *SelfKey.Login.Service* app noted earlier.

#### Configure the **SelfKey.Login.Client** project

1. In the **SelfKey.Login.Client** project, open the [appsettings.json](SelfKey.Login.Client/appsettings.json) file.
1. Replace the `ClientId` value with the **Application ID** of the *SelfKey.Login.Client* app noted earlier.
1. Replace the `ClientSecret` value with the secret value for the *SelfKey.Login.Client* app noted earlier.
1. Replace the `ServiceResourceId` value with the **Application ID** of the *SelfKey.Login.Service* app noted earlier.
1. Replace the `Scopes` value with the scope URI of the *SelfKey.Login.Service* app noted earlier.

### Step 3: Run the sample

Open [SelfKey.AspNetCore.Demo.sln](SelfKey.AspNetCore.Demo.sln) in Visual Studio.  In the solution properties, set both projects as startup projects.  Set **SelfKey.Login.Service** to run first.  Build and run the solution.

On startup, the web API displays an empty web page (Chrome shows `HTTP ERROR 401`).  This is a listening endpoint and is expected behavior.

Explore the sample by signing in into the web app, and verifying a JSON data object.  Example JSON data ([User.example.json](User.example.json)) is provided in the solution which describes a `User` object containing the nonce, signature and public key which may be submitted for verification using the demo application.
