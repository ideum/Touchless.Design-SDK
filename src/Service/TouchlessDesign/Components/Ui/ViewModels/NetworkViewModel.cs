using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class NetworkViewModel : VM<ConfigNetwork> {

    //public BoolProperty TcpEnabled { get; } = new BoolProperty { Name = "TCP", Prop = true };
    //public IntClampedProperty TcpPort { get; } = new IntClampedProperty { Name = "TCP Port", Prop = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };
    //public BoolProperty WsEnabled { get; } = new BoolProperty { Name = "WebSocket", Prop = true };
    //public IntClampedProperty WsPort { get; } = new IntClampedProperty { Name = "Websocket Port", Prop = 8081, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };
    //public BoolProperty UdpEnabled { get; } = new BoolProperty { Name = "UDP", Prop = true};
    //public IntClampedProperty UdpBroadcastRate { get; } = new IntClampedProperty { Name = "UDP Broadcast Rate (ms)", Prop = 1000, Minimum = 1, Maximum = 15000, SmallChange = 10, LargeChange = 100 };
    //public IntClampedProperty UdpBroadcastPort { get; } = new IntClampedProperty { Name = "UDP Broadcast Port", Prop = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };
    //public IntClampedProperty UdpDataPort { get; } = new IntClampedProperty { Name = "UDP Data Port", Prop = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };

    public static readonly DependencyProperty ClientReconnectIntervalProperty = Reg<NetworkViewModel, int>("ClientReconnectInterval", 1000, PropertyTypes.Restart);

    public int ClientReconnectInterval {
      get { return (int)GetValue(ClientReconnectIntervalProperty); }
      set { SetValue(ClientReconnectIntervalProperty, value); }
    }


    public static readonly DependencyProperty TcpEnabledProperty = Reg<NetworkViewModel, bool>("TcpEnabled", true, PropertyTypes.Restart);

    public bool TcpEnabled {
      get { return (bool)GetValue(TcpEnabledProperty); }
      set { SetValue(TcpEnabledProperty, value); }
    }

    public static readonly DependencyProperty TcpPortProperty = Reg<NetworkViewModel, int>("TcpPort", 4949, PropertyTypes.Restart);

    public int TcpPort {
      get { return (int)GetValue(TcpPortProperty); }
      set { SetValue(TcpPortProperty, value); }
    }

    public static readonly DependencyProperty SdkLoopbackProperty = Reg<NetworkViewModel, bool>("SdkLoopback", true, PropertyTypes.Restart);

    public bool SdkLoopback {
      get { return (bool)GetValue(SdkLoopbackProperty); }
      set { SetValue(SdkLoopbackProperty, value); }
    }

    public static readonly DependencyProperty WsEnabledProperty = Reg<NetworkViewModel, bool>("WsEnabled", true, PropertyTypes.Restart);

    public bool WsEnabled {
      get { return (bool)GetValue(WsEnabledProperty); }
      set { SetValue(WsEnabledProperty, value); }
    }

    public static readonly DependencyProperty WsPortProperty = Reg<NetworkViewModel, int>("WsPort", 4950, PropertyTypes.Restart);

    public int WsPort {
      get { return (int)GetValue(WsPortProperty); }
      set { SetValue(WsPortProperty, value); }
    }

    public static readonly DependencyProperty UdpEnabledProperty = Reg<NetworkViewModel, bool>("UdpEnabled", true, PropertyTypes.Restart);

    public bool UdpEnabled {
      get { return (bool)GetValue(UdpEnabledProperty); }
      set { SetValue(UdpEnabledProperty, value); }
    }
    
    public static readonly DependencyProperty UdpBroadcastRateProperty = Reg<NetworkViewModel, int>("UdpBroadcastRate", 1000, PropertyTypes.Restart);

    public int UdpBroadcastRate {
      get { return (int)GetValue(UdpBroadcastRateProperty); }
      set { SetValue(UdpBroadcastRateProperty, value); }
    }

    public static readonly DependencyProperty UdpBroadcastPortProperty = Reg<NetworkViewModel, int>("UdpBroadcastPort", 4951, PropertyTypes.Restart);

    public int UdpBroadcastPort {
      get { return (int)GetValue(UdpBroadcastPortProperty); }
      set { SetValue(UdpBroadcastPortProperty, value); }
    }

    public static readonly DependencyProperty UdpDataPortProperty = Reg<NetworkViewModel, int>("UdpDataPort", 4952, PropertyTypes.Restart);

    public int UdpDataPort {
      get { return (int)GetValue(UdpDataPortProperty); }
      set { SetValue(UdpDataPortProperty, value); }
    }

    public NetworkViewModel() {

    }

    protected override void AssignModel() {
      Model = AppComponent.Config.Network;
    }

    public override void ApplyValuesToModel() {
      Model.TcpEnabled = TcpEnabled;
      Model.TcpData.Port = TcpPort;
      Model.TcpData.SdkLoopback = SdkLoopback;
      //Model.WsEnabled = WsEnabled;
      //Model.WsData.Port = WsPort;
      Model.UdpEnabled = UdpEnabled;
      Model.UdpBroadcast.Port = UdpBroadcastPort;
      Model.UdpBroadcastInterval_ms = UdpBroadcastRate;
      Model.UdpData.Port = UdpDataPort;
    }

    public override void UpdateValuesFromModel() {
      TcpEnabled = Model.TcpEnabled;
      TcpPort = Model.TcpData.Port;
      SdkLoopback = Model.TcpData.SdkLoopback;
      //WsEnabled = Model.WsEnabled;
      //WsPort = Model.WsData.Port;
      UdpEnabled = Model.UdpEnabled;
      UdpBroadcastPort = Model.UdpBroadcast.Port;
      UdpBroadcastRate = Model.UdpBroadcastInterval_ms;
      UdpDataPort = Model.UdpData.Port;

    }
  }
}