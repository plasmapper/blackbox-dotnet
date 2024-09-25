using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BlackBoxConfigurator.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(App app, Model.SystemModel systemModel)
        {
            WindowHeader = App.WindowHeader;

            Connection = new(systemModel.Connection);
            Exception = new Parameter<string, Exception?>(systemModel.Exception, value => value is null ? "" : value.Message);

            RestartedState = new Parameter<State, bool>(systemModel.Restarted, value => value ? State.On : State.Off);
            HardwareNameAndVersion = new(systemModel.HardwareNameAndVersion);
            HardwareUid = new(systemModel.HardwareUid);
            FirmwareNameAndVersion = new(systemModel.FirmwareNameAndVersion);
            DeviceName = new(systemModel.DeviceName);

            HardwareInterfacesExist = new Parameter<bool, List<string>>(systemModel.HardwareInterfaceNames, value => value.Count > 0);
            HardwareInterfaceNames = new(systemModel.HardwareInterfaceNames);
            HardwareInterfaceType = new(systemModel.HardwareInterfaceType);
            HardwareInterfaceIndex = new(systemModel.HardwareInterfaceIndex);
            HardwareInterfaceIsSelected = new Parameter<bool, int>(systemModel.HardwareInterfaceIndex, value => value >= 0);
            HardwareInterfaceIsUart = new Parameter<bool, PL.BlackBox.HardwareInterfaceType>(systemModel.HardwareInterfaceType, value =>
                value == PL.BlackBox.HardwareInterfaceType.Uart);
            HardwareInterfaceIsNetworkInterface = new Parameter<bool, PL.BlackBox.HardwareInterfaceType>(systemModel.HardwareInterfaceType, value =>
                new[] { PL.BlackBox.HardwareInterfaceType.NetworkInterface, PL.BlackBox.HardwareInterfaceType.Ethernet, PL.BlackBox.HardwareInterfaceType.WifiStation }.Contains(value));
            HardwareInterfaceIsWiFi = new Parameter<bool, PL.BlackBox.HardwareInterfaceType>(systemModel.HardwareInterfaceType, value =>
                value == PL.BlackBox.HardwareInterfaceType.WifiStation);
            HardwareInterfaceConfiguration = new(systemModel.HardwareInterfaceConfiguration);
            HardwareInterfaceState = new(systemModel.HardwareInterfaceState);

            ServersExist = new Parameter<bool, List<string>>(systemModel.ServerNames, value => value.Count > 0);
            ServerNames = new(systemModel.ServerNames);
            ServerType = new(systemModel.ServerType);
            ServerIndex = new(systemModel.ServerIndex);
            ServerIsSelected = new Parameter<bool, int>(systemModel.ServerIndex, value => value >= 0);
            ServerIsNetworkServer = new Parameter<bool, PL.BlackBox.ServerType>(systemModel.ServerType, value =>
                new[] { PL.BlackBox.ServerType.NetworkServer, PL.BlackBox.ServerType.NetworkModbusServer, PL.BlackBox.ServerType.HttpServer, PL.BlackBox.ServerType.MdnsServer }.Contains(value));
            ServerIsModbusServer = new Parameter<bool, PL.BlackBox.ServerType>(systemModel.ServerType, value =>
                new[] { PL.BlackBox.ServerType.StreamModbusServer, PL.BlackBox.ServerType.NetworkModbusServer }.Contains(value));
            ServerConfiguration = new(systemModel.ServerConfiguration);

            Restart = new Command(systemModel.Restart);
            ClearRestartedFlag = new Command(systemModel.ClearRestartedFlag);
            SaveAllConfigurations = new Command(systemModel.SaveAllConfigurations);
        }

        public string WindowHeader { get; }

        public Connection Connection { get; }
        public Parameter<string> Exception { get; }

        public Parameter<State> RestartedState { get; }
        public Parameter<string> HardwareNameAndVersion { get; }
        public Parameter<string> HardwareUid { get; }
        public Parameter<string> FirmwareNameAndVersion { get; }
        public Parameter<string> DeviceName { get; }

        public Parameter<bool> HardwareInterfacesExist { get; }
        public Parameter<List<string>> HardwareInterfaceNames { get; }
        public Parameter<PL.BlackBox.HardwareInterfaceType> HardwareInterfaceType { get; }
        public Parameter<int> HardwareInterfaceIndex { get; }
        public Parameter<bool> HardwareInterfaceIsSelected { get; }
        public Parameter<bool> HardwareInterfaceIsUart { get; }
        public Parameter<bool> HardwareInterfaceIsNetworkInterface { get; }
        public Parameter<bool> HardwareInterfaceIsWiFi { get; }
        public HardwareInterfaceConfiguration HardwareInterfaceConfiguration { get; }
        public HardwareInterfaceState HardwareInterfaceState { get; }

        public Parameter<bool> ServersExist { get; }
        public Parameter<List<string>> ServerNames { get; }
        public Parameter<PL.BlackBox.ServerType> ServerType { get; }
        public Parameter<int> ServerIndex { get; }
        public Parameter<bool> ServerIsSelected { get; }
        public Parameter<bool> ServerIsNetworkServer { get; }
        public Parameter<bool> ServerIsModbusServer { get; }
        public ServerConfiguration ServerConfiguration { get; }

        public ICommand Restart { get; }
        public ICommand ClearRestartedFlag { get; }
        public ICommand SaveAllConfigurations { get; }
    }
}
