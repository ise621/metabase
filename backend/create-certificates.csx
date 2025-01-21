using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// TODO Is there a better way to manage these keys? Is there gonna be a problem in two-years time when the keys become invalid?
// Inspired by https://github.com/openiddict/openiddict-core/blob/78901e3e7e3ee47cf7846a71f758dc9ca110b1a2/src/OpenIddict.Server/OpenIddictServerBuilder.cs#L661-L679
// and https://github.com/openiddict/openiddict-core/blob/78901e3e7e3ee47cf7846a71f758dc9ca110b1a2/src/OpenIddict.Server/OpenIddictServerBuilder.cs#L661-L679
foreach (var (fileName, name, flags, password) in new[] {
    ("jwt-encryption-certificate.pfx", "Encryption", X509KeyUsageFlags.KeyEncipherment, Args[0]),
    ("jwt-signing-certificate.pfx", "Signing", X509KeyUsageFlags.DigitalSignature, Args[1])
})
{
    var path = Path.Join("src", fileName);
    var certificate =
        File.Exists(path)
        ? X509CertificateLoader.LoadPkcs12FromFile(path, password)
        : null;
    if (certificate is null || certificate.NotAfter <= DateTime.Now)
    {
        var subject = new X500DistinguishedName($"CN=Metabase OpenId Connect Server {name} Certificate");
        // certificates.LastOrDefault(certificate => certificate.NotBefore < DateTime.Now && certificate.NotAfter > DateTime.Now);
        // TODO Is RSA sufficiently secure? Or should we use ECDSA?
        using (var algorithm = RSA.Create(keySizeInBits: 2048))
        {
            var request = new CertificateRequest(
                subject,
                algorithm,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(flags, critical: true)
                );
            certificate = request.CreateSelfSigned(
                notBefore: DateTimeOffset.UtcNow,
                notAfter: DateTimeOffset.UtcNow.AddYears(2)
            );
            File.WriteAllBytes(
                path,
                certificate.Export(X509ContentType.Pfx, password)
            );
        }
    }
}