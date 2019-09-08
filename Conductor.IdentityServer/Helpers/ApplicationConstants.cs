namespace Conductor.IdentityServer.Helpers
{
    /// <summary>
    /// Define all application related constants here
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// Name of the environment variable containing swagger client name
        /// </summary>
        public const string SwaggerClient = "SWAGGER_CLIENT";

        /// <summary>
        /// Name of the API
        /// </summary>
        public const string ConductorApi = "ConductorApi";

        /// <summary>
        /// Name of the test client
        /// </summary>
        public const string ConductorTestClient = "ConductorApi_UnitTest";

        /// <summary>
        /// Name of the swagger client
        /// </summary>
        public const string ConductorSwaggerClient = "ConductorApi_Swagger";

        /// <summary>
        /// Secret key for test client to access
        /// </summary>
        public const string ConductorTestClientSecret = "3c8f10d5-db6f-4620-ad75-63b31cadc071";

    }
}
