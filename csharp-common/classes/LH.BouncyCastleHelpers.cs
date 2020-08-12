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
 * 也可使用 NamedSignatureAlgorithms 类中提供的常用算法。
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
 *
 * BUG: RC5_64 对称加密算法密钥不支持默认 round 数，因此不能使用通用的 KeyParameter 类型。必须创建 RC5Parameters 并指定 round。
 *      RC5 算法没有此问题。
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
 * CTR、SIC 最小值为 (BlockSize / 2) 位，或 (BlockSize - 64) 位中的较大值。最大值为块大小。
 * SIC 和 CTR 相同只是增加了限制，不允许块大小小于 128 的算法。
 * 总之，大多数算法的可用 IV 大小都和块大小相等。且太小的 IV 常以固定值填充以增长到指定长度，所以一般选择块大小的 IV 就可以了。
 */

/*
 * 校验算法的补充说明
 *
 * 可使用 PkcsObjectIdentifiers、OiwObjectIdentifiers、NistObjectIdentifiers、TeleTrusTObjectIdentifiers、
 *     CryptoProObjectIdentifiers、MiscObjectIdentifiers、RosstandartObjectIdentifiers、UAObjectIdentifiers、GMObjectIdentifiers 中的校验相关的算法 oid。
 * 也可使用 NamedHashAlgorithms 类中提供的常用算法。
 *
 * 使用 DigestUtilities.Algorithms 获取可用算法的别名。
 */

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
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
    public static class CertificateHelper
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
        public static void ExtractCsr(Pkcs10CertificationRequest csr, out X509Name dn, out AsymmetricKeyParameter publicKey, out X509Extensions extensions)
        {
            if (csr is null)
            {
                throw new ArgumentNullException(nameof(csr));
            }

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
        /// <param name="signatureAlgorithm">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="keyPair">使用者密钥对。</param>
        /// <param name="dn">使用者 DN。</param>
        /// <param name="extensions">使用者扩展属性。</param>
        /// <returns></returns>
        public static Pkcs10CertificationRequest GenerateCsr(string signatureAlgorithm, AsymmetricCipherKeyPair keyPair, X509Name dn, X509Extensions extensions)
        {
            if (keyPair is null)
            {
                throw new ArgumentNullException(nameof(keyPair));
            }
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, keyPair.Private, Common.SecureRandom);
            var set = new DerSet(extensions);
            var attribute = new DerSet(new AttributePkcs(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest, set));
            return new Pkcs10CertificationRequest(signatureFactory, dn, keyPair.Public, attribute);
        }

        #endregion 证书请求

        #region 证书

        /// <summary>
        /// 创建颁发机构自签名证书。
        /// </summary>
        /// <param name="signatureAlgorithm">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="keyPair">颁发机构密钥对。</param>
        /// <param name="dn">颁发机构 DN。</param>
        /// <param name="extensions">扩展属性。</param>
        /// <param name="start">启用时间。</param>
        /// <param name="days">从启用时间开始的有效天数。</param>
        /// <returns></returns>
        public static X509Certificate GenerateIssuerCert(string signatureAlgorithm,
                                                           AsymmetricCipherKeyPair keyPair,
                                                           X509Name dn,
                                                           X509Extensions extensions,
                                                           DateTime start,
                                                           int days)
        {
            if (keyPair is null)
            {
                throw new ArgumentNullException(nameof(keyPair));
            }

            return GenerateCert(signatureAlgorithm, keyPair.Private, dn, dn, keyPair.Public, extensions, start, days);
        }

        /// <summary>
        /// 创建使用者证书。
        /// </summary>
        /// <param name="signatureAlgorithm">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
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
        public static X509Certificate GenerateSubjectCert(string signatureAlgorithm,
                                                            AsymmetricKeyParameter issuerPrivateKey,
                                                            X509Certificate issuerCert,
                                                            X509Name subjectDN,
                                                            AsymmetricKeyParameter subjectPublicKey,
                                                            X509Extensions extensions,
                                                            DateTime start,
                                                            int days)
        {
            if (issuerCert is null)
            {
                throw new ArgumentNullException(nameof(issuerCert));
            }

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
            return GenerateCert(signatureAlgorithm, issuerPrivateKey, issuerCert.SubjectDN, subjectDN, subjectPublicKey, extensions, start, days);
        }

        private static X509Certificate GenerateCert(string signatureAlgorithm,
                                                    AsymmetricKeyParameter issuerPrivateKey,
                                                    X509Name issuerDN,
                                                    X509Name subjectDN,
                                                    AsymmetricKeyParameter subjectPublicKey,
                                                    X509Extensions extensions,
                                                    DateTime start,
                                                    int days)
        {
            var sn = new BigInteger(128, Common.SecureRandom);
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerPrivateKey, Common.SecureRandom);
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
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static Pkcs12Store GeneratePfx(string keyAlias, AsymmetricKeyParameter privateKey, Dictionary<string, X509Certificate> namedCerts, string password)
        {
            if (namedCerts is null)
            {
                throw new ArgumentNullException(nameof(namedCerts));
            }
            using (var ms = new MemoryStream())
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
                store.Save(ms, pass, Common.SecureRandom);
                ms.Flush();
                return new Pkcs12Store(ms, pass);
            }
        }

        #endregion P12 证书
    }

    /// <summary>
    /// 加密辅助。
    /// </summary>
    public static class EncryptionHelper
    {
        #region 非对称加解密

        /// <summary>
        /// 非对称加密算法解密。
        /// </summary>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="asymmetricPrivateKey">非对称算法私钥。</param>
        /// <param name="data">要解密的数据。内部实现分段解密，可传入全部数据字节。</param>
        /// <returns></returns>
        public static byte[] AsymmetricDecrypt(string asymmetricCipherString, ICipherParameters asymmetricPrivateKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return AsymmetricDecrypt(asymmetricCipherString, asymmetricPrivateKey, data, 0, data.Length);
        }

        /// <summary>
        /// 非对称加密算法解密。
        /// </summary>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="asymmetricPrivateKey">非对称算法私钥。</param>
        /// <param name="buffer">包含要解密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。内部实现分段解密，可传入全部数据字节长度。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        public static byte[] AsymmetricDecrypt(string asymmetricCipherString, ICipherParameters asymmetricPrivateKey, byte[] buffer, int offset, int count)
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
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="asymmetricPublicKey">非对称算法公钥。</param>
        /// <param name="data">要加密的数据。内部实现分段加密，可传入全部数据字节。</param>
        /// <returns></returns>
        public static byte[] AsymmetricEncrypt(string asymmetricCipherString, ICipherParameters asymmetricPublicKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return AsymmetricEncrypt(asymmetricCipherString, asymmetricPublicKey, data, 0, data.Length);
        }

        /// <summary>
        /// 非对称加密算法加密。
        /// </summary>
        /// <param name="asymmetricCipherString">加密算法设置。可使用 NamedAsymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="asymmetricPublicKey">非对称算法公钥。</param>
        /// <param name="buffer">包含要加密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。内部实现分段加密，可传入全部数据字节长度。</param>
        /// <returns></returns>
        public static byte[] AsymmetricEncrypt(string asymmetricCipherString, ICipherParameters asymmetricPublicKey, byte[] buffer, int offset, int count)
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
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="data">要解密的数据。</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(string symmetricCipherString, ICipherParameters symmetricKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return SymmetricDecrypt(symmetricCipherString, symmetricKey, data, 0, data.Length);
        }

        /// <summary>
        /// 对称加密算法解密。
        /// </summary>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="buffer">包含要解密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(string symmetricCipherString, ICipherParameters symmetricKey, byte[] buffer, int offset, int count)
        {
            var algorithm = CipherUtilities.GetCipher(symmetricCipherString);
            algorithm.Init(false, symmetricKey);
            return algorithm.DoFinal(buffer, offset, count);
        }

        /// <summary>
        /// 对称加密算法加密。
        /// </summary>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="data">要加密的数据。</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(string symmetricCipherString, ICipherParameters symmetricKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return SymmetricEncrypt(symmetricCipherString, symmetricKey, data, 0, data.Length);
        }

        /// <summary>
        /// 对称加密算法加密。
        /// </summary>
        /// <param name="symmetricCipherString">加密算法设置。可使用 NamedSymmetricCipherStrings 类中提供的已命名参数。根据密钥参数选择。</param>
        /// <param name="symmetricKey">对称算法密钥。</param>
        /// <param name="buffer">包含要加密的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(string symmetricCipherString, ICipherParameters symmetricKey, byte[] buffer, int offset, int count)
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
    public static class HashHelper
    {
        /// <summary>
        /// 校验。
        /// </summary>
        /// <param name="algorithmName">校验算法。可使用 NamedHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="data">要校验的数据。</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string algorithmName, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return ComputeHash(algorithmName, data, 0, data.Length);
        }

        /// <summary>
        /// 校验。
        /// </summary>
        /// <param name="algorithmName">校验算法。可使用 NamedHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="buffer">包含要校验的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string algorithmName, byte[] buffer, int offset, int count)
        {
            IDigest algorithm = DigestUtilities.GetDigest(algorithmName);
            algorithm.BlockUpdate(buffer, offset, count);
            byte[] hash = new byte[algorithm.GetDigestSize()];
            algorithm.DoFinal(hash, 0);
            return hash;
        }

        /// <summary>
        /// HMAC 校验。
        /// </summary>
        /// <param name="algorithmName">校验算法。可使用 NamedHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="hmacKey">HMAC 密钥。随机字节数组。</param>
        /// <param name="data">要校验的数据。</param>
        /// <returns></returns>
        public static byte[] HmacComputeHash(string algorithmName, byte[] hmacKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return HmacComputeHash(algorithmName, hmacKey, data, 0, data.Length);
        }

        /// <summary>
        /// HMAC 校验。
        /// </summary>
        /// <param name="algorithmName">校验算法 oid。可使用 NamedHashAlgorithms 类中提供的常用算法。可用算法清单参见注释中的补充说明。</param>
        /// <param name="hmacKey">HMAC 密钥。随机字节数组。</param>
        /// <param name="buffer">包含要校验的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        public static byte[] HmacComputeHash(string algorithmName, byte[] hmacKey, byte[] buffer, int offset, int count)
        {
            IDigest digest = DigestUtilities.GetDigest(algorithmName);
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
    public static class KeyExchangeHelper
    {
        /// <summary>
        /// 创建密钥交换协议，并输出公钥。
        /// </summary>
        /// <param name="parameters">曲线参数。</param>
        /// <param name="publicKey">用于传递给对方的公钥。</param>
        /// <returns></returns>
        public static IBasicAgreement CreateAgreement(DHParameters parameters, out AsymmetricKeyParameter publicKey)
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
        public static DHParameters CreateParametersA(int keySize)
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
        public static DHParameters CreateParametersB(BigInteger aP, BigInteger aG)
        {
            return new DHParameters(aP, aG);
        }
    }

    /// <summary>
    /// 密钥辅助。
    /// </summary>
    public static class KeyParametersHelper
    {
        #region 非对称密钥

        /// <summary>
        /// 创建 ECDSA 密钥对。
        /// </summary>
        /// <param name="curve">曲线。可使用 NamedCurves 类中提供的常用曲线。或 SecObjectIdentifiers 中的命名曲线。</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GenerateEcdsaKeyPair(string curve)
        {
            var ec = SecNamedCurves.GetByName(curve);
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
        public static AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize)
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
        public static AsymmetricCipherKeyPair GenerateSM2KeyPair()
        {
            var ec = GMNamedCurves.GetByOid(GMObjectIdentifiers.sm2p256v1);
            var domain = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
            var kgp = new ECKeyGenerationParameters(domain, Common.SecureRandom);
            var generator = new ECKeyPairGenerator();
            generator.Init(kgp);
            return generator.GenerateKeyPair();
        }

        #endregion 非对称密钥

        #region 对称密钥

        /// <summary>
        /// 创建 RC5 对称加密算法密钥，默认 12 round。 RC5_64 对称加密算法密钥不支持默认 round 数，因此不能使用通用的 KeyParameter 类型。
        /// </summary>
        /// <param name="rgbKey">密钥数组。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        public static ICipherParameters GenerateRC5Key(byte[] rgbKey, byte[] rgbIV)
        {
            ICipherParameters parameters = new RC5Parameters(rgbKey, 12);
            if (rgbIV != null && rgbIV.Length > 0)
            {
                parameters = new ParametersWithIV(parameters, rgbIV);
            }
            return parameters;
        }

        /// <summary>
        /// 创建对称加密算法密钥。
        /// </summary>
        /// <param name="rgbKey">密钥数组。</param>
        /// <param name="rgbIV">初始化向量数组。不使用 IV 的模式可以传入 null。允许的长度参见模式说明。</param>
        /// <returns></returns>
        public static ICipherParameters GenerateSymmetricKey(byte[] rgbKey, byte[] rgbIV)
        {
            ICipherParameters parameters = new KeyParameter(rgbKey);
            if (rgbIV != null && rgbIV.Length > 0)
            {
                parameters = new ParametersWithIV(parameters, rgbIV);
            }
            return parameters;
        }

        #endregion 对称密钥
    }

    /// <summary>
    /// 证书辅助。
    /// </summary>
    public static class PemHelper
    {
        /// <summary>
        /// 将证书转换为 PEM 格式文本。
        /// </summary>
        /// <param name="cert">证书。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static string Cert2Pem(X509Certificate cert)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(cert);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 将证书请求转换为 PEM 格式文本。
        /// </summary>
        /// <param name="csr">证书请求。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static string Csr2Pem(Pkcs10CertificationRequest csr)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(csr);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 将密钥转换为 PEM 格式文本。
        /// </summary>
        /// <param name="key">密钥。</param>]
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static string Key2Pem(AsymmetricKeyParameter key)
        {
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(key);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 使用密码保护，将私钥转换为 PEM 格式文本。
        /// </summary>
        /// <param name="privateKey">私钥。</param>
        /// <param name="pemEncryptionAlgorithm">加密算法。可使用 NamedPemEncryptionAlgorithms 类中提供的已命名算法。名称是 OpenSSL 定义的 DEK 格式。</param>
        /// <param name="password">设置密码。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        public static string Key2Pem(AsymmetricKeyParameter privateKey, string pemEncryptionAlgorithm, string password)
        {
            if (privateKey is null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (privateKey.IsPrivate)
            {
                using (var writer = new StringWriter())
                {
                    var pemWriter = new PemWriter(writer);
                    pemWriter.WriteObject(privateKey, pemEncryptionAlgorithm, password.ToCharArray(), Common.SecureRandom);
                    return writer.ToString();
                }
            }
            else
            {
                throw new ArgumentException("不是私钥。", nameof(privateKey));
            }
        }

        /// <summary>
        /// 将 PEM 格式文本转换为证书。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static X509Certificate Pem2Cert(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as X509Certificate;
            }
        }

        /// <summary>
        /// 将 PEM 格式文本转换为证书请求。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static Pkcs10CertificationRequest Pem2Csr(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as Pkcs10CertificationRequest;
            }
        }

        /// <summary>
        /// 将 PEM 格式文本转换为密钥。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static AsymmetricKeyParameter Pem2Key(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as AsymmetricKeyParameter;
            }
        }

        /// <summary>
        /// 将 PEM 格式文本转换为密钥对。
        /// </summary>
        /// <param name="pem">PEM 格式文本。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static AsymmetricCipherKeyPair Pem2KeyPair(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader).ReadObject();
                return obj as AsymmetricCipherKeyPair;
            }
        }

        /// <summary>
        /// 指定密码，将 PEM 格式文本转换为密钥对。
        /// </summary>
        /// <param name="pem">带加密标签的 PEM 格式文本。</param>
        /// <param name="password">设置密码。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static AsymmetricCipherKeyPair Pem2KeyPair(string pem, string password)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (var reader = new StringReader(pem))
            {
                var obj = new PemReader(reader, new Password(password)).ReadObject();
                return obj as AsymmetricCipherKeyPair;
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
    }

    /// <summary>
    /// 签名辅助。
    /// </summary>
    public static class SignatureHelper
    {
        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="algorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <returns></returns>
        public static byte[] Sign(string algorithmName, AsymmetricKeyParameter privateKey, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Sign(algorithmName, privateKey, data, 0, data.Length);
        }

        /// <summary>
        /// 签名。
        /// </summary>
        /// <param name="algorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="privateKey">本地私钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <returns></returns>
        public static byte[] Sign(string algorithmName, AsymmetricKeyParameter privateKey, byte[] buffer, int offset, int count)
        {
            var signer = SignerUtilities.InitSigner(algorithmName, true, privateKey, Common.SecureRandom);
            signer.BlockUpdate(buffer, offset, count);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="algorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="data">要计算签名的数据。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        public static bool Verify(string algorithmName, AsymmetricKeyParameter publicKey, byte[] data, byte[] signature)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Verify(algorithmName, publicKey, data, 0, data.Length, signature);
        }

        /// <summary>
        /// 验证签名。
        /// </summary>
        /// <param name="algorithmName">签名算法。可使用 NamedSignatureAlgorithms 类中提供的常用算法。参见注释中的补充说明。</param>
        /// <param name="publicKey">对方发送的公钥。</param>
        /// <param name="buffer">包含要计算签名的数据的缓冲区。</param>
        /// <param name="offset">缓冲区偏移。</param>
        /// <param name="count">从缓冲区读取的字节数。</param>
        /// <param name="signature">对方发送的签名。</param>
        /// <returns></returns>
        public static bool Verify(string algorithmName, AsymmetricKeyParameter publicKey, byte[] buffer, int offset, int count, byte[] signature)
        {
            //var verifier = SignerUtilities.InitSigner(signatureAlgorithmOid, false, publicKey, null);
            var verifier = SignerUtilities.GetSigner(algorithmName);
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
    public sealed class X509NameGenerator
    {
        #region 成员

        private readonly Dictionary<DerObjectIdentifier, string> _attributes = new Dictionary<DerObjectIdentifier, string>();

        /// <summary>
        /// 创建器中是否有值。
        /// </summary>
        public bool IsEmpty => _attributes.Count == 0;

        #endregion 成员

        /// <summary>
        /// 增加 X509Name 属性。
        /// </summary>
        /// <param name="oid">属性 oid。</param>
        /// <param name="value">属性值。</param>
        public void AddX509Name(DerObjectIdentifier oid, string value)
        {
            _attributes.Add(oid, value);
        }

        /// <summary>
        /// 创建 X509Name。
        /// </summary>
        /// <returns></returns>
        public X509Name Generate()
        {
            var ordering = new DerObjectIdentifier[_attributes.Count];
            _attributes.Keys.CopyTo(ordering, 0);
            return new X509Name(ordering, _attributes);
        }

        /// <summary>
        /// 清除所有值。
        /// </summary>
        public void Reset()
        {
            _attributes.Clear();
        }
    }

    #endregion 扩展

    #region Common

    /// <summary>
    /// 公共对象。
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 随机数生成器。
        /// </summary>
        public static readonly SecureRandom SecureRandom = new SecureRandom();
    }

    /// <summary>
    /// 已命名非对称加密算法设置。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class NamedAsymmetricCipherStrings
    {
        public const string RSA_OAEP = "RSA//OAEPPADDING";
        public const string RSA_PKCS1 = "RSA//PKCS1PADDING";
    }

    /// <summary>
    /// 常用曲线。
    /// </summary>
    public static class NamedCurves
    {
        public const string SECP256R1 = "SecP256r1";
        public const string SECP384R1 = "SecP384r1";
        public const string SECP521R1 = "SecP521r1";
    }

    /// <summary>
    /// 已命名校验算法。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class NamedHashAlgorithms
    {
        public const string MD5 = "MD5";
        public const string SHA1 = "SHA1";
        public const string SHA256 = "SHA256";
        public const string SHA3_256 = "SHA3-256";
        public const string SHA3_384 = "SHA3-384";
        public const string SHA3_512 = "SHA3-512";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";
        public const string SM3 = "SM3";
    }

    /// <summary>
    /// 已命名 PEM 加密算法。OpenSSL 定义的 DEK 格式。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class NamedPemEncryptionAlgorithms
    {
        public const string AES_128_CBC = "AES-128-CBC";
        public const string AES_128_ECB = "AES-128-ECB";
        public const string AES_192_CBC = "AES-192-CBC";
        public const string AES_192_ECB = "AES-192-ECB";
        public const string AES_256_CBC = "AES-256-CBC";
        public const string AES_256_ECB = "AES-256-ECB";
        public const string DES_CBC = "DES-CBC";
        public const string DES_ECB = "DES-ECB";
        public const string DES_EDE3_CBC = "DES-EDE3-CBC";
        public const string DES_EDE3_ECB = "DES-EDE3-ECB";
    }

    /// <summary>
    /// 已命名签名算法。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class NamedSignatureAlgorithms
    {
        public const string SHA256_WITH_DSA = "SHA256WithDSA";
        public const string SHA256_WITH_ECDSA = "1.2.840.10045.4.3.2";
        public const string SHA256_WITH_RSA = "SHA256WithRSA";
        public const string SHA384_WITH_DSA = "SHA384WithDSA";
        public const string SHA384_WITH_ECDSA = "SHA384WithECDSA";
        public const string SHA384_WITH_RSA = "SHA384WithRSA";
        public const string SHA512_WITH_DSA = "SHA512WithDSA";
        public const string SHA512_WITH_ECDSA = "SHA512WithECDSA";
        public const string SHA512_WITH_RSA = "SHA512WithRSA";
        public const string SM3_WITH_SM2 = "1.2.156.10197.1.501";
    }

    /// <summary>
    /// 已命名对称加密算法设置。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class NamedSymmetricCipherStrings
    {
        public const string AES_CBC_PKCS7 = "AES/CBC/PKCS7PADDING";
        public const string AES_ECB_PKCS7 = "AES/ECB/PKCS7PADDING";
        public const string DES_CBC_PKCS7 = "DES/CBC/PKCS7PADDING";
        public const string DES_ECB_PKCS7 = "DES/ECB/PKCS7PADDING";
        public const string DESEDE_CBC_PKCS7 = "DESEDE/CBC/PKCS7PADDING";
        public const string DESEDE_ECB_PKCS7 = "DESEDE/ECB/PKCS7PADDING";
        public const string SM4_CBC_PKCS7 = "SM4/CBC/PKCS7PADDING";
        public const string SM4_ECB_PKCS7 = "SM4/ECB/PKCS7PADDING";
    }

    #endregion Common
}