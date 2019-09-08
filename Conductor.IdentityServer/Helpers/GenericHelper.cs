using System;

namespace Conductor.IdentityServer.Helpers
{
    public class GenericHelper
    {

        /// <summary>
        /// Get the value from environment variable and converts it to a uri
        /// </summary>
        /// <param name="environmentVariable">environment variable name</param>
        /// <returns>Uri object</returns>
        public static Uri GetUriFromEnvironmentVariable(string environmentVariable)
        {
            var address = Environment.GetEnvironmentVariable(environmentVariable);
            // get from settings


            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Variable not found");

            if (Uri.TryCreate(address, UriKind.Absolute, out Uri uri))
            {
                return uri;
            }

            throw new Exception("Invalid uri in the environment variable");
        }

        /// <summary>
        /// Combines path to base uri
        /// </summary>
        /// <param name="baseUri">base uri</param>
        /// <param name="path">path to be combined</param>
        /// <returns></returns>
        public static Uri CombineUri(Uri baseUri, string path)
        {
            if (Uri.TryCreate(baseUri, path, out Uri uri))
            {
                return uri;
            }
            throw new Exception("Invalid path");
        }

        /// <summary>
        /// Get the value from environment variable and converts it to a uri
        /// </summary>
        /// <param name="environmentVariable">environment variable name</param>
        /// <param name="path">path to be combined</param>
        /// <returns>Uri object</returns>
        public static Uri GetUriFromEnvironmentAndCombine(string environmentVariable, string path)
        => CombineUri(GetUriFromEnvironmentVariable(environmentVariable), path);
    }
}
