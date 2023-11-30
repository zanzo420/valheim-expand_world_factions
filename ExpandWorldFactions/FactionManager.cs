
using HarmonyLib;

namespace ExpandWorldData.Factions;

[HarmonyPatch(typeof(BaseAI), nameof(BaseAI.IsEnemy), typeof(Character), typeof(Character))]
public class IsEnemy
{

  static bool Prefix(Character a, Character b, ref bool __result)
  {
    if (a == b) return false;
    if (!FactionManager.TryGetData(a.GetFaction(), out var data)) return true;
    var tamed = a.IsTamed();
    var targetTamed = b.IsTamed();
    // No point for tamed creatures to ever attack each other.
    if (tamed && targetTamed) return false;
    if (tamed)
    {
      // Tamed makes the system quite complicated.
      // To simplify, just make tames hostile to whatever is hostile to them.
      __result = BaseAI.IsEnemy(b, a);
      return false;
    }
    // targetTamed needed to avoid infinite recursion if aggravatable is also tamed.
    if (a.IsPlayer() && !targetTamed && b.GetBaseAI() && b.GetBaseAI().IsAggravatable())
    {
      // Players can always attack aggravatable creatures.
      // UI requires false for green health bar.
      // So for UI, show whatever is hostile to the player.
      __result = BaseAI.IsEnemy(b, a);
      return false;
    }
    // Because of above check, a is not tamed here.
    if (!data.AttackSame)
    {
      var group = a.GetGroup();
      if (group.Length > 0 && group == b.GetGroup())
      {
        __result = false;
        return false;
      }
    }
    var targetFaction = b.GetFaction();
    if (targetTamed && FactionManager.TryGetData(targetFaction, out var targetData))
      targetFaction = targetData.TamedFaction;
    var ai = a.GetBaseAI();
    if (ai && ai.IsAlerted())
      __result = !data.AlertedFriendly.Contains(targetFaction);
    else if (ai && ai.IsAggravated())
      __result = !data.AggravatedFriendly.Contains(targetFaction);
    else
      __result = !data.Friendly.Contains(targetFaction);
    return false;
  }
}
[HarmonyPatch(typeof(BaseAI), nameof(BaseAI.Awake))]
public class BaseAIAwake
{
  public static void Setup(BaseAI obj)
  {
    var zdo = obj.m_nview?.GetZDO();
    if (zdo == null) return;
    var factionStr = zdo.GetString(FactionHash);
    if (factionStr != "")
    {
      obj.m_character.m_faction = FactionManager.GetFaction(factionStr);
    }
    else
    {
      var faction = zdo.GetInt(FactionHash);
      if (faction != 0)
      {
        obj.m_character.m_faction = (Character.Faction)faction;
      }
    }
    obj.m_aggravatable = FactionManager.Aggravatable.Contains(obj.m_character.m_faction);
  }
  static readonly int FactionHash = "faction".GetStableHashCode();
  static void Postfix(BaseAI __instance) => Setup(__instance);
}