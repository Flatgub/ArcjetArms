using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHexSelection 
{
    public SingleHexResult result;
    public InterfaceManager manager;

    public SingleHexSelection(InterfaceManager manager, ICollection<Hex> candidates, SingleHexResult result)
    {
        this.manager = manager;
        this.result = result;
        foreach (Hex hex in candidates)
        {
            if (manager.grid.Contains(hex))
            {
                // add a selection hex to the interfacemanager
                manager.GenerateSelectionHex(hex, this);
            }
        }
    }

    public void OnCandidateSelected(Hex hex)
    {
        result.AddSelection(hex);
        manager.ClearSelectionHexes();
        //and then we die
    }

    public void Cancel()
    {
        result.Cancel();
        manager.ClearSelectionHexes();
        //and then we die
    }
}
