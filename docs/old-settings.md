# Settings Overview

Settings can be configured either with the `appsettings.json` (or more likely an environment copy like `appsettings.Production.json`) or with environment variables as per standard AspNetCore. Environment variables take the form of the json object hierarchy, like `Branding:ApplicationName` or `Branding__ApplicationName`.

You only need to set something if you want to change it from the default.

At a minimum, for quick start up, set the Account:Admin* values and the Account:OverrideCode (or disable Account:Authentication:Require2FA).

Then choose your Database settings.

#### Branding
| Branding       | Default | Description |
| -------------- | ------- | ----------- |
| ApplicationName | "Identity" | |
| Title | "OpenID Connect" | |
| LogoUrl | "logo.png" | |
| UiHost | "/ui" | assumes identity-ui app is served from `/ui` |
| IncludeSwagger | true | open api documenation can be disabled |

#### Caching
If running multiple replicas, consider running redis for distributed
caching or specify a shared folder.

| Caching        | Default | Description |
| -------------- | ------- | ----------- |
| Key | "idsrv" | redis key for this application (and replicas) |
| RedisUrl | "" | set url if running multiple replicas and redis |
| SharedFolder | "" | path to shared folder if running multiple replicas without redis |
| DataProtectionFolder | ".dpk" | folder to store data protection keys (if no redis) |

#### Database
Default is InMemory, so be sure to change this for production or any other
environment beyond just kicking the tires.

| Database       | Default | Description |
| -------------- | ------- | ----------- |
| Provider | InMemory | "InMemory", "PostgreSQL", "SqlServer" |
| ConnectionString | "IdentityServer" | provider specific connection string |
| SeedFile | "seed-data.json" | file for seeding custom data at startup |

#### AppMail
The application sends mail, primarily for retrieval of 2fa and verification codes.
It talks to an AppMailRelay host.

| AppMail        | Default | Description |
| -------------- | ------- | ----------- |
| Url | "" | url to AppMailRelay |
| Key | "" | api key for AppMailRelay |
| From | "" | mailto address for sender (if different than AppMailRelay's default sender) |

#### Account
Seed the admin account for Dev or Production, if you don't want to go through the
normal registration process.

| Account        | Default | Description |
| -------------- | ------- | ----------- |
| AdminEmail | "" | seed account if none exist |
| AdminPassword | "" | seed account if none exist |
| AdminGuid | "" | seed account if none exist |
| OverrideCode | "" | initial override code for admin 2fa |

#### Account:Authentication
Most of these defaults should be fine.

| Account:Authentication | Default | Description |
| ---------------------- | ------- | ----------- |
| Require2FA | true; | must provide 2fa code to login |
| RequireNotice | true; | must acknowledge notice page to login |
| CertificateIssuers | "" | describe accepted client certificates for display |
| NoticeFile | "wwwroot/html/notice.html"; | gets imported into ui |
| TroubleFile | "wwwroot/html/trouble.html"; | gets imported into ui |
| LockThreshold | 0 | number of failed logins before exponential delay lock |
| AllowRememberLogin | true; |  |
| RememberMeLoginDays | 30; |  |
| SigningCertificate | "signer.pfx" | pkcs12 file with cert/key for signing tokens; without this a temp key will be used which can lead to invalid tokens if the app gets restarted |
| SigningCertificatePassword| "" | password for pkcs12 file |
| ClientCertHeader | "X-ARR-ClientCert"; | header to check for client certificate |
| ClientCertSubjectHeader |  "ssl-client-subject-dn"; | header from ssl terminator (ie. nginx) |
| ClientCertIssuerHeader | "ssl-client-issuer-dn"; | header from ssl terminator (ie. nginx) |
| ClientCertVerifyHeader | "ssl-client-verify"; | header from ssl terminator (ie. nginx) |
| ClientCertSerialHeader | "ssl-client-serial"; | header from ssl terminator (ie. nginx) |

#### Account:Password
Set as desired.

| Account:Password | Default | Description |
| ---------------- | ------- | ----------- |
| ComplexityExpression | `@"(?=^.{8,}$)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.[~!@#$%^&*\(\)\-_=+\[\]\{\}\\|;:'"",<\.>/?\ ``]).*$"` | |
| ComplexityText | "At least 8 characters containing uppercase and lowercase letters, numbers, and symbols" | |
| History | 0 | number of remembered passwords |
| Age | 0 | days until password must be reset |
| ResetTokenExpirationMinutes | 60 | |
| InitialResetCode | "" | for offline environments, initial code for users to set password |

#### Account:Registration

| Account:Registration | Default | Description |
| -------------------- | ------- | ----------- |
| AllowManual | false | if allowing user self-registration, consider limiting by domain |
| AllowedDomains | "site.local" | delimited by space, comma, tab, or pipe |
| StoreName | true | store name as account property |
| StoreEmail | true | store email as account property |
| AllowMultipleUsernames | false | allow accounts to have multiple login email addresses |

#### Account:CertValidation
This is only used if certificates get passed to this app for validation.
Generally, it's recommended to offload cert validation to the ssl terminator
(i.e. nginx)

| Account:CertValidation | Default | Description |
| ---------------------- | ------- | ----------- |
| IssuerCertificatesPath |"issuers" | folder of x509 pem certs of trusted issuers |
| CheckRevocationOnline | false | |
| CheckChainRevocation | false | |
| VerificationTimeoutSeconds | 0 | |

#### Account:Profile
Settings for managing user profiles. We use our own JAvatar as an image store.

| Account:Profile | Default | Description |
| --------------- | ------- | ----------- |
| ForcePublic | false | authenticated users can see profiles |
| Domain | "site.local" | domain appended for id@domain for accounts with no email address |
| ImageServerUrl | "" | address or path to image server (JAvatar) |
| OrganizationUnitImagePath | "ou" | image server path |
| OrganizationImagePath | "o" | image server path |
| ProfileImagePath | "p" | image server path |

#### Header Settings
See appsettings.json for default Cors, Security Headers, and Forwarding options.  If you are behind a reverse proxy, you'll need to accept the `x-forwarded-*` headers from the proxy.  You might need to configure KnownNetworks or KnownProxies in order to achieve that.  Note the LogHeaders setting to aid in troubleshooting.

> *Note*: When developing with identity-ui, you'll need to allows CORS for it:  `appsettings.Development.json`
```json
"Headers": {
    "Cors": {
      "Origins": ["http://localhost:4200"],
      "AllowAnyMethod": true,
      "AllowAnyHeader": true,
      "AllowCredentials": true
    }
}
```
#### Logging
Logging is defaulted to Information.  Change using `Logging:LogLevel:Default=Debug`.
