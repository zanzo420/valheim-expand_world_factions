using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace ExpandWorldData.Factions;


public class FactionYaml
{
  public string faction = "";

  [DefaultValue(-1)]
  public int id = 0;
  public string friendly = "";

  [DefaultValue(null)]
  public string? aggravatedFriendly = null;

  [DefaultValue(null)]
  public string? alertedFriendly = null;
  [DefaultValue(false)]
  public bool attackSame = false;
  [DefaultValue("")]
  public string tamedFaction = "";
}

public class DefaultFactions
{
  public static List<FactionYaml> Data = [
    new FactionYaml { faction = "Players", id = (int)Character.Faction.Players, friendly = "Players" },
    new FactionYaml { faction = "AnimalsVeg", id = (int)Character.Faction.AnimalsVeg, friendly = "AnimalsVeg" },
    new FactionYaml { faction = "ForestMonsters", id = (int)Character.Faction.ForestMonsters, friendly = "AnimalsVeg, Boss, ForestMonsters" },
    new FactionYaml { faction = "Undead", id = (int)Character.Faction.Undead, friendly = "Boss, Demon, Undead" },
    new FactionYaml { faction = "Demon", id = (int)Character.Faction.Demon, friendly = "Boss, Demon, Undead" },
    new FactionYaml { faction = "MountainMonsters", id = (int)Character.Faction.MountainMonsters, friendly = "Boss, MountainMonsters" },
    new FactionYaml { faction = "PlainsMonsters", id = (int)Character.Faction.PlainsMonsters, friendly = "Boss, PlainsMonsters" },
    new FactionYaml { faction = "SeaMonsters", id = (int)Character.Faction.SeaMonsters, friendly = "Boss, SeaMonsters" },
    new FactionYaml { faction = "Boss", id = (int)Character.Faction.Boss, friendly = "All" },
    new FactionYaml { faction = "MistlandsMonsters", id = (int)Character.Faction.MistlandsMonsters, friendly = "AnimalsVeg, Boss, MistlandsMonsters" },
    new FactionYaml { faction = "Dverger", id = (int)Character.Faction.Dverger, friendly = "AnimalsVeg, Boss, Dverger, Players", aggravatedFriendly = "AnimalsVeg, Boss, Dverger" },
  ];

  public static readonly Dictionary<string, Character.Faction> OriginalFactions = new() {
    { "Players", Character.Faction.Players },
    { "AnimalsVeg", Character.Faction.AnimalsVeg },
    { "ForestMonsters", Character.Faction.ForestMonsters },
    { "Undead", Character.Faction.Undead },
    { "Demon", Character.Faction.Demon },
    { "MountainMonsters", Character.Faction.MountainMonsters },
    { "PlainsMonsters", Character.Faction.PlainsMonsters },
    { "SeaMonsters", Character.Faction.SeaMonsters },
    { "Boss", Character.Faction.Boss },
    { "MistlandsMonsters", Character.Faction.MistlandsMonsters },
    { "Dverger", Character.Faction.Dverger },
  };
}

public class FactionData
{
  public Character.Faction Faction = Character.Faction.Players;
  public bool AttackSame;
  public Character.Faction TamedFaction = Character.Faction.Players;
  public HashSet<Character.Faction> Friendly = [];
  public HashSet<Character.Faction> AggravatedFriendly = [];
  public HashSet<Character.Faction> AlertedFriendly = [];

  public FactionData(FactionYaml yaml, Func<string, Character.Faction> getFaction, Func<HashSet<Character.Faction>> getFactions)
  {
    Faction = getFaction(yaml.faction);
    AttackSame = yaml.attackSame;
    TamedFaction = yaml.tamedFaction == "" ? Character.Faction.Players : getFaction(yaml.tamedFaction);
    Friendly = GetFactions(yaml.friendly, getFaction, getFactions);
    if (yaml.aggravatedFriendly == null)
      AggravatedFriendly = Friendly;
    else
      AggravatedFriendly = GetFactions(yaml.aggravatedFriendly, getFaction, getFactions);
    if (yaml.alertedFriendly == null)
      AlertedFriendly = Friendly;
    else
      AlertedFriendly = GetFactions(yaml.alertedFriendly, getFaction, getFactions);
  }

  private HashSet<Character.Faction> GetFactions(string data, Func<string, Character.Faction> getFaction, Func<HashSet<Character.Faction>> getFactions) =>
   data.ToLowerInvariant() == "all" ? getFactions() :
    Helper.Split(data).Select(getFaction).ToHashSet();
}
