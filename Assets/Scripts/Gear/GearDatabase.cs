using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GearDatabase
{
    private static readonly SortedList<int, GearData> allGear;
    private static readonly Dictionary<string, GearData> gearByName;
    private static readonly string gearDirectory;

    static GearDatabase()
    {
        allGear = new SortedList<int, GearData>();
        gearByName = new Dictionary<string, GearData>();
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

    /// <summary>
    /// Load all gear from the resources folders into the allGear list.
    /// </summary>
    public static void LoadAllGear()
    {
        LoadAllGearInFolder(gearDirectory);
    }

    /// <summary>
    /// Recursively load all GearData assets from this folder and any folders belowit
    /// </summary>
    /// <param name="folder">The folder to load files from</param>
    private static void LoadAllGearInFolder(string folder)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folder);

        //collect all gear at this directory
        foreach (FileInfo card in dirInfo.GetFiles("*.asset"))
        {
            //TODO: referencing carddatabase here is bad, this function should just be a helper
            string gearpath = CardDatabase.ConvertRealDirToResourceDir(card.FullName);
            gearpath = gearpath.Substring(0, gearpath.Length - 6); //remove .asset from the path
            GearData gearasset = Resources.Load<GearData>(gearpath);
            allGear.Add(gearasset.gearID, gearasset);
            gearByName.Add(gearasset.gearName, gearasset);
        }

        //recursively load cards in subdirectories
        foreach (DirectoryInfo d in dirInfo.GetDirectories())
        {
            LoadAllGearInFolder(d.FullName);
        }
    }
}
