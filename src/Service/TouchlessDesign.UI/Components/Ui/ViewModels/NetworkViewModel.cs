﻿using TouchlessDesign.Components.Ui.ViewModels.Properties;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class NetworkViewModel : PropertyBase {
    public override object BaseValue { get; set; }

    public IntClampedProperty TcpPort { get; } = new IntClampedProperty { Name = "TCP Port", Value = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };

    public IntClampedProperty WsPort { get; } = new IntClampedProperty { Name = "Websocket Port", Value = 8081, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };

    public IntClampedProperty UdpBroadcastPort { get; } = new IntClampedProperty { Name = "UDP Broadcast Port", Value = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };

    public IntClampedProperty UdpBroadcastRate { get; } = new IntClampedProperty { Name = "UDP Broadcast Rate", Value = 1000, Minimum = 1, Maximum = 15000, SmallChange = 10, LargeChange = 100 };

    public IntClampedProperty UdpDataPort { get; } = new IntClampedProperty { Name = "UDP Data Port", Value = 8080, Minimum = 1, Maximum = 65535, SmallChange = 1, LargeChange = 10 };


    public NetworkViewModel() {
      var properties = new IProperty[] {TcpPort, WsPort, UdpBroadcastPort, UdpBroadcastRate, UdpDataPort};
      foreach (var p in properties) {
        p.Changed += HandlePropertyChanged;
      }
    }

    private void HandlePropertyChanged(IProperty obj) {
      InvokeChanged();
    }
  }
}