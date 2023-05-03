using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;

namespace demoidp.IdentityServer;

internal static class IdentityServerBuilderExtensions
{
    internal static IIdentityServerBuilder AddEmbeddedSigningCredential(
        this IIdentityServerBuilder builder)
    {
        var type = typeof(StringExtensions).GetTypeInfo();
        var rootNamespace = type.Namespace;
        var assembly = type.Assembly;
        
        var manifestResourceStream = assembly?.GetManifestResourceStream($"{rootNamespace}.key.rsa");

        using var reader = new StreamReader(manifestResourceStream);
        var json = reader.ReadToEnd();

        var rsaKey = JsonSerializer.Deserialize<RsaKey>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var rsaSecurityKey = CryptoHelper.CreateRsaSecurityKey(rsaKey.Parameters, rsaKey.KeyId);
        return builder.AddSigningCredential(rsaSecurityKey, IdentityServerConstants.RsaSigningAlgorithm.RS256);
    }

    private class RsaKey
    {
        public string KeyId { get; set; }

        public RSAParameters Parameters { get; set; }
    }
}