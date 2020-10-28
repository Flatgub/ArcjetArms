using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GearLoadout;

public static class GearDatabase
{
    private static readonly SortedList<int, GearData> allGear;
    private static readonly Dictionary<string, GearData> gearByName;
    private static readonly Dictionary<GearSlotTypes, List<GearData>> gearBySlotType;
    private static readonly string gearDirectory;
    private static bool loaded = false;

    public static bool IsLoaded { get { return loaded; } }

    static GearDatabase()
    {
        allGear = new SortedList<int, GearData>();
        gearByName = new Dictionary<string, GearData>();
        gearBySlotType = new Dictionary<GearSlotTypes, List<GearData>>();
        gearDirectory = Application.dataPath + "/Resources/Gear";
    }


    /// <summary>
    /// Get the gear from the given ID
    /// </summary>
    /// <param name="id">the ID of the gear to get</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if no gear exists for that ID</exception>
    /// <returns></returns>
    public static GearData GetGearDataByID(int id)
    {
        if (allGear.TryGetValue(id, out GearData gd))
        {
            return gd;
        }
        else
        {
            throw new IndexOutOfRangeException("No such gear exists for ID " + id);
        }
    }

    public static GearData GetGearDataByName(string name)
    {
        if (gearByName.TryGetValue(name, out GearData gd))
        {
            return gd;
        }
        else
        {
            throw new IndexOutOfRangeException("No such gear exists for name '" + name + "'");
        }
    }

    public static int GetGearIDByName(string name)
    {
        if (gearByName.TryGetValue(name, out GearData gd))
        {
            return gd.gearID;
        }
        else
        {
            throw new IndexOutOfRangeException("No such gear exists for name '" + name + "'");
        }
    }

    public static List<GearData> GetAllGearBySlotType(GearSlotTypes slot)
    {
        if (gearBySlotType.TryGetValue(slot, out List<GearData> gd))
        {
            return gd;
        }
        else
        {
            return new List<GearData>();
        }
    }

    /// <summary>
    /// Load all gear from the resources folders into the allGear list.
    /// </summary>
    public static void LoadAllGear()
    {
        if (!loaded)
        {
            LoadAllGearInFolder(gearDirectory);
            loaded = true;
        }
    }

    /// <summary>
    /// Recursively load all GearData assets from this folder and any folders belowit
    /// </summary>
    /// <param name="folder">The folder to load files from</param>
    private static void LoadAllGearInFolder(string folder)
    {
        GearData[] gearloaded = Resources.LoadAll<GearData>("Gear");
        foreach (GearData gearasset in gearloaded)
        {
            LoadGearAsset(gearasset);
        }
    }

    private static void LoadGearAsset(GearData gear)
    {
        if (allGear.ContainsKey(gear.gearID))
        {
            GearData conflict = allGear[gear.gearID];

            string warning =
                string.Format("Gear '{0}' has conflicting ID with '{1}' (id {2}), skipping...",
                gear.gearName, conflict.gearName, gear.gearID);

            Debug.LogWarning(warning);
            return;
        }

        allGear.Add(gear.gearID, gear);
        gearByName.Add(gear.gearName, gear);
        //see if the list for this catagory already exists
        if (gearBySlotType.TryGetValue(gear.requiredSlot, out List<GearData> catagory))
        {
            catagory.Add(gear);
        }
        else
        {
            List<GearData> newcatagory = new List<GearData>();
            newcatagory.Add(gear);
            gearBySlotType.Add(gear.requiredSlot, newcatagory);
        }
    }

    public static List<GearData> GetAllGearData()
    {
        List<GearData> output = new List<GearData>();
        foreach (KeyValuePair<int, GearData> pair in allGear)
        {
            output.Add(pair.Value);
        }

        return output;
        ;
    }

    public static LootPool GenerateMasterLootPool()
    {
        if (!loaded)
        {
            Debug.LogError("Made lootpool when lootpool wasn't loaded!");
        }

        LootPool master = new LootPool();

        foreach (KeyValuePair<int, GearData> pair in allGear)
        {
            GearData gear = pair.Value;
            if (gear.rarity != LootPool.LootRarity.DontSpawn)
            {
                //only one of each can be equipped at a time, so the lootpool only needs one copy
                if (gear.requiredSlot == GearSlotTypes.Head
                    || gear.requiredSlot == GearSlotTypes.Body
                    || gear.requiredSlot == GearSlotTypes.Core)
                {
                    master.AddGear(gear);
                }
                else
                {
                    master.AddGear(gear, n: 2);
                }
            }
        }
        return master;
    }

}
