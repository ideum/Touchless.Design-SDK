using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class StatusViewModel : DependencyObject {
    public static readonly DependencyProperty NormalBrushProperty = DependencyProperty.Register(
      "NormalBrush", typeof(Brush), typeof(StatusViewModel), new PropertyMetadata(default(Brush)));

    public Brush NormalBrush {
      get { return (Brush) GetValue(NormalBrushProperty); }
      set { SetValue(NormalBrushProperty, value); }
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
      "SelectedBrush", typeof(Brush), typeof(StatusViewModel), new PropertyMetadata(default(Brush)));

    public Brush SelectedBrush {
      get { return (Brush) GetValue(SelectedBrushProperty); }
      set { SetValue(SelectedBrushProperty, value); }
    }

    public static readonly DependencyProperty ClickStatusProperty = DependencyProperty.Register(
      "ClickStatus", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string ClickStatus {
      get { return (string) GetValue(ClickStatusProperty); }
      set { SetValue(ClickStatusProperty, value); }
    }

    public static readonly DependencyProperty ClickStatusBrushProperty = DependencyProperty.Register(
      "ClickStatusBrush", typeof(Brush), typeof(StatusViewModel), new PropertyMetadata(default(Brush)));

    public Brush ClickStatusBrush {
      get { return (Brush) GetValue(ClickStatusBrushProperty); }
      set { SetValue(ClickStatusBrushProperty, value); }
    }

    public static readonly DependencyProperty MouseDownStatusProperty = DependencyProperty.Register(
      "MouseDownStatus", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string MouseDownStatus {
      get { return (string) GetValue(MouseDownStatusProperty); }
      set { SetValue(MouseDownStatusProperty, value); }
    }

    public static readonly DependencyProperty MouseDownStatusColorProperty = DependencyProperty.Register(
      "MouseDownStatusColor", typeof(Brush), typeof(StatusViewModel), new PropertyMetadata(default(Brush)));

    public Brush MouseDownStatusColor {
      get { return (Brush) GetValue(MouseDownStatusColorProperty); }
      set { SetValue(MouseDownStatusColorProperty, value); }
    }

    public static readonly DependencyProperty MouseEmulationProperty = DependencyProperty.Register(
      "MouseEmulation", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string MouseEmulation {
      get { return (string) GetValue(MouseEmulationProperty); }
      set { SetValue(MouseEmulationProperty, value); }
    }

    public static readonly DependencyProperty OverallOpacityProperty = DependencyProperty.Register(
      "OverallOpacity", typeof(double), typeof(StatusViewModel), new PropertyMetadata(default(double)));

    public double OverallOpacity {
      get { return (double) GetValue(OverallOpacityProperty); }
      set { SetValue(OverallOpacityProperty, value); }
    }

    public static readonly DependencyProperty HoverStateProperty = DependencyProperty.Register(
      "HoverState", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string HoverState {
      get { return (string) GetValue(HoverStateProperty); }
      set { SetValue(HoverStateProperty, value); }
    }

    public static readonly DependencyProperty ConnectedClientsProperty = DependencyProperty.Register(
      "ConnectedClients", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string ConnectedClients {
      get { return (string) GetValue(ConnectedClientsProperty); }
      set { SetValue(ConnectedClientsProperty, value); }
    }


    public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register(
      "MousePosition", typeof(string), typeof(StatusViewModel), new PropertyMetadata(default(string)));

    public string MousePosition {
      get { return (string) GetValue(MousePositionProperty); }
      set { SetValue(MousePositionProperty, value); }
    }

    private DispatcherTimer _dispatcher;

    public void Start() {
      if (_dispatcher == null) {
        _dispatcher = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);
        _dispatcher.Interval = TimeSpan.FromMilliseconds(33);
        _dispatcher.Tick += HandleDispatcherTimerTick;
      }
      _dispatcher.Start();
    }

    private void HandleDispatcherTimerTick(object sender, EventArgs e) {
      UpdateValues();
    }

    public void QueueUpdateValues() {
      Application.Current.Dispatcher.BeginInvoke( new Action(UpdateValues), DispatcherPriority.Normal);
    }

    public void UpdateValues() {
      var i = AppComponent.Input;
      var n = AppComponent.Ipc;
      var em = i.IsEmulationEnabled.Value;
      var mc = $"{i.MouseDownConfidence:0.000}";
      i.GetComputedPosition(out var x, out var y);
      OverallOpacity = em ? 1 : 0.5;
      HoverState = i.stateUser != null ? i.stateUser.HoverState.Value.ToString() : "N/A";
      MouseEmulation = em ? "Enabled" : "Disabled";
      MouseDownStatus = i.stateUser != null ? (i.stateUser.IsButtonDown.Value ? $"Down ({mc})" : $"Up ({mc})") : "N/A";
      MousePosition = $"x:{x,5}, y:{y,5}";
      MouseDownStatusColor = i.stateUser != null ? (i.stateUser.IsButtonDown.Value ? SelectedBrush : NormalBrush) : NormalBrush;
      ClickStatus = i.GetIsClicking() ? "True" : "False";
      ClickStatusBrush = i.GetIsClicking() ? SelectedBrush : NormalBrush;
      ConnectedClients = n.ClientsCount.ToString();
    }

    public void Stop() {
      _dispatcher?.Stop();
    }
  }
}