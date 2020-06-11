# Importing Data

### Admin Account
For simple start up seeding, set the `Account` settings:
```json
"Account": {
    "AdminEmail": "admin@this.ws",
    "AdminPassword": "321ChangeMe!",
    "AdminGuid": "",  # if necessary
    "OverrideCode": "123456"
}
```

### Dev Clients
Other projects that rely on the IdentityServer can script an API call to quickly set up a simple client.  The `POST /api/resource/devimport` endpoint accepts a `ResourceImport` model; for example:
```json
{
    "Apis": [ "project-api" ],
    "Clients": [
        {
            "Id": "project-ui",
            "DisplayName": "",
            "GrantType": "authorization_code",
            "Scopes": "openid profile project-api",
            "Secret": "",
            "RedirectUrl": "http://localhost:5004/oidc"
        },
        {
            "Id": "project-swagger",
            "GrantType": "implicit",
            "Scopes": "openid project-api",
            "RedirectUrl": "http://localhost:5004/api/oauth2-redirect.html"
        }
    ]
}
```
Use `curl` or some other mechanism to post this application/json data to the endpoint.

The model is purposefully simple, and the intent is for quick dev setup.  If you need more detail, then tweak it with the ui after importing, or create a seed file as described below.

### Seed Data
For more detailed database seeding, Users, Resources, and Clients can be added on startup using a `seed-data.json` file. (The filename is configuration with the `Database:SeedFile` setting.)  See [example-seed-data.json](../src/IdentityServer/example-seed-data.json).

If secrets are stored separately, they can be added to the settings as an array of key-value pairs. This applies to user passwords and client shared-secrets.

In `appsettings.Production.json` that would look like:
```json
{
    "SeedData": [
        { "Key": "client-id", "Value": "client-secret" },
        { "Key": "anotherclient-id", "Value": "anotherclient-secret" },
        { "Key": "user@some.ws", "Value": "321ChangeMe!" },
    ]
}
```

Or as Environment variables:
```
SeedData:[0]:Key=client-id
SeedData:[0]:Value=client-secret
SeedData:[1]:Key=user@some.ws
SeedData:[1]:Value=anotherclient-secret
```
