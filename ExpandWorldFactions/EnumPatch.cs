using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
namespace ExpandWorldData.Factions;

#pragma warning disable IDE0051
[HarmonyPatch]
public class TryParseFaction
{
  static IEnumerable<MethodBase> TargetMethods()
  {
    return typeof(Enum).GetMethods()
        .Where(method => method.Name == "TryParse")
        .Take(2)
        .Select(method => method.MakeGenericMethod(typeof(Character.Faction)))
        .Cast<MethodBase>();
  }

  static bool Prefix(string value, ref Character.Faction result, ref bool __result)
  {
    __result = FactionManager.TryGetFaction(value, out result);
    return false;
  }
}
#pragma warning restore  IDE0051
[HarmonyPatch(typeof(Enum), nameof(Enum.GetValues))]
public class GetValues
{
  static bool Prefix(Type enumType, ref Array __result)
  {
    if (enumType == typeof(Character.Faction))
    {
      __result = FactionManager.FactionToDisplayName.Keys.ToArray();
      return false;
    }
    return true;
  }
}
[HarmonyPatch(typeof(Enum), nameof(Enum.GetNames))]
public class GetNames
{
  static bool Prefix(Type enumType, ref string[] __result)
  {
    if (enumType == typeof(Character.Faction))
    {
      __result = FactionManager.FactionToDisplayName.Values.ToArray();
      return false;
    }
    return true;
  }
}
[HarmonyPatch(typeof(Enum), nameof(Enum.GetName))]
public class GetName
{
  static bool Prefix(Type enumType, object value, ref string __result)
  {
    if (enumType == typeof(Character.Faction))
    {
      if (FactionManager.TryGetDisplayName((Character.Faction)value, out var result))
        __result = result;
      else
        __result = "None";
      return false;
    }
    return true;
  }
}
[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), typeof(Type), typeof(string))]
public class EnumParse
{
  static bool Prefix(Type enumType, string value, ref object __result)
  {
    if (enumType == typeof(Character.Faction))
    {
      if (FactionManager.TryGetFaction(value, out var faction))
      {
        __result = faction;
        return false;
      }
      // Let the original function handle the throwing.
    }
    return true;
  }
}
[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), typeof(Type), typeof(string), typeof(bool))]
public class ParseIgnoreCase
{
  static bool Prefix(Type enumType, string value, ref object __result)
  {
    if (enumType == typeof(Character.Faction))
    {
      if (FactionManager.TryGetFaction(value, out var faction))
      {
        __result = faction;
        return false;
      }
      // Let the original function handle the throwing.
    }
    return true;
  }
}
