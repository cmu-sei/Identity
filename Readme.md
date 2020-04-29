# IdentityServer

An application to manage authentication with OpenID Connect and OAuth2.


### Features
* Email/Password authentication
* Client Certificates authentication
* 2FA via email or totp
* Offline codes for isolated environments
* User Profile management
* Resource and Client management
* Supports PostgreSQL and SqlServer

### Documentation

For now, see the /docs folder.

See [appsettings.conf](src/IdentityServer/appsettings.conf) to alter configuration.
The app also applies `appsettings.<Environment>.conf`.
Then it checks for a conf file pointed at by env APPSETTINGS_PATH or ./conf/appsettings.conf.

This app is generally integrated with *identity-ui*, *JAvatar* and *AppMailRelay*.

### Build

* Install .NET Core SDK 3.1;
* Clone this repository;
* `bash pullcdn.sh` or `pullcdn.ps1`
* `dotnet build`

### Support

Please use the issue tracker for bug reports and feature requests.

### Acknowlegements

This project relies on the great work of these projects:
* [IdentityServer4](https://github.com/IdentityServer/IdentityServer4)
* [ASP.NET Core](https://github.com/aspnet)
