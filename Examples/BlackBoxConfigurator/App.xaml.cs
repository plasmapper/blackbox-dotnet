using System.Windows;
using System.Windows.Threading;

namespace BlackBoxConfigurator
{
    public partial class App : Application
    {
        private readonly PL.BlackBox.ModbusClient _client;
        private readonly Model.SystemModel _systemModel;
        private readonly Thread _monitorThread;
        private bool _stopMonitorThread = false;

        public App()
        {
            _client = new(new PL.Modbus.SerialStream(new System.IO.Ports.SerialPort()));
            _client.ReadTimeout = 1000;
            _systemModel = new(_client);
            _monitorThread = new Thread(HardwareMonitor);
        }

        internal static string WindowHeader { get; } = "BlackBox Configurator";

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                _monitorThread.Start();

                new View.MainWindow(new ViewModel.MainWindowViewModel(this, _systemModel)).Show();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Current.Shutdown();
            }
        }

        internal void HardwareMonitor()
        {
            while (!_stopMonitorThread)
            {
                try
                {
                    // Get serial port names
                    var portNames = System.IO.Ports.SerialPort.GetPortNames().ToList();
                    if (!portNames.SequenceEqual(_systemModel.Connection.SerialPortNames.Value))
                        _systemModel.Connection.SerialPortNames.Value = portNames;

                    // Read device configuration
                    var deviceConfiguration = _client.ReadDeviceConfiguration();
                    _systemModel.DeviceName.Value = deviceConfiguration.Name;

                    // Read device state
                    var deviceState = _client.ReadDeviceState();
                    _systemModel.Restarted.Value = deviceState.Restarted;
                    _systemModel.HardwareNameAndVersion.Value = $"{deviceState.HardwareInfo.Name} v{deviceState.HardwareInfo.Version.Major}." +
                        $"{deviceState.HardwareInfo.Version.Minor}.{deviceState.HardwareInfo.Version.Patch}";
                    _systemModel.HardwareUid.Value = deviceState.HardwareInfo.Uid;
                    _systemModel.FirmwareNameAndVersion.Value = $"{deviceState.FirmwareInfo.Name} v{deviceState.FirmwareInfo.Version.Major}." +
                        $"{deviceState.FirmwareInfo.Version.Minor}.{deviceState.FirmwareInfo.Version.Patch}";

                    // Read hardware interface names
                    if (_systemModel.HardwareInterfaceNames.Value.Count != deviceState.NumberOfHardwareInterfaces)
                    {
                        List<string> names = new();
                        for (ushort i = 0; i < deviceState.NumberOfHardwareInterfaces; i++)
                            names.Add(_client.GetHardwareInterface(i).ReadState().Name);
                        _systemModel.HardwareInterfaceNames.Value = names;
                    }

                    // Read server names
                    if (_systemModel.ServerNames.Value.Count != deviceState.NumberOfServers)
                    {
                        List<string> names = new();
                        for (ushort i = 0; i < deviceState.NumberOfServers; i++)
                            names.Add(_client.GetServer(i).ReadState().Name);
                        _systemModel.ServerNames.Value = names;
                    }

                    if (deviceState.NumberOfHardwareInterfaces > 0 && _systemModel.HardwareInterfaceIndex.Value >= 0)
                    {
                        var hardwareInterface = _client.GetHardwareInterface((ushort)_systemModel.HardwareInterfaceIndex.Value);
                        _systemModel.HardwareInterfaceType.Value = hardwareInterface.Type;

                        // Read hardware interface configuration
                        var configuration = hardwareInterface.ReadConfiguration();
                        _systemModel.HardwareInterfaceConfiguration.Enabled.Value = configuration.IsEnabled;
                        if (configuration.UartBaudRate is not null)
                            _systemModel.HardwareInterfaceConfiguration.UartBaudRate.Value = configuration.UartBaudRate.Value;
                        if (configuration.UartDataBits is not null)
                            _systemModel.HardwareInterfaceConfiguration.UartDataBits.Value = configuration.UartDataBits.Value;
                        if (configuration.UartParity is not null)
                            _systemModel.HardwareInterfaceConfiguration.UartParity.Value = configuration.UartParity.Value;
                        if (configuration.UartStopBits is not null)
                            _systemModel.HardwareInterfaceConfiguration.UartStopBits.Value = configuration.UartStopBits.Value;
                        if (configuration.UartFlowControl is not null)
                            _systemModel.HardwareInterfaceConfiguration.UartFlowControl.Value = configuration.UartFlowControl.Value;
                        if (configuration.IpV4DhcpClientIsEnabled is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV4DhcpClientEnabled.Value = configuration.IpV4DhcpClientIsEnabled.Value;
                        if (configuration.IpV6DhcpClientIsEnabled is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV6DhcpClientEnabled.Value = configuration.IpV6DhcpClientIsEnabled.Value;
                        if (configuration.IpV4Address is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV4Address.Value = configuration.IpV4Address;
                        if (configuration.IpV4Netmask is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV4Netmask.Value = configuration.IpV4Netmask;
                        if (configuration.IpV4Gateway is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV4Gateway.Value = configuration.IpV4Gateway;
                        if (configuration.IpV6GlobalAddress is not null)
                            _systemModel.HardwareInterfaceConfiguration.IpV6GlobalAddress.Value = configuration.IpV6GlobalAddress;
                        if (configuration.WiFiSsid is not null)
                            _systemModel.HardwareInterfaceConfiguration.WiFiSsid.Value = configuration.WiFiSsid;

                        // Read hardware interface state
                        var state = hardwareInterface.ReadState();
                        if (state.IsConnected is not null)
                            _systemModel.HardwareInterfaceState.IsConnected.Value = state.IsConnected.Value;
                        if (state.IpV6LocalAddress is not null)
                            _systemModel.HardwareInterfaceState.IpV6LocalAddress.Value = state.IpV6LocalAddress;
                    }

                    if (deviceState.NumberOfServers > 0 && _systemModel.ServerIndex.Value >= 0)
                    {
                        _systemModel.ServerIndex.Value = Math.Min(_systemModel.ServerIndex.Value, (ushort)(deviceState.NumberOfServers - 1));

                        var server = _client.GetServer((ushort)_systemModel.ServerIndex.Value);
                        _systemModel.ServerType.Value = server.Type;

                        // Read server configuration
                        var configuration = server.ReadConfiguration();
                        _systemModel.ServerConfiguration.Enabled.Value = configuration.IsEnabled;
                        if (configuration.NetworkPort is not null)
                            _systemModel.ServerConfiguration.NetworkPort.Value = configuration.NetworkPort.Value;
                        if (configuration.MaxNumberOfClients is not null)
                            _systemModel.ServerConfiguration.MaxNumberOfClients.Value = configuration.MaxNumberOfClients.Value;
                        if (configuration.ModbusProtocol is not null)
                            _systemModel.ServerConfiguration.ModbusProtocol.Value = configuration.ModbusProtocol.Value;
                        if (configuration.ModbusStationAddress is not null)
                            _systemModel.ServerConfiguration.ModbusStationAddress.Value = configuration.ModbusStationAddress.Value;
                    }

                    _systemModel.Exception.Value = null;
                }
                catch (Exception e)
                {
                    try
                    {
                        _systemModel.HardwareInterfaceNames.Value = new();
                        _systemModel.HardwareInterfaceIndex.Value = -1;
                        _systemModel.ServerNames.Value = new();
                        _systemModel.ServerIndex.Value = -1;
                        _systemModel.Exception.Value = e;   
                    }
                    catch { }
                }
                Thread.Sleep(100);
            }

            try
            {
                _client.Dispose();
            }
            catch { }
        }

        internal static void UserCommand(Action command)
        {
            try
            {
                command();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        internal static MessageBoxResult ShowError(string text) =>
            MessageBox.Show(text, App.WindowHeader, MessageBoxButton.OK, MessageBoxImage.Error);

        internal static MessageBoxResult ShowError(Exception exception) =>
            MessageBox.Show(ExceptionToString(exception), App.WindowHeader, MessageBoxButton.OK, MessageBoxImage.Error);

        protected override void OnExit(ExitEventArgs e)
        {
            _stopMonitorThread = true;
            base.OnExit(e);
        }

        private static string ExceptionToString(Exception? exception)
        {
            string exceptionString = "";
            if (exception != null)
            {
                if (exception is AggregateException ae)
                    exceptionString = string.Join(" ", ae.Flatten().InnerExceptions.Select((ie) => ie.Message));
                else
                    exceptionString = exception.Message;
            }
            return exceptionString;
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowError(e.Exception);
            e.Handled = true;
        }
    }
}
