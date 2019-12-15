using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;

namespace ScratchPad
{
    class Program
    {
        static string PrivateKey = "MHcCAQEEIEVB7uPYNa0BSvKQPhXVPf0cVilo88STthQrwzIEHnfSoAoGCCqGSM49AwEHoUQDQgAEGlmSn1KFXFsQW1GjivT1cES9AD/Sl/bqwcYqdsDFRL4b56cYGK413FFPNRQS8TworgBDHIJSi1toDJ19WzhLXw==";
        //static string PublicKey = "AAAAE2VjZHNhLXNoYTItbmlzdHAyNTYAAAAIbmlzdHAyNTYAAABBBBpZkp9ShVxbEFtRo4r09XBEvQA/0pf26sHGKnbAxUS+G+enGBiuNdxRTzUUEvE8KK4AQxyCUotbaAydfVs4S18=";
        static string RealPublicKey = "GlmSn1KFXFsQW1GjivT1cES9AD/Sl/bqwcYqdsDFRL4b56cYGK413FFPNRQS8TworgBDHIJSi1toDJ19WzhLXw==";

        static string Key2 = "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgevZzL1gdAFr88hb2OF/2NxApJCzGCEDdfSp6VQO30hyhRANCAAQRWz+jn65BtOMvdyHKcvjBeBSDZH2r1RTwjmYSi9R/zpBnuQ4EiMnCqfMPWiZqB4QdbAd0E7oH50VpuZ1P087G";
        static string Key2Pub = "hcEuk1hs8QsZT24s96dnlORoFLF+Alh1wxVkKdSs0mH8CM7SEAOhONKi8xM1/kEDufovcKwvvxx+z3r1SvNpGA==";

        static void Main(string[] args)
        {
            MakeToken();
            //var token = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJmaXJzdE5hbWUiOiJ0ZXN0IiwibGFzdE5hbWUiOiJ0ZXN0IiwibmJmIjoxNTc2NDQyODg5LCJleHAiOjE1NzY3MDIwODksImlhdCI6MTU3NjQ0Mjg4OX0.VK05y04Uxjsev8_kxOyk5UtkWJTwBWnJ47_riM5EwYEibJiusfbI_34PD2AM4iYiP_5RxkUIc6TiG2LG2FdbWg";
            //Console.WriteLine(VerifyToken(token));
        }

        static void MakeToken()
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            //var keyDataStr = File.ReadAllText(@"C:\dev\jwt\test1.key");
            //var tr = new System.IO.StringReader(keyDataStr);
            //var pr = new Org.BouncyCastle.OpenSsl.PemReader(tr);
            //var pem = pr.ReadPemObject();
            var e1 = ECDsa.Create(); // (ECCurve.NamedCurves.nistP256);
            //var privKey = Convert.FromBase64String(PrivateKey);
            var privKey = Convert.FromBase64String(Key2);
            e1.ImportECPrivateKey(privKey, out int rb1);

            var params1 = e1.ExportParameters(false);
            var pubStr = Convert.ToBase64String(params1.Q.X.Concat(params1.Q.Y).ToArray());
            Console.WriteLine(pubStr);
            Console.WriteLine();


            var key = new ECDsaSecurityKey(e1);           
            
            var sc = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim( "email", "test@test.com" ),
                new Claim( "firstName", "test" ),
                new Claim( "lastName", "test" )
            }),
                Expires = now.AddDays(3),                
                SigningCredentials = sc,
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine(tokenString);

            Console.ReadLine();
        }

        public static bool VerifyToken(string jwt)
        {            
            string[] jwtParts = jwt.Split('.');

            //var sha256 = SHA256.Create();
            //var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(jwtParts[0] + '.' + jwtParts[1]));
            //var a = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken()
            //Microsoft.IdentityModel.JsonWebTokens.JwtTokenUtilities.CreateEncodedSignature()

            //var keyDataStr = "AAAAE2VjZHNhLXNoYTItbmlzdHAyNTYAAAAIbmlzdHAyNTYAAABBBBpZkp9ShVxbEFtRo4r09XBEvQA/0pf26sHGKnbAxUS+G+enGBiuNdxRTzUUEvE8KK4AQxyCUotbaAydfVs4S18=";  //File.ReadAllText(@"C:\dev\jwt\test1.key.pub");
            var pubKey = Convert.FromBase64String(RealPublicKey);  //(RealPublicKey);
            var e1 = ECDsa.Create();
            e1.ImportParameters(new ECParameters()
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint()
                {
                    X = pubKey.Take(32).ToArray(),
                    Y = pubKey.Skip(32).Take(32).ToArray()
                }
            });

            var key = new ECDsaSecurityKey(e1);
            var sc = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt);

            var tvp = new TokenValidationParameters()
            {
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false
            };
            var cp = tokenHandler.ValidateToken(jwt, tvp, out var vt);
            
            

            return false;

        }


        private static ECDsa LoadPrivateKey(byte[] key)
        {
            var privKeyInt = new Org.BouncyCastle.Math.BigInteger(+1, key);
            var parameters = SecNamedCurves.GetByName("secp256r1");
            var ecPoint = parameters.G.Multiply(privKeyInt);
            var privKeyX = ecPoint.Normalize().XCoord.ToBigInteger().ToByteArrayUnsigned();
            var privKeyY = ecPoint.Normalize().YCoord.ToBigInteger().ToByteArrayUnsigned();

            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privKeyInt.ToByteArrayUnsigned(),                
                Q = new ECPoint
                {
                    X = privKeyX,
                    Y = privKeyY
                }
            });
        }

        private static byte[] FromHexString(string hex)
        {
            var numberChars = hex.Length;
            var hexAsBytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                hexAsBytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return hexAsBytes;
        }

        private static ECDsa LoadPublicKey(byte[] key)
        {
            var pubKeyX = key.Skip(1).Take(32).ToArray();
            var pubKeyY = key.Skip(33).ToArray();

            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    X = pubKeyX,
                    Y = pubKeyY
                }
            });
        }


        private readonly DerObjectIdentifier _curveId = SecObjectIdentifiers.SecP256k1;

        public byte[] GeneratePrivateKey()
        {
            return SecureRandom.GetNextBytes(SecureRandom.GetInstance("SHA256PRNG"), 32);
        }

        public byte[] BuildPrivateKeyFromPhrase(string phrase)
        {
            using (var hasher = System.Security.Cryptography.SHA256.Create())
            {
                var privateKey = hasher.ComputeHash(Encoding.Unicode.GetBytes(phrase));
                return privateKey;
            }
        }

        public byte[] GetPublicKey(byte[] privateKey)
        {
            var privKeyInt = new BigInteger(privateKey);
            var parameters = NistNamedCurves.GetByOid(_curveId);
            var qa = parameters.G.Multiply(privKeyInt);

            return qa.GetEncoded();
        }

        public byte[] Sign(byte[] data, byte[] privateKey)
        {
            var cp = GetPrivateKeyParameters(privateKey);
            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(true, cp);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public bool Verify(byte[] data, byte[] sig, byte[] publicKey)
        {
            var cp = GetPublicKeyParams(publicKey);
            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(false, cp);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(sig);
        }

        private ECPrivateKeyParameters GetPrivateKeyParameters(byte[] privateKey)
        {
            var parameters = NistNamedCurves.GetByOid(_curveId);
            var privKeyInt = new BigInteger(privateKey);
            var dp = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N);
            return new ECPrivateKeyParameters("ECDSA", privKeyInt, dp);
        }

        private ECPublicKeyParameters GetPublicKeyParams(byte[] publicKey)
        {
            var parameters = NistNamedCurves.GetByOid(_curveId);
            var dp = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N);
            var q = parameters.Curve.DecodePoint(publicKey);
            return new ECPublicKeyParameters("ECDSA", q, dp);
        }


    }
}
