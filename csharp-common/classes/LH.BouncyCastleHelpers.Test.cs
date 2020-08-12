using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using MicrosoftX509 = System.Security.Cryptography.X509Certificates;

namespace LH.BouncyCastleHelpers
{
    /// <summary>
    /// 测试。
    /// </summary>
    public static class Test
    {
        /// <summary>
        /// 测试。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:指定 StringComparison", Justification = "<挂起>")]
        public static void TestAll()
        {
            //
            // ===========================  密钥测试  ===========================
            //
            var caKeyPair = KeyParametersHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);
            var serverKeyPair = KeyParametersHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);
            var clientKeyPair = KeyParametersHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);

            //
            // 密钥读写测试。
            //
            string publicKeyPem = PemHelper.Key2Pem(caKeyPair.Public);
            string privateKeyPem = PemHelper.Key2Pem(caKeyPair.Private);
            string privateKeyEncPem = PemHelper.Key2Pem(caKeyPair.Private, NamedPemEncryptionAlgorithms.AES_256_CBC, "123456");
            //
            _ = PemHelper.Pem2Key(publicKeyPem);
            caKeyPair = PemHelper.Pem2KeyPair(privateKeyPem);
            caKeyPair = PemHelper.Pem2KeyPair(privateKeyEncPem, "123456");
            //
            // ===========================  证书测试  ===========================
            //
            X509NameGenerator nameGenerator = new X509NameGenerator();
            nameGenerator.AddX509Name(X509Name.C, "CN");
            nameGenerator.AddX509Name(X509Name.CN, "LH.Net.Sockets TEST Root CA");
            var caDN = nameGenerator.Generate();
            nameGenerator.Reset();
            nameGenerator.AddX509Name(X509Name.C, "CN");
            nameGenerator.AddX509Name(X509Name.CN, "LH.Net.Sockets TEST TCP Server");
            var serverDN = nameGenerator.Generate();
            nameGenerator.Reset();
            nameGenerator.AddX509Name(X509Name.C, "CN");
            nameGenerator.AddX509Name(X509Name.CN, "LH.Net.Sockets TEST TCP Client");
            var clientDN = nameGenerator.Generate();
            //
            // 机构证书。
            //
            var caCert = CertificateHelper.GenerateIssuerCert(NamedSignatureAlgorithms.SHA256_WITH_ECDSA,
                                                              caKeyPair,
                                                              caDN,
                                                              null,
                                                              DateTime.UtcNow.AddDays(-1),
                                                              365);
            //
            // P12 证书。
            //
            //
            var namedCerts = new Dictionary<string, X509Certificate>() { { "CERT_1", caCert } };
            var store = CertificateHelper.GeneratePfx("KEY", caKeyPair.Private, namedCerts, "123456");
            var pub = store.GetCertificate("CERT_1").Certificate.GetPublicKey();
            var pri = store.GetKey("KEY").Key;
            _ = store.GetCertificateChain("KEY");
            caKeyPair = new AsymmetricCipherKeyPair(pub, pri);
            //
            // 使用者证书请求。
            //
            X509ExtensionsGenerator extensionsGenerator = new X509ExtensionsGenerator();
            extensionsGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
            extensionsGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.KeyCertSign | KeyUsage.CrlSign));
            var serverExtensions = extensionsGenerator.Generate();
            var clientExtensions = serverExtensions;
            var serverCsr = CertificateHelper.GenerateCsr(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, serverKeyPair, serverDN, serverExtensions);
            var clientCsr = CertificateHelper.GenerateCsr(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, clientKeyPair, clientDN, clientExtensions);
            //
            // 证书请求读写测试。
            //
            string serverCsrPem = PemHelper.Csr2Pem(serverCsr);
            string clientCsrPem = PemHelper.Csr2Pem(clientCsr);
            serverCsr = PemHelper.Pem2Csr(serverCsrPem);
            clientCsr = PemHelper.Pem2Csr(clientCsrPem);
            //
            // 使用者证书。
            //
            CertificateHelper.ExtractCsr(serverCsr, out serverDN, out AsymmetricKeyParameter serverPublicKey, out serverExtensions);
            CertificateHelper.ExtractCsr(clientCsr, out clientDN, out AsymmetricKeyParameter clientPublicKey, out clientExtensions);
            var serverCert = CertificateHelper.GenerateSubjectCert(caCert.CertificateStructure.SignatureAlgorithm.Algorithm.Id,
                                                                   caKeyPair.Private,
                                                                   caCert,
                                                                   serverDN,
                                                                   serverPublicKey,
                                                                   serverExtensions,
                                                                   DateTime.UtcNow.AddDays(-1),
                                                                   90);
            var clientCert = CertificateHelper.GenerateSubjectCert(caCert.CertificateStructure.SignatureAlgorithm.Algorithm.Id,
                                                                   caKeyPair.Private,
                                                                   caCert,
                                                                   clientDN,
                                                                   clientPublicKey,
                                                                   clientExtensions,
                                                                   DateTime.UtcNow.AddDays(-1),
                                                                   90);
            //
            // 证书读写测试。
            //
            string caCertPem = PemHelper.Cert2Pem(caCert);
            string serverCertPem = PemHelper.Cert2Pem(serverCert);
            string clientCertPem = PemHelper.Cert2Pem(clientCert);
            caCert = PemHelper.Pem2Cert(caCertPem);
            serverCert = PemHelper.Pem2Cert(serverCertPem);
            clientCert = PemHelper.Pem2Cert(clientCertPem);
            //
            // 打印证书。
            //
            Console.WriteLine("===========================  CA Cert  ===========================");
            Console.WriteLine(caCert.ToString());
            Console.WriteLine("=========================  Server Cert  =========================");
            Console.WriteLine(serverCert.ToString());
            Console.WriteLine("=========================  Client Cert  =========================");
            Console.WriteLine(clientCert.ToString());
            Console.WriteLine();
            //
            // BouncyCastle 证书验证。
            //
            bool validated;
            try
            {
                serverCert.Verify(caCert.GetPublicKey());
                validated = true;
            }
            catch
            {
                validated = false;
            }
            Console.WriteLine("BouncyCastle verify server cert - " + validated);
            try
            {
                clientCert.Verify(caCert.GetPublicKey());
                validated = true;
            }
            catch
            {
                validated = false;
            }
            Console.WriteLine("BouncyCastle verify client cert - " + validated);
            //
            // Microsoft 证书链验证。
            //
            var caCertMS = new MicrosoftX509.X509Certificate2(caCert.GetEncoded());
            var serverCertMS = new MicrosoftX509.X509Certificate2(serverCert.GetEncoded());
            var clientCertMS = new MicrosoftX509.X509Certificate2(clientCert.GetEncoded());
            var chain = new MicrosoftX509.X509Chain();
            chain.ChainPolicy.RevocationMode = MicrosoftX509.X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = MicrosoftX509.X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.ExtraStore.Add(caCertMS);
            Console.WriteLine("Microsoft chain verify server cert - " + chain.Build(serverCertMS));
            Console.WriteLine("Microsoft chain verify client cert - " + chain.Build(clientCertMS));
            (caCertMS as IDisposable).Dispose();
            (serverCertMS as IDisposable).Dispose();
            (clientCertMS as IDisposable).Dispose();
            (chain as IDisposable).Dispose();
            //
            // =============================  签名  =============================
            //
            var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==========================  Signature  ==========================");
            var signature = SignatureHelper.Sign(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, serverKeyPair.Private, data);
            validated = SignatureHelper.Verify(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, serverCert.GetPublicKey(), data, signature);
            Console.WriteLine("Server terminal verify signature - " + validated);
            signature = SignatureHelper.Sign(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, clientKeyPair.Private, data);
            validated = SignatureHelper.Verify(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, clientCert.GetPublicKey(), data, signature);
            Console.WriteLine("Client terminal verify signature - " + validated);
            //
            // ==========================  非对称加密  ==========================
            //
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("===================  Asymmetric encryption  =====================");
            var rsaKeyPair = KeyParametersHelper.GenerateRsaKeyPair(2048);
            var testBytes = new byte[400];
            Common.SecureRandom.NextBytes(testBytes);
            var encBytes = EncryptionHelper.AsymmetricEncrypt(NamedAsymmetricCipherStrings.RSA_OAEP, rsaKeyPair.Public, testBytes);
            var decBytes = EncryptionHelper.AsymmetricDecrypt(NamedAsymmetricCipherStrings.RSA_OAEP, rsaKeyPair.Private, encBytes);
            Console.WriteLine("Original hash - ");
            Console.WriteLine(BitConverter.ToString(HashHelper.ComputeHash(NamedHashAlgorithms.SHA3_256, testBytes)).Replace("-", string.Empty));
            Console.WriteLine("Decrypted hash  - ");
            Console.WriteLine(BitConverter.ToString(HashHelper.ComputeHash(NamedHashAlgorithms.SHA3_256, decBytes)).Replace("-", string.Empty));
            //
            // ===========================  密钥交换  ===========================
            //
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("=========================  KeyExchange  =========================");
            var parametersA = KeyExchangeHelper.CreateParametersA(256);
            var agreementA = KeyExchangeHelper.CreateAgreement(parametersA, out AsymmetricKeyParameter publicKeyA);
            var parametersB = KeyExchangeHelper.CreateParametersB(parametersA.P, parametersA.G);
            var agreementB = KeyExchangeHelper.CreateAgreement(parametersB, out AsymmetricKeyParameter publicKeyB);
            var pmsA = agreementA.CalculateAgreement(publicKeyB);
            var pmsB = agreementB.CalculateAgreement(publicKeyA);
            Console.WriteLine("Key exchange alice's pms - ");
            Console.WriteLine(pmsA);
            Console.WriteLine("Key exchange bob's pms - ");
            Console.WriteLine(pmsB);
            //
            // ===========================  对称加密  ===========================
            //
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("====================  Symmetric encryption  =====================");
            var pms = pmsA.ToByteArrayUnsigned();
            var symmetricKey = new byte[128 / 8];
            var symmetricIV = new byte[128 / 8];
            Array.Copy(pms, 0, symmetricKey, 0, symmetricKey.Length);
            Array.Copy(pms, 0, symmetricIV, 0, symmetricIV.Length);
            var symmetricKeyParameters = KeyParametersHelper.GenerateSymmetricKey(symmetricKey, symmetricIV);
            testBytes = new byte[1422];
            Common.SecureRandom.NextBytes(testBytes);
            encBytes = EncryptionHelper.SymmetricEncrypt(NamedSymmetricCipherStrings.SM4_CBC_PKCS7, symmetricKeyParameters, testBytes);
            decBytes = EncryptionHelper.SymmetricDecrypt(NamedSymmetricCipherStrings.SM4_CBC_PKCS7, symmetricKeyParameters, encBytes);
            Console.WriteLine("Original hash - ");
            Console.WriteLine(BitConverter.ToString(HashHelper.ComputeHash(NamedHashAlgorithms.SM3, testBytes)).Replace("-", string.Empty));
            Console.WriteLine("Decrypted hash  - ");
            Console.WriteLine(BitConverter.ToString(HashHelper.ComputeHash(NamedHashAlgorithms.SM3, decBytes)).Replace("-", string.Empty));
            //
            //
            //
            Console.ReadKey(true);
        }
    }
}