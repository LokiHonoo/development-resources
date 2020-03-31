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
 *
 * 使用 SignerUtilities.Algorithms 获取可用算法的别名。
 *
 * 根据签名私钥类型选择。如 RSA 密钥可使用 SHA256WithRSA、SHA512WithRSA。
 *
 * 部分算法不能被 Microsoft 证书链验证。如 SHA224WithRSA（1.2.840.113549.1.1.14）。参见 http://oidref.com 关于算法的 Description。
 *
 * BUG: 使用签名算法查询类 DefaultSignatureAlgorithmIdentifierFinder 时 SHA256WithECDSA 指向 SHA224WithECDSA。
 *      若使用此方法获取 oid 应注意修正。
 *      BouncyCastle 1.8.6 尚未修复此 BUG。
 */

/*
 * 对称加密算法的补充说明
 *
 * 部分对称加密算法没有独立的 oid。如果传入 oid.Id， BouncyCastle 忽略模式版本创建算法对象。
 *
 * KEY 大小根据算法有可选值和固定值。
 * 64  - DES
 * 128 - AES128, CAMELLIA128, DESEDE, CAST5, BLOWFISH, SAFER-SK128
 * 192 - AES192, CAMELLIA192, DESEDE3, TDEA
 * 256 - AES256, CAMELLIA256
 *
 * IV 大小在 CBC 模式下根据算法为固定值。
 * 64  - BLOWFISH, CHACHA, DES, DESEDE, DESEDE3, SALSA20
 * 96  - CHACHA7539
 * 128 - AES, AES128, AES192, AES256, CAMELLIA, CAMELLIA128, CAMELLIA192, CAMELLIA256, NOEKEON, SEED, SM4
 * CFB、OFB 最小值为 8，最大值为块大小，并以 8 位递增。设置时除了传入正确的 IV 外还需要在 CipherString 中加入长度。如 AES/CFB40/PKCS7PADDING
 * CTR 最小值为块大小减去 64。最大值为块大小。DES 算法最小值为块大小减去 32。
 * SIC 和 CTR 相同只是增加了限制，不允许块大小小于 128 的算法。
 * 总之，大多数算法的可用 IV 大小都和块大小相等。且太小的 IV 常以固定值填充以增长到指定长度，所以一般选择块大小的 IV 就可以了。
 */

/*
 * 校验算法的补充说明
 *
 * 可使用 PkcsObjectIdentifiers、OiwObjectIdentifiers、NistObjectIdentifiers、TeleTrusTObjectIdentifiers、
 *     CryptoProObjectIdentifiers、MiscObjectIdentifiers、RosstandartObjectIdentifiers、UAObjectIdentifiers、GMObjectIdentifiers 中的校验相关的算法 oid。
 * 也可使用 CommonHashAlgorithms 类中提供的常用算法。
 *
 * 使用 DigestUtilities.Algorithms 获取可用算法的别名。
 */

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
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
    #region Helper

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
            try
            {
                issuerCert.CheckValidity(start.AddDays(days));
            }
            catch
            {
                throw new ArgumentException("签署的有效期超出了颁发机构证书的有效期。", nameof(issuerCert));
            }
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
        /// <param name="keyAlias">私钥的别名。</param>
        /// <param name="privateKey">私钥。</param>
        /// <param name="namedCerts">设置了别名的证书集合。</param>
        /// <param name="password">设置密码。</param>
        /// <param name="output">输出到流。</param>
        internal static void GeneratePfx(string keyAlias, AsymmetricKeyParameter privateKey, Dictionary<string, X509Certificate> namedCerts, string password, Stream output)
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
        /// 创建 P12 证书。
        /// </summary>
        /// <param name="keyAlias">私钥的别名。</param>
        /// <param name="privateKey">私钥。</param>
        /// <param name="namedCerts">设置了别名的证书集合。</param>
        /// <param name="password">设置密码。</param>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static byte[] GeneratePfx(string keyAlias, AsymmetricKeyParameter privateKey, Dictionary<string, X509Certificate> namedCerts, string password)
        {
            using (var ms = new MemoryStream())
            {
                GeneratePfx(keyAlias, privateKey, namedCerts, password, ms);
                return ms.ToArray();
            }
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

        /// <summary>
        /// 读取 P12 证书。
        /// </summary>
        /// <param name="raw">二进制对象。</param>
        /// <param name="password">设置密码。</param>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static Pkcs12Store ReadPfx(byte[] raw, string password)
        {
            using (var ms = new MemoryStream(raw))
            {
                return ReadPfx(ms, password);
            }
        }

        #endregion P12 证书
    }

    /// <summary>
    /// 加密辅助。
    /// </summary>
    internal static class EncryptionHelper
    {
        #region 非对称加解密

        /// <summary>
        /// 非对称加密算法解密。
        /// </summary>
        /// <param name="asymmetricPrivateKey">非对称算法私钥。</param>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="data">要解密的数据。内部实现分段解密，可传入全部数据字节。</param>
        /// <returns></returns>
        internal static byte[] AsymmetricDecrypt(ICipherParameters asymmetricPrivateKey, string asymmetricCipherString, byte[] data)
        {
            return AsymmetricDecrypt(asymmetricPrivateKey, asymmetricCipherString, data, 0, data.Length);
        }

        /// <summary>
        /// 非对称加密算法解密。
        /// </summary>
        /// <param name="asymmetricPrivateKey">非对称算法私钥。</param>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="buffer">包含要解密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。内部实现分段解密，可传入全部数据字节长度。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static byte[] AsymmetricDecrypt(ICipherParameters asymmetricPrivateKey, string asymmetricCipherString, byte[] buffer, int offset, int count)
        {
            var algorithm = CipherUtilities.GetCipher(asymmetricCipherString);
            algorithm.Init(false, asymmetricPrivateKey);
            var inLen = algorithm.GetBlockSize();
            if (count <= inLen)
            {
                return algorithm.DoFinal(buffer, offset, count);
            }
            else
            {
                var block = Math.DivRem(count, inLen, out int mod);
                if (mod > 0)
                {
                    throw new Exception("密文长度错误。");
                }
                else
                {
                    var outLen = algorithm.GetOutputSize(0);
                    var result = new List<byte>(outLen * block);
                    for (int i = 0; i < block; i++)
                    {
                        result.AddRange(algorithm.DoFinal(buffer, inLen * i + offset, inLen));
                    }
                    return result.ToArray();
                }
            }
        }

        /// <summary>
        /// 非对称加密算法加密。
        /// </summary>
        /// <param name="asymmetricPublicKey">非对称算法公钥。</param>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="data">要加密的数据。内部实现分段加密，可传入全部数据字节。</param>
        /// <returns></returns>
        internal static byte[] AsymmetricEncrypt(ICipherParameters asymmetricPublicKey, string asymmetricCipherString, byte[] data)
        {
            return AsymmetricEncrypt(asymmetricPublicKey, asymmetricCipherString, data, 0, data.Length);
        }

        /// <summary>
        /// 非对称加密算法加密。
        /// </summary>
        /// <param name="asymmetricPublicKey">非对称算法公钥。</param>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="buffer">包含要加密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。内部实现分段加密，可传入全部数据字节长度。</param>
        /// <returns></returns>
        internal static byte[] AsymmetricEncrypt(ICipherParameters asymmetricPublicKey, string asymmetricCipherString, byte[] buffer, int offset, int count)
        {
            var algorithm = CipherUtilities.GetCipher(asymmetricCipherString);
            algorithm.Init(true, asymmetricPublicKey);
            var inLen = algorithm.GetBlockSize();
            if (count <= inLen)
            {
                return algorithm.DoFinal(buffer, offset, count);
            }
            else
            {
                var block = Math.DivRem(count, inLen, out int mod);
                var outLen = algorithm.GetOutputSize(0);
                if (mod > 0)
                {
                    var result = new byte[block * outLen + outLen];
                    for (int i = 0; i < block; i++)
                    {
                        algorithm.DoFinal(buffer, inLen * i + offset, inLen, result, outLen * i);
                    }
                    algorithm.DoFinal(buffer, inLen * block + offset, mod, result, outLen * block);
                    return result;
                }
                else
                {
                    var result = new byte[block * outLen];
                    for (int i = 0; i < block; i++)
                    {
                        algorithm.DoFinal(buffer, inLen * i + offset, inLen, result, outLen * i);
                    }
                    return result;
                }
            }
        }

        #endregion 非对称加解密

        #region 对称加解密

        /// <summary>
        /// 对称加密算法解密。
        /// </summary>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="data">要解密的数据。</param>
        /// <returns></returns>
        internal static byte[] SymmetricDecrypt(ICipherParameters symmetricKey, string symmetricCipherString, byte[] data)
        {
            return SymmetricDecrypt(symmetricKey, symmetricCipherString, data, 0, data.Length);
        }

        /// <summary>
        /// 对称加密算法解密。
        /// </summary>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="buffer">包含要解密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] SymmetricDecrypt(ICipherParameters symmetricKey, string symmetricCipherString, byte[] buffer, int offset, int count)
        {
            var algorithm = CipherUtilities.GetCipher(symmetricCipherString);
            algorithm.Init(false, symmetricKey);
            return algorithm.DoFinal(buffer, offset, count);
        }

        /// <summary>
        /// 对称加密算法加密。
        /// </summary>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="data">要加密的数据。</param>
        /// <returns></returns>
        internal static byte[] SymmetricEncrypt(ICipherParameters symmetricKey, string symmetricCipherString, byte[] data)
        {
            return SymmetricEncrypt(symmetricKey, symmetricCipherString, data, 0, data.Length);
        }

        /// <summary>
        /// 对称加密算法加密。
        /// </summary>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="buffer">包含要加密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] SymmetricEncrypt(ICipherParameters symmetricKey, string symmetricCipherString, byte[] buffer, int offset, int count)
        {
            var algorithm = CipherUtilities.GetCipher(symmetricCipherString);
            algorithm.Init(true, symmetricKey);
            return algorithm.DoFinal(buffer, offset, count);
        }

        #endregion 对称加解密
    }

    /// <summary>
    /// 校验辅助。
    /// </summary>
    internal static class HashHelper
    {
        /// <summary>
        /// 校验。
        /// </summary>
        /// <param name="hashAlgorithmOid">校验算法 oid。可使用 CommonHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="data">要校验的数据。</param>
        /// <returns></returns>
        internal static byte[] ComputeHash(DerObjectIdentifier hashAlgorithmOid, byte[] data)
        {
            return ComputeHash(hashAlgorithmOid, data, 0, data.Length);
        }

        /// <summary>
        /// 校验。
        /// </summary>
        /// <param name="hashAlgorithmOid">校验算法 oid。可使用 CommonHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="buffer">包含要校验的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] ComputeHash(DerObjectIdentifier hashAlgorithmOid, byte[] buffer, int offset, int count)
        {
            IDigest algorithm = DigestUtilities.GetDigest(hashAlgorithmOid);
            algorithm.BlockUpdate(buffer, offset, count);
            byte[] hash = new byte[algorithm.GetDigestSize()];
            algorithm.DoFinal(hash, 0);
            return hash;
        }

        /// <summary>
        /// HMAC 校验。
        /// </summary>
        /// <param name="hashAlgorithmOid">校验算法 oid。可使用 CommonHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="hmacKey">HMAC 密钥。随机字节数组。</param>
        /// <param name="data">要校验的数据。</param>
        /// <returns></returns>
        internal static byte[] HmacComputeHash(DerObjectIdentifier hashAlgorithmOid, byte[] hmacKey, byte[] data)
        {
            return HmacComputeHash(hashAlgorithmOid, hmacKey, data, 0, data.Length);
        }

        /// <summary>
        /// HMAC 校验。
        /// </summary>
        /// <param name="hashAlgorithmOid">校验算法 oid。可使用 CommonHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="hmacKey">HMAC 密钥。随机字节数组。</param>
        /// <param name="buffer">包含要校验的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] HmacComputeHash(DerObjectIdentifier hashAlgorithmOid, byte[] hmacKey, byte[] buffer, int offset, int count)
        {
            IDigest digest = DigestUtilities.GetDigest(hashAlgorithmOid);
            HMac algorithm = new HMac(digest);
            algorithm.Init(new KeyParameter(hmacKey));
            algorithm.BlockUpdate(buffer, offset, count);
            byte[] hash = new byte[algorithm.GetMacSize()];
            algorithm.DoFinal(hash, 0);
            return hash;
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
    /// 密钥辅助。
    /// </summary>
    internal static class KeyParametersHelper
    {
        #region 非对称密钥

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
        /// <param name="pemEncryptionAlgorithm">加密算法。可使用 NamedPemEncryptionAlgorithms 类中提供的已命名算法。名称是 OpenSSL 定义的 DEK 格式。</param>
        /// <param name="password">设置密码。</param>
        /// <param name="privateKeyEncPem">带加密标签的 PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static void ParseKey(AsymmetricKeyParameter privateKey, string pemEncryptionAlgorithm, string password, out string privateKeyEncPem)
        {
            if (privateKey.IsPrivate)
            {
                using (var writer = new StringWriter())
                {
                    var pemWriter = new PemWriter(writer);
                    pemWriter.WriteObject(privateKey, pemEncryptionAlgorithm, password.ToCharArray(), Common.SecureRandom);
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

        private sealed class Password : IPasswordFinder
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

        #endregion 非对称密钥

        #region 对称密钥

        /// <summary>
        /// 创建 AES 密钥。
        /// </summary>
        /// <param name="rgbKey">密钥数组。长度可以是 16、24、32 字节。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateAesKey(byte[] rgbKey, byte[] rgbIV)
        {
            return GenerateSymmetricKey("AES", rgbKey, rgbIV);
        }

        /// <summary>
        /// 创建 AES 密钥。
        /// </summary>
        /// <param name="keyBuffer">从中截取密钥的数组缓冲区。</param>
        /// <param name="keyCutOffset">截取密钥的偏移。</param>
        /// <param name="keyCutCount">截取的密钥长度。长度可以是 16、24、32 字节。</param>
        /// <param name="ivBuffer">从中截取初始化向量的数组缓冲区。</param>
        /// <param name="ivCutOffset">截取初始化向量的偏移。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateAesKey(byte[] keyBuffer, int keyCutOffset, int keyCutCount, byte[] ivBuffer, int ivCutOffset)
        {
            return GenerateSymmetricKey("DESEDE", keyBuffer, keyCutOffset, keyCutCount, ivBuffer, ivCutOffset, 8);
        }

        /// <summary>
        /// 创建 DESEDE3 密钥。
        /// </summary>
        /// <param name="rgbKey">密钥数组。长度是 24 字节。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateDesEde3Key(byte[] rgbKey, byte[] rgbIV)
        {
            return GenerateSymmetricKey("DESEDE", rgbKey, rgbIV);
        }

        /// <summary>
        /// 创建 DESEDE3 密钥。
        /// </summary>
        /// <param name="keyBuffer">从中截取密钥的数组缓冲区。</param>
        /// <param name="keyCutOffset">截取密钥的偏移。</param>
        /// <param name="ivBuffer">从中截取初始化向量的数组缓冲区。</param>
        /// <param name="ivCutOffset">截取初始化向量的偏移。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateDesEde3Key(byte[] keyBuffer, int keyCutOffset, byte[] ivBuffer, int ivCutOffset)
        {
            return GenerateSymmetricKey("DESEDE", keyBuffer, keyCutOffset, 24, ivBuffer, ivCutOffset, 8);
        }

        /// <summary>
        /// 创建 DES 密钥。
        /// </summary>
        /// <param name="rgbKey">密钥数组。长度是 8 字节。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateDesKey(byte[] rgbKey, byte[] rgbIV)
        {
            return GenerateSymmetricKey("DES", rgbKey, rgbIV);
        }

        /// <summary>
        /// 创建 DES 密钥。
        /// </summary>
        /// <param name="keyBuffer">从中截取密钥的数组缓冲区。</param>
        /// <param name="keyCutOffset">截取密钥的偏移。</param>
        /// <param name="ivBuffer">从中截取初始化向量的数组缓冲区。</param>
        /// <param name="ivCutOffset">截取初始化向量的偏移。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateDesKey(byte[] keyBuffer, int keyCutOffset, byte[] ivBuffer, int ivCutOffset)
        {
            return GenerateSymmetricKey("DES", keyBuffer, keyCutOffset, 8, ivBuffer, ivCutOffset, 8);
        }

        /// <summary>
        /// 创建 SM4 密钥。
        /// </summary>
        /// <param name="rgbKey">密钥数组。长度是 16 字节。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateSM4Key(byte[] rgbKey, byte[] rgbIV)
        {
            return GenerateSymmetricKey("SM4", rgbKey, rgbIV);
        }

        /// <summary>
        /// 创建 SM4 密钥。
        /// </summary>
        /// <param name="keyBuffer">从中截取密钥的数组缓冲区。</param>
        /// <param name="keyCutOffset">截取密钥的偏移。</param>
        /// <param name="ivBuffer">从中截取初始化向量的数组缓冲区。</param>
        /// <param name="ivCutOffset">截取初始化向量的偏移。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateSM4Key(byte[] keyBuffer, int keyCutOffset, byte[] ivBuffer, int ivCutOffset)
        {
            return GenerateSymmetricKey("SM4", keyBuffer, keyCutOffset, 16, ivBuffer, ivCutOffset, 16);
        }

        /// <summary>
        /// 创建对称加密算法密钥。
        /// </summary>
        /// <param name="symmetricAlgorithm">加密算法。可使用 NamedSymmetricAlgorithms 类中提供的已命名算法。参见注释中的补充说明。</param>
        /// <param name="rgbKey">密钥数组。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateSymmetricKey(string symmetricAlgorithm, byte[] rgbKey, byte[] rgbIV)
        {
            ICipherParameters parameters = ParameterUtilities.CreateKeyParameter(symmetricAlgorithm, rgbKey);
            if (rgbIV != null)
            {
                parameters = new ParametersWithIV(parameters, rgbIV);
            }
            return parameters;
        }

        /// <summary>
        /// 创建对称加密算法密钥。
        /// </summary>
        /// <param name="symmetricAlgorithm">加密算法。可使用 NamedSymmetricAlgorithms 类中提供的已命名算法。参见注释中的补充说明。</param>
        /// <param name="keyBuffer">从中截取密钥的数组缓冲区。</param>
        /// <param name="keyCutOffset">截取密钥的偏移。</param>
        /// <param name="keyCutCount">截取的密钥长度。</param>
        /// <param name="ivBuffer">从中截取初始化向量的数组缓冲区。</param>
        /// <param name="ivCutOffset">截取初始化向量的偏移。</param>
        /// <param name="ivCutCount">截取的初始化向量长度。</param>
        /// <returns></returns>
        internal static ICipherParameters GenerateSymmetricKey(string symmetricAlgorithm,
                                                               byte[] keyBuffer,
                                                               int keyCutOffset,
                                                               int keyCutCount,
                                                               byte[] ivBuffer,
                                                               int ivCutOffset,
                                                               int ivCutCount)
        {
            ICipherParameters parameters = ParameterUtilities.CreateKeyParameter(symmetricAlgorithm, keyBuffer, keyCutOffset, keyCutCount);
            if (ivBuffer != null)
            {
                parameters = new ParametersWithIV(parameters, ivBuffer, ivCutOffset, ivCutCount);
            }
            return parameters;
        }

        #endregion 对称密钥
    }

    /// <summary>
    /// 签名辅助。
    /// </summary>
    internal static class SignatureHelper
    {
        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <returns></returns>
        internal static byte[] Sign(DerObjectIdentifier signatureAlgorithmOid, AsymmetricKeyParameter privateKey, byte[] data)
        {
            return Sign(signatureAlgorithmOid, privateKey, data, 0, data.Length);
        }

        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        internal static byte[] Sign(DerObjectIdentifier signatureAlgorithmOid, AsymmetricKeyParameter privateKey, byte[] buffer, int offset, int count)
        {
            var signer = SignerUtilities.InitSigner(signatureAlgorithmOid, true, privateKey, Common.SecureRandom);
            signer.BlockUpdate(buffer, offset, count);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(DerObjectIdentifier signatureAlgorithmOid, AsymmetricKeyParameter publicKey, byte[] data, byte[] signature)
        {
            return Verify(signatureAlgorithmOid, publicKey, data, 0, data.Length, signature);
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="signatureAlgorithmOid">签名算法 oid。可使用 CommonSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        internal static bool Verify(DerObjectIdentifier signatureAlgorithmOid, AsymmetricKeyParameter publicKey, byte[] buffer, int offset, int count, byte[] signature)
        {
            //var verifier = SignerUtilities.InitSigner(signatureAlgorithmOid, false, publicKey, null);
            var verifier = SignerUtilities.GetSigner(signatureAlgorithmOid);
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(buffer, offset, count);
            return verifier.VerifySignature(signature);
        }
    }

    #endregion Helper

    #region 扩展

    /// <summary>
    /// X509Name 创建器。
    /// </summary>
    internal sealed class X509NameGenerator
    {
        #region 成员

        private readonly Dictionary<DerObjectIdentifier, string> _attributes = new Dictionary<DerObjectIdentifier, string>();

        /// <summary>
        /// 创建器中是否有值。
        /// </summary>
        internal bool IsEmpty => _attributes.Count == 0;

        #endregion 成员

        /// <summary>
        /// 增加 X509Name 属性。
        /// </summary>
        /// <param name="oid">属性 oid。</param>
        /// <param name="value">属性值。</param>
        internal void AddX509Name(DerObjectIdentifier oid, string value)
        {
            _attributes.Add(oid, value);
        }

        /// <summary>
        /// 创建 X509Name。
        /// </summary>
        /// <returns></returns>
        internal X509Name Generate()
        {
            var ordering = new DerObjectIdentifier[_attributes.Count];
            _attributes.Keys.CopyTo(ordering, 0);
            return new X509Name(ordering, _attributes);
        }

        /// <summary>
        /// 清除所有值。
        /// </summary>
        internal void Reset()
        {
            _attributes.Clear();
        }
    }

    #endregion 扩展

    #region Common

    /// <summary>
    /// 公共对象。
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// 随机数生成器。
        /// </summary>
        internal static readonly SecureRandom SecureRandom = new SecureRandom();
    }

    /// <summary>
    /// 常用曲线。
    /// </summary>
    internal static class CommonCurves
    {
        internal static readonly DerObjectIdentifier SecP256r1 = SecObjectIdentifiers.SecP256r1;
        internal static readonly DerObjectIdentifier SecP384r1 = SecObjectIdentifiers.SecP384r1;
        internal static readonly DerObjectIdentifier SecP521r1 = SecObjectIdentifiers.SecP521r1;
    }

    /// <summary>
    /// 常用校验算法。
    /// </summary>
    internal static class CommonHashAlgorithms
    {
        internal static readonly DerObjectIdentifier MD5 = PkcsObjectIdentifiers.MD5;
        internal static readonly DerObjectIdentifier SHA1 = OiwObjectIdentifiers.IdSha1;
        internal static readonly DerObjectIdentifier SHA256 = NistObjectIdentifiers.IdSha256;
        internal static readonly DerObjectIdentifier SHA3_256 = NistObjectIdentifiers.IdSha3_256;
        internal static readonly DerObjectIdentifier SHA3_384 = NistObjectIdentifiers.IdSha3_384;
        internal static readonly DerObjectIdentifier SHA3_512 = NistObjectIdentifiers.IdSha3_512;
        internal static readonly DerObjectIdentifier SHA384 = NistObjectIdentifiers.IdSha384;
        internal static readonly DerObjectIdentifier SHA512 = NistObjectIdentifiers.IdSha512;
        internal static readonly DerObjectIdentifier SM3 = GMObjectIdentifiers.sm3;
    }

    /// <summary>
    /// 常用签名算法。
    /// </summary>
    internal static class CommonSignatureAlgorithms
    {
        internal static readonly DerObjectIdentifier SHA256WithDSA = NistObjectIdentifiers.DsaWithSha256;
        internal static readonly DerObjectIdentifier SHA256WithECDSA = X9ObjectIdentifiers.ECDsaWithSha256;
        internal static readonly DerObjectIdentifier SHA256WithRSA = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
        internal static readonly DerObjectIdentifier SHA384WithDSA = NistObjectIdentifiers.DsaWithSha384;
        internal static readonly DerObjectIdentifier SHA384WithECDSA = X9ObjectIdentifiers.ECDsaWithSha384;
        internal static readonly DerObjectIdentifier SHA384WithRSA = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
        internal static readonly DerObjectIdentifier SHA512WithDSA = NistObjectIdentifiers.DsaWithSha512;
        internal static readonly DerObjectIdentifier SHA512WithECDSA = X9ObjectIdentifiers.ECDsaWithSha512;
        internal static readonly DerObjectIdentifier SHA512WithRSA = PkcsObjectIdentifiers.Sha512WithRsaEncryption;
        internal static readonly DerObjectIdentifier SM3WithSM2 = GMObjectIdentifiers.sm2sign_with_sm3;
    }

    /// <summary>
    /// 已命名非对称加密算法设置。
    /// </summary>
    internal static class NamedAsymmetricCipherStrings
    {
        internal const string RSA_OAEP = "RSA//OAEPPADDING";
        internal const string RSA_PKCS1 = "RSA//PKCS1PADDING";
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
        internal const string DES_CBC = "DES-CBC";
        internal const string DES_ECB = "DES-ECB";
        internal const string DES_EDE3_CBC = "DES-EDE3-CBC";
        internal const string DES_EDE3_ECB = "DES-EDE3-ECB";
    }

    /// <summary>
    /// 已命名对称加密算法。
    /// </summary>
    internal static class NamedSymmetricAlgorithms
    {
        internal const string AES = "AES";
        internal const string DES = "DES";
        internal const string DESEDE = "DESEDE";
        internal const string SM4 = "SM4";
    }

    /// <summary>
    /// 已命名对称加密算法设置。
    /// </summary>
    internal static class NamedSymmetricCipherStrings
    {
        internal const string AES_CBC_PKCS7 = "AES/CBC/PKCS7PADDING";
        internal const string AES_ECB_PKCS7 = "AES/ECB/PKCS7PADDING";
        internal const string DES_CBC_PKCS7 = "DES/CBC/PKCS7PADDING";
        internal const string DES_ECB_PKCS7 = "DES/ECB/PKCS7PADDING";
        internal const string DESEDE_CBC_PKCS7 = "DESEDE/CBC/PKCS7PADDING";
        internal const string DESEDE_ECB_PKCS7 = "DESEDE/ECB/PKCS7PADDING";
        internal const string SM4_CBC_PKCS7 = "SM4/CBC/PKCS7PADDING";
        internal const string SM4_ECB_PKCS7 = "SM4/ECB/PKCS7PADDING";
    }

    #endregion Common
}