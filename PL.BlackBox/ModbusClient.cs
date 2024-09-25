using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Linq;
using System.Net;

namespace PL.BlackBox
{
    /// <summary>
    /// BlackBox Modbus client.
    /// </summary>
    public class ModbusClient : Modbus.Client, IClient
    {
        /// <summary>
        /// Initializes a new instance of the BlackBox Modbus client class with serial interface.
        /// </summary>
        /// <param name="serialPort">Serial port.</param>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="stationAddress">Server station address.</param>
        public ModbusClient(SerialPort serialPort, Modbus.Protocol protocol = Modbus.Protocol.Rtu, byte stationAddress = 1) :
            base(serialPort, protocol, stationAddress)
        { }

        /// <summary>
        /// Initializes a new instance of the BlackBox Modbus client class with network interface.
        /// </summary>
        /// <param name="ipAddress">IP address.</param>
        /// <param name="port">Port.</param>
        /// <param name="protocol">Modbus protocol.</param>
        /// <param name="stationAddress">Server station address.</param>
        public ModbusClient(string ipAddress, ushort port = 502, Modbus.Protocol protocol = Modbus.Protocol.Tcp, byte stationAddress = 255) :
            base(ipAddress, port, protocol, stationAddress)
        { }

        public DeviceConfiguration ReadDeviceConfiguration()
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                return new DeviceConfiguration()
                {
                    Name = RegistersToString(ReadHoldingRegisters(2, 16))
                };
            }
        }

        public DeviceState ReadDeviceState()
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                var stateRegisters = ReadInputRegisters(0, 61);
                return new DeviceState()
                {
                    Restarted = (stateRegisters[1] & 0x01) != 0,
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
            }
        }

        public void Restart()
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                try
                {
                    WriteSingleCoil(0, true);
                }
                catch { }
            }
        }

        public void SaveAllConfigurations()
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                WriteSingleCoil(1, true);
            }
        }

        public void ClearRestartedFlag()
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                WriteSingleCoil(16, true);
            }
        }

        public string SetDeviceName(string deviceName)
        {
            lock (this)
            {
                CheckBlackBoxCompatibility();
                WriteMultipleHoldingRegisters(2, StringToRegisters(deviceName, 16));
                return ReadDeviceConfiguration().Name;
            }
        }

        public IHardwareInterface GetHardwareInterface(ushort index)
        {
            lock (this)
            {
                return new HardwareInterface(this, index, SelectHardwareInterface(index));
            }
        }

        public IServer GetServer(ushort index)
        {
            lock (this)
            {
                return new Server(this, index, SelectServer(index));
            }
        }

        private HardwareInterfaceType SelectHardwareInterface(ushort index)
        {
            CheckBlackBoxCompatibility();
            WriteSingleHoldingRegister(18, index);
            if (ReadHoldingRegisters(18, 1)[0] != index)
                throw new Exception("Selecting hardware interface failed.");
            return (HardwareInterfaceType)ReadInputRegisters(102, 1)[0];
        }

        private ServerType SelectServer(ushort index)
        {
            CheckBlackBoxCompatibility();
            WriteSingleHoldingRegister(19, index);
            if (ReadHoldingRegisters(19, 1)[0] != index)
                throw new Exception("Selecting server failed.");
            return (ServerType)ReadInputRegisters(202, 1)[0];
        }

        private class HardwareInterface : IHardwareInterface
        {
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
                lock (_client)
                {
                    Select();
                    var configurationRegisters = _client.ReadHoldingRegisters(100, 64);
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

                    if (new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
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
            }

            public HardwareInterfaceState ReadState()
            {
                lock (_client)
                {
                    Select();
                    var stateRegisters = _client.ReadInputRegisters(100, 27);
                    var state = new HardwareInterfaceState
                    {
                        HardwareInterfaceType = Type,
                        Name = RegistersToString(stateRegisters.Skip(3).Take(16).ToList())
                    };

                    if (new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                    {
                        state.IsConnected = (stateRegisters[0] & 0x01) != 0;
                        state.IpV6LocalAddress = RegistersToIpV6Address(stateRegisters.Skip(19).Take(8).ToList());
                    }

                    return state;
                }
            }

            public bool Enable()
            {
                lock (_client)
                {
                    Select();
                    _client.WriteSingleCoil(100, true);
                    return _client.ReadCoils(100, 1)[0];
                }
            }

            public bool Disable()
            {
                lock (_client)
                {
                    Select();
                    _client.WriteSingleCoil(100, false);
                    return _client.ReadCoils(100, 1)[0];
                }
            }

            public uint SetUartBaudRate(uint baudRate)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(102, Uint32ToRegisters(baudRate));
                    return RegistersToUint32(_client.ReadHoldingRegisters(102, 2));
                }
            }

            public ushort SetUartDataBits(ushort dataBits)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(104, dataBits);
                    return _client.ReadHoldingRegisters(104, 1)[0];
                }
            }

            public UartParity SetUartParity(UartParity parity)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(105, (ushort)parity);
                    return (UartParity)_client.ReadHoldingRegisters(105, 1)[0];
                }
            }

            public UartStopBits SetUartStopBits(UartStopBits stopBits)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(106, (ushort)stopBits);
                    return (UartStopBits)_client.ReadHoldingRegisters(106, 1)[0];
                }
            }

            public UartFlowControl SetUartFlowControl(UartFlowControl flowControl)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.Uart)
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(107, (ushort)flowControl);
                    return (UartFlowControl)_client.ReadHoldingRegisters(107, 1)[0];
                }
            }

            public bool EnableIpV4DhcpClient()
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleCoil(101, true);
                    return _client.ReadCoils(101, 1)[0];
                }
            }

            public bool DisableIpV4DhcpClient()
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleCoil(101, false);
                    return _client.ReadCoils(101, 1)[0];
                }
            }

            public bool EnableIpV6DhcpClient()
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleCoil(102, true);
                    return _client.ReadCoils(102, 1)[0];
                }
            }

            public bool DisableIpV6DhcpClient()
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleCoil(102, false);
                    return _client.ReadCoils(102, 1)[0];
                }
            }

            public IPAddress SetIpV4Address(IPAddress ipV4Address)
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(102, IpV4AddressToRegisters(ipV4Address));
                    return RegistersToIpV4Address(_client.ReadHoldingRegisters(102, 2));
                }
            }

            public IPAddress SetIpV4Netmask(IPAddress ipV4Netmask)
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(104, IpV4AddressToRegisters(ipV4Netmask));
                    return RegistersToIpV4Address(_client.ReadHoldingRegisters(104, 2));
                }
            }

            public IPAddress SetIpV4Gateway(IPAddress ipV4Gateway)
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(106, IpV4AddressToRegisters(ipV4Gateway));
                    return RegistersToIpV4Address(_client.ReadHoldingRegisters(106, 2));
                }
            }

            public IPAddress SetIpV6GlobalAddress(IPAddress ipV6GlobalAddress)
            {
                lock (_client)
                {
                    Select();
                    if (!new[] { HardwareInterfaceType.NetworkInterface, HardwareInterfaceType.Ethernet, HardwareInterfaceType.WifiStation }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(108, IpV6AddressToRegisters(ipV6GlobalAddress));
                    return RegistersToIpV4Address(_client.ReadHoldingRegisters(108, 8));
                }
            }

            public string SetWiFiSsid(string ssid)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(116, StringToRegisters(ssid, 16));
                    return RegistersToString(_client.ReadHoldingRegisters(116, 16));
                }
            }

            public void SetWiFiPassword(string password)
            {
                lock (_client)
                {
                    Select();
                    if (Type != HardwareInterfaceType.WifiStation)
                        throw new NotSupportedException();
                    _client.WriteMultipleHoldingRegisters(132, StringToRegisters(password, 32));
                }
            }

            private void Select()
            {
                var type = _client.SelectHardwareInterface(_index);
                if (type != Type)
                    throw new Exception("Hardware interface control instance is invalid.");
            }
        }

        private class Server : IServer
        {
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
                lock (_client)
                {
                    Select();
                    var configurationRegisters = _client.ReadHoldingRegisters(200, 6);
                    var configuration = new ServerConfiguration
                    {
                        ServerType = Type,
                        IsEnabled = (configurationRegisters[0] & 0x01) != 0
                    };

                    if (new[] { ServerType.NetworkServer, ServerType.HttpServer, ServerType.MdnsServer }.Contains(Type))
                    {
                        configuration.NetworkPort = configurationRegisters[2];
                        configuration.MaxNumberOfClients = configurationRegisters[3];
                    }

                    if (new[] { ServerType.StreamModbusServer, ServerType.NetworkModbusServer }.Contains(Type))
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
            }

            public ServerState ReadState()
            {
                lock (_client)
                {
                    Select();
                    var stateRegisters = _client.ReadInputRegisters(200, 19);
                    var state = new ServerState
                    {
                        ServerType = Type,
                        Name = RegistersToString(stateRegisters.Skip(3).Take(16).ToList())
                    };

                    return state;
                }
            }

            public bool Enable()
            {
                lock (_client)
                {
                    Select();
                    _client.WriteSingleCoil(200, true);
                    return _client.ReadCoils(200, 1)[0];
                }
            }

            public bool Disable()
            {
                lock (_client)
                {
                    Select();
                    _client.WriteSingleCoil(200, false);
                    return _client.ReadCoils(200, 1)[0];
                }
            }

            public ushort SetPort(ushort port)
            {
                lock (_client)
                {
                    Select();
                    if (new[] { ServerType.NetworkServer, ServerType.HttpServer, ServerType.MdnsServer }.Contains(Type))
                    {
                        _client.WriteSingleHoldingRegister(202, port);
                        return _client.ReadHoldingRegisters(202, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        _client.WriteSingleHoldingRegister(204, port);
                        return _client.ReadHoldingRegisters(204, 1)[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public ushort SetMaxNumberOfClients(ushort maxNumberOfClients)
            {
                lock (_client)
                {
                    Select();
                    if (new[] { ServerType.NetworkServer, ServerType.HttpServer, ServerType.MdnsServer }.Contains(Type))
                    {
                        _client.WriteSingleHoldingRegister(203, maxNumberOfClients);
                        return _client.ReadHoldingRegisters(203, 1)[0];
                    }
                    if (Type == ServerType.NetworkModbusServer)
                    {
                        _client.WriteSingleHoldingRegister(205, maxNumberOfClients);
                        return _client.ReadHoldingRegisters(205, 1)[0];
                    }
                    throw new NotSupportedException();
                }
            }

            public ModbusProtocol SetModbusProtocol(ModbusProtocol protocol)
            {
                lock (this)
                {
                    Select();
                    if (!new[] { ServerType.StreamModbusServer, ServerType.NetworkModbusServer }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(202, (ushort)protocol);
                    return (ModbusProtocol)_client.ReadHoldingRegisters(202, 1)[0];
                }
            }

            public byte SetModbusStationAddress(byte stationAddress)
            {
                lock (this)
                {
                    Select();
                    if (!new[] { ServerType.StreamModbusServer, ServerType.NetworkModbusServer }.Contains(Type))
                        throw new NotSupportedException();
                    _client.WriteSingleHoldingRegister(203, stationAddress);
                    return (byte)_client.ReadHoldingRegisters(203, 1)[0];
                }
            }

            private void Select()
            {
                var type = _client.SelectServer(_index);
                if (type != Type)
                    throw new Exception("Server control instance is invalid.");
            }
        }

        private void CheckBlackBoxCompatibility()
        {
            var blackBoxInfo = ReadInputRegisters(2, 3);
            if (RegistersToString(blackBoxInfo.Take(2).ToList()) != "PLBB" || blackBoxInfo[2] != 1)
                throw new Exception($"The device is not a valid BlackBox device.");
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