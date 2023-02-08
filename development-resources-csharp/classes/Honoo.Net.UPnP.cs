/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

/*
 * using (UPnP uPnP = new UPnP())
 * {
 *     UPnPDevice[] devices = await uPnP.Discover();
 *     UPnPService service = devices[0].FindServices(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1)[0];
 *     await uPnP.AddPortMapping(service, false, "TCP", 4788, IPAddress.Parse("192.168.1.1"), 4788, "test", 0, true);
 *     UPnPPortMappingEntry entry = await uPnP.GetSpecificPortMappingEntry(service, false, "TCP", 4788);
 *     await uPnP.DeletePortMapping(service, false, "TCP", 4788);
 * }
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP.
    /// </summary>
    public sealed class UPnP : IDisposable
    {
        #region Properties

        /// <summary>
        /// urn:schemas-upnp-org:device:InternetGatewayDevice:1
        /// </summary>
        public const string URN_UPNP_DEVICE_INTERNET_GATEWAY_DEVICE_1 = "urn:schemas-upnp-org:device:InternetGatewayDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:device:WANConnectionDevice:1
        /// </summary>
        public const string URN_UPNP_DEVICE_WAN_CONNECTION_DEVICE_1 = "urn:schemas-upnp-org:device:WANConnectionDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:device:WANDevice:1
        /// </summary>
        public const string URN_UPNP_DEVICE_WAN_DEVICE_1 = "urn:schemas-upnp-org:device:WANDevice:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:Layer3Forwarding:1
        /// </summary>
        public const string URN_UPNP_SERVICE_LAYER_3_FORWARDING_1 = "urn:schemas-upnp-org:service:Layer3Forwarding:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1
        /// </summary>
        public const string URN_UPNP_SERVICE_WAN_COMMON_INTERFACE_CONFIG_1 = "urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANIPConnection:1
        /// </summary>
        public const string URN_UPNP_SERVICE_WAN_IP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANIPConnection:1";

        /// <summary>
        /// urn:schemas-upnp-org:service:WANPPPConnection:1
        /// </summary>
        public const string URN_UPNP_SERVICE_WAN_PPP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANPPPConnection:1";

        private readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler() { AllowAutoRedirect = false, UseCookies = false });
        private bool _disposed = false;

        #endregion Properties

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnP class.
        /// </summary>
        public UPnP()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", new string[] { "Mozilla/5.0", "(Windows NT 6.1; Win64; x64; rv:47.0)", "Gecko/20100101", "Firefox/47.0" });
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-store");
            _httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        ~UPnP()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        /// <param name="disposing">Releases managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _httpClient.Dispose();
                _disposed = true;
            }
        }

        #endregion Construction

        #region Base

        /// <summary>
        /// Get the value of the specified argument from the download xml string.
        /// </summary>
        /// <param name="responseXmlString">Response xml string.</param>
        /// <param name="argument">Argument for device's service responses.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        private static string GetResponseValue(string responseXmlString, string argument)
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
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append HTTP Header "MAN" if throw 405 WebException. MAN value: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="action">action name.</param>
        /// <param name="arguments">action arguments. The arguments must conform to the order specified. Set 'null' if haven't arguments.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        private async Task<string> PostAction(UPnPService service, bool man, string action, params KeyValuePair<string, string>[] arguments)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            sb.AppendLine("  <s:Body>");
            sb.AppendLine("    <u:" + action + " xmlns:u=\"" + service.ServiceType + "\">");
            if (arguments != null && arguments.Length > 0)
            {
                foreach (KeyValuePair<string, string> argument in arguments)
                {
                    sb.AppendLine("      <" + argument.Key + ">" + argument.Value + "</" + argument.Key + ">");
                }
            }
            sb.AppendLine("    </u:" + action + ">");
            sb.AppendLine("  </s:Body>");
            sb.AppendLine("</s:Envelope>");
            sb.AppendLine();
            string body = sb.ToString();
            StringContent content = new StringContent(body, Encoding.UTF8);
            content.Headers.ContentType.MediaType = "text/xml";
            content.Headers.Add("SOAPAction", "\"" + service.ServiceType + "#" + action + "\"");
            if (man)
            {
                content.Headers.Add("MAN", "\"http://schemas.xmlsoap.org/soap/envelope/\"; ns=01");
            }
            HttpResponseMessage message = await _httpClient.PostAsync(service.BaseUrl.AbsoluteUri + service.ControlUrl.TrimStart('/'), content);
            string response = await message.Content.ReadAsStringAsync();
            if (!message.IsSuccessStatusCode)
            {
                message.EnsureSuccessStatusCode();
            }
            return response;
        }

        #endregion Base

        /// <summary>
        /// Add port mapping.
        /// </summary>
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="protocol">The protocol to mapping. This property accepts the following protocol: TCP, UDP.</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalIPAddress">The public IPAddress to mapping.</param>
        /// <param name="internalPort">The public port to mapping.</param>
        /// <param name="description">Mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <param name="enabled">Enable.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task AddPortMapping(UPnPService service,
                                         bool man,
                                         string protocol,
                                         int externalPort,
                                         IPAddress internalIPAddress,
                                         int internalPort,
                                         string description,
                                         int leaseDuration,
                                         bool enabled)
        {
            if (internalIPAddress is null)
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
            await PostAction(service, man, "AddPortMapping", arguments);
        }

        /// <summary>
        /// Delete port mapping.
        /// </summary>
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following protocol: TCP, UDP.</param>
        /// <param name="externalPort">The external port to delete mapping.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task DeletePortMapping(UPnPService service, bool man, string protocol, int externalPort)
        {
            KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("NewRemoteHost", string.Empty),
                new KeyValuePair<string, string>("NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewProtocol", protocol)
            };
            await PostAction(service, man, "DeletePortMapping", arguments);
        }

        /// <summary>
        /// Discover UPnP root devices.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<UPnPDevice[]> Discover()
        {
            List<UPnPDevice> devives = new List<UPnPDevice>();
            List<string> responses = new List<string>();
            using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                client.Client.ReceiveTimeout = 2000;
                client.Client.ReceiveBufferSize = 8 * 1024;
                client.Client.EnableBroadcast = true;
                client.Client.MulticastLoopback = false;
                StringBuilder request = new StringBuilder();
                request.AppendLine("M-SEARCH * HTTP/1.1");
                request.AppendLine("HOST: 239.255.255.250:1900");
                request.AppendLine("MAN: \"ssdp:discover\"");
                request.AppendLine("MX: 2");
                // request.AppendLine("ST: ssdp:all");
                // request.AppendLine("ST: upnp:rootdevice");
                // request.AppendLine("ST: uuid:xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
                request.AppendLine("ST: urn:schemas-upnp-org:service:WANIPConnection:1");
                request.AppendLine();
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
                    catch
                    {
                        break;
                    }
                }
            }
            if (responses.Count > 0)
            {
                foreach (var response in responses)
                {
                    if (response.Contains("HTTP/1.1 200 OK"))
                    {
                        string find = "LOCATION:";
                        int index = response.IndexOf(find, StringComparison.InvariantCulture);
                        if (index >= 0)
                        {
                            index += find.Length;
                            int count = response.IndexOf("\r\n", index, StringComparison.InvariantCulture) - index;
                            if (count > 0)
                            {
                                string descriptionUrl = response.Substring(index, count).Trim();
                                try
                                {
                                    string down = await _httpClient.GetStringAsync(descriptionUrl);
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(down);
                                    XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                                    nm.AddNamespace("ns", "urn:schemas-upnp-org:device-1-0");
                                    Uri uri = new Uri(doc.SelectSingleNode("/ns:root/ns:URLBase", nm).InnerText.Trim());
                                    XmlNode deviceNode = doc.SelectSingleNode("/ns:root/ns:device", nm);
                                    UPnPDevice device = new UPnPDevice(uri, deviceNode, nm);
                                    if (device != null)
                                    {
                                        devives.Add(device);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }

            return devives.ToArray();
        }

        /// <summary>
        /// Get external IPAddress.
        /// </summary>
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<IPAddress> GetExternalIPAddress(UPnPService service, bool man)
        {
            string down = await PostAction(service, man, "GetExternalIPAddress", null);
            string ip = GetResponseValue(down, "NewExternalIPAddress");
            return IPAddress.Parse(ip);
        }

        /// <summary>
        /// Get NAT RSIP status.
        /// </summary>
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<UPnPNatRsipStatus> GetNATRSIPStatus(UPnPService service, bool man)
        {
            string down = await PostAction(service, man, "GetNATRSIPStatus", null);
            return new UPnPNatRsipStatus(Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewRSIPAvailable"), CultureInfo.InvariantCulture)),
                                     Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewNATEnabled"), CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Get specific port mapping entry.
        /// </summary>
        /// <param name="service">Service "urn:schemas-upnp-org:service:WANIPConnection:1" or "urn:schemas-upnp-org:service:WANPPPConnection:1".</param>
        /// <param name="man">Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01</param>
        /// <param name="protocol">The protocol to query. This property accepts the following protocol: TCP, UDP.</param>
        /// <param name="externalPort">The external port to query.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<UPnPPortMappingEntry> GetSpecificPortMappingEntry(UPnPService service, bool man, string protocol, int externalPort)
        {
            KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("NewRemoteHost", string.Empty),
                new KeyValuePair<string, string>("NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("NewProtocol", protocol)
            };
            string down = await PostAction(service, man, "GetSpecificPortMappingEntry", arguments);
            return new UPnPPortMappingEntry(IPAddress.Parse(GetResponseValue(down, "NewInternalClient")),
                                            int.Parse(GetResponseValue(down, "NewInternalPort"), CultureInfo.InvariantCulture),
                                            GetResponseValue(down, "NewPortMappingDescription"),
                                            Convert.ToInt32(GetResponseValue(down, "NewLeaseDuration"), CultureInfo.InvariantCulture),
                                            Convert.ToBoolean(int.Parse(GetResponseValue(down, "NewEnabled"), CultureInfo.InvariantCulture))); ;
        }
    }

    #region Components

    /// <summary>
    /// UPnP device.
    /// </summary>
    public sealed class UPnPDevice
    {
        #region Properties

        /// <summary>
        /// Base url.
        /// </summary>
        public Uri BaseUrl { get; }

        /// <summary>
        /// Child devices.
        /// </summary>
        public UPnPDevice[] Devices { get; }

        /// <summary>
        /// Device type.
        /// </summary>
        public string DeviceType { get; }

        /// <summary>
        /// Friendly name.
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Manufacturer.
        /// </summary>
        public string Manufacturer { get; }

        /// <summary>
        /// Manufacturer url.
        /// </summary>
        public string ManufacturerUrl { get; }

        /// <summary>
        /// Model description.
        /// </summary>
        public string ModelDescription { get; }

        /// <summary>
        /// Model name.
        /// </summary>
        public string ModelName { get; }

        /// <summary>
        /// Model number.
        /// </summary>
        public string ModelNumber { get; }

        /// <summary>
        /// Model url.
        /// </summary>
        public string ModelUrl { get; }

        /// <summary>
        /// SerialNumber.
        /// </summary>
        public string SerialNumber { get; }

        /// <summary>
        /// Services.
        /// </summary>
        public UPnPService[] Services { get; }

        /// <summary>
        /// Unique device name.
        /// </summary>
        public string Udn { get; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPDevice class.
        /// </summary>
        /// <param name="baseUri">Base uri,</param>
        /// <param name="deviceNode">Device XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <exception cref="Exception"/>
        internal UPnPDevice(Uri baseUri, XmlNode deviceNode, XmlNamespaceManager nm)
        {
            List<UPnPDevice> devices = new List<UPnPDevice>();
            List<UPnPService> services = new List<UPnPService>();
            //
            this.BaseUrl = baseUri;
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
                devices.Add(new UPnPDevice(baseUri, childDeviceNode, nm));
            }
            //
            XmlNodeList serviceNodes = deviceNode.SelectNodes("ns:serviceList/ns:service", nm);
            foreach (XmlNode serviceNode in serviceNodes)
            {
                services.Add(new UPnPService(baseUri, serviceNode, nm));
            }
            //
            this.Devices = devices.ToArray();
            this.Services = services.ToArray();
        }

        /// <summary>
        /// Find the specified type of device.
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <returns></returns>
        public UPnPDevice[] FindDevices(string deviceType)
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
        public UPnPService[] FindServices(string serviceType)
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
    /// UPnP NAT RSIP status.
    /// </summary>
    public sealed class UPnPNatRsipStatus
    {
        #region Properties

        /// <summary>
        /// NAT enabled.
        /// </summary>
        public bool NatEnabled { get; }

        /// <summary>
        /// RSIP available.
        /// </summary>
        public bool RsipAvailable { get; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the NatRsipStatus class.
        /// </summary>
        /// <param name="rsipAvailable">RSIP available.</param>
        /// <param name="natEnabled">NAT enabled.</param>
        internal UPnPNatRsipStatus(bool rsipAvailable, bool natEnabled)
        {
            this.RsipAvailable = rsipAvailable;
            this.NatEnabled = natEnabled;
        }
    }

    /// <summary>
    /// UPnP port mapping entry.
    /// </summary>
    public sealed class UPnPPortMappingEntry
    {
        #region Properties

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Enabled.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Internal IPAddress.
        /// </summary>
        public IPAddress InternalIPAddress { get; }

        /// <summary>
        /// Internal port.
        /// </summary>
        public int InternalPort { get; }

        /// <summary>
        /// Lease duration. This property unit is seconds.
        /// </summary>
        public int LeaseDuration { get; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the PortMappingEntry class.
        /// </summary>
        /// <param name="internalIPAddress">Internal IPAddress.</param>
        /// <param name="internalPort">Internal port.</param>
        /// <param name="description">Description.</param>
        /// <param name="leaseDuration">Lease duration. This property unit is seconds.</param>
        /// <param name="enabled">Enabled.</param>
        internal UPnPPortMappingEntry(IPAddress internalIPAddress, int internalPort, string description, int leaseDuration, bool enabled)
        {
            this.InternalIPAddress = internalIPAddress;
            this.InternalPort = internalPort;
            this.Enabled = enabled;
            this.LeaseDuration = leaseDuration;
            this.Description = description;
        }
    }

    /// <summary>
    /// UPnP service.
    /// </summary>
    public sealed class UPnPService
    {
        #region Properties

        /// <summary>
        /// Base url.
        /// </summary>
        public Uri BaseUrl { get; }

        /// <summary>
        /// Control url.
        /// </summary>
        public string ControlUrl { get; }

        /// <summary>
        /// Event sub url.
        /// </summary>
        public string EventSubUrl { get; }

        /// <summary>
        /// Scpd url.
        /// </summary>
        public string ScpdUrl { get; }

        /// <summary>
        /// Service ID.
        /// </summary>
        public string ServiceID { get; }

        /// <summary>
        /// Service type.
        /// </summary>
        public string ServiceType { get; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPService class.
        /// </summary>
        /// <param name="baseUri">Base uri,</param>
        /// <param name="serviceNode">Service XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <exception cref="Exception"/>
        internal UPnPService(Uri baseUri, XmlNode serviceNode, XmlNamespaceManager nm)
        {
            this.BaseUrl = baseUri;
            this.ServiceType = serviceNode.SelectSingleNode("ns:serviceType", nm).InnerText.Trim();
            this.ServiceID = serviceNode.SelectSingleNode("ns:serviceId", nm).InnerText.Trim();
            this.ScpdUrl = serviceNode.SelectSingleNode("ns:SCPDURL", nm).InnerText.Trim();
            this.ControlUrl = serviceNode.SelectSingleNode("ns:controlURL", nm).InnerText.Trim();
            this.EventSubUrl = serviceNode.SelectSingleNode("ns:eventSubURL", nm).InnerText.Trim();
        }
    }

    #endregion Components
}