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
            new DeviceConfiguration() { Name = RegistersToString(ReadHoldingRegisters(2, 16)) };

        /// <inheritdoc />
        public async Task<DeviceConfiguration> ReadDeviceConfigurationAsync(CancellationToken cancellationToken = default) =>
            new DeviceConfiguration() { Name = RegistersToString(await ReadHoldingRegistersAsync(2, 16, cancellationToken).ConfigureAwait(false)) };

        /// <inheritdoc />
        public DeviceState ReadDeviceState() => ReadDeviceState(true);

        /// <inheritdoc />
        public Task<DeviceState> ReadDeviceStateAsync(CancellationToken cancellationToken = default) => ReadDeviceStateAsync(true, cancellationToken);

        /// <inheritdoc />
        public void Restart()
        {
            try
            {
                WriteSingleCoil(0, true);
            }
            catch { }
        }

        /// <inheritdoc />
        public async Task RestartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteSingleCoilAsync(0, true, cancellationToken).ConfigureAwait(false);
            }
            catch { }
        }

        /// <inheritdoc />
        public void SaveAllConfigurations() => WriteSingleCoil(1, true);

        /// <inheritdoc />
        public Task SaveAllConfigurationsAsync(CancellationToken cancellationToken = default) => WriteSingleCoilAsync(1, true, cancellationToken);

        /// <inheritdoc />
        public void ClearRestartedFlag() => WriteSingleCoil(16, true);

        /// <inheritdoc />
        public Task ClearRestartedFlagAsync(CancellationToken cancellationToken = default) => WriteSingleCoilAsync(16, true, cancellationToken);

        /// <inheritdoc />
        public string SetDeviceName(string deviceName)
        {
            using (var session = CreateSession())
            {
                session.WriteMultipleHoldingRegisters(2, StringToRegisters(deviceName, 16));
                return RegistersToString(session.ReadHoldingRegisters(2, 16));
            }
        }

        /// <inheritdoc />
        public async Task<string> SetDeviceNameAsync(string deviceName, CancellationToken cancellationToken = default)
        {
            using (var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false))
            {
                await session.WriteMultipleHoldingRegistersAsync(2, StringToRegisters(deviceName, 16), cancellationToken).ConfigureAwait(false);
                return RegistersToString(await session.ReadHoldingRegistersAsync(2, 16, cancellationToken).ConfigureAwait(false));
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
            const byte startAddress = 0, registerCount = 61;

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
                stateRegisters = ReadInputRegisters(0, 61);

            return BuildDeviceState(stateRegisters);
        }

        // Called with checkCompatibility = false only while already inside CommandCoreAsync (i.e. while the session lock is
        // already held), so it must talk to the wire directly via base.CommandCoreAsync instead of going through
        // ReadInputRegistersAsync, which would try to open a new session and deadlock.
        private async Task<DeviceState> ReadDeviceStateAsync(bool checkCompatibility, CancellationToken cancellationToken)
        {
            const byte startAddress = 0, registerCount = 61;

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
                stateRegisters = await ReadInputRegistersAsync(0, 61, cancellationToken).ConfigureAwait(false);

            return BuildDeviceState(stateRegisters);
        }

        private static DeviceState BuildDeviceState(List<ushort> stateRegisters) => new DeviceState()
        {
            Restarted = (stateRegisters[1] & 0x01) != 0,
            BlackBoxSignature = RegistersToString(stateRegisters.Skip(2).Take(2).ToList()),
            BlackBoxMemoryMapVersion = stateRegisters[4],
            HardwareInfo = new HardwareInfo()
            {
                Name = RegistersToString(stateRegisters.Skip(5).Take(16).ToList()),
                Version = new Version()
                {
                    Major = stateRegisters[21],
                    Minor = stateRegisters[22],
                    Patch = stateRegisters[23]
                },
                Uid = RegistersToString(stateRegisters.Skip(24).Take(16).ToList())
            },
            FirmwareInfo = new FirmwareInfo()
            {
                Name = RegistersToString(stateRegisters.Skip(40).Take(16).ToList()),
                Version = new Version()
                {
                    Major = stateRegisters[56],
                    Minor = stateRegisters[57],
                    Patch = stateRegisters[58]
                }
            },
            NumberOfHardwareInterfaces = stateRegisters[59],
            NumberOfServers = stateRegisters[60]
        };

        private HardwareInterfaceType SelectHardwareInterface(Modbus.IClientSession session, ushort index)
        {
            session.WriteSingleHoldingRegister(18, index);
            if (session.ReadHoldingRegisters(18, 1)[0] != index)
                throw new Exception("Selecting hardware interface failed.");
            return (HardwareInterfaceType)session.ReadInputRegisters(102, 1)[0];
        }

        private async Task<HardwareInterfaceType> SelectHardwareInterfaceAsync(Modbus.IClientSession session, ushort index, CancellationToken cancellationToken)
        {
            await session.WriteSingleHoldingRegisterAsync(18, index, cancellationToken).ConfigureAwait(false);
            if ((await session.ReadHoldingRegistersAsync(18, 1, cancellationToken).ConfigureAwait(false))[0] != index)
                throw new Exception("Selecting hardware interface failed.");
            return (HardwareInterfaceType)(await session.ReadInputRegistersAsync(102, 1, cancellationToken).ConfigureAwait(false))[0];
        }

        private ServerType SelectServer(Modbus.IClientSession session, ushort index)
        {
            session.WriteSingleHoldingRegister(19, index);
            if (session.ReadHoldingRegisters(19, 1)[0] != index)
                throw new Exception("Selecting server failed.");
            return (ServerType)session.ReadInputRegisters(202, 1)[0];
        }

        private async Task<ServerType> SelectServerAsync(Modbus.IClientSession session, ushort index, CancellationToken cancellationToken)
        {
            await session.WriteSingleHoldingRegisterAsync(19, index, cancellationToken).ConfigureAwait(false);
            if ((await session.ReadHoldingRegistersAsync(19, 1, cancellationToken).ConfigureAwait(false))[0] != index)
                throw new Exception("Selecting server failed.");
            return (ServerType)(await session.ReadInputRegistersAsync(202, 1, cancellationToken).ConfigureAwait(false))[0];
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

            public HardwareInterfaceType Type { get; }

            public HardwareInterfaceConfiguration ReadConfiguration()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildConfiguration(session.ReadHoldingRegisters(100, 64));
                }
            }

            public async Task<HardwareInterfaceConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildConfiguration(await session.ReadHoldingRegistersAsync(100, 64, cancellationToken).ConfigureAwait(false));
                }
            }

            private HardwareInterfaceConfiguration BuildConfiguration(List<ushort> configurationRegisters)
            {
                var configuration = new HardwareInterfaceConfiguration
                {
                    HardwareInterfaceType = Type,
                    IsEnabled = (configurationRegisters[0] & 0x01) != 0
                };

                if (Type == HardwareInterfaceType.Uart)
                {
                    configuration.UartBaudRate = RegistersToUint32(configurationRegisters.Skip(2).Take(2).ToList());
                    configuration.UartDataBits = configurationRegisters[4];
                    configuration.UartParity = (UartParity)configurationRegisters[5];
                    configuration.UartStopBits = (UartStopBits)configurationRegisters[6];
                    configuration.UartFlowControl = (UartFlowControl)configurationRegisters[7];
                }

                if (_networkInterfaceTypes.Contains(Type))
                {
                    configuration.IpV4DhcpClientIsEnabled = (configurationRegisters[0] & 0x02) != 0;
                    configuration.IpV6DhcpClientIsEnabled = (configurationRegisters[0] & 0x04) != 0;
                    configuration.IpV4Address = RegistersToIpV4Address(configurationRegisters.Skip(2).Take(2).ToList());
                    configuration.IpV4Netmask = RegistersToIpV4Address(configurationRegisters.Skip(4).Take(2).ToList());
                    configuration.IpV4Gateway = RegistersToIpV4Address(configurationRegisters.Skip(6).Take(2).ToList());
                    configuration.IpV6GlobalAddress = RegistersToIpV6Address(configurationRegisters.Skip(8).Take(8).ToList());
                }

                if (Type == HardwareInterfaceType.WifiStation)
                    configuration.WiFiSsid = RegistersToString(configurationRegisters.Skip(16).Take(16).ToList());

                return configuration;
            }

            public HardwareInterfaceState ReadState()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildState(session.ReadInputRegisters(100, 27));
                }
            }

            public async Task<HardwareInterfaceState> ReadStateAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildState(await session.ReadInputRegistersAsync(100, 27, cancellationToken).ConfigureAwait(false));
                }
            }

            private HardwareInterfaceState BuildState(List<ushort> stateRegisters)
            {
                var state = new HardwareInterfaceState
                {
                    HardwareInterfaceType = Type,
                    Name = RegistersToString(stateRegisters.Skip(3).Take(16).ToList())
                };

                if (_networkInterfaceTypes.Contains(Type))
                {
                    state.IsConnected = (stateRegisters[0] & 0x01) != 0;
                    state.IpV6LocalAddress = RegistersToIpV6Address(stateRegisters.Skip(19).Take(8).ToList());
                }

                return state;
            }

            public bool Enable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(100, true);
                    return session.ReadCoils(100, 1)[0];
                }
            }

            public async Task<bool> EnableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(100, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(100, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool Disable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(100, false);
                    return session.ReadCoils(100, 1)[0];
                }
            }

            public async Task<bool> DisableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(100, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(100, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public uint SetUartBaudRate(uint baudRate)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(102, Uint32ToRegisters(baudRate));
                    return RegistersToUint32(session.ReadHoldingRegisters(102, 2));
                }
            }

            public async Task<uint> SetUartBaudRateAsync(uint baudRate, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(102, Uint32ToRegisters(baudRate), cancellationToken).ConfigureAwait(false);
                    return RegistersToUint32(await session.ReadHoldingRegistersAsync(102, 2, cancellationToken).ConfigureAwait(false));
                }
            }

            public ushort SetUartDataBits(ushort dataBits)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(104, dataBits);
                    return session.ReadHoldingRegisters(104, 1)[0];
                }
            }

            public async Task<ushort> SetUartDataBitsAsync(ushort dataBits, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(104, dataBits, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadHoldingRegistersAsync(104, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartParity SetUartParity(UartParity parity)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(105, (ushort)parity);
                    return (UartParity)session.ReadHoldingRegisters(105, 1)[0];
                }
            }

            public async Task<UartParity> SetUartParityAsync(UartParity parity, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(105, (ushort)parity, cancellationToken).ConfigureAwait(false);
                    return (UartParity)(await session.ReadHoldingRegistersAsync(105, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartStopBits SetUartStopBits(UartStopBits stopBits)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(106, (ushort)stopBits);
                    return (UartStopBits)session.ReadHoldingRegisters(106, 1)[0];
                }
            }

            public async Task<UartStopBits> SetUartStopBitsAsync(UartStopBits stopBits, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(106, (ushort)stopBits, cancellationToken).ConfigureAwait(false);
                    return (UartStopBits)(await session.ReadHoldingRegistersAsync(106, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public UartFlowControl SetUartFlowControl(UartFlowControl flowControl)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(107, (ushort)flowControl);
                    return (UartFlowControl)session.ReadHoldingRegisters(107, 1)[0];
                }
            }

            public async Task<UartFlowControl> SetUartFlowControlAsync(UartFlowControl flowControl, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(107, (ushort)flowControl, cancellationToken).ConfigureAwait(false);
                    return (UartFlowControl)(await session.ReadHoldingRegistersAsync(107, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool EnableIpV4DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleCoil(101, true);
                    return session.ReadCoils(101, 1)[0];
                }
            }

            public async Task<bool> EnableIpV4DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleCoilAsync(101, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(101, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool DisableIpV4DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleCoil(101, false);
                    return session.ReadCoils(101, 1)[0];
                }
            }

            public async Task<bool> DisableIpV4DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleCoilAsync(101, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(101, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool EnableIpV6DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleCoil(102, true);
                    return session.ReadCoils(102, 1)[0];
                }
            }

            public async Task<bool> EnableIpV6DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleCoilAsync(102, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(102, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool DisableIpV6DhcpClient()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleCoil(102, false);
                    return session.ReadCoils(102, 1)[0];
                }
            }

            public async Task<bool> DisableIpV6DhcpClientAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleCoilAsync(102, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(102, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public IPAddress SetIpV4Address(IPAddress ipV4Address)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(102, IpV4AddressToRegisters(ipV4Address));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(102, 2));
                }
            }

            public async Task<IPAddress> SetIpV4AddressAsync(IPAddress ipV4Address, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(102, IpV4AddressToRegisters(ipV4Address), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(102, 2, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV4Netmask(IPAddress ipV4Netmask)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(104, IpV4AddressToRegisters(ipV4Netmask));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(104, 2));
                }
            }

            public async Task<IPAddress> SetIpV4NetmaskAsync(IPAddress ipV4Netmask, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(104, IpV4AddressToRegisters(ipV4Netmask), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(104, 2, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV4Gateway(IPAddress ipV4Gateway)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(106, IpV4AddressToRegisters(ipV4Gateway));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(106, 2));
                }
            }

            public async Task<IPAddress> SetIpV4GatewayAsync(IPAddress ipV4Gateway, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(106, IpV4AddressToRegisters(ipV4Gateway), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(106, 2, cancellationToken).ConfigureAwait(false));
                }
            }

            public IPAddress SetIpV6GlobalAddress(IPAddress ipV6GlobalAddress)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(108, IpV6AddressToRegisters(ipV6GlobalAddress));
                    return RegistersToIpV4Address(session.ReadHoldingRegisters(108, 8));
                }
            }

            public async Task<IPAddress> SetIpV6GlobalAddressAsync(IPAddress ipV6GlobalAddress, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_networkInterfaceTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(108, IpV6AddressToRegisters(ipV6GlobalAddress), cancellationToken).ConfigureAwait(false);
                    return RegistersToIpV4Address(await session.ReadHoldingRegistersAsync(108, 8, cancellationToken).ConfigureAwait(false));
                }
            }

            public string SetWiFiSsid(string ssid)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(116, StringToRegisters(ssid, 16));
                    return RegistersToString(session.ReadHoldingRegisters(116, 16));
                }
            }

            public async Task<string> SetWiFiSsidAsync(string ssid, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(116, StringToRegisters(ssid, 16), cancellationToken).ConfigureAwait(false);
                    return RegistersToString(await session.ReadHoldingRegistersAsync(116, 16, cancellationToken).ConfigureAwait(false));
                }
            }

            public void SetWiFiPassword(string password)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    session.WriteMultipleHoldingRegisters(132, StringToRegisters(password, 32));
                }
            }

            public async Task SetWiFiPasswordAsync(string password, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    await session.WriteMultipleHoldingRegistersAsync(132, StringToRegisters(password, 32), cancellationToken).ConfigureAwait(false);
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

            public ServerType Type { get; }

            public ServerConfiguration ReadConfiguration()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildConfiguration(session.ReadHoldingRegisters(200, 6));
                }
            }

            public async Task<ServerConfiguration> ReadConfigurationAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildConfiguration(await session.ReadHoldingRegistersAsync(200, 6, cancellationToken).ConfigureAwait(false));
                }
            }

            private ServerConfiguration BuildConfiguration(List<ushort> configurationRegisters)
            {
                var configuration = new ServerConfiguration
                {
                    ServerType = Type,
                    IsEnabled = (configurationRegisters[0] & 0x01) != 0
                };

                if (_serverWithPortTypes.Contains(Type))
                {
                    configuration.NetworkPort = configurationRegisters[2];
                    configuration.MaxNumberOfClients = configurationRegisters[3];
                }

                if (_modbusServerTypes.Contains(Type))
                {
                    configuration.ModbusProtocol = (ModbusProtocol)configurationRegisters[2];
                    configuration.ModbusStationAddress = (byte)configurationRegisters[3];
                }

                if (Type == ServerType.NetworkModbusServer)
                {
                    configuration.NetworkPort = configurationRegisters[4];
                    configuration.MaxNumberOfClients = configurationRegisters[5];
                }

                return configuration;
            }

            public ServerState ReadState()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    return BuildState(session.ReadInputRegisters(200, 19));
                }
            }

            public async Task<ServerState> ReadStateAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    return BuildState(await session.ReadInputRegistersAsync(200, 19, cancellationToken).ConfigureAwait(false));
                }
            }

            private ServerState BuildState(List<ushort> stateRegisters) => new ServerState
            {
                ServerType = Type,
                Name = RegistersToString(stateRegisters.Skip(3).Take(16).ToList())
            };

            public bool Enable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(200, true);
                    return session.ReadCoils(200, 1)[0];
                }
            }

            public async Task<bool> EnableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(200, true, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(200, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public bool Disable()
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    session.WriteSingleCoil(200, false);
                    return session.ReadCoils(200, 1)[0];
                }
            }

            public async Task<bool> DisableAsync(CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    await session.WriteSingleCoilAsync(200, false, cancellationToken).ConfigureAwait(false);
                    return (await session.ReadCoilsAsync(200, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public ushort SetPort(ushort port)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        session.WriteSingleHoldingRegister(202, port);
                        return session.ReadHoldingRegisters(202, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        session.WriteSingleHoldingRegister(204, port);
                        return session.ReadHoldingRegisters(204, 1)[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public async Task<ushort> SetPortAsync(ushort port, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        await session.WriteSingleHoldingRegisterAsync(202, port, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(202, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        await session.WriteSingleHoldingRegisterAsync(204, port, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(204, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public ushort SetMaxNumberOfClients(ushort maxNumberOfClients)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        session.WriteSingleHoldingRegister(203, maxNumberOfClients);
                        return session.ReadHoldingRegisters(203, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        session.WriteSingleHoldingRegister(205, maxNumberOfClients);
                        return session.ReadHoldingRegisters(205, 1)[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public async Task<ushort> SetMaxNumberOfClientsAsync(ushort maxNumberOfClients, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (_serverWithPortTypes.Contains(Type))
                    {
                        await session.WriteSingleHoldingRegisterAsync(203, maxNumberOfClients, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(203, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        await session.WriteSingleHoldingRegisterAsync(205, maxNumberOfClients, cancellationToken).ConfigureAwait(false);
                        return (await session.ReadHoldingRegistersAsync(205, 1, cancellationToken).ConfigureAwait(false))[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public ModbusProtocol SetModbusProtocol(ModbusProtocol protocol)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(202, (ushort)protocol);
                    return (ModbusProtocol)session.ReadHoldingRegisters(202, 1)[0];
                }
            }

            public async Task<ModbusProtocol> SetModbusProtocolAsync(ModbusProtocol protocol, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(202, (ushort)protocol, cancellationToken).ConfigureAwait(false);
                    return (ModbusProtocol)(await session.ReadHoldingRegistersAsync(202, 1, cancellationToken).ConfigureAwait(false))[0];
                }
            }

            public byte SetModbusStationAddress(byte stationAddress)
            {
                using (var session = _client.CreateSession())
                {
                    Select(session);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException();
                    session.WriteSingleHoldingRegister(203, stationAddress);
                    return (byte)session.ReadHoldingRegisters(203, 1)[0];
                }
            }

            public async Task<byte> SetModbusStationAddressAsync(byte stationAddress, CancellationToken cancellationToken = default)
            {
                using (var session = await _client.CreateSessionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await SelectAsync(session, cancellationToken).ConfigureAwait(false);
                    if (!_modbusServerTypes.Contains(Type))
                        throw new NotSupportedException();
                    await session.WriteSingleHoldingRegisterAsync(203, stationAddress, cancellationToken).ConfigureAwait(false);
                    return (byte)(await session.ReadHoldingRegistersAsync(203, 1, cancellationToken).ConfigureAwait(false))[0];
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

        private static List<ushort> StringToRegisters(string stringValue, int numberOfRegisters)
        {
            byte[] stringValueAsByteArray = Encoding.ASCII.GetBytes(stringValue);
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
