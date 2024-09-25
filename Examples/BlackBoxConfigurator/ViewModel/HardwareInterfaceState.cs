using System.Net;

namespace BlackBoxConfigurator.ViewModel
{
    internal class HardwareInterfaceState
    {
        public HardwareInterfaceState(Model.HardwareInterfaceState state)
        {
            ConnectedState = new Parameter<State, bool>(state.IsConnected, value => value ? State.On : State.Off);
            IpV6LocalAddress = new(state.IpV6LocalAddress);
        }

        public Parameter<State> ConnectedState { get; }
        public Parameter<IPAddress> IpV6LocalAddress { get; }
    }
}