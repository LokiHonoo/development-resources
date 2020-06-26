﻿/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace LH.Net.UPnP
{
    /// <summary>
    /// UPnP.
    /// </summary>
    [SuppressMessage("Build", "CA1724:类型名 UPnP 与命名空间名称“LH.Net.UPnP”整体或部分冲突。请更改其中任一名称以消除冲突。", Justification = "<挂起>")]
    internal static class UPnP
    {
        /// <summary>
        /// urn:schemas-upnp-org:device:InternetGatewayDevice:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string DEVICE_TYPE_INTERNET_GATEWAY_DEVICE_1 = "urn:schemas-upnp-org:device:InternetGatewayDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:device:WANConnectionDevice:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string DEVICE_TYPE_WAN_CONNECTION_DEVICE_1 = "urn:schemas-upnp-org:device:WANConnectionDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:device:WANDevice:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string DEVICE_TYPE_WAN_DEVICE_1 = "urn:schemas-upnp-org:device:WANDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:Layer3Forwarding:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string SERVICE_TYPE_LAYER_3_FORWARDING_1 = "urn:schemas-upnp-org:service:Layer3Forwarding:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string SERVICE_TYPE_WAN_COMMON_INTERFACE_CONFIG_1 = "urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANIPConnection:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string SERVICE_TYPE_WAN_IP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANIPConnection:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANPPPConnection:1
        /// </summary>
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        internal const string SERVICE_TYPE_WAN_PPP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANPPPConnection:1";

        #region Base

        /// <summary>
        /// Get the value of the specified argument from the download xml string.
        /// </summary>
        /// <param name="responseXmlString">Response xml string.</param>
        /// <param name="argument">Argument for device's service responses.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>")]
        internal static string GetResponseValue(string responseXmlString, string argument)
        {
            if (string.IsNullOrEmpty(responseXmlString))
            {
                throw new ArgumentNullException(nameof(responseXmlString));
            }
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(nameof(argument));
            }
            int index1 = responseXmlString.IndexOf(argument, StringComparison.InvariantCulture);
            if (index1 >= 0)
            {
                index1 += argument.Length + 1;
                int index2 = responseXmlString.IndexOf(argument, index1, StringComparison.InvariantCulture);
                if (index2 >= index1)
                {
                    index2 -= 2;
                }
                return responseXmlString.Substring(index1, index2 - index1).Trim();
            }
            else
            {
                throw new Exception("Argument \"" + argument + "\"not found.");
            }
        }

        /// <summary>
        /// Post action, return download xml string. Query actions from service's SCPD xml.
        /// </summary>
        /// <param name="service">service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="action">action name.</param>
        /// <param name="arguments">action arguments. The arguments must conform to the order specified. Set 'null' if haven't arguments.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static string PostAction(UPnPService service, bool man, string action, params KeyValuePair<string, string>[] arguments)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            using (WebClient wc = new WebClient())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\"?>\r\n");
                sb.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n");
                sb.Append("  <s:Body>\r\n");
                sb.Append("    <u:" + action + " xmlns:u=\"" + service.ServiceType + "\">\r\n");
                if (arguments != null && arguments.Length > 0)
                {
                    foreach (KeyValuePair<string, string> argument in arguments)
                    {
                        sb.Append("      <" + argument.Key + ">" + argument.Value + "</" + argument.Key + ">\r\n");
                    }
                }
                sb.Append("    </u:" + action + ">\r\n");
                sb.Append("  </s:Body>\r\n");
                sb.Append("</s:Envelope>\r\n");
                byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0(WindowsNT6.1;rv:2.0.1)Gecko/20100101Firefox/4.0.1");
                wc.Headers.Add(HttpRequestHeader.CacheControl, "no-store");
                wc.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
                if (man)
                {
                    wc.Headers.Add("MAN", "\"http://schemas.xmlsoap.org/soap/envelope/\"; ns=01");
                }
                wc.Headers.Add("SOAPAction", "\"" + service.ServiceType + "#" + action + "\"");
                byte[] down = wc.UploadData(service.ControlUrl, "POST", data);
                return Encoding.UTF8.GetString(down);
            }
        }

        #endregion Base

        /// <summary>
        /// Add port mapping.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="protocol">The protocol to mapping. This property accepts the following protocol: TCP, UDP.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="internalIPAddress">The internal IPAddress to mapping.</param>
        /// <param name="enabled">Enable.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <param name="description">Mapping description.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        internal static void AddPortMapping(UPnPService service,
                                          bool man,
                                          int externalPort,
                                          string protocol,
                                          int internalPort,
                                          IPAddress internalIPAddress,
                                          bool enabled,
                                          int leaseDuration,
                                          string description)
        {
            if (internalIPAddress == null)
            {
                throw new ArgumentNullException(nameof(internalIPAddress));
            }
            KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("NewRemoteHost", string.Empty),
                new KeyValuePair<string, string>("NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewProtocol", protocol),
                new KeyValuePair<string, string>("NewInternalPort", internalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewInternalClient", internalIPAddress.ToString()),
                new KeyValuePair<string, string>("NewEnabled", (enabled ? "1" : "0")),
                new KeyValuePair<string, string>("NewLeaseDuration", leaseDuration.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewPortMappingDescription", description)
            };
            PostAction(service, man, "AddPortMapping", arguments);
        }

        /// <summary>
        /// Delete port mapping.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="externalPort">The external port to delete mapping.</param>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following protocol: TCP, UDP.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        internal static void DeletePortMapping(UPnPService service, bool man, int externalPort, string protocol)
        {
            KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("NewRemoteHost", string.Empty),
                new KeyValuePair<string, string>("NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewProtocol", protocol)
            };
            PostAction(service, man, "DeletePortMapping", arguments);
        }

        /// <summary>
        /// Discover UPnP root devices.
        /// </summary>
        /// <param name="addressFamilyFilter">Search for devices of the specified address family.</param>
        /// <param name="mx">Maximum search time. Unit is seconds.</param>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        internal static UPnPRootDevice[] Discover(AddressFamily addressFamilyFilter, int mx)
        {
            List<UPnPRootDevice> dis = new List<UPnPRootDevice>();
            List<string> responses = new List<string>();
            using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Client.ReceiveTimeout = mx * 1000;
                client.Client.ReceiveBufferSize = 8 * 1024;
                client.Client.EnableBroadcast = true;
                client.Client.MulticastLoopback = true;
                client.Client.Ttl = 1;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                StringBuilder request = new StringBuilder();
                request.Append("M-SEARCH * HTTP/1.1\r\n");
                request.Append("HOST: 239.255.255.250:1900\r\n");
                request.Append("MAN: \"ssdp:discover\"\r\n");
                request.Append("MX: " + mx + "\r\n");
                request.Append("ST: upnp:rootdevice\r\n");
                request.Append("\r\n");
                byte[] data = Encoding.UTF8.GetBytes(request.ToString());
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                client.Send(data, data.Length, ep);

                while (true)
                {
                    try
                    {
                        byte[] received = client.Receive(ref ep);
                        responses.Add(Encoding.UTF8.GetString(received));
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            }
            if (responses.Count > 0)
            {
                using (WebClient wc = new WebClient())
                {
                    foreach (string response in responses)
                    {
                        if (response.Contains("200 OK"))
                        {
                            string descriptionUrl;
                            string find;
                            int index;
                            int count;
                            find = "LOCATION:";
                            index = response.IndexOf(find, StringComparison.InvariantCulture);
                            if (index >= 0)
                            {
                                index += find.Length;
                                count = response.IndexOf("\r\n", index, StringComparison.InvariantCulture) - index;
                                if (count > 0)
                                {
                                    descriptionUrl = response.Substring(index, count).Trim();
                                    try
                                    {
                                        Uri uri = new Uri(descriptionUrl);
                                        IPAddress ipAddress = IPAddress.Parse(uri.Host);
                                        if ((addressFamilyFilter & ipAddress.AddressFamily) != ipAddress.AddressFamily)
                                        {
                                            continue;
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                            bool exist = false;
                            foreach (UPnPRootDevice di in dis)
                            {
                                if (descriptionUrl == di.DescriptionUrl)
                                {
                                    exist = true;
                                    break;
                                }
                            }
                            if (!exist)
                            {
                                try
                                {
                                    byte[] down = wc.DownloadData(descriptionUrl);
                                    UPnPRootDevice di = new UPnPRootDevice(descriptionUrl, Encoding.UTF8.GetString(down));
                                    if (di == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        dis.Add(di);
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            return dis.ToArray();
        }

        /// <summary>
        /// Get external(internet) IPAddress.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        internal static IPAddress GetExternalIPAddress(UPnPService service, bool man)
        {
            string down = PostAction(service, man, "GetExternalIPAddress", null);
            string ip = GetResponseValue(down, "NewExternalIPAddress");
            return IPAddress.Parse(ip);
        }

        /// <summary>
        /// Get NAT RSIP status.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        internal static NatRsipStatus GetNATRSIPStatus(UPnPService service, bool man)
        {
            string down = PostAction(service, man, "GetNATRSIPStatus", null);
            return new NatRsipStatus(Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewRSIPAvailable"), CultureInfo.InvariantCulture)),
                                     Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewNATEnabled"), CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Get specific port mapping entry.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="index">The mapping index.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details.", Justification = "<Pending>")]
        internal static PortMappingEntry GetSpecificPortMappingEntry(UPnPService service, bool man, int index)
        {
            try
            {
                string down = PostAction(service, man, "GetSpecificPortMappingEntry", new KeyValuePair<string, string>("NewPortMappingIndex", index.ToString(CultureInfo.InvariantCulture)));
                return new PortMappingEntry(true,
                    IPAddress.Parse(GetResponseValue(down, "NewInternalClient")),
                    int.Parse(GetResponseValue(down, "NewInternalPort"), CultureInfo.InvariantCulture),
                    Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewEnabled"), CultureInfo.InvariantCulture)),
                    GetResponseValue(down, "NewPortMappingDescription"));
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.InternalServerError)
                {
                    return new PortMappingEntry(false, null, 0, false, string.Empty);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Get specific port mapping entry.
        /// </summary>
        /// <param name="service">Router WANIPConnection/WANPPPConnection service.</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="externalPort">The external port to query.</param>
        /// <param name="protocol">The protocol to query. This property accepts the following protocol: TCP, UDP.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        internal static PortMappingEntry GetSpecificPortMappingEntry(UPnPService service, bool man, int externalPort, string protocol)
        {
            KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("NewRemoteHost", string.Empty),
                new KeyValuePair<string, string>("NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewProtocol", protocol)
            };
            try
            {
                string down = PostAction(service, man, "GetSpecificPortMappingEntry", arguments);
                return new PortMappingEntry(true,
                    IPAddress.Parse(GetResponseValue(down, "NewInternalClient")),
                    int.Parse(GetResponseValue(down, "NewInternalPort"), CultureInfo.InvariantCulture),
                    Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewEnabled"), CultureInfo.InvariantCulture)),
                    GetResponseValue(down, "NewPortMappingDescription"));
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.InternalServerError)
                {
                    return new PortMappingEntry(false, null, 0, false, string.Empty);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    #region Components

    /// <summary>
    /// NAT RSIP status.
    /// </summary>
    internal sealed class NatRsipStatus
    {
        #region Members

        /// <summary>
        /// NAT enabled.
        /// </summary>
        internal bool NatEnabled { get; }

        /// <summary>
        /// RSIP available.
        /// </summary>
        internal bool RsipAvailable { get; }

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the NatRsipStatus class.
        /// </summary>
        /// <param name="rsipAvailable">RSIP available.</param>
        /// <param name="natEnabled">NAT enabled.</param>
        internal NatRsipStatus(bool rsipAvailable, bool natEnabled)
        {
            this.RsipAvailable = rsipAvailable;
            this.NatEnabled = natEnabled;
        }
    }

    /// <summary>
    /// Port mapping entry.
    /// </summary>
    internal sealed class PortMappingEntry
    {
        #region Members

        /// <summary>
        /// Description.
        /// </summary>
        internal string Description { get; }

        /// <summary>
        /// Enabled.
        /// </summary>
        internal bool Enabled { get; }

        /// <summary>
        /// Internal IPAddress.
        /// </summary>
        internal IPAddress InternalIPAddress { get; }

        /// <summary>
        /// Internal port.
        /// </summary>
        internal int InternalPort { get; }

        /// <summary>
        /// Mapped.
        /// </summary>
        internal bool Mapped { get; }

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the PortMappingEntry class.
        /// </summary>
        /// <param name="mapped">Mapped.</param>
        /// <param name="internalIPAddress">Internal IPAddress.</param>
        /// <param name="internalPort">Internal port.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Description.</param>
        internal PortMappingEntry(bool mapped, IPAddress internalIPAddress, int internalPort, bool enabled, string description)
        {
            this.Mapped = mapped;
            this.InternalIPAddress = internalIPAddress;
            this.InternalPort = internalPort;
            this.Enabled = enabled;
            this.Description = description;
        }
    }

    /// <summary>
    /// UPnP device.
    /// </summary>
    internal sealed class UPnPDevice
    {
        #region Members

        /// <summary>
        /// Child devices.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        internal UPnPDevice[] Devices { get; }

        /// <summary>
        /// Device type.
        /// </summary>
        internal string DeviceType { get; }

        /// <summary>
        /// Friendly name.
        /// </summary>
        internal string FriendlyName { get; }

        /// <summary>
        /// Manufacturer.
        /// </summary>
        internal string Manufacturer { get; }

        /// <summary>
        /// Manufacturer url.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string ManufacturerUrl { get; }

        /// <summary>
        /// Model description.
        /// </summary>
        internal string ModelDescription { get; }

        /// <summary>
        /// Model name.
        /// </summary>
        internal string ModelName { get; }

        /// <summary>
        /// Model number.
        /// </summary>
        internal string ModelNumber { get; }

        /// <summary>
        /// Model url.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string ModelUrl { get; }

        /// <summary>
        /// Parent device.
        /// </summary>
        /// <exception cref="Exception"/>
        internal UPnPDevice Parent { get; }

        /// <summary>
        /// Root device info.
        /// </summary>
        internal UPnPRootDevice Root { get; }

        /// <summary>
        /// SerialNumber.
        /// </summary>
        internal string SerialNumber { get; }

        /// <summary>
        /// Services.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        internal UPnPService[] Services { get; }

        /// <summary>
        /// Unique device name.
        /// </summary>
        internal string Udn { get; }

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the Device class.
        /// </summary>
        /// <param name="uri">Must specify device base uri, Because the description file does not contain uri.</param>
        /// <param name="root">Root device info.</param>
        /// <param name="parent">Parent device.</param>
        /// <param name="deviceNode">Device XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <exception cref="Exception"/>
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        internal UPnPDevice(Uri uri, UPnPRootDevice root, UPnPDevice parent, XmlNode deviceNode, XmlNamespaceManager nm)
        {
            if (deviceNode == null)
            {
                throw new ArgumentNullException(nameof(deviceNode));
            }
            List<UPnPDevice> childDevices = new List<UPnPDevice>();
            List<UPnPService> services = new List<UPnPService>();
            //
            this.Root = root;
            this.Parent = parent;
            this.DeviceType = deviceNode.SelectSingleNode("ns:deviceType", nm).InnerText.Trim();
            this.FriendlyName = deviceNode.SelectSingleNode("ns:friendlyName", nm).InnerText.Trim();
            this.Manufacturer = deviceNode.SelectSingleNode("ns:manufacturer", nm).InnerText.Trim();
            this.ManufacturerUrl = deviceNode.SelectSingleNode("ns:manufacturerURL", nm).InnerText.Trim();
            this.ModelDescription = deviceNode.SelectSingleNode("ns:modelDescription", nm).InnerText.Trim();
            this.ModelName = deviceNode.SelectSingleNode("ns:modelName", nm).InnerText.Trim();
            this.ModelNumber = deviceNode.SelectSingleNode("ns:modelNumber", nm).InnerText.Trim();
            this.ModelUrl = deviceNode.SelectSingleNode("ns:modelURL", nm).InnerText.Trim();
            this.SerialNumber = deviceNode.SelectSingleNode("ns:serialNumber", nm).InnerText.Trim();
            this.Udn = deviceNode.SelectSingleNode("ns:UDN", nm).InnerText.Replace("uuid:", string.Empty).Trim();
            XmlNodeList childDeviceNodes = deviceNode.SelectNodes("ns:deviceList/ns:device", nm);
            //
            foreach (XmlNode childDeviceNode in childDeviceNodes)
            {
                childDevices.Add(new UPnPDevice(uri, root, this, childDeviceNode, nm));
            }
            //
            XmlNodeList serviceNodes = deviceNode.SelectNodes("ns:serviceList/ns:service", nm);
            foreach (XmlNode serviceNode in serviceNodes)
            {
                services.Add(new UPnPService(uri, root, this, serviceNode, nm));
            }
            //
            this.Devices = childDevices.ToArray();
            this.Services = services.ToArray();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} \"{1}\"", this.FriendlyName, this.DeviceType);
        }

        /// <summary>
        /// Find the specified type of device.
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <returns></returns>
        internal UPnPDevice[] FindDevices(string deviceType)
        {
            List<UPnPDevice> devices = new List<UPnPDevice>();
            if (this.DeviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase))
            {
                devices.Add(this);
            }
            foreach (UPnPDevice childDevice in this.Devices)
            {
                devices.AddRange(childDevice.FindDevices(deviceType));
            }
            return devices.ToArray();
        }

        /// <summary>
        /// Find the specified type of service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <returns></returns>
        internal UPnPService[] FindServices(string serviceType)
        {
            List<UPnPService> services = new List<UPnPService>();
            foreach (UPnPService service in this.Services)
            {
                if (service.ServiceType.Equals(serviceType, StringComparison.OrdinalIgnoreCase))
                {
                    services.Add(service);
                }
            }
            foreach (UPnPDevice childDevice in this.Devices)
            {
                services.AddRange(childDevice.FindServices(serviceType));
            }
            return services.ToArray();
        }
    }

    /// <summary>
    /// UPnP root device.
    /// </summary>
    internal sealed class UPnPRootDevice
    {
        #region Members

        /// <summary>
        /// Description Url. This parameter is used only for records.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string DescriptionUrl { get; }

        /// <summary>
        /// Root device escription xml string.
        /// </summary>
        internal string DescriptionXmlString { get; }

        /// <summary>
        /// Root device.
        /// </summary>
        internal UPnPDevice Device { get; }

        /// <summary>
        /// Root device base uri.
        /// </summary>
        internal Uri Uri { get; }

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the RootDevice class.
        /// </summary>
        /// <param name="descriptionUrl">Specify root device escription full url.</param>
        /// <param name="descriptionXmlString">Root device escription xml string.</param>
        /// <exception cref="Exception"/>
        [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "<Pending>")]
        internal UPnPRootDevice(string descriptionUrl, string descriptionXmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(descriptionXmlString);
            XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
            nm.AddNamespace("ns", "urn:schemas-upnp-org:device-1-0");
            XmlNode deviceNode = doc.SelectSingleNode("/ns:root/ns:device", nm);
            Uri uri = new Uri(descriptionUrl);
            this.Uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", uri.Scheme, uri.Authority));
            this.DescriptionUrl = descriptionUrl;
            this.DescriptionXmlString = descriptionXmlString;
            this.Device = new UPnPDevice(uri, this, null, deviceNode, nm);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} \"{1}\"", this.Device.FriendlyName, this.Device.DeviceType);
        }

        /// <summary>
        /// Find the specified type of device.
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <returns></returns>
        internal UPnPDevice[] FindDevices(string deviceType)
        {
            return this.Device.FindDevices(deviceType);
        }

        /// <summary>
        /// Find the specified type of service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <returns></returns>
        internal UPnPService[] FindServices(string serviceType)
        {
            return this.Device.FindServices(serviceType);
        }
    }

    /// <summary>
    /// UPnP service.
    /// </summary>
    internal sealed class UPnPService
    {
        #region Members

        /// <summary>
        /// Control url.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string ControlUrl { get; }

        /// <summary>
        /// Event sub url.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string EventSubUrl { get; }

        /// <summary>
        /// Parent device.
        /// </summary>
        /// <exception cref="Exception"/>
        internal UPnPDevice Parent { get; }

        /// <summary>
        /// Root device info.
        /// </summary>
        internal UPnPRootDevice Root { get; }

        /// <summary>
        /// Scpd url.
        /// </summary>
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        internal string ScpdUrl { get; }

        /// <summary>
        /// Service ID.
        /// </summary>
        internal string ServiceID { get; }

        /// <summary>
        /// Service type.
        /// </summary>
        internal string ServiceType { get; }

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the Service class.
        /// </summary>
        /// <param name="uri">Must specify device base uri, Because the description file does not contain uri.</param>
        /// <param name="root">Root device.</param>
        /// <param name="parent">Parent device.</param>
        /// <param name="serviceNode">Service XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <exception cref="Exception"/>
        internal UPnPService(Uri uri, UPnPRootDevice root, UPnPDevice parent, XmlNode serviceNode, XmlNamespaceManager nm)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (serviceNode == null)
            {
                throw new ArgumentNullException(nameof(serviceNode));
            }
            this.Root = root;
            this.Parent = parent;
            this.ServiceType = serviceNode.SelectSingleNode("ns:serviceType", nm).InnerText.Trim();
            this.ServiceID = serviceNode.SelectSingleNode("ns:serviceId", nm).InnerText.Trim();
            this.ScpdUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}", uri.AbsoluteUri, serviceNode.SelectSingleNode("ns:SCPDURL", nm).InnerText.Trim().Trim('/'));
            this.ControlUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}", uri.AbsoluteUri, serviceNode.SelectSingleNode("ns:controlURL", nm).InnerText.Trim().Trim('/'));
            this.EventSubUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}", uri.AbsoluteUri, serviceNode.SelectSingleNode("ns:eventSubURL", nm).InnerText.Trim().Trim('/'));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} \"{1}\"", this.ServiceID, this.ServiceType);
        }
    }

    #endregion Components
}