# Authentication

Conductor supports integration authentication using the [OpenID Connect](https://openid.net/connect/) protocol.

By default, authentication is disabled.  To enable it, 
* Set the `AUTH` environment variable to `true` 
* Set the `ALG` environment variable to the signing algorithm (`RS256` or `ES256`)
* Set the `PUBLICKEY` variable to a Base64 encoded public key.

If authentication is enabled then you need to include a signed [JWT bearer token](https://jwt.io/) along with every request.  The is done by adding the `Authorization: Bearer <<token>>` header to each request.
The token should be a valid JWT token that was signed with the corresponding private key to the public one in the environment variable.

The token must also include a scope claim that indicate the level of access.  The following scopes are used within Conductor.

* `conductor:admin` - Adminstrative tasks.
* `conductor:author` - Authoring of workflow definitions and steps.
* `conductor:controller` - Starting, stopping, suspending and resuming workflows.
* `conductor:viewer` - Querying the status of a workflow.

A minimal JWT payload the include all the scopes would look as follows

```json
{
  "scope": "conductor:admin conductor:author conductor:controller conductor:viewer"
}
```

Some authentication servers that support [OpenID Connect](https://openid.net/connect/) include

* [Auth0](https://auth0.com/) - A cloud service
* [Keycloak](https://github.com/keycloak/keycloak/) - Open source auth server

