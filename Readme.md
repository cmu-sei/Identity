
# ⚠️ DEPRECATED

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

### Run
Default settings are provided for "no-config" startup, meaning you can simple build the code and run.
However, these are *not* production worthy settings.
Namely, it uses an In-Memory database.
You'll want to change that to PostgreSQL or SqlServer.

Additionally, you'll want to set the `Account__AdminEmail` and `Account__AdminPassword` to have creds
to log in with.  Require2FA is true by default, so you should also add a `Account__OverrideCode` to
use as a temporary 2FA code, or set Require2FA to false.

If you haven't yet pulled the bootstrap dependencies, `bash pullcdn.sh` or `pullcdn.ps1` to do so.  This is a one-time task, or infrequent anyway.

`dotnet run` in the src/IdentityServer folder will get the app running at `http://localhost:5000` and swagger is at `/api`

### Support

Please use the issue tracker for bug reports and feature requests.

### Acknowlegements

This project relies on the great work of these projects:
* [IdentityServer4](https://github.com/IdentityServer/IdentityServer4)
* [ASP.NET Core](https://github.com/aspnet)
