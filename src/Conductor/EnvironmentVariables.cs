using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conductor
{
    public static class EnvironmentVariables
    {        
        public static string DbHost => Environment.GetEnvironmentVariable("dbhost");
        public static string Redis => Environment.GetEnvironmentVariable("redis");
        public static string Auth => Environment.GetEnvironmentVariable("auth");
        public static string PublicKey => Environment.GetEnvironmentVariable("publickey");
        public static string Alg => Environment.GetEnvironmentVariable("alg");
    }
}
