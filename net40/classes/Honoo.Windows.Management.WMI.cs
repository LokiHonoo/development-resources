/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Xml;
using System.Xml.Linq;

namespace Honoo.Windows.Management
{
    /// <summary>
    /// Win32Class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
    internal enum Win32Class
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

        Win32_1394Controller,
        Win32_1394ControllerDevice,
        Win32_Account,
        Win32_AccountSID,
        Win32_ACE,
        Win32_ActionCheck,
        Win32_ActiveRoute,
        Win32_AllocatedResource,
        Win32_ApplicationCommandLine,
        Win32_ApplicationService,
        Win32_AssociatedBattery,
        Win32_AssociatedProcessorMemory,
        Win32_AutochkSetting,
        Win32_BaseBoard,
        Win32_BaseService,
        Win32_Battery,
        Win32_Binary,
        Win32_BindImageAction,
        Win32_BIOS,
        Win32_BootConfiguration,
        Win32_Bus,
        Win32_CacheMemory,
        Win32_CDROMDrive,
        Win32_CheckCheck,
        Win32_CIMLogicalDeviceCIMDataFile,
        Win32_ClassicCOMApplicationClasses,
        Win32_ClassicCOMClass,
        Win32_ClassicCOMClassSetting,
        Win32_ClassicCOMClassSettings,
        Win32_ClassInfoAction,
        Win32_ClientApplicationSetting,
        Win32_CodecFile,
        Win32_CollectionStatistics,
        Win32_COMApplication,
        Win32_COMApplicationClasses,
        Win32_COMApplicationSettings,
        Win32_COMClass,
        Win32_ComClassAutoEmulator,
        Win32_ComClassEmulator,
        Win32_CommandLineAccess,
        Win32_ComponentCategory,
        Win32_ComputerShutdownEvent,
        Win32_ComputerSystem,
        Win32_ComputerSystemEvent,
        Win32_ComputerSystemProcessor,
        Win32_ComputerSystemProduct,
        Win32_ComputerSystemWindowsProductActivationSetting,
        Win32_COMSetting,
        Win32_Condition,
        Win32_ConnectionShare,
        Win32_ControllerHasHub,
        Win32_CreateFolderAction,
        Win32_CurrentProbe,
        Win32_CurrentTime,
        Win32_DCOMApplication,
        Win32_DCOMApplicationAccessAllowedSetting,
        Win32_DCOMApplicationLaunchAllowedSetting,
        Win32_DCOMApplicationSetting,
        Win32_DefragAnalysis,
        Win32_DependentService,
        Win32_Desktop,
        Win32_DesktopMonitor,
        Win32_DeviceBus,
        Win32_DeviceChangeEvent,
        Win32_DeviceMemoryAddress,
        Win32_DeviceSettings,
        Win32_DFSNode,
        Win32_DFSNodeTarget,
        Win32_DFSTarget,
        Win32_Directory,
        Win32_DirectorySpecification,
        Win32_DiskDrive,
        Win32_DiskDrivePhysicalMedia,
        Win32_DiskDriveToDiskPartition,
        Win32_DiskPartition,
        Win32_DiskQuota,
        Win32_DisplayConfiguration,
        Win32_DisplayControllerConfiguration,
        Win32_DMAChannel,
        Win32_DriverForDevice,
        Win32_DriverVXD,
        Win32_DuplicateFileAction,
        Win32_Environment,
        Win32_EnvironmentSpecification,
        Win32_ExtensionInfoAction,
        Win32_Fan,
        Win32_FileSpecification,
        Win32_FloppyController,
        Win32_FloppyDrive,
        Win32_FontInfoAction,
        Win32_Group,
        Win32_GroupInDomain,
        Win32_GroupUser,
        Win32_HeatPipe,
        Win32_IDEController,
        Win32_IDEControllerDevice,
        Win32_ImplementedCategory,
        Win32_InfraredDevice,
        Win32_IniFileSpecification,
        Win32_InstalledSoftwareElement,
        Win32_IP4PersistedRouteTable,
        Win32_IP4RouteTable,
        Win32_IP4RouteTableEvent,
        Win32_IRQResource,
        Win32_JobObjectStatus,
        Win32_Keyboard,
        Win32_LaunchCondition,
        Win32_LoadOrderGroup,
        Win32_LoadOrderGroupServiceDependencies,
        Win32_LoadOrderGroupServiceMembers,
        Win32_LocalTime,
        Win32_LoggedOnUser,
        Win32_LogicalDisk,
        Win32_LogicalDiskRootDirectory,
        Win32_LogicalDiskToPartition,
        Win32_LogicalFileAccess,
        Win32_LogicalFileAuditing,
        Win32_LogicalFileGroup,
        Win32_LogicalFileOwner,
        Win32_LogicalFileSecuritySetting,
        Win32_LogicalMemoryConfiguration,
        Win32_LogicalProgramGroup,
        Win32_LogicalProgramGroupDirectory,
        Win32_LogicalProgramGroupItem,
        Win32_LogicalProgramGroupItemDataFile,
        Win32_LogicalShareAccess,
        Win32_LogicalShareAuditing,
        Win32_LogicalShareSecuritySetting,
        Win32_LogonSession,
        Win32_LogonSessionMappedDisk,
        Win32_LUID,
        Win32_LUIDandAttributes,
        Win32_ManagedSystemElementResource,
        Win32_MappedLogicalDisk,
        Win32_MemoryArray,
        Win32_MemoryArrayLocation,
        Win32_MemoryDevice,
        Win32_MemoryDeviceArray,
        Win32_MemoryDeviceLocation,
        Win32_MethodParameterClass,
        Win32_MIMEInfoAction,
        Win32_ModuleLoadTrace,
        Win32_ModuleTrace,
        Win32_MotherboardDevice,
        Win32_MountPoint,
        Win32_MoveFileAction,
        Win32_MSIResource,
        Win32_NamedJobObject,
        Win32_NamedJobObjectActgInfo,
        Win32_NamedJobObjectLimit,
        Win32_NamedJobObjectLimitSetting,
        Win32_NamedJobObjectProcess,
        Win32_NamedJobObjectSecLimit,
        Win32_NamedJobObjectSecLimitSetting,
        Win32_NamedJobObjectStatistics,
        Win32_NetworkAdapter,
        Win32_NetworkAdapterConfiguration,
        Win32_NetworkAdapterSetting,
        Win32_NetworkClient,
        Win32_NetworkConnection,
        Win32_NetworkLoginProfile,
        Win32_NetworkProtocol,
        Win32_NTDomain,
        Win32_NTEventlogFile,
        Win32_NTLogEvent,
        Win32_NTLogEventComputer,
        Win32_NTLogEventLog,
        Win32_NTLogEventUser,
        Win32_ODBCAttribute,
        Win32_ODBCDataSourceAttribute,
        Win32_ODBCDataSourceSpecification,
        Win32_ODBCDriverAttribute,
        Win32_ODBCDriverSoftwareElement,
        Win32_ODBCDriverSpecification,
        Win32_ODBCSourceAttribute,
        Win32_ODBCTranslatorSpecification,
        Win32_OnBoardDevice,
        Win32_OperatingSystem,
        Win32_OperatingSystemAutochkSetting,
        Win32_OperatingSystemQFE,
        Win32_OptionalFeature,
        Win32_OSRecoveryConfiguration,
        Win32_PageFile,
        Win32_PageFileElementSetting,
        Win32_PageFileSetting,
        Win32_PageFileUsage,
        Win32_ParallelPort,
        Win32_Patch,
        Win32_PatchFile,
        Win32_PatchPackage,
        Win32_PCMCIAController,
        Win32_Perf,
        Win32_PerfFormattedData,
        Win32_PerfFormattedData_ASP_ActiveServerPages,
        Win32_PerfFormattedData_ContentFilter_IndexingServiceFilter,
        Win32_PerfFormattedData_ContentIndex_IndexingService,
        Win32_PerfFormattedData_InetInfo_InternetInformationServicesGlobal,
        Win32_PerfFormattedData_ISAPISearch_HttpIndexingService,
        Win32_PerfFormattedData_MSDTC_DistributedTransactionCoordinator,
        Win32_PerfFormattedData_NTFSDRV_SMTPNTFSStoreDriver,
        Win32_PerfFormattedData_PerfDisk_LogicalDisk,
        Win32_PerfFormattedData_PerfDisk_PhysicalDisk,
        Win32_PerfFormattedData_PerfNet_Browser,
        Win32_PerfFormattedData_PerfNet_Redirector,
        Win32_PerfFormattedData_PerfNet_Server,
        Win32_PerfFormattedData_PerfNet_ServerWorkQueues,
        Win32_PerfFormattedData_PerfOS_Cache,
        Win32_PerfFormattedData_PerfOS_Memory,
        Win32_PerfFormattedData_PerfOS_Objects,
        Win32_PerfFormattedData_PerfOS_PagingFile,
        Win32_PerfFormattedData_PerfOS_Processor,
        Win32_PerfFormattedData_PerfOS_System,
        Win32_PerfFormattedData_PerfProc_FullImage_Costly,
        Win32_PerfFormattedData_PerfProc_Image_Costly,
        Win32_PerfFormattedData_PerfProc_JobObject,
        Win32_PerfFormattedData_PerfProc_JobObjectDetails,
        Win32_PerfFormattedData_PerfProc_Process,
        Win32_PerfFormattedData_PerfProc_ProcessAddressSpace_Costly,
        Win32_PerfFormattedData_PerfProc_Thread,
        Win32_PerfFormattedData_PerfProc_ThreadDetails_Costly,
        Win32_PerfFormattedData_PSched_PSchedFlow,
        Win32_PerfFormattedData_PSched_PSchedPipe,
        Win32_PerfFormattedData_RemoteAccess_RASPort,
        Win32_PerfFormattedData_RemoteAccess_RASTotal,
        Win32_PerfFormattedData_RSVP_ACSRSVPInterfaces,
        Win32_PerfFormattedData_RSVP_ACSRSVPService,
        Win32_PerfFormattedData_SMTPSVC_SMTPServer,
        Win32_PerfFormattedData_Spooler_PrintQueue,
        Win32_PerfFormattedData_TapiSrv_Telephony,
        Win32_PerfFormattedData_Tcpip_ICMP,
        Win32_PerfFormattedData_Tcpip_IP,
        Win32_PerfFormattedData_Tcpip_NBTConnection,
        Win32_PerfFormattedData_Tcpip_NetworkInterface,
        Win32_PerfFormattedData_Tcpip_TCP,
        Win32_PerfFormattedData_Tcpip_UDP,
        Win32_PerfFormattedData_TermService_TerminalServices,
        Win32_PerfFormattedData_TermService_TerminalServicesSession,
        Win32_PerfFormattedData_W3SVC_WebService,
        Win32_PerfRawData,
        Win32_PerfRawData_ASP_ActiveServerPages,
        Win32_PerfRawData_ContentFilter_IndexingServiceFilter,
        wWin32_PerfRawData_ContentIndex_IndexingService,
        Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal,
        Win32_PerfRawData_ISAPISearch_HttpIndexingService,
        Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator,
        Win32_PerfRawData_NTFSDRV_SMTPNTFSStoreDriver,
        Win32_PerfRawData_PerfDisk_LogicalDisk,
        Win32_PerfRawData_PerfDisk_PhysicalDisk,
        Win32_PerfRawData_PerfNet_Browser,
        Win32_PerfRawData_PerfNet_Redirector,
        Win32_PerfRawData_PerfNet_Server,
        Win32_PerfRawData_PerfNet_ServerWorkQueues,
        Win32_PerfRawData_PerfOS_Cache,
        Win32_PerfRawData_PerfOS_Memory,
        Win32_PerfRawData_PerfOS_Objects,
        Win32_PerfRawData_PerfOS_PagingFile,
        Win32_PerfRawData_PerfOS_Processor,
        Win32_PerfRawData_PerfOS_System,
        Win32_PerfRawData_PerfProc_FullImage_Costly,
        Win32_PerfRawData_PerfProc_Image_Costly,
        Win32_PerfRawData_PerfProc_JobObject,
        Win32_PerfRawData_PerfProc_JobObjectDetails,
        Win32_PerfRawData_PerfProc_Process,
        Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly,
        Win32_PerfRawData_PerfProc_Thread,
        Win32_PerfRawData_PerfProc_ThreadDetails_Costly,
        Win32_PerfRawData_PSched_PSchedFlow,
        Win32_PerfRawData_PSched_PSchedPipe,
        Win32_PerfRawData_RemoteAccess_RASPort,
        Win32_PerfRawData_RemoteAccess_RASTotal,
        Win32_PerfRawData_RSVP_ACSRSVPInterfaces,
        Win32_PerfRawData_RSVP_ACSRSVPService,
        Win32_PerfRawData_SMTPSVC_SMTPServer,
        Win32_PerfRawData_Spooler_PrintQueue,
        Win32_PerfRawData_TapiSrv_Telephony,
        Win32_PerfRawData_Tcpip_ICMP,
        Win32_PerfRawData_Tcpip_IP,
        Win32_PerfRawData_Tcpip_NBTConnection,
        Win32_PerfRawData_Tcpip_NetworkInterface,
        Win32_PerfRawData_Tcpip_TCP,
        Win32_PerfRawData_Tcpip_UDP,
        Win32_PerfRawData_TermService_TerminalServices,
        Win32_PerfRawData_TermService_TerminalServicesSession,
        Win32_PerfRawData_W3SVC_WebService,
        Win32_PhysicalMedia,
        Win32_PhysicalMemory,
        Win32_PhysicalMemoryArray,
        Win32_PhysicalMemoryLocation,
        Win32_PingStatus,
        Win32_PnPAllocatedResource,
        Win32_PnPDevice,
        Win32_PnPEntity,
        Win32_PnPSignedDriver,
        Win32_PnPSignedDriverCIMDataFile,
        Win32_PointingDevice,
        Win32_PortableBattery,
        Win32_PortConnector,
        Win32_PortResource,
        Win32_POTSModem,
        Win32_POTSModemToSerialPort,
        Win32_PowerManagementEvent,
        Win32_Printer,
        Win32_PrinterConfiguration,
        Win32_PrinterController,
        Win32_PrinterDriver,
        Win32_PrinterDriverDll,
        Win32_PrinterSetting,
        Win32_PrinterShare,
        Win32_PrintJob,
        Win32_PrivilegesStatus,
        Win32_Process,
        Win32_Processor,
        Win32_ProcessStartTrace,
        Win32_ProcessStartup,
        Win32_ProcessStopTrace,
        Win32_ProcessTrace,
        Win32_Product,
        Win32_ProductCheck,
        Win32_ProductResource,
        Win32_ProductSoftwareFeatures,
        Win32_ProgIDSpecification,
        Win32_ProgramGroup,
        Win32_ProgramGroupContents,
        Win32_ProgramGroupOrItem,
        Win32_Property,
        Win32_ProtocolBinding,
        Win32_Proxy,
        Win32_PublishComponentAction,
        Win32_QuickFixEngineering,
        Win32_QuotaSetting,
        Win32_Refrigeration,
        Win32_Registry,
        Win32_RegistryAction,
        Win32_ReliabilityRecords,
        Win32_ReliabilityStabilityMetrics,
        Win32_RemoveFileAction,
        Win32_RemoveIniAction,
        Win32_ReserveCost,
        Win32_ScheduledJob,
        Win32_SCSIController,
        Win32_SCSIControllerDevice,
        Win32_SecurityDescriptor,
        Win32_SecurityDescriptorHelper,
        Win32_SecuritySetting,
        Win32_SecuritySettingAccess,
        Win32_SecuritySettingAuditing,
        Win32_SecuritySettingGroup,
        Win32_SecuritySettingOfLogicalFile,
        Win32_SecuritySettingOfLogicalShare,
        Win32_SecuritySettingOfObject,
        Win32_SecuritySettingOwner,
        Win32_SelfRegModuleAction,
        Win32_SerialPort,
        Win32_SerialPortConfiguration,
        Win32_SerialPortSetting,
        Win32_ServerConnection,
        Win32_ServerFeature,
        Win32_ServerSession,
        Win32_Service,
        Win32_ServiceControl,
        Win32_ServiceSpecification,
        Win32_ServiceSpecificationService,
        Win32_Session,
        Win32_SessionConnection,
        Win32_SessionProcess,
        Win32_SettingCheck,
        Win32_ShadowBy,
        Win32_ShadowContext,
        Win32_ShadowCopy,
        Win32_ShadowDiffVolumeSupport,
        Win32_ShadowFor,
        Win32_ShadowOn,
        Win32_ShadowProvider,
        Win32_ShadowStorage,
        Win32_ShadowVolumeSupport,
        Win32_Share,
        Win32_ShareToDirectory,
        Win32_ShortcutAction,
        Win32_ShortcutFile,
        Win32_ShortcutSAP,
        Win32_SID,
        Win32_SIDandAttributes,
        Win32_SMBIOSMemory,
        Win32_SoftwareElement,
        Win32_SoftwareElementAction,
        Win32_SoftwareElementCheck,
        Win32_SoftwareElementCondition,
        Win32_SoftwareElementResource,
        Win32_SoftwareFeature,
        Win32_SoftwareFeatureAction,
        Win32_SoftwareFeatureCheck,
        Win32_SoftwareFeatureParent,
        Win32_SoftwareFeatureSoftwareElements,
        Win32_SoundDevice,
        Win32_StartupCommand,
        Win32_SubDirectory,
        Win32_SystemAccount,
        Win32_SystemBIOS,
        Win32_SystemBootConfiguration,
        Win32_SystemConfigurationChangeEvent,
        Win32_SystemDesktop,
        Win32_SystemDevices,
        Win32_SystemDriver,
        Win32_SystemDriverPNPEntity,
        Win32_SystemEnclosure,
        Win32_SystemLoadOrderGroups,
        Win32_SystemLogicalMemoryConfiguration,
        Win32_SystemMemoryResource,
        Win32_SystemNetworkConnections,
        Win32_SystemOperatingSystem,
        Win32_SystemPartitions,
        Win32_SystemProcesses,
        Win32_SystemProgramGroups,
        Win32_SystemResources,
        Win32_SystemServices,
        Win32_SystemSetting,
        Win32_SystemSlot,
        Win32_SystemSystemDriver,
        Win32_SystemTimeZone,
        Win32_SystemTrace,
        Win32_SystemUsers,
        Win32_TapeDrive,
        Win32_TCPIPPrinterPort,
        Win32_TemperatureProbe,
        Win32_Thread,
        Win32_ThreadStartTrace,
        Win32_ThreadStopTrace,
        Win32_ThreadTrace,
        Win32_TimeZone,
        Win32_TokenGroups,
        Win32_TokenPrivileges,
        Win32_Trustee,
        Win32_TypeLibraryAction,
        Win32_UninterruptiblePowerSupply,
        Win32_USBController,
        Win32_USBControllerDevice,
        Win32_USBHub,
        Win32_UserAccount,
        Win32_UserDesktop,
        Win32_UserInDomain,
        Win32_UserProfile,
        Win32_UTCTime,
        Win32_VideoConfiguration,
        Win32_VideoController,
        Win32_VideoSettings,
        Win32_VoltageProbe,
        Win32_Volume,
        Win32_VolumeChangeEvent,
        Win32_VolumeQuota,
        Win32_VolumeQuotaSetting,
        Win32_VolumeUserQuota,
        Win32_WindowsProductActivation,
        Win32_WMIElementSetting,
        Win32_WMISetting,

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }

    /// <summary>
    /// Windows Management Instrumentation.
    /// </summary>
    internal static class WMI
    {
        /// <summary>
        /// Query WMI data. Simple demo.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <param name="baseObjects"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static ManagementStatus Query(ManagementScope scope, ObjectQuery query, EnumerationOptions options, out ManagementObjectCollection baseObjects)
        {
            if (scope is null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            using (var mos = new ManagementObjectSearcher(scope, query, options))
            {
                try
                {
                    baseObjects = mos.Get();
                    return ManagementStatus.NoError;
                }
                catch (ManagementException ex)
                {
                    baseObjects = null;
                    return ex.ErrorCode;
                }
            }
        }

        /// <summary>
        /// Query WMI data. Simple demo.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <param name="format"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static ManagementStatus Query(ManagementScope scope, ObjectQuery query, EnumerationOptions options, TextFormat format, out string[] texts)
        {
            if (scope is null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var list = new List<string>();
            using (var mos = new ManagementObjectSearcher(scope, query, options))
            {
                try
                {
                    using (var moc = mos.Get())
                    {
                        if (moc.Count > 0)
                        {
                            foreach (var mbo in moc)
                            {
                                string text = mbo.GetText(format);
                                switch (format)
                                {
                                    case TextFormat.WmiDtd20:
                                        using (var reader = XmlReader.Create(new StringReader(text)))
                                        {
                                            XElement element = XElement.Load(reader);
                                            list.Add(element.ToString());
                                        }
                                        break;

                                    case TextFormat.CimDtd20:
                                        using (var reader = XmlReader.Create(new StringReader(text)))
                                        {
                                            XElement element = XElement.Load(reader);
                                            list.Add(element.ToString());
                                        }
                                        break;

                                    default:
                                        list.Add(text);
                                        break;
                                }
                            }
                        }
                        texts = list.ToArray();
                        return ManagementStatus.NoError;
                    }
                }
                catch (ManagementException ex)
                {
                    texts = null;
                    return ex.ErrorCode;
                }
            }
        }

        /// <summary>
        /// Query WMI data. Simple demo.
        /// </summary>
        /// <param name="win32Class"></param>
        /// <param name="timeout"></param>
        /// <param name="baseObjects"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static ManagementStatus Query(Win32Class win32Class, TimeSpan timeout, out ManagementObjectCollection baseObjects)
        {
            var scope = new ManagementScope();
            var query = new SelectQuery(win32Class.ToString());
            var options = new EnumerationOptions() { Timeout = timeout };
            return Query(scope, query, options, out baseObjects);
        }

        /// <summary>
        /// Query WMI data. Simple demo.
        /// </summary>
        /// <param name="win32Class"></param>
        /// <param name="timeout"></param>
        /// <param name="format"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static ManagementStatus Query(Win32Class win32Class, TimeSpan timeout, TextFormat format, out string[] texts)
        {
            var scope = new ManagementScope();
            var query = new SelectQuery(win32Class.ToString());
            var options = new EnumerationOptions() { Timeout = timeout };
            return Query(scope, query, options, format, out texts);
        }
    }
}