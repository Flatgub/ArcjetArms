using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GearLoadout;

public class LoadoutSlot
{
    public string name;
    public GearSlotTypes type;
    public bool hidden;
    public GearData contains;

    public LoadoutSlot(string name, GearSlotTypes slot, bool hidden)
    {
        this.name = name;
        this.type = slot;
        this.hidden = hidden;
        this.contains = null;
    }
}
