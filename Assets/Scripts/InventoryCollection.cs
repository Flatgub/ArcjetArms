using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GearLoadout;

public class InventoryCollection 
{
    private Dictionary<int, int> contents;

    public InventoryCollection()
    {
        contents = new Dictionary<int, int>();
    }

    public void AddItem(GearData gear, int n = 1)
    {
        int gearID = gear.gearID;
        if (contents.TryGetValue(gearID, out int existingQuantity))
        {
            
            contents[gearID] = existingQuantity + n;
        }
        else
        {
            contents.Add(gearID, n);
        }
    }

    public void RemoveItem(GearData gear)
    {
        int gearID = gear.gearID;
        if (contents.TryGetValue(gearID, out int existingQuantity))
        {
            int newQuantity = existingQuantity - 1;
            if (newQuantity > 0)
            {
                contents[gearID] = newQuantity;
            }
            else
            {
                contents.Remove(gearID);
            }
        }
    }

    public bool ContainsItem(GearData gear)
    {
        return contents.ContainsKey(gear.gearID);
    }

    /// <summary>
    /// Returns the total number of gear pieces in the inventory.
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        int count = 0;
        foreach (KeyValuePair<int, int> pair in contents)
        {
            count += pair.Value;
        }
        return count;
    }

    /// <summary>
    /// Get a list of each kind of unique gear piece in the inventory. 
    /// Duplicate pieces aren't shown
    /// </summary>
    /// <returns></returns>
    public List<GearData> GetAllGearTypes()
    {
        List<GearData> output = new List<GearData>();
        foreach (KeyValuePair<int, int> pair in contents)
        {
            output.Add(GearDatabase.GetGearDataByID(pair.Key));
        }
        return output;
    }

    /// <summary>
    /// Get a list of each kind of unique gear piece in the inventory that fits in a given slot
    /// </summary>
    /// <param name="slotFilter">The requiredSlot to filter for</param>
    /// <returns></returns>
    public List<GearData> GetAllGearTypesOfSlot(GearSlotTypes slotFilter)
    {
        List<GearData> output = new List<GearData>();
        foreach (KeyValuePair<int, int> pair in contents)
        {
            GearData gear = GearDatabase.GetGearDataByID(pair.Key);
            if (gear.requiredSlot == slotFilter)
            {
                output.Add(gear);
            }
        }
        return output;
    }
}
