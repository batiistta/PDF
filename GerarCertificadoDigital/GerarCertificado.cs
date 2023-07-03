using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using iTextSharp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace GerarCertificadoDigital
{
    public class GerarCertificado
    {
        public void Gerar(string caminhoDoCertificado, string senhaDoCertificado)
        {

            // Criar chave privada
            AsymmetricCipherKeyPair chave = GenerateKeyPair();

            // Criar informações do titular do certificado
            X509Name informacoesTitular = new X509Name("CN=Gabriel, O=Organização, C=BR");

            // Criar o certificado autoassinado
            X509Certificate certificado = GenerateSelfSignedCertificate(chave, informacoesTitular);

            // Criar o arquivo do certificado
            using (FileStream arquivoCertificado = new FileStream(caminhoDoCertificado, FileMode.Create))
            {
                // Criar o objeto PKCS12 para armazenar o certificado e a chave privada
                Pkcs12Store store = new Pkcs12StoreBuilder().Build();

                // Adicionar o certificado e a chave privada ao objeto PKCS12
                X509CertificateEntry certEntry = new X509CertificateEntry(certificado);
                store.SetCertificateEntry("cert", certEntry);
                store.SetKeyEntry("key", new AsymmetricKeyEntry(chave.Private), new[] { certEntry });

                // Salvar o PKCS12 no arquivo
                store.Save(arquivoCertificado, senhaDoCertificado.ToCharArray(), new SecureRandom());

                // MessageBox.Show($"Certificado gerado com sucesso." + caminhoDoCertificado);
            }


        }

        private AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var generator = new RsaKeyPairGenerator();
            var keygenParam = new KeyGenerationParameters(new SecureRandom(), 2048);
            generator.Init(keygenParam);
            return generator.GenerateKeyPair();
        }

        private X509Certificate GenerateSelfSignedCertificate(AsymmetricCipherKeyPair chave, X509Name informacoesTitular)
        {
            var geradorCertificado = new X509V3CertificateGenerator();
            var numeroSerial = BigInteger.ProbablePrime(120, new Random());
            geradorCertificado.SetSerialNumber(numeroSerial);
            geradorCertificado.SetSubjectDN(informacoesTitular);
            geradorCertificado.SetIssuerDN(informacoesTitular);
            geradorCertificado.SetNotBefore(DateTime.Now);
            geradorCertificado.SetNotAfter(DateTime.Now.AddYears(1));
            geradorCertificado.SetPublicKey(chave.Public);
            geradorCertificado.SetSignatureAlgorithm("SHA256WithRSA");

            return geradorCertificado.Generate(chave.Private);
        }
    }
}