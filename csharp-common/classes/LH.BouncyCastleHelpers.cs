/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2019. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

/*
 * PackageReference: Portable.BouncyCastle 1.8.5+
 */

/*
 * 签名算法名称 signatureAlgorithmName 的补充说明
 *
 * 可使用 PkcsObjectIdentifiers、X9ObjectIdentifiers、NistObjectIdentifiers 中的签名相关的算法 OID。
 * 也可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。
 * 名称根据签名私钥类型选择。如 RSA 密钥可使用 SHA256WithRSA、SHA512WithRSA。
 *
 * 部分算法名称不能被 Microsoft 证书链验证。如 SHA224WithRSA（1.2.840.113549.1.1.14）。参见 http://oidref.com 关于算法的 Description。
 *
 * BUG: 至 BouncyCastle 1.8.6， SHA256WithECDSA 指向 SHA224WithECDSA。传入 OID （1.2.840.10045.4.3.2）可使用正确的 SHA256WithECDSA 算法。
 */

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace LH.BouncyCastleHelpers
{
    /// <summary>
    /// 证书辅助。
    /// </summary>
    internal static class CertificateHelper
    {
        #region 证书请求

        /// <summary>
        /// 创建证书请求。
        /// </summary>
        /// <param name="subjectPrivateKeyPair">使用者私钥密钥对。</param>
        /// <param name="signatureAlgorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="subjectDN">使用者 DN。</param>
        /// <param name="attributes">附加属性。</param>
        /// <returns></returns>
        [SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        internal static Pkcs10CertificationRequest GenerateCsr(AsymmetricCipherKeyPair subjectPrivateKeyPair,
                                                               string signatureAlgorithmName,
                                                               X509Name subjectDN,
                                                               Asn1Set attributes)
        {
            ISignatureFactory signatureFactory;
            try
            {
                var signatureAlgorithm = new DefaultSignatureAlgorithmIdentifierFinder().Find(signatureAlgorithmName);
                signatureFactory = new Asn1SignatureFactory(signatureAlgorithm.Algorithm.Id, subjectPrivateKeyPair.Private, Common.SecureRandom);
            }
            catch
            {
                signatureFactory = new Asn1SignatureFactory(signatureAlgorithmName, subjectPrivateKeyPair.Private, Common.SecureRandom);
            }
            return new Pkcs10CertificationRequest(signatureFactory, subjectDN, subjectPrivateKeyPair.Public, attributes);
        }

        /// <summary>
        /// 将证书请求转换为 PEM 格式文本。
        /// </summary>
        /// <param name="csr">证书请求。</param>
        /// <param name="pem">PEM 格式文本。</param>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static void ParseCsr(Pkcs10CertificationRequest csr, out string pem)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(csr);
                pem = writer.ToString();
            }
        }

        /// <summary>
        /// 从 PEM 格式文本读取证书请求。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static Pkcs10CertificationRequest ReadCsr(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as Pkcs10CertificationRequest;
            }
        }

        #endregion 证书请求

        #region 证书

        /// <summary>
        /// 创建 DN。
        /// </summary>
        /// <param name="attributes">DN 属性集合。</param>
        /// <returns></returns>
        internal static X509Name GenerateDN(Dictionary<DerObjectIdentifier, string> attributes)
        {
            var ordering = new DerObjectIdentifier[attributes.Count];
            attributes.Keys.CopyTo(ordering, 0);
            return new X509Name(ordering, attributes);
        }

        /// <summary>
        /// 创建颁发机构自签名证书。
        /// </summary>
        /// <param name="keyPair">密钥对。</param>
        /// <param name="signatureAlgorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="dn">颁发机构 DN。</param>
        /// <param name="extensions">扩展属性。</param>
        /// <param name="start">启用时间。</param>
        /// <param name="days">从启用时间开始的有效天数。</param>
        /// <returns></returns>
        internal static X509Certificate GenerateIssuerCert(AsymmetricCipherKeyPair keyPair,
                                                           string signatureAlgorithmName,
                                                           X509Name dn,
                                                           X509Extensions extensions,
                                                           DateTime start,
                                                           int days)
        {
            return GenerateCert(keyPair.Private, signatureAlgorithmName, dn, keyPair.Public, dn, extensions, start, days);
        }

        /// <summary>
        /// 创建使用者证书。
        /// </summary>
        /// <param name="issuerPrivateKey">颁发机构的私钥。</param>
        /// <param name="signatureAlgorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="issuerCert">颁发机构的证书。</param>
        /// <param name="subjectCsr">使用者证书请求。</param>
        /// <param name="extensions">扩展属性。</param>
        /// <param name="start">启用时间。</param>
        /// <param name="days">从启用时间开始的有效天数。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        [SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        internal static X509Certificate GenerateSubjectCert(AsymmetricKeyParameter issuerPrivateKey,
                                                            string signatureAlgorithmName,
                                                            X509Certificate issuerCert,
                                                            Pkcs10CertificationRequest subjectCsr,
                                                            X509Extensions extensions,
                                                            DateTime start,
                                                            int days)
        {
            try
            {
                issuerCert.CheckValidity();
            }
            catch
            {
                throw new ArgumentException("颁发机构证书已过期。", nameof(issuerCert));
            }
            //try
            //{
            //    issuerCert.CheckValidity(start.AddDays(days));
            //}
            //catch
            //{
            //    throw new ArgumentException("签署的有效期超出了颁发机构证书的有效期。", nameof(issuerCert));
            //}
            var csrInfo = subjectCsr.GetCertificationRequestInfo();
            var subjectPublicKey = subjectCsr.GetPublicKey();
            if (!SignatureHelper.Verify(subjectPublicKey, subjectCsr.SignatureAlgorithm.Algorithm.Id, csrInfo.GetEncoded(), subjectCsr.GetSignatureOctets()))
            {
                throw new ArgumentException("证书请求的签名异常。", nameof(subjectCsr));
            }
            return GenerateCert(issuerPrivateKey, signatureAlgorithmName, issuerCert.SubjectDN, subjectPublicKey, csrInfo.Subject, extensions, start, days);
        }

        /// <summary>
        /// 将证书转换为 PEM 格式文本。
        /// </summary>
        /// <param name="cert">证书。</param>
        /// <param name="pem">PEM 格式文本。</param>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static void ParseCert(X509Certificate cert, out string pem)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(cert);
                pem = writer.ToString();
            }
        }

        /// <summary>
        /// 从 PEM 格式文本读取证书。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static X509Certificate ReadCert(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as X509Certificate;
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<挂起>")]
        private static X509Certificate GenerateCert(AsymmetricKeyParameter issuerPrivateKey,
                                                    string signatureAlgorithmName,
                                                    X509Name issuerDN,
                                                    AsymmetricKeyParameter subjectPublicKey,
                                                    X509Name subjectDN,
                                                    X509Extensions extensions,
                                                    DateTime start,
                                                    int days)
        {
            //var issuerPrivateKeyKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(issuerPrivateKey);
            //var issuerPrivateKeyAlgorithm = issuerPrivateKeyKeyInfo.PrivateKeyAlgorithm;
            //var subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectPublicKey);
            //var subjectPublicKeyAlgorithm = issuerPrivateKeyKeyInfo.PrivateKeyAlgorithm;
            //if (issuerPrivateKeyAlgorithm.Parameters.Equals(GMObjectIdentifiers.sm2p256v1))
            //{
            //    signatureAlgorithmName = "SM3WithSM2";
            //}
            var sn = new BigInteger(128, Common.SecureRandom);
            ISignatureFactory signatureFactory;
            try
            {
                var signatureAlgorithm = new DefaultSignatureAlgorithmIdentifierFinder().Find(signatureAlgorithmName);
                signatureFactory = new Asn1SignatureFactory(signatureAlgorithm.Algorithm.Id, issuerPrivateKey, Common.SecureRandom);
            }
            catch
            {
                signatureFactory = new Asn1SignatureFactory(signatureAlgorithmName, issuerPrivateKey, Common.SecureRandom);
            }
            var generator = new X509V3CertificateGenerator();
            generator.SetSerialNumber(sn);
            generator.SetIssuerDN(issuerDN);
            generator.SetSubjectDN(subjectDN);
            generator.SetNotBefore(start);
            generator.SetNotAfter(start.AddDays(days));
            generator.SetPublicKey(subjectPublicKey);
            if (extensions != null)
            {
                foreach (DerObjectIdentifier oid in extensions.ExtensionOids)
                {
                    X509Extension extension = extensions.GetExtension(oid);
                    generator.AddExtension(oid, extension.IsCritical, extension.Value);
                }
            }
            return generator.Generate(signatureFactory);
        }

        #endregion 证书

        #region P12 证书

        /// <summary>
        /// 创建 P12 证书。
        /// </summary>
        /// <param name="chainAlias">证书链和私钥的别名。</param>
        /// <param name="privateKey">私钥。</param>
        /// <param name="namedCerts">设置了别名的证书集合。</param>
        /// <param name="password">设置密码。</param>
        /// <param name="output">输出到流。</param>
        internal static void GeneratePfx(string chainAlias,
                                         AsymmetricKeyParameter privateKey,
                                         IDictionary<string, X509Certificate> namedCerts,
                                         string password,
                                         Stream output)
        {
            var store = new Pkcs12StoreBuilder().Build();
            var certEntries = new List<X509CertificateEntry>();
            foreach (var namedCert in namedCerts)
            {
                var certEntry = new X509CertificateEntry(namedCert.Value);
                store.SetCertificateEntry(namedCert.Key, certEntry);
                certEntries.Add(certEntry);
            }
            store.SetKeyEntry(chainAlias, new AsymmetricKeyEntry(privateKey), certEntries.ToArray());
            var pass = string.IsNullOrEmpty(password) ? null : password.ToCharArray();
            store.Save(output, pass, Common.SecureRandom);
            output.Flush();
        }

        /// <summary>
        /// 读取 P12 证书。
        /// </summary>
        /// <param name="input">从流中读取。</param>
        /// <param name="password">设置密码。</param>
        internal static Pkcs12Store ReadPfx(Stream input, string password)
        {
            var pass = string.IsNullOrEmpty(password) ? null : password.ToCharArray();
            return new Pkcs12Store(input, pass);
        }

        #endregion P12 证书
    }

    /// <summary>
    /// 全局对象。
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// 随机数生成器。
        /// </summary>
        internal static SecureRandom SecureRandom { get; } = new SecureRandom();
    }

    /// <summary>
    /// 密钥辅助。
    /// </summary>
    internal static class CryptoHelper
    {
        /// <summary>
        /// 创建 ECDSA 密钥对。
        /// </summary>
        /// <param name="curveName">使用的曲线名称。可使用 NamedCurves 类中提供的常用算法名称。或 SecObjectIdentifiers 中的命名曲线。</param>
        /// <returns></returns>
        internal static AsymmetricCipherKeyPair GenerateEcdsaKeyPair(string curveName)
        {
            var x9 = SecNamedCurves.GetByName(curveName);
            var domain = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H);
            var kgp = new ECKeyGenerationParameters(domain, Common.SecureRandom);
            var generator = new ECKeyPairGenerator();
            generator.Init(kgp);
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// 创建 RSA 密钥对。
        /// </summary>
        /// <param name="keySize">密钥长度。必须大于等于 2048且是 64 的倍数。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize)
        {
            if (keySize < 2048)
            {
                throw new Exception("密钥长度必须大于等于 2048。");
            }
            if (keySize % 64 != 0)
            {
                throw new Exception("密钥长度必须是 64 的倍数。");
            }
            var kgp = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), Common.SecureRandom, keySize, 25);
            var generator = new RsaKeyPairGenerator();
            generator.Init(kgp);
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// 创建 SM2 密钥对。
        /// </summary>
        /// <returns></returns>
        internal static AsymmetricCipherKeyPair GenerateSM2KeyPair()
        {
            var x9 = GMNamedCurves.GetByName("sm2p256v1");
            var domain = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H);
            var kgp = new ECKeyGenerationParameters(domain, Common.SecureRandom);
            var generator = new ECKeyPairGenerator();
            generator.Init(kgp);
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// 将密钥转换为 PEM 格式文本。
        /// </summary>
        /// <param name="key">密钥。</param>
        /// <param name="pem">PEM 格式文本。</param>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static void ParseKey(AsymmetricKeyParameter key, out string pem)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(key);
                pem = writer.ToString();
            }
        }

        /// <summary>
        /// 使用密码保护，将私钥转换为 PEM 格式文本。
        /// </summary>
        /// <param name="privateKey">私钥。</param>
        /// <param name="encryptionAlgorithm">加密算法。可使用 NamedPemEncryptionAlgorithms 类中提供的常用算法名称。名称是 OpenSSL 定义的 DEK 格式。</param>
        /// <param name="password">设置密码。</param>
        /// <param name="privateKeyEncPem">带加密标签的 PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static void ParseKey(AsymmetricKeyParameter privateKey, string encryptionAlgorithm, string password, out string privateKeyEncPem)
        {
            if (privateKey.IsPrivate)
            {
                using (var writer = new StringWriter())
                {
                    var pemWriter = new PemWriter(writer);
                    pemWriter.WriteObject(privateKey, encryptionAlgorithm, password.ToCharArray(), Common.SecureRandom);
                    privateKeyEncPem = writer.ToString();
                }
            }
            else
            {
                throw new ArgumentException("不是私钥。", nameof(privateKey));
            }
        }

        /// <summary>
        /// 从 PEM 格式文本读取密钥对。
        /// </summary>
        /// <param name="privateKeyPem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static AsymmetricCipherKeyPair ReadKeyPair(string privateKeyPem)
        {
            using (var reader = new StringReader(privateKeyPem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as AsymmetricCipherKeyPair;
            }
        }

        /// <summary>
        /// 从密码保护的 PEM 格式文本读取密钥对。
        /// </summary>
        /// <param name="privateKeyEncPem">带加密标签的 PEM 格式文本。</param>
        /// <param name="password">设置密码。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static AsymmetricCipherKeyPair ReadKeyPair(string privateKeyEncPem, string password)
        {
            using (var reader = new StringReader(privateKeyEncPem))
            {
                var obj = new PemReader(reader, new Password(password)).ReadObject();
                return obj as AsymmetricCipherKeyPair;
            }
        }

        /// <summary>
        /// 从 PEM 格式文本读取公钥。
        /// </summary>
        /// <param name="publicKeyPem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static AsymmetricKeyParameter ReadPublicKey(string publicKeyPem)
        {
            using (var reader = new StringReader(publicKeyPem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as AsymmetricKeyParameter;
            }
        }
    }

    /// <summary>
    /// 密钥交换辅助。
    /// </summary>
    internal static class KeyExchangeHelper
    {
        /// <summary>
        /// 创建密钥交换协议，并输出公钥。
        /// </summary>
        /// <param name="parameters">曲线参数。</param>
        /// <param name="publicKey">用于传递给对方的公钥。</param>
        /// <returns></returns>
        internal static IBasicAgreement CreateAgreement(DHParameters parameters, out AsymmetricKeyParameter publicKey)
        {
            var generator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            var kgp = new DHKeyGenerationParameters(Common.SecureRandom, parameters);
            generator.Init(kgp);
            var keyPair = generator.GenerateKeyPair();
            var agreement = AgreementUtilities.GetBasicAgreement("ECDH");
            agreement.Init(keyPair.Private);
            publicKey = keyPair.Public;
            return agreement;
        }

        /// <summary>
        /// 创建 Alice 曲线参数。
        /// </summary>
        /// <param name="keySize">密钥长度。</param>
        /// <returns></returns>
        internal static DHParameters CreateParametersA(int keySize)
        {
            var generator = new DHParametersGenerator();
            generator.Init(keySize, 25, Common.SecureRandom);
            return generator.GenerateParameters();
        }

        /// <summary>
        /// 以 Alice 曲线参数为参照，创建 Bob 曲线参数。
        /// </summary>
        /// <param name="aP">Alice 曲线参数的 P 值。</param>
        /// <param name="aG">Alice 曲线参数的 G 值。</param>
        /// <returns></returns>
        internal static DHParameters CreateParametersB(BigInteger aP, BigInteger aG)
        {
            return new DHParameters(aP, aG);
        }
    }

    /// <summary>
    /// 已命名曲线。
    /// </summary>
    internal static class NamedCurves
    {
        internal const string SECP256R1 = "secp256r1";
        internal const string SECP384R1 = "secp384r1";
        internal const string SECP521R1 = "secp521r1";
    }

    /// <summary>
    /// 已命名 PEM 加密算法。OpenSSL 定义的 DEK 格式。
    /// </summary>
    internal static class NamedPemEncryptionAlgorithms
    {
        internal const string AES_128_CBC = "AES-128-CBC";
        internal const string AES_128_ECB = "AES-128-ECB";
        internal const string AES_192_CBC = "AES-192-CBC";
        internal const string AES_192_ECB = "AES-192-ECB";
        internal const string AES_256_CBC = "AES-256-CBC";
        internal const string AES_256_ECB = "AES-256-ECB";
        internal const string DES_EDE3_CBC = "DES-EDE3-CBC";
        internal const string DES_EDE3_ECB = "DES-EDE3-ECB";
    }

    /// <summary>
    /// 已命名签名算法。
    /// </summary>
    internal static class NamedSignatureAlgorithms
    {
        /// <summary>
        /// BUG: SHA256WithECDSA 名称指向 SHA224WithECDSA。使用 OID 名称可正确签名。
        /// </summary>
        internal const string SHA256_WITH_ECDSA = "1.2.840.10045.4.3.2";

        internal const string SHA256_WITH_RSA = "SHA256WithRSA";
        internal const string SHA384_WITH_ECDSA = "SHA384WithECDSA";
        internal const string SHA384_WITH_RSA = "SHA384WithRSA";
        internal const string SHA512_WITH_ECDSA = "SHA512WithECDSA";
        internal const string SHA512_WITH_RSA = "SHA512WithRSA";
        internal const string SM3_WITH_SM2 = "SM3WithSM2";
    }

    /// <summary>
    /// 签名辅助。
    /// </summary>
    internal static class SignatureHelper
    {
        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="signatureAlgorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <returns></returns>
        internal static byte[] Sign(AsymmetricKeyParameter privateKey, string signatureAlgorithmName, byte[] data)
        {
            return Sign(privateKey, signatureAlgorithmName, data, 0, data.Length);
        }

        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="signatureAlgorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] Sign(AsymmetricKeyParameter privateKey, string signatureAlgorithmName, byte[] buffer, int offset, int count)
        {
            var signer = SignerUtilities.GetSigner(signatureAlgorithmName);
            signer.Init(true, privateKey);
            signer.BlockUpdate(buffer, offset, count);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="publicKey">从对方证书中导出的公钥。</param>
        /// <param name="signatureAlgorithmName">对方协商的签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(AsymmetricKeyParameter publicKey, string signatureAlgorithmName, byte[] data, byte[] signature)
        {
            return Verify(publicKey, signatureAlgorithmName, data, 0, data.Length, signature);
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="publicKey">从对方证书中导出的公钥。</param>
        /// <param name="signatureAlgorithmName">对方协商的签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法名称。参见注释中的补充说明。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(AsymmetricKeyParameter publicKey, string signatureAlgorithmName, byte[] buffer, int offset, int count, byte[] signature)
        {
            var verifier = SignerUtilities.GetSigner(signatureAlgorithmName);
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(buffer, offset, count);
            return verifier.VerifySignature(signature);
        }
    }

    /// <summary></summary>
    internal sealed class Password : IPasswordFinder
    {
        private readonly char[] _chars;

        internal Password(string password)
        {
            _chars = password.ToCharArray();
        }

        /// <summary></summary>
        /// <returns></returns>
        public char[] GetPassword()
        {
            return _chars;
        }
    }
}