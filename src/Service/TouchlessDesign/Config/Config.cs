namespace TouchlessDesign.Config {
  public class Config {
    public ConfigDisplay Display;
    public ConfigGeneral General;
    public ConfigInput Input;
    public ConfigNetwork Network;

    public Config(string dataDir) {
      Display = ConfigDisplay.Get(dataDir);
      General = ConfigGeneral.Get(dataDir);
      Input = ConfigInput.Get(dataDir);
      Network = ConfigNetwork.Get(dataDir);
    }
  }
}