using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Helpers
{
    public class ApplicationConstants
    {

        /// <summary>
        /// Name of the Authentication settings section
        /// </summary>
        public const string AuthenticationSettings = nameof(ApplicationConstants.AuthenticationSettings);

        /// <summary>
        /// Address where identity server is running
        /// </summary>
        public const string AuthenticationAuthority = "AUTHENTICATION_AUTHORITY";

        /// <summary>
        /// Address of the swagger client
        /// </summary>
        public const string SwaggerClient = "SWAGGER_CLIENT";

        /// <summary>
        /// Name of the API
        /// </summary>
        public const string ApiName = "ConductorApi";

        /// <summary>
        /// Id of the swagger client
        /// </summary>
        public const string SwaggerClientId = "ConductorApi_Swagger";

        /// <summary>
        /// Name of the test client
        /// </summary>
        public const string ConductorTestClient = "ConductorApi_UnitTest";

        /// <summary>
        /// Used to display api name in the swagger UI screen
        /// </summary>
        public const string ApiDisplayName = "Conductor Api";

        /// <summary>
        /// Used to display description of api access needed
        /// </summary>
        public const string ApiDescription = "Conductor Api - full access";

        /// <summary>
        /// Used for passing oauth2
        /// </summary>
        public const string OAuth2 = "oauth2";
    }
}
