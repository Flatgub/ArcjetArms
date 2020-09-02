using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEntityResult : DelayedResult<Entity>
{
    /// <summary>
    /// Used by the selection promps to set the result of the selection, which can only be done
    /// once
    /// </summary>
    public void AddSelection(Entity ent)
    {
        if (isReady)
        {
            throw new InvalidOperationException("SingleEntityResult already has a result");
        }
        else if (isCancelled)
        {
            throw new InvalidOperationException("SingleEntityResult was cancelled and isn't " +
                "accepting results");
        }
        else
        {
            result = ent;
            isReady = true;
        }
    }
}
