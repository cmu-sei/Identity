# Release Notes

## Release 2008 v1.3.5

Improve checking for valid domain to properly handle subdomains.  Now `me@sub.my.domain` will succeed when `my.domain` or `domain` is allowed.

Align confirmation-code timeout with confirmation-cookie timeout.  Even though codes are valid for 60 minutes, by default, they fail if submitted after the 5 minute timeout of the confirmation-cookie.  These both default to 60 minutes now.

Send mail as html if body starts with `<!doctype html` (case-insensitive).

Fix an issue where new api resources are created incompletely.

Fix url construction and token validation when hosted in virtual directory.

Fix issue with client cors urls.  If no CORS url(s) supplied, defaults to redirect url host.
