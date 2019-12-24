# Authentication

By default, authentication is disabled.  To enable it, set the `AUTH` environment variable to `true` and the `PUBLICKEY` variable to a valid [ECDsa](https://en.wikipedia.org/wiki/Elliptic_Curve_Digital_Signature_Algorithm) public key.

If authentication is enabled then you need to include a signed [JWT bearer token](https://jwt.io/) along with every request.  The is done by adding the `Authorization: Bearer <<token>>` header to each request.
The token should be a valid JWT token that was signed with the corresponding private key to the public one in the environment variable.

The token must also include role claims that indicate the level of access.  The following roles are used within Conductor.
* `Admin` - Adminstrative tasks.
* `Author` - Authoring of workflow definitions and steps.
* `Controller` - Starting, stopping, suspending and resuming workflows.
* `Viewer` - Querying the status of a workflow.

A minimal JWT payload the include all the roles would look as follows

```json
{
  "role": [
    "Admin",
    "Author",
    "Controller",
    "Viewer"
  ]
}
```

https://github.com/keycloak/keycloak
https://www.keycloak.org/
https://hub.docker.com/r/jboss/keycloak