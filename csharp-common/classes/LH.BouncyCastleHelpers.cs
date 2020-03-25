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
 * 签名算法的补充说明
 *
 * 可使用 PkcsObjectIdentifiers、X9ObjectIdentifiers、NistObjectIdentifiers、GMObjectIdentifiers 中的签名相关的算法 oid。
 * 也可使用 CommonSignatureAlgorithms 类中提供的常用算法。
 * 根据签名私钥类型选择。如 RSA 密钥可使用 SHA256WithRSA、SHA512WithRSA。
 *
 * 部分算法不能被 Microsoft 证书链验证。如 SHA224WithRSA（1.2.840.113549.1.1.14）。参见 http://oidref.com 关于算法的 Description。
 *
 * BUG: 使用签名算法查询类 DefaultSignatureAlgorithmIdentifierFinder 时 SHA256WithECDSA 指向 SHA224WithECDSA。
 *      若使用此方法获取 oid 应注意修正。
 *      BouncyCastle 1.8.6 尚未修复此 BUG。
 */

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
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
        /// 取出证书请求中的设置。
        /// </summary>
        /// <param name="csr">使用者证书请求。</param>
        /// <param name="dn">使用者 DN。</param>
        /// <param name="publicKey">使用者公钥。</param>
        /// <param name="extensions">使用者扩展属性。</param>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        [SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        internal static void ExtractCsr(Pkcs10CertificationRequest csr, out X509Name dn, out AsymmetricKeyParameter publicKey, out X509Extensions extensions)
        {
            var csrInfo = csr.GetCertificationRequestInfo();
            publicKey = csr.GetPublicKey();
            if (!csr.Verify(publicKey))
            {
                throw new ArgumentException("证书请求的签名异常。", nameof(csr));
            }
            //
            dn = csrInfo.Subject;
            //
            var attributes = new Dictionary<DerObjectIdentifier, X509Extension>();
            if (csrInfo.Attributes != null)
            {
                foreach (DerSequence attribute in csrInfo.Attributes)
                {
                    var oid = attribute[0] as DerObjectIdentifier;
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest))
                    {
                        var set = attribute[1] as Asn1Set;
                        foreach (DerSequence exts in set)
                        {
                            foreach (DerSequence ext in exts)
                            {
                                oid = ext[0] as DerObjectIdentifier;
                                if (ext.Count == 3)
                                {
                                    attributes.Add(oid, new X509Extension(true, ext[2] as DerOctetString));
                                }
                                else
                                {
                                    attributes.Add(oid, new X509Extension(false, ext[1] as DerOctetString));
                                }
                            }
                        }
                    }
                }
            }
            extensions = attributes.Count > 0 ? new X509Extensions(attributes) : null;
        }

        /// <summary>
        /// 创建证书请求。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="keyPair">使用者密钥对。</param>
        /// <param name="dn">使用者 DN。</param>
        /// <param name="extensions">使用者扩展属性。</param>
        /// <returns></returns>
        internal static Pkcs10CertificationRequest GenerateCsr(DerObjectIdentifier signatureAlgorithmOid, AsymmetricCipherKeyPair keyPair, X509Name dn, X509Extensions extensions)
        {
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithmOid.Id, keyPair.Private, Common.SecureRandom);
            var set = new DerSet(extensions);
            var attribute = new DerSet(new AttributePkcs(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest, set));
            return new Pkcs10CertificationRequest(signatureFactory, dn, keyPair.Public, attribute);
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
        /// 创建颁发机构自签名证书。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="keyPair">颁发机构密钥对。</param>
        /// <param name="dn">颁发机构 DN。</param>
        /// <param name="extensions">扩展属性。</param>
        /// <param name="start">启用时间。</param>
        /// <param name="days">从启用时间开始的有效天数。</param>
        /// <returns></returns>
        internal static X509Certificate GenerateIssuerCert(DerObjectIdentifier signatureAlgorithmOid,
                                                           AsymmetricCipherKeyPair keyPair,
                                                           X509Name dn,
                                                           X509Extensions extensions,
                                                           DateTime start,
                                                           int days)
        {
            return GenerateCert(signatureAlgorithmOid, keyPair.Private, dn, dn, keyPair.Public, extensions, start, days);
        }

        /// <summary>
        /// 创建使用者证书。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="issuerPrivateKey">颁发机构的私钥。</param>
        /// <param name="issuerCert">颁发机构的证书。</param>
        /// <param name="subjectDN">使用者 DN。</param>
        /// <param name="subjectPublicKey">使用者公钥。</param>
        /// <param name="extensions">扩展属性。</param>
        /// <param name="start">启用时间。</param>
        /// <param name="days">从启用时间开始的有效天数。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        [SuppressMessage("Design", "CA1031:不捕获常规异常类型", Justification = "<挂起>")]
        internal static X509Certificate GenerateSubjectCert(DerObjectIdentifier signatureAlgorithmOid,
                                                            AsymmetricKeyParameter issuerPrivateKey,
                                                            X509Certificate issuerCert,
                                                            X509Name subjectDN,
                                                            AsymmetricKeyParameter subjectPublicKey,
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
            return GenerateCert(signatureAlgorithmOid, issuerPrivateKey, issuerCert.SubjectDN, subjectDN, subjectPublicKey, extensions, start, days);
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

        private static X509Certificate GenerateCert(DerObjectIdentifier signatureAlgorithmOid,
                                                    AsymmetricKeyParameter issuerPrivateKey,
                                                    X509Name issuerDN,
                                                    X509Name subjectDN,
                                                    AsymmetricKeyParameter subjectPublicKey,
                                                    X509Extensions extensions,
                                                    DateTime start,
                                                    int days)
        {
            var sn = new BigInteger(128, Common.SecureRandom);
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithmOid.Id, issuerPrivateKey, Common.SecureRandom);
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
                    generator.AddExtension(oid, extension.IsCritical, extension.GetParsedValue());
                }
            }
            return generator.Generate(signatureFactory);
        }

        #endregion 证书

        #region P12 证书

        /// <summary>
        /// 创建 P12 证书。
        /// </summary>
        /// <param name="chainAlias">私钥的别名。</param>
        /// <param name="privateKey">私钥。</param>
        /// <param name="namedCerts">设置了别名的证书集合。</param>
        /// <param name="password">设置密码。</param>
        /// <param name="output">输出到流。</param>
        internal static void GeneratePfx(string keyAlias,
                                         AsymmetricKeyParameter privateKey,
                                         Dictionary<string, X509Certificate> namedCerts,
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
            store.SetKeyEntry(keyAlias, new AsymmetricKeyEntry(privateKey), certEntries.ToArray());
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
    /// 常用曲线。
    /// </summary>
    internal static class CommonCurves
    {
        internal static DerObjectIdentifier SecP256r1 { get; } = SecObjectIdentifiers.SecP256r1;
        internal static DerObjectIdentifier SecP384r1 { get; } = SecObjectIdentifiers.SecP384r1;
        internal static DerObjectIdentifier SecP521r1 { get; } = SecObjectIdentifiers.SecP521r1;
    }

    /// <summary>
    /// 常用签名算法。
    /// </summary>
    internal static class CommonSignatureAlgorithms
    {
        internal static DerObjectIdentifier SHA256WithDSA { get; } = NistObjectIdentifiers.DsaWithSha256;
        internal static DerObjectIdentifier SHA256WithECDSA { get; } = X9ObjectIdentifiers.ECDsaWithSha256;
        internal static DerObjectIdentifier SHA256WithRSA { get; } = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
        internal static DerObjectIdentifier SHA384WithDSA { get; } = NistObjectIdentifiers.DsaWithSha384;
        internal static DerObjectIdentifier SHA384WithECDSA { get; } = X9ObjectIdentifiers.ECDsaWithSha384;
        internal static DerObjectIdentifier SHA384WithRSA { get; } = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
        internal static DerObjectIdentifier SHA512WithDSA { get; } = NistObjectIdentifiers.DsaWithSha512;
        internal static DerObjectIdentifier SHA512WithECDSA { get; } = X9ObjectIdentifiers.ECDsaWithSha512;
        internal static DerObjectIdentifier SHA512WithRSA { get; } = PkcsObjectIdentifiers.Sha512WithRsaEncryption;
        internal static DerObjectIdentifier SM3WithSM2 { get; } = GMObjectIdentifiers.sm2sign_with_sm3;
    }

    /// <summary>
    /// 密钥辅助。
    /// </summary>
    internal static class CryptoHelper
    {
        /// <summary>
        /// 创建 ECDSA 密钥对。
        /// </summary>
        /// <param name="curveOid">曲线 oid。可使用 CommonCurves 类中提供的常用曲线。或 SecObjectIdentifiers 中的命名曲线。</param>
        /// <returns></returns>
        internal static AsymmetricCipherKeyPair GenerateEcdsaKeyPair(DerObjectIdentifier curveOid)
        {
            var ec = SecNamedCurves.GetByOid(curveOid);
            var domain = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
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
            var ec = GMNamedCurves.GetByOid(GMObjectIdentifiers.sm2p256v1);
            var domain = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
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
    /// 签名辅助。
    /// </summary>
    internal static class SignatureHelper
    {
        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="algorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <returns></returns>
        internal static byte[] Sign(DerObjectIdentifier algorithmOid, AsymmetricKeyParameter privateKey, byte[] data)
        {
            return Sign(algorithmOid, privateKey, data, 0, data.Length);
        }

        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="algorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] Sign(DerObjectIdentifier algorithmOid, AsymmetricKeyParameter privateKey, byte[] buffer, int offset, int count)
        {
            var signer = SignerUtilities.GetSigner(algorithmOid);
            signer.Init(true, privateKey);
            signer.BlockUpdate(buffer, offset, count);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="algorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(DerObjectIdentifier algorithmOid, AsymmetricKeyParameter publicKey, byte[] data, byte[] signature)
        {
            return Verify(algorithmOid, publicKey, data, 0, data.Length, signature);
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="algorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(DerObjectIdentifier algorithmOid, AsymmetricKeyParameter publicKey, byte[] buffer, int offset, int count, byte[] signature)
        {
            var verifier = SignerUtilities.GetSigner(algorithmOid);
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(buffer, offset, count);
            return verifier.VerifySignature(signature);
        }
    }

    internal sealed class Password : IPasswordFinder
    {
        private readonly char[] _chars;

        internal Password(string password)
        {
            _chars = password.ToCharArray();
        }

        public char[] GetPassword()
        {
            return _chars;
        }
    }

    /// <summary>
    /// X509Name 创建器。
    /// </summary>
    internal sealed class X509NameGenerator
    {
        private readonly Dictionary<DerObjectIdentifier, string> _attributes = new Dictionary<DerObjectIdentifier, string>();

        internal bool IsEmpty => _attributes.Count == 0;

        internal void AddX509Name(DerObjectIdentifier oid, string value)
        {
            _attributes.Add(oid, value);
        }

        internal X509Name Generate()
        {
            var ordering = new DerObjectIdentifier[_attributes.Count];
            _attributes.Keys.CopyTo(ordering, 0);
            return new X509Name(ordering, _attributes);
        }

        internal void Reset()
        {
            _attributes.Clear();
        }
    }
}