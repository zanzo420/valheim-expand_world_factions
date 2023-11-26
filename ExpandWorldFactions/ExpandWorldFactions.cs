using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;

namespace ExpandWorldData.Factions;
[BepInPlugin(GUID, NAME, VERSION)]
public class EWF : BaseUnityPlugin
{
  public const string GUID = "expand_world_factions";
  public const string NAME = "Expand World Factions";
  public const string VERSION = "1.0";
#nullable disable
  private static ManualLogSource Log;
  public static EWF Instance;
  public static CustomSyncedValue<string> valueFactionData;
#nullable enable
  public static ConfigSync ConfigSync = new(GUID)
  {
    DisplayName = NAME,
    CurrentVersion = VERSION,
    ModRequired = true,
    IsLocked = true
  };
  public static void LogWarning(string message) => Log.LogWarning(message);
  public static void LogError(string message) => Log.LogError(message);
  public static void LogInfo(string message) => Log.LogInfo(message);


  public static string YamlDirectory = Path.Combine(Paths.ConfigPath, "expand_world");
  public static string BackupDirectory = Path.Combine(Paths.ConfigPath, "expand_world_backups");
  public void Awake()
  {
    Instance = this;
    Log = Logger;
    if (!Directory.Exists(YamlDirectory))
      Directory.CreateDirectory(YamlDirectory);
    valueFactionData = new(ConfigSync, "faction_data");
    valueFactionData.ValueChanged += FactionManager.FromSetting;
    new Harmony(GUID).PatchAll();
  }
  public void Start()
  {
    try
    {
      FactionManager.SetupWatcher();
    }
    catch (Exception e)
    {
      Log.LogError(e);
    }
  }
}
