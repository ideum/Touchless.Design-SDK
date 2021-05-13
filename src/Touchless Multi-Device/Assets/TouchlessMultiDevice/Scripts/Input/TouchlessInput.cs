using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Timer = System.Threading.Timer;
using TouchlessDesignCore.Components;
using TouchlessDesignCore.Components.Ipc;
using TouchlessDesignCore;
using TouchlessDesignCore.Components.Remote;
using TouchlessDesignCore.Components.Input;
using TouchlessDesignCore.Components.Input.Providers;
using TouchlessDesignCore.Components.Ipc.Networking.Tcp;
using TouchlessDesignCore.Components.Input.Providers.Remote;

public class TouchlessInput : TouchlessComponent
{
  public bool IsNoTouch { get; private set; }

  protected override void DoStart()
  {
    InitializeInputProvider();
  }

  protected override void DoStop()
  {
    //_remoteClientActive = false;
    //_remoteClient?.Connection.Close();

    DeinitializeInputProvider();
  }

  #region Input Provider
  private IInputProvider _provider;

  private void InitializeInputProvider()
  {
    var providerInterfaceType = typeof(IInputProvider);
    Type providerType = typeof(RemoteProvider);

    object instance = null;
    try
    {
      instance = Activator.CreateInstance(providerType);
      _provider = (IInputProvider)instance;
    }
    catch (Exception e)
    {
      Debug.LogError($"Exception thrown when instantiating or casting provider '{instance?.GetType().Name}': {e}");
      return;
    }

    if (_provider == null)
    {
      Debug.LogError($"Unknown error. Provider could not be instantiated/cast to {providerInterfaceType.Name}");
      return;
    }

    _provider.DataDir = DataDir;

    try
    {
      _provider.Start();
    }
    catch (Exception e)
    {
      Debug.LogError($"Exception caught while starting {_provider.GetType().Name}. {e}");
    }
  }

  public void DeinitializeInputProvider()
  {
    _provider?.Stop();
  }

  #endregion
}
