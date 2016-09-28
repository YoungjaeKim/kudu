using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Web.DataProtection;
using System;
using System.Security.Cryptography;

namespace Kudu.Core.Infrastructure
{
    public static class SecurityUtility
    {
        public static string GenerateSecretString()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[40];
                rng.GetBytes(data);
                String secret = Convert.ToBase64String(data);
                // Replace pluses as they are problematic as URL values
                return secret.Replace('+', 'a');
            }
        }

        public static string EncryptSecretString(string content)
        {
            var provider = DataProtectionProvider.CreateAzureDataProtector();
            var protector = provider.CreateProtector("function-secrets");
            return protector.Protect(content);
        }

        public static string DecryptSecretString(string content)
        {
            var provider = DataProtectionProvider.CreateAzureDataProtector();
            var protector = provider.CreateProtector("function-secrets");
            return protector.Unprotect(content);
        }

    }
}

