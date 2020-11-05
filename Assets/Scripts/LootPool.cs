using System.Collections.Generic;
using UnityEngine;

public class LootPool
{
    private List<GearData> pool;
    private Dictionary<GearData, int> quantities;
    private int poolSum = 0;
    public List<GearData> activePool;

    public int Count
    {
        get
        {
            return pool.Count;
        }
    }

    public enum LootRarity
    {
        DontSpawn = -1,
        Rare = 1,
        Uncommon = 5, 
        Common = 10
    }

    public LootPool()
    {
        pool = new List<GearData>();
        quantities = new Dictionary<GearData, int>();
    }

    public LootPool Clone()
    {
        LootPool newpool = new LootPool();
        newpool.pool.AddRange(pool);
        newpool.quantities = new Dictionary<GearData, int>(quantities);
        return newpool;
    }

    public void AddGear(GearData gear, int n = 1)
    {
        if (quantities.TryGetValue(gear, out int current))
        {
            quantities[gear] = current + n;
        }
        else
        {
            pool.Add(gear);
            quantities.Add(gear, n);
        }

        //if (gear.rarity != LootRarity.DontSpawn)
        //{
         //   poolSum += (int)gear.rarity * n;
        //}
        
    }

    public void RemoveGear(GearData gear)
    {
        if (quantities.TryGetValue(gear, out int current))
        {
            int newcurrent = current - 1;
            if (newcurrent > 0)
            {
                quantities[gear] = newcurrent;
            }
            else
            {
                quantities.Remove(gear);
                pool.Remove(gear);
            }

            //if (gear.rarity != LootRarity.DontSpawn)
           // {
            //    poolSum -= (int)gear.rarity;
           // }
        }
    }

    public void SubtractLoadout(GearLoadout loadout)
    {
        List<GearData> gearInLoadout = loadout.ToList();
        foreach (GearData gear in gearInLoadout)
        {
            RemoveGear(gear);
        }
    }

    public void SubtractInventory(InventoryCollection inv)
    {
        List<GearData> geartypes = inv.GetAllGearTypes();
        foreach (GearData gear in geartypes)
        {
            int count = inv.GetCountOf(gear);
            for (int i = 0; i < count; i++)
            {
                RemoveGear(gear);
            }
        }
    }

    public void MakeActive()
    {
        if (activePool == null)
        {
            activePool = new List<GearData>();
            foreach (GearData type in pool)
            {
                int count = quantities[type];
                for (int i = 0; i < count; i++)
                {
                    activePool.Add(type);
                }

                if (type.rarity != LootRarity.DontSpawn)
                {
                    poolSum += (int)type.rarity * count;
                }
            }
            activePool.Shuffle();
        }
    }

    public void Finish()
    {
        activePool = null;
        poolSum = 0;
    }

    public GearData Pop()
    {
        if (activePool == null)
        {
            MakeActive();
        }

        int budget = Random.Range(0, poolSum);
        GearData selected = null;

        int index = 0;
        while (budget >= 0 && index < activePool.Count)
        {
            selected = activePool[index];
            budget -= (int)selected.rarity;
            index++;
        }
        if (index != 0)
        {
            index--;
        } 
        
        activePool.RemoveAt(index--);
        poolSum -= (int)selected.rarity;

        return selected;
    }
}
