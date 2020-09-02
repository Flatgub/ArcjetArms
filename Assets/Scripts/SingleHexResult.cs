using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SingleHexResult is used as asynchronous container for getting the result of a selection.
/// Methods which require the player to make a selection using the UI will immediately return a
/// SingleHexResult, but data must not be extracted until <see cref="IsReady"/> returns true.
/// </summary>
/// <remarks>The primary use case for SingleHexResult is in coroutines, where you request a 
/// selection from the user and then <see cref="WaitUntil"/>(<see cref="IsReady"/>)</remarks>,
/// at which point the coroutine will resume execution when the choice has been made
public class SingleHexResult : DelayedResult<Hex>
{

    /// <summary>
    /// Used by the InterfaceManager to set the result of the selection, which can only be done
    /// once
    /// </summary>
    public void AddSelection(Hex hex)
    {
        if (isReady)
        {
            throw new InvalidOperationException("SelectionResult already has a result");
        }
        else if (isCancelled)
        {
            throw new InvalidOperationException("SelectionResult was cancelled and isn't " +
                "accepting results");
        }
        else
        {
            result = hex;
            isReady = true;
        }
    }

}

