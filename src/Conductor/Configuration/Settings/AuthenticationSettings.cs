using System.ComponentModel.DataAnnotations;

namespace Conductor.Configuration.Settings
{
    /// <summary>
    /// Authentication configuration settings like identityserver address
    /// </summary>
    public class AuthenticationSettings
    {
        /// <summary>
        /// IdenityServer4 authorization end point
        /// </summary>
        [Required]
        public string AuthorizationUrl { get; set; }

        /// <summary>
        /// Api scope
        /// </summary>
        [Required]
        public string Scope { get; set; }

    }
}
