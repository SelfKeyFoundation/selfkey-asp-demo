# Overview

SelfKey ASP.NET Core Demo provides a demo project using an ASP.NET Core Web API to demonstrate how to use SelfKey.NET to leverage the signing and verification of messages via a RESTful web interface.

## SelfKey.Login.Service

 * This project consists of a controller class (`SelfKeyController`) which exposes several routing methods.
 * The `Sign` method takes a JSON payload based on **SelfKey.Login.Data** and a private key which is then signed using **SelfKey.Login.Api**.  The payload is modified with a signature and returned as a result.
 * The `Verify` method takes a JSON payload based on **SelfKey.Login.Data** which is then verified using **SelfKey.Login.Api**.  A status code of 200 (OK) is returned if successful; otherwise 400 (Bad Request) is returned.

# Development

To develop and build locally, open `SelfKey.AspNetCore.Demo.sln` in Visual Studio 2017 15.7 or newer.  You will also need [.NET Core SDK 2.1](https://www.microsoft.com/net/download/dotnet-core/2.1) or newer installed.  Press F5 to run in the debugger.  It will open your browser at a listening endpoint.

## Usage

A Postman collection ([SelfKey.postman_colletion.json](SelfKey.postman_colletion.json)) is provided in the solution.  This provides two example requests:

 * A `Sign` request which contains an empty signature.
 * A `Verify` request which contains the signature that would be in the payload returned by the `Sign` request.
