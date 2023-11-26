using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Service;

namespace ExpandWorldData.Factions;
public class FactionManager
{
  public static string FileName = "expand_factions.yaml";
  public static string FilePath = Path.Combine(EWF.YamlDirectory, FileName);
  public static string Pattern = "expand_factions*.yaml";


  ///<summary>Lower case faction names for easier data loading.</summary>
  private static Dictionary<string, Character.Faction> NameToFaction = DefaultFactions.OriginalFactions.ToDictionary(kvp => kvp.Key.ToLowerInvariant(), kvp => kvp.Value);
  ///<summary>Original faction names because some mods rely on Enum.GetName(s) returning uppercase values.</summary>
  public static Dictionary<Character.Faction, string> FactionToDisplayName = DefaultFactions.OriginalFactions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
  private static readonly Dictionary<Character.Faction, FactionData> Data = [];
  public static HashSet<Character.Faction> Aggravatable = [];
  public static bool TryGetData(Character.Faction faction, out FactionData data) => Data.TryGetValue(faction, out data);
  public static bool TryGetFaction(string name, out Character.Faction faction) => NameToFaction.TryGetValue(name.ToLowerInvariant(), out faction);
  public static Character.Faction GetFaction(string name)
  {
    if (TryGetFaction(name, out var faction)) return faction;
    EWF.LogWarning($"Failed to find faction {name}.");
    return Character.Faction.Players;
  }
  public static HashSet<Character.Faction> GetFactions() => NameToFaction.Values.Where(f => f != Character.Faction.Players).ToHashSet();
  public static bool TryGetDisplayName(Character.Faction faction, out string name) => FactionToDisplayName.TryGetValue(faction, out name);

  public static void ToFile()
  {
    if (Helper.IsClient()) return;
    if (File.Exists(FilePath)) return;
    var yaml = Yaml.Write(DefaultFactions.Data);
    File.WriteAllText(FilePath, yaml);
  }
  public static void FromFile(string lines)
  {
    if (Helper.IsClient()) return;
    EWF.valueFactionData.Value = lines;
    Load(lines);
  }
  public static void NamesFromFile(string lines)
  {
    LoadNames(lines);
  }
  public static void FromSetting()
  {
    if (Helper.IsClient()) Load(EWF.valueFactionData.Value);
  }

  private static List<FactionYaml> Parse(string yaml)
  {
    List<FactionYaml> rawData = DefaultFactions.Data;
    try
    {
      rawData = Yaml.Read<FactionYaml>(yaml, FileName, EWF.LogError);
    }
    catch (Exception e)
    {
      EWF.LogWarning($"Failed to load any faction data.");
      EWF.LogError(e.Message);
      EWF.LogError(e.StackTrace);
    }
    return rawData;
  }
  public static void SetNames(Dictionary<Character.Faction, string> names)
  {
    FactionToDisplayName = names;
    NameToFaction = FactionToDisplayName.ToDictionary(kvp => kvp.Value.ToLowerInvariant(), kvp => kvp.Key);
    EWF.LogInfo($"Received {FactionToDisplayName.Count} faction names.");
  }
  private static void LoadNames(string yaml)
  {
    var rawData = Parse(yaml);
    if (rawData.Count > 0)
      EWF.LogInfo($"Preloading factions names ({rawData.Count} entries).");
    Load(rawData);
  }
  private static void Load(string yaml)
  {
    if (yaml == "") return;
    var rawData = Parse(yaml);
    if (rawData.Count > 0)
      EWF.LogInfo($"Reloading faction data ({rawData.Count} entries).");
    Load(rawData);

    Data.Clear();
    Aggravatable.Clear();
    foreach (var item in rawData)
    {
      var faction = NameToFaction[item.faction.ToLowerInvariant()];
      Data[faction] = new(item, GetFaction, GetFactions);
      if (Data[faction].AggravatedFriendly != Data[faction].Friendly)
        Aggravatable.Add(faction);
    }
    foreach (var baseAi in BaseAI.Instances) BaseAIAwake.Setup(baseAi);
  }

  private static void Load(List<FactionYaml> data)
  {
    NameToFaction.Clear();
    FactionToDisplayName.Clear();
    foreach (var item in data)
    {
      var faction = (Character.Faction)item.id;
      FactionToDisplayName.Add(faction, item.faction);
      NameToFaction.Add(item.faction.ToLowerInvariant(), faction);
    }
  }
  public static void SetupWatcher()
  {
    static void callback(string lines)
    {
      if (ZNet.m_instance == null) NamesFromFile(lines);
      else FromFile(lines);
    }
    Watcher.Setup(EWF.YamlDirectory, EWF.BackupDirectory, Pattern, callback);
  }
}

[HarmonyPatch(typeof(ZNet), nameof(ZNet.Awake))]
public class Loader
{
  static void Postfix()
  {
    if (Helper.IsClient()) return;
    FactionManager.ToFile();
    FactionManager.FromFile(Watcher.ReadFiles(EWF.YamlDirectory, FactionManager.Pattern));
  }
}