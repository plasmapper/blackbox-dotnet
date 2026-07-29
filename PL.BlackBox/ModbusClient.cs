using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox Modbus client.
    /// </summary>
    public class ModbusClient : Modbus.Client, IClient
    {
        private const string _blackBoxSignature = "PLBB";
        private const ushort _blackBoxMemoryMapVersion = 1;
        private bool _deviceCompatibilityChecked = false;

        private static class Coils
        {
            public const ushort Restart = 0;
            public const ushort SaveAllConfigurations = 1;
            public const ushort ClearRestartedBit = 16;

            public const ushort HardwareInterfaceEnabled = 100;
            public const ushort HardwareInterfaceIpV4DhcpClientEnabled = 101;
            public const ushort HardwareInterfaceIpV6DhcpClientEnabled = 102;

            public const ushort ServerEnabled = 200;
        }

        private static class HoldingRegisters
        {
            public const ushort DeviceName = 2;
            public const ushort DeviceNameCount = 16;

            public const ushort SelectedHardwareInterfaceIndex = 18;
            public const ushort SelectedServerIndex = 19;

            public const ushort HardwareInterfaceConfiguration = 100;
            public const ushort HardwareInterfaceConfigurationCount = 64;

            public const ushort ServerConfiguration = 200;
            public const ushort ServerConfigurationCount = 6;
        }

        private static class InputRegisters
        {
            public const ushort DeviceState = 0;
            public const ushort DeviceStateCount = 61;

            public const ushort HardwareInterfaceType = 102;

            public const ushort HardwareInterfaceState = 100;
            public const ushort HardwareInterfaceStateCount = 27;

            public const ushort ServerType = 202;

            public const ushort ServerState = 200;
            public const ushort ServerStateCount = 19;
        }

        // Offsets of individual fields within the 61-register device state block (InputRegisters.DeviceState).
        private static class DeviceStateOffsets
        {
            public const int StickyStatusBits = 1;
            public const int RestartedBit = 0x01;

            public const int BlackBoxSignature = 2;
            public const int BlackBoxSignatureCount = 2;
            
            public const int BlackBoxMemoryMapVersion = 4;
            
            public const int HardwareName = 5;
            public const int HardwareNameCount = 16;
            
            public const int HardwareVersionMajor = 21;
            public const int HardwareVersionMinor = 22;
            public const int HardwareVersionPatch = 23;
            
            public const int HardwareUid = 24;
            public const int HardwareUidCount = 16;
            
            public const int FirmwareName = 40;
            public const int FirmwareNameCount = 16;
            
            public const int FirmwareVersionMajor = 56;
            public const int FirmwareVersionMinor = 57;
            public const int FirmwareVersionPatch = 58;
            public const int NumberOfHardwareInterfaces = 59;
            public const int NumberOfServers = 60;
        }

        // Offsets of individual fields within the currently selected hardware interface's configuration block
        // (HoldingRegisters.HardwareInterfaceConfiguration). Some offsets are shared by fields of different
        // hardware interface types, since the device reuses the same registers for whichever fields apply.
        private static class HardwareInterfaceConfigurationOffsets
        {
            public const int ControlBits = 0;
            public const int EnabledBit = 0x01;
            public const int IpV4DhcpClientEnabledBit = 0x02;
            public const int IpV6DhcpClientEnabledBit = 0x04;

            public const int UartBaudRate = 2;
            public const int IpV4Address = 2;
            public const int IpV4Count = 2;

            public const int UartDataBits = 4;
            public const int IpV4Netmask = 4;
            public const int UartParity = 5;
            public const int UartStopBits = 6;
            public const int IpV4Gateway = 6;
            public const int UartFlowControl = 7;
            public const int IpV6GlobalAddress = 8;
            public const int IpV6GlobalAddressCount = 8;

            public const int WiFiSsid = 16;
            public const int WiFiSsidCount = 16;

            public const int WiFiPassword = 32;
            public const int WiFiPasswordCount = 32;
        }

        // Offsets of individual fields within the currently selected hardware interface's state block
        // (InputRegisters.HardwareInterfaceState).
        private static class HardwareInterfaceStateOffsets
        {
            public const int StatusBits = 0;
            public const int ConnectedBit = 0x01;

            public const int Name = 3;
            public const int NameCount = 16;

            public const int IpV6LocalAddress = 19;
            public const int IpV6LocalAddressCount = 8;
        }

        // Offsets of individual fields within the currently selected server's configuration block
        // (HoldingRegisters.ServerConfiguration). Some offsets are shared by fields of different server types,
        // since the device reuses the same registers for whichever fields apply.
        private static class ServerConfigurationOffsets
        {
            public const int ControlBits = 0;
            public const int EnabledBit = 0x01;

            public const int NetworkPort = 2;
            public const int ModbusProtocol = 2;
            public const int MaxNumberOfClients = 3;
            public const int ModbusStationAddress = 3;
            public const int NetworkModbusServerPort = 4;
            public const int NetworkModbusServerMaxNumberOfClients = 5;
        }

        // Offsets of individual fields within the currently selected server's state block (InputRegisters.ServerState).
        private static class ServerStateOffsets
        {
            public const int Name = 3;
            public const int NameCount = 16;
        }

        /// <summary>
        /// Initializes a new instance of the BlackBox Modbus client class.
        /// </summary>
        /// <param name="stream">Data stream. Share the same instance across multiple clients to talk to multiple devices over one connection.</param>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="stationAddress">Server station address.</param>
        public ModbusClient(Modbus.Stream stream, Modbus.Protocol protocol, byte stationAddress) :
            base(stream, protocol, stationAddress)
        { }

        /// <summary>
        /// Initializes a new instance of the BlackBox Modbus client class with a serial data stream.
        /// </summary>
        /// <param name="stream">Serial data stream. Share the same instance across multiple clients to talk to multiple devices on the same serial port.</param>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="stationAddress">Server station address.</param>
        public ModbusClient(Modbus.SerialStream stream, Modbus.Protocol protocol = Modbus.Protocol.Rtu, byte stationAddress = 1) :
            base(stream, protocol, stationAddress)
        { }

        /// <summary>
        /// Initializes a new instance of the BlackBox Modbus client class with a network data stream.
        /// </summary>
        /// <param name="stream">Network data stream. Share the same instance across multiple clients to talk to multiple devices over the same connection.</param>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="stationAddress">Server station address.</param>
        public ModbusClient(Modbus.NetworkStream stream, Modbus.Protocol protocol = Modbus.Protocol.Tcp, byte stationAddress = 255) :
            base(stream, protocol, stationAddress)
        { }

        /// <inheritdoc />
        protected override byte[] CommandCore(byte functionCode, byte[] data)
        {
            try
            {
                if (!_deviceCompatibilityChecked)
                    DeviceCompatibilityValidator(ReadDeviceState(false));

                return base.CommandCore(functionCode, data);
            }
            catch
            {
                _deviceCompatibilityChecked = false;
                throw;
            }
        }

        /// <inheritdoc />
        protected override async Task<byte[]> CommandCoreAsync(byte functionCode, byte[] data, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_deviceCompatibilityChecked)
                    DeviceCompatibilityValidator(await ReadDeviceStateAsync(false, cancellationToken).ConfigureAwait(false));

                return await base.CommandCoreAsync(functionCode, data, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _deviceCompatibilityChecked = false;
                throw;
            }
        }

        /// <inheritdoc />
        public DeviceConfiguration ReadDeviceConfiguration() =>
            new DeviceConfiguration() { Name = RegistersToString(ReadHoldingRegisters(HoldingRegisters.DeviceName, HoldingRegisters.DeviceNameCount)) };

        /// <inheritdoc />
        public async Task<DeviceConfiguration> ReadDeviceConfigurationAsync(CancellationToken cancellationToken = default) =>
            new DeviceConfiguration() { Name = RegistersToString(await ReadHoldingRegistersAsync(HoldingRegisters.DeviceName, HoldingRegisters.DeviceNameCount, cancellationToken).ConfigureAwait(false)) };

        /// <inheritdoc />
        public DeviceState ReadDeviceState() => ReadDeviceState(true);

        /// <inheritdoc />
        public Task<DeviceState> ReadDeviceStateAsync(CancellationToken cancellationToken = default) => ReadDeviceStateAsync(true, cancellationToken);

        /// <inheritdoc />
        public void Restart()
        {
            try
            {
                WriteSingleCoil(Coils.Restart, true);
            }
            catch { }
        }

        /// <inheritdoc />
        public async Task RestartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteSingleCoilAsync(Coils.Restart, true, cancellationToken).ConfigureAwait(false);
            }
            catch { }
        }

        /// <inheritdoc />
        public void SaveAllConfigurations() => WriteSingleCoil(Coils.SaveAllConfigurations, true);

        /// <inheritdoc />
        public Task SaveAllConfigurationsAsync(CancellationToken cancellationToken = default) => WriteSingleCoilAsync(Coils.SaveAllConfigurations, true, cancellationToken);

        /// <inheritdoc />
        public void ClearRestartedFlag() => WriteSingleCoil(Coils.ClearRestartedBit, true);

        /// <inheritdoc />
        public Task ClearRestartedFlagAsync(CancellationToken cancellationToken = default) => WriteSingleCoilAsync(Coils.ClearRestartedBit, true, cancellationToken);

        /// <inheritdoc />
        public string SetDeviceName(string deviceName)
        {
            using (var session = CreateSession())
            {
                session.WriteMultipleHoldingRegisters(HoldingRegisters.DeviceName, StringToRegisters(deviceName, HoldingRegisters.DeviceNameCount, nameof(deviceName)));
                return RegistersToString(session.ReadHoldingRegisters(HoldingRegisters.DeviceName, HoldingRegisters.DeviceNameCount));
            }
        }

        /// <inheritdoc />
        public async Task<string> SetDeviceNameAsync(string deviceName, CancellationToken cancellationToken = default)
        {
            using (var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false))
            {
                await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.DeviceName, StringToRegisters(deviceName, HoldingRegisters.DeviceNameCount, nameof(deviceName)), cancellationToken).ConfigureAwait(false);
                return RegistersToString(await session.ReadHoldingRegistersAsync(HoldingRegisters.DeviceName, HoldingRegisters.DeviceNameCount, cancellationToken).ConfigureAwait(false));
            }
        }

        /// <inheritdoc />
        public IHardwareInterface GetHardwareInterface(ushort index)
        {
            using (var session = CreateSession())
                return new HardwareInterface(this, index, SelectHardwareInterface(session, index));
        }

        /// <inheritdoc />
        public async Task<IHardwareInterface> GetHardwareInterfaceAsync(ushort index, CancellationToken cancellationToken = default)
        {
            using (var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                return new HardwareInterface(this, index, await SelectHardwareInterfaceAsync(session, index, cancellationToken).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public IServer GetServer(ushort index)
        {
            using (var session = CreateSession())
                return new Server(this, index, SelectServer(session, index));
        }

        /// <inheritdoc />
        public async Task<IServer> GetServerAsync(ushort index, CancellationToken cancellationToken = default)
        {
            using (var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                return new Server(this, index, await SelectServerAsync(session, index, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Checks if the connected device is compatible with the software and throws an exception if it is not.
        /// </summary>
        /// <param name="deviceState">Device state.</param>
        protected virtual void DeviceCompatibilityValidator(DeviceState deviceState)
        {
            if (deviceState.BlackBoxSignature != _blackBoxSignature || deviceState.BlackBoxMemoryMapVersion != _blackBoxMemoryMapVersion)
                throw new NotSupportedException($"The device is not a valid BlackBox device.");
            _deviceCompatibilityChecked = true;
        }

        // Called with checkCompatibility = false only while already inside CommandCore (i.e. while the session lock is
        // already held), so it must talk to the wire directly via base.CommandCore instead of going through
        // ReadInputRegisters, which would try to open a new session and deadlock.
        private DeviceState ReadDeviceState(bool checkCompatibility)
        {
            const byte startAddress = (byte)InputRegisters.DeviceState, registerCount = (byte)InputRegisters.DeviceStateCount;

            List<ushort> stateRegisters;
            if (!checkCompatibility)
            {
                byte[] commandData = new byte[4] { 0, startAddress, 0, registerCount };
                byte[] responseData = base.CommandCore((byte)Modbus.FunctionCode.ReadInputRegisters, commandData).Skip(1).ToArray();

                stateRegisters = new List<ushort>();
                int maxIndex = Math.Min(registerCount * 2, responseData.Length / 2 * 2);
                for (int j = 0; j < maxIndex; j += 2)
                {
                    Array.Reverse(responseData, j, 2);
                    stateRegisters.Add(BitConverter.ToUInt16(responseData, j));
                }
            }
            else
                stateRegisters = ReadInputRegisters(InputRegisters.DeviceState, InputRegisters.DeviceStateCount);

            return BuildDeviceState(stateRegisters);
        }

        // Called with checkCompatibility = false only while already inside CommandCoreAsync (i.e. while the session lock is
        // already held), so it must talk to the wire directly via base.CommandCoreAsync instead of going through
        // ReadInputRegistersAsync, which would try to open a new session and deadlock.
        private async Task<DeviceState> ReadDeviceStateAsync(bool checkCompatibility, CancellationToken cancellationToken)
        {
            const byte startAddress = (byte)InputRegisters.DeviceState, registerCount = (byte)InputRegisters.DeviceStateCount;

            List<ushort> stateRegisters;
            if (!checkCompatibility)
            {
                byte[] commandData = new byte[4] { 0, startAddress, 0, registerCount };
                byte[] responseData = (await base.CommandCoreAsync((byte)Modbus.FunctionCode.ReadInputRegisters, commandData, cancellationToken).ConfigureAwait(false)).Skip(1).ToArray();

                stateRegisters = new List<ushort>();
                int maxIndex = Math.Min(registerCount * 2, responseData.Length / 2 * 2);
                for (int j = 0; j < maxIndex; j += 2)
                {
                    Array.Reverse(responseData, j, 2);
                    stateRegisters.Add(BitConverter.ToUInt16(responseData, j));
                }
            }
            else
                stateRegisters = await ReadInputRegistersAsync(InputRegisters.DeviceState, InputRegisters.DeviceStateCount, cancellationToken).ConfigureAwait(false);

            return BuildDeviceState(stateRegisters);
        }

        private static DeviceState BuildDeviceState(List<ushort> stateRegisters) => new DeviceState()
        {
            Restarted = (stateRegisters[DeviceStateOffsets.StickyStatusBits] & DeviceStateOffsets.RestartedBit) != 0,
            BlackBoxSignature = RegistersToString(stateRegisters.Skip(DeviceStateOffsets.BlackBoxSignature).Take(DeviceStateOffsets.BlackBoxSignatureCount).ToList()),
            BlackBoxMemoryMapVersion = stateRegisters[DeviceStateOffsets.BlackBoxMemoryMapVersion],
            HardwareInfo = new HardwareInfo()
            {
                Name = RegistersToString(stateRegisters.Skip(DeviceStateOffsets.HardwareName).Take(DeviceStateOffsets.HardwareNameCount).ToList()),
                Version = new Version()
                {
                    Major = stateRegisters[DeviceStateOffsets.HardwareVersionMajor],
                    Minor = stateRegisters[DeviceStateOffsets.HardwareVersionMinor],
                    Patch = stateRegisters[DeviceStateOffsets.HardwareVersionPatch]
                },
                Uid = RegistersToString(stateRegisters.Skip(DeviceStateOffsets.HardwareUid).Take(DeviceStateOffsets.HardwareUidCount).ToList())
            },
            FirmwareInfo = new FirmwareInfo()
            {
                Name = RegistersToString(stateRegisters.Skip(DeviceStateOffsets.FirmwareName).Take(DeviceStateOffsets.FirmwareNameCount).ToList()),
                Version = new Version()
                {
                    Major = stateRegisters[DeviceStateOffsets.FirmwareVersionMajor],
                    Minor = stateRegisters[DeviceStateOffsets.FirmwareVersionMinor],
                    Patch = stateRegisters[DeviceStateOffsets.FirmwareVersionPatch]
                }
            },
            NumberOfHardwareInterfaces = stateRegisters[DeviceStateOffsets.NumberOfHardwareInterfaces],
            NumberOfServers = stateRegisters[DeviceStateOffsets.NumberOfServers]
        };

        private HardwareInterfaceType SelectHardwareInterface(Modbus.IClientSession session, ushort index)
        {
            session.WriteSingleHoldingRegister(HoldingRegisters.SelectedHardwareInterfaceIndex, index);
            if (session.ReadHoldingRegisters(HoldingRegisters.SelectedHardwareInterfaceIndex, 1)[0] != index)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Hardware interface index is out of range.");
            return (HardwareInterfaceType)session.ReadInputRegisters(InputRegisters.HardwareInterfaceType, 1)[0];
        }

        private async Task<HardwareInterfaceType> SelectHardwareInterfaceAsync(Modbus.IClientSession session, ushort index, CancellationToken cancellationToken)
        {
            await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.SelectedHardwareInterfaceIndex, index, cancellationToken).ConfigureAwait(false);
            if ((await session.ReadHoldingRegistersAsync(HoldingRegisters.SelectedHardwareInterfaceIndex, 1, cancellationToken).ConfigureAwait(false))[0] != index)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Hardware interface index is out of range.");
            return (HardwareInterfaceType)(await session.ReadInputRegistersAsync(InputRegisters.HardwareInterfaceType, 1, cancellationToken).ConfigureAwait(false))[0];
        }

        private ServerType SelectServer(Modbus.IClientSession session, ushort index)
        {
            session.WriteSingleHoldingRegister(HoldingRegisters.SelectedServerIndex, index);
            if (session.ReadHoldingRegisters(HoldingRegisters.SelectedServerIndex, 1)[0] != index)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Server index is out of range.");
            return (ServerType)session.ReadInputRegisters(InputRegisters.ServerType, 1)[0];
        }

        private async Task<ServerType> SelectServerAsync(Modbus.IClientSession session, ushort index, CancellationToken cancellationToken)
        {
            await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.SelectedServerIndex, index, cancellationToken).ConfigureAwait(false);
            if ((await session.ReadHoldingRegistersAsync(HoldingRegisters.SelectedServerIndex, 1, cancellationToken).ConfigureAwait(false))[0] != index)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Server index is out of range.");
            return (ServerType)(await session.ReadInputRegistersAsync(InputRegisters.ServerType, 1, cancellationToken).ConfigureAwait(false))[0];
        }

        private class HardwareInterface : IHardwareInterface
        {
            private static readonly HardwareInterfaceType[] _networkInterfaceTypes = { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation };

            private readonly ModbusClient _client;
            private readonly ushort _index;

            public HardwareInterface(ModbusClient client, ushort index, HardwareInterfaceType type)
            {
                _client = client;
                _index = index;
                Type = type;
            }

            public ushort Index => _index;

            public HardwareInterfaceType Type { get; }

            public HardwareInterfaceConfiguration ReadConfiguration()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildConfiguration(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration, HoldingRegisters.HardwareInterfaceConfigurationCount));
                }
            }

            public async Task<HardwareInterfaceConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildConfiguration(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration, HoldingRegisters.HardwareInterfaceConfigurationCount, cancellationToken).ConfigureAwait(false));
                }
            }

            private HardwareInterfaceConfiguration BuildConfiguration(List<ushort> configurationRegisters)
            {
                var configuration = new HardwareInterfaceConfiguration
                {
                    HardwareInterfaceType = Type,
                    IsEnabled = (configurationRegisters[HardwareInterfaceConfigurationOffsets.ControlBits] & HardwareInterfaceConfigurationOffsets.EnabledBit) != 0
                };

                if (Type == HardwareInterfaceType.Uart)
                {
                    configuration.UartBaudRate = RegistersToUint32(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.UartBaudRate).Take(2).ToList());
                    configuration.UartDataBits = configurationRegisters[HardwareInterfaceConfigurationOffsets.UartDataBits];
                    configuration.UartParity = (UartParity)configurationRegisters[HardwareInterfaceConfigurationOffsets.UartParity];
                    configuration.UartStopBits = (UartStopBits)configurationRegisters[HardwareInterfaceConfigurationOffsets.UartStopBits];
                    configuration.UartFlowControl = (UartFlowControl)configurationRegisters[HardwareInterfaceConfigurationOffsets.UartFlowControl];
                }

                if (_networkInterfaceTypes.Contains(Type))
                {
                    configuration.IpV4DhcpClientIsEnabled = (configurationRegisters[HardwareInterfaceConfigurationOffsets.ControlBits] & HardwareInterfaceConfigurationOffsets.IpV4DhcpClientEnabledBit) != 0;
                    configuration.IpV6DhcpClientIsEnabled = (configurationRegisters[HardwareInterfaceConfigurationOffsets.ControlBits] & HardwareInterfaceConfigurationOffsets.IpV6DhcpClientEnabledBit) != 0;
                    configuration.IpV4Address = RegistersToIpV4Address(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.IpV4Address).Take(HardwareInterfaceConfigurationOffsets.IpV4Count).ToList());
                    configuration.IpV4Netmask = RegistersToIpV4Address(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.IpV4Netmask).Take(HardwareInterfaceConfigurationOffsets.IpV4Count).ToList());
                    configuration.IpV4Gateway = RegistersToIpV4Address(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.IpV4Gateway).Take(HardwareInterfaceConfigurationOffsets.IpV4Count).ToList());
                    configuration.IpV6GlobalAddress = RegistersToIpV6Address(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.IpV6GlobalAddress).Take(HardwareInterfaceConfigurationOffsets.IpV6GlobalAddressCount).ToList());
                }

                if (Type == HardwareInterfaceType.WifiStation)
                    configuration.WiFiSsid = RegistersToString(configurationRegisters.Skip(HardwareInterfaceConfigurationOffsets.WiFiSsid).Take(HardwareInterfaceConfigurationOffsets.WiFiSsidCount).ToList());

                return configuration;
            }

            public HardwareInterfaceState ReadState()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildState(session.ReadInputRegisters(InputRegisters.HardwareInterfaceState, InputRegisters.HardwareInterfaceStateCount));
                }
            }

            public async Task<HardwareInterfaceState> ReadStateAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildState(await session.ReadInputRegistersAsync(InputRegisters.HardwareInterfaceState, InputRegisters.HardwareInterfaceStateCount, cancellationToken).ConfigureAwait(false));
                }
            }

            private HardwareInterfaceState BuildState(List<ushort> stateRegisters)
            {
                var state = new HardwareInterfaceState
                {
                    HardwareInterfaceType = Type,
                    Name = RegistersToString(stateRegisters.Skip(HardwareInterfaceStateOffsets.Name).Take(HardwareInterfaceStateOffsets.NameCount).ToList())
                };

                if (_networkInterfaceTypes.Contains(Type))
                {
                    state.IsConnected = (stateRegisters[HardwareInterfaceStateOffsets.StatusBits] & HardwareInterfaceStateOffsets.ConnectedBit) != 0;
                    state.IpV6LocalAddress = RegistersToIpV6Address(stateRegisters.Skip(HardwareInterfaceStateOffsets.IpV6LocalAddress).Take(HardwareInterfaceStateOffsets.IpV6LocalAddressCount).ToList());
                }

                return state;
            }

            public bool Enable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(Coils.HardwareInterfaceEnabled, true);
                    return session.ReadCoils(Coils.HardwareInterfaceEnabled, 1)[0];
                }
            }

            public async Task<bool> EnableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceEnabled, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool Disable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(Coils.HardwareInterfaceEnabled, false);
                    return session.ReadCoils(Coils.HardwareInterfaceEnabled, 1)[0];
                }
            }

            public async Task<bool> DisableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceEnabled, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public uint SetUartBaudRate(uint baudRate)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartBaudRate, Uint32ToRegisters(baudRate));
                    return RegistersToUint32(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartBaudRate, 2));
                }
            }

            public async Task<uint> SetUartBaudRateAsync(uint baudRate, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartBaudRate, Uint32ToRegisters(baudRate), cancellationToken).ConfigureAwait(false);
                    return RegistersToUint32(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartBaudRate, 2, cancellationToken).ConfigureAwait(false));
                }
            }

            public ushort SetUartDataBits(ushort dataBits)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartDataBits, dataBits);
                    return session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartDataBits, 1)[0];
                }
            }

            public async Task<ushort> SetUartDataBitsAsync(ushort dataBits, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartDataBits, dataBits, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartDataBits, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartParity SetUartParity(UartParity parity)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartParity, (ushort)parity);
                    return (UartParity)session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartParity, 1)[0];
                }
            }

            public async Task<UartParity> SetUartParityAsync(UartParity parity, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartParity, (ushort)parity, cancellationToken).ConfigureAwait(false);
                    return (UartParity)(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartParity, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartStopBits SetUartStopBits(UartStopBits stopBits)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartStopBits, (ushort)stopBits);
                    return (UartStopBits)session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartStopBits, 1)[0];
                }
            }

            public async Task<UartStopBits> SetUartStopBitsAsync(UartStopBits stopBits, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartStopBits, (ushort)stopBits, cancellationToken).ConfigureAwait(false);
                    return (UartStopBits)(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartStopBits, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartFlowControl SetUartFlowControl(UartFlowControl flowControl)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartFlowControl, (ushort)flowControl);
                    return (UartFlowControl)session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartFlowControl, 1)[0];
                }
            }

            public async Task<UartFlowControl> SetUartFlowControlAsync(UartFlowControl flowControl, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException("Type is not Uart.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartFlowControl, (ushort)flowControl, cancellationToken).ConfigureAwait(false);
                    return (UartFlowControl)(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.UartFlowControl, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool EnableIpV4DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteSingleCoil(Coils.HardwareInterfaceIpV4DhcpClientEnabled, true);
                    return session.ReadCoils(Coils.HardwareInterfaceIpV4DhcpClientEnabled, 1)[0];
                }
            }

            public async Task<bool> EnableIpV4DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceIpV4DhcpClientEnabled, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceIpV4DhcpClientEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool DisableIpV4DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteSingleCoil(Coils.HardwareInterfaceIpV4DhcpClientEnabled, false);
                    return session.ReadCoils(Coils.HardwareInterfaceIpV4DhcpClientEnabled, 1)[0];
                }
            }

            public async Task<bool> DisableIpV4DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceIpV4DhcpClientEnabled, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceIpV4DhcpClientEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool EnableIpV6DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteSingleCoil(Coils.HardwareInterfaceIpV6DhcpClientEnabled, true);
                    return session.ReadCoils(Coils.HardwareInterfaceIpV6DhcpClientEnabled, 1)[0];
                }
            }

            public async Task<bool> EnableIpV6DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceIpV6DhcpClientEnabled, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceIpV6DhcpClientEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool DisableIpV6DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteSingleCoil(Coils.HardwareInterfaceIpV6DhcpClientEnabled, false);
                    return session.ReadCoils(Coils.HardwareInterfaceIpV6DhcpClientEnabled, 1)[0];
                }
            }

            public async Task<bool> DisableIpV6DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteSingleCoilAsync(Coils.HardwareInterfaceIpV6DhcpClientEnabled, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.HardwareInterfaceIpV6DhcpClientEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public IPAddress SetIpV4Address(IPAddress ipV4Address)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Address, IpV4AddressToRegisters(ipV4Address));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Address, HardwareInterfaceConfigurationOffsets.IpV4Count));
                }
            }

            public async Task<IPAddress> SetIpV4AddressAsync(IPAddress ipV4Address, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Address, IpV4AddressToRegisters(ipV4Address), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Address, HardwareInterfaceConfigurationOffsets.IpV4Count, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV4Netmask(IPAddress ipV4Netmask)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Netmask, IpV4AddressToRegisters(ipV4Netmask));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Netmask, HardwareInterfaceConfigurationOffsets.IpV4Count));
                }
            }

            public async Task<IPAddress> SetIpV4NetmaskAsync(IPAddress ipV4Netmask, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Netmask, IpV4AddressToRegisters(ipV4Netmask), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Netmask, HardwareInterfaceConfigurationOffsets.IpV4Count, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV4Gateway(IPAddress ipV4Gateway)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Gateway, IpV4AddressToRegisters(ipV4Gateway));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Gateway, HardwareInterfaceConfigurationOffsets.IpV4Count));
                }
            }

            public async Task<IPAddress> SetIpV4GatewayAsync(IPAddress ipV4Gateway, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Gateway, IpV4AddressToRegisters(ipV4Gateway), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV4Gateway, HardwareInterfaceConfigurationOffsets.IpV4Count, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV6GlobalAddress(IPAddress ipV6GlobalAddress)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV6GlobalAddress, IpV6AddressToRegisters(ipV6GlobalAddress));
                    return RegistersToIpV6Address(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV6GlobalAddress, HardwareInterfaceConfigurationOffsets.IpV6GlobalAddressCount));
                }
            }

            public async Task<IPAddress> SetIpV6GlobalAddressAsync(IPAddress ipV6GlobalAddress, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException("Type is not NetworkInterface, Ethernet or WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV6GlobalAddress, IpV6AddressToRegisters(ipV6GlobalAddress), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV6Address(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.IpV6GlobalAddress, HardwareInterfaceConfigurationOffsets.IpV6GlobalAddressCount, cancellationToken).ConfigureAwait(false));
                }
            }

            public string SetWiFiSsid(string ssid)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException("Type is not WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiSsid, StringToRegisters(ssid, HardwareInterfaceConfigurationOffsets.WiFiSsidCount, nameof(ssid)));
                    return RegistersToString(session.ReadHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiSsid, HardwareInterfaceConfigurationOffsets.WiFiSsidCount));
                }
            }

            public async Task<string> SetWiFiSsidAsync(string ssid, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException("Type is not WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiSsid, StringToRegisters(ssid, HardwareInterfaceConfigurationOffsets.WiFiSsidCount, nameof(ssid)), cancellationToken).ConfigureAwait(false);
                    return RegistersToString(await session.ReadHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiSsid, HardwareInterfaceConfigurationOffsets.WiFiSsidCount, cancellationToken).ConfigureAwait(false));
                }
            }

            public void SetWiFiPassword(string password)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException("Type is not WifiStation.");
                    session.WriteMultipleHoldingRegisters(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiPassword, StringToRegisters(password, HardwareInterfaceConfigurationOffsets.WiFiPasswordCount, nameof(password)));
                }
            }

            public async Task SetWiFiPasswordAsync(string password, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException("Type is not WifiStation.");
                    await session.WriteMultipleHoldingRegistersAsync(HoldingRegisters.HardwareInterfaceConfiguration + HardwareInterfaceConfigurationOffsets.WiFiPassword, StringToRegisters(password, HardwareInterfaceConfigurationOffsets.WiFiPasswordCount, nameof(password)), cancellationToken).ConfigureAwait(false);
                }
            }

            private void Select(Modbus.IClientSession session)
            {
                var type = _client.SelectHardwareInterface(session, _index);
                if (type != Type)
                    throw new Exception("Hardware interface control instance is invalid.");
            }

            private async Task SelectAsync(Modbus.IClientSession session, CancellationToken cancellationToken)
            {
                var type = await _client.SelectHardwareInterfaceAsync(session, _index, cancellationToken).ConfigureAwait(false);
                if (type != Type)
                    throw new Exception("Hardware interface control instance is invalid.");
            }
        }

        private class Server : IServer
        {
            private static readonly ServerType[] _serverWithPortTypes = { ServerType.NetworkServer, ServerType.HttpServer, ServerType.MdnsServer };
            private static readonly ServerType[] _modbusServerTypes = { ServerType.StreamModbusServer, ServerType.NetworkModbusServer };

            private readonly ModbusClient _client;
            private readonly ushort _index;

            public Server(ModbusClient client, ushort index, ServerType type)
            {
                _client = client;
                _index = index;
                Type = type;
            }

            public ushort Index => _index;

            public ServerType Type { get; }

            public ServerConfiguration ReadConfiguration()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildConfiguration(session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration, HoldingRegisters.ServerConfigurationCount));
                }
            }

            public async Task<ServerConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildConfiguration(await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration, HoldingRegisters.ServerConfigurationCount, cancellationToken).ConfigureAwait(false));
                }
            }

            private ServerConfiguration BuildConfiguration(List<ushort> configurationRegisters)
            {
                var configuration = new ServerConfiguration
                {
                    ServerType = Type,
                    IsEnabled = (configurationRegisters[ServerConfigurationOffsets.ControlBits] & ServerConfigurationOffsets.EnabledBit) != 0
                };

                if (_serverWithPortTypes.Contains(Type))
                {
                    configuration.NetworkPort = configurationRegisters[ServerConfigurationOffsets.NetworkPort];
                    configuration.MaxNumberOfClients = configurationRegisters[ServerConfigurationOffsets.MaxNumberOfClients];
                }

                if (_modbusServerTypes.Contains(Type))
                {
                    configuration.ModbusProtocol = (ModbusProtocol)configurationRegisters[ServerConfigurationOffsets.ModbusProtocol];
                    configuration.ModbusStationAddress = (byte)configurationRegisters[ServerConfigurationOffsets.ModbusStationAddress];
                }

                if (Type == ServerType.NetworkModbusServer)
                {
                    configuration.NetworkPort = configurationRegisters[ServerConfigurationOffsets.NetworkModbusServerPort];
                    configuration.MaxNumberOfClients = configurationRegisters[ServerConfigurationOffsets.NetworkModbusServerMaxNumberOfClients];
                }

                return configuration;
            }

            public ServerState ReadState()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildState(session.ReadInputRegisters(InputRegisters.ServerState, InputRegisters.ServerStateCount));
                }
            }

            public async Task<ServerState> ReadStateAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildState(await session.ReadInputRegistersAsync(InputRegisters.ServerState, InputRegisters.ServerStateCount, cancellationToken).ConfigureAwait(false));
                }
            }

            private ServerState BuildState(List<ushort> stateRegisters) => new ServerState
            {
                ServerType = Type,
                Name = RegistersToString(stateRegisters.Skip(ServerStateOffsets.Name).Take(ServerStateOffsets.NameCount).ToList())
            };

            public bool Enable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(Coils.ServerEnabled, true);
                    return session.ReadCoils(Coils.ServerEnabled, 1)[0];
                }
            }

            public async Task<bool> EnableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(Coils.ServerEnabled, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.ServerEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool Disable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(Coils.ServerEnabled, false);
                    return session.ReadCoils(Coils.ServerEnabled, 1)[0];
                }
            }

            public async Task<bool> DisableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(Coils.ServerEnabled, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(Coils.ServerEnabled, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public ushort SetPort(ushort port)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkPort, port);
                        return session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkPort, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerPort, port);
                        return session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerPort, 1)[0];
                    }
                    throw new NotSupportedException("Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.");
                }
            }

            public async Task<ushort> SetPortAsync(ushort port, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkPort, port, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkPort, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerPort, port, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerPort, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    throw new NotSupportedException("Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.");
                }
            }

            public ushort SetMaxNumberOfClients(ushort maxNumberOfClients)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.MaxNumberOfClients, maxNumberOfClients);
                        return session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.MaxNumberOfClients, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerMaxNumberOfClients, maxNumberOfClients);
                        return session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerMaxNumberOfClients, 1)[0];
                    }
                    throw new NotSupportedException("Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.");
                }
            }

            public async Task<ushort> SetMaxNumberOfClientsAsync(ushort maxNumberOfClients, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.MaxNumberOfClients, maxNumberOfClients, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.MaxNumberOfClients, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerMaxNumberOfClients, maxNumberOfClients, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.NetworkModbusServerMaxNumberOfClients, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    throw new NotSupportedException("Type is not NetworkServer, NetworkModbusServer, HttpServer or MdnsServer.");
                }
            }

            public ModbusProtocol SetModbusProtocol(ModbusProtocol protocol)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException("Type is not StreamModbusServer or NetworkModbusServer.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusProtocol, (ushort)protocol);
                    return (ModbusProtocol)session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusProtocol, 1)[0];
                }
            }

            public async Task<ModbusProtocol> SetModbusProtocolAsync(ModbusProtocol protocol, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException("Type is not StreamModbusServer or NetworkModbusServer.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusProtocol, (ushort)protocol, cancellationToken).ConfigureAwait(false);
                    return (ModbusProtocol)(await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusProtocol, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public byte SetModbusStationAddress(byte stationAddress)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException("Type is not StreamModbusServer or NetworkModbusServer.");
                    session.WriteSingleHoldingRegister(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusStationAddress, stationAddress);
                    return (byte)session.ReadHoldingRegisters(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusStationAddress, 1)[0];
                }
            }

            public async Task<byte> SetModbusStationAddressAsync(byte stationAddress, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException("Type is not StreamModbusServer or NetworkModbusServer.");
                    await session.WriteSingleHoldingRegisterAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusStationAddress, stationAddress, cancellationToken).ConfigureAwait(false);
                    return (byte)(await session.ReadHoldingRegistersAsync(HoldingRegisters.ServerConfiguration + ServerConfigurationOffsets.ModbusStationAddress, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            private void Select(Modbus.IClientSession session)
            {
                var type = _client.SelectServer(session, _index);
                if (type != Type)
                    throw new Exception("Server control instance is invalid.");
            }

            private async Task SelectAsync(Modbus.IClientSession session, CancellationToken cancellationToken)
            {
                var type = await _client.SelectServerAsync(session, _index, cancellationToken).ConfigureAwait(false);
                if (type != Type)
                    throw new Exception("Server control instance is invalid.");
            }
        }

        private static string RegistersToString(List<ushort> registers)
        {
            byte[] byteArray = new byte[registers.Count * 2];
            Buffer.BlockCopy(registers.ToArray(), 0, byteArray, 0, registers.Count * 2);
            return Encoding.ASCII.GetString(byteArray).Split('\0')[0];
        }

        private static List<ushort> StringToRegisters(string stringValue, int numberOfRegisters, string paramName)
        {
            if (stringValue.Any(c => c > 127))
                throw new ArgumentException("Value must contain only ASCII characters.", paramName);

            byte[] stringValueAsByteArray = Encoding.ASCII.GetBytes(stringValue);
            if (stringValueAsByteArray.Length > numberOfRegisters * 2)
                throw new ArgumentException($"Value must be at most {numberOfRegisters * 2} characters long.", paramName);
            ushort[] registers = new ushort[numberOfRegisters];
            Buffer.BlockCopy(stringValueAsByteArray, 0, registers, 0, stringValueAsByteArray.Length);
            return new List<ushort>(registers);
        }

        private static uint RegistersToUint32(List<ushort> registers) =>
            registers[0] + ((uint)registers[1] << 16);

        private static List<ushort> Uint32ToRegisters(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            return new List<ushort> { (ushort)(bytes[0] + (bytes[1] << 8)), (ushort)(bytes[2] + (bytes[3] << 8)) };
        }

        private static IPAddress RegistersToIpV4Address(List<ushort> registers) =>
            new IPAddress(new byte[] { (byte)(registers[0] & 0xFF), (byte)(registers[0] >> 8), (byte)(registers[1] & 0xFF), (byte)(registers[1] >> 8) });

        private static List<ushort> IpV4AddressToRegisters(IPAddress value)
        {
            var bytes = value.GetAddressBytes();
            return new List<ushort> { (ushort)(bytes[0] + (bytes[1] << 8)), (ushort)(bytes[2] + (bytes[3] << 8)) };
        }

        private static IPAddress RegistersToIpV6Address(List<ushort> registers)
        {
            var bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i * 2] = (byte)(registers[i] & 0xFF);
                bytes[i * 2 + 1] = (byte)(registers[i] >> 8);
            }
            return new IPAddress(bytes);
        }

        private static List<ushort> IpV6AddressToRegisters(IPAddress value)
        {
            var bytes = value.GetAddressBytes();
            var registers = new List<ushort>();
            for (int i = 0; i < 8; i++)
                registers.Add((ushort)(bytes[i * 2] + (bytes[i * 2 + 1] << 8)));
            return registers;
        }
    }
}
