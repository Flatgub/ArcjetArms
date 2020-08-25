using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SelectionResult is used as asynchronous container for getting the result of a selection.
/// Methods which require the player to make a selection using the UI will immediately return a
/// SelectionResult, but data must not be extracted until <see cref="IsReady"/> returns true.
/// </summary>
/// <remarks>The primary use case for SelectionResult is in coroutines, where you request a 
/// selection from the user and then <see cref="WaitUntil"/>(<see cref="IsReady"/>)</remarks>,
/// at which point the coroutine will resume execution when the choice has been made
public class SelectionResult
{
    private bool hasResult;
    private bool wasCancelled;
    private Hex result;

    /// <summary>
    /// Used by the InterfaceManager to set the result of the selection, which can only be done
    /// once
    /// </summary>
    public void SetResult(Hex hex)
    {
        if (hasResult)
        {
            throw new InvalidOperationException("SelectionResult already has a result");
        }
        else if (wasCancelled)
        {
            throw new InvalidOperationException("SelectionResult was cancelled and isn't " +
                "accepting results");
        }
        else
        {
            result = hex;
            hasResult = true;
        }
    }

    /// <summary>
    /// Get the result from the selection, if the selection is ready.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown when GetResult is called before the
    /// a result has been inserted</exception>
    public Hex GetResult()
    {
        if (!hasResult)
        {
            throw new InvalidOperationException("SelectionResult is not ready");
        }
        else
        {

        }

        return result;
    }

    /// <summary>
    /// Mark the selection as cancelled, representing no result but ending the selection
    /// </summary>
    public void CancelSelection()
    {
        wasCancelled = true;
    }

    /// <summary>
    /// Check whether the SelectionResult is ready and a value can be extracted or if the selection
    /// was cancelled </summary>
    /// <returns><c>true</c> if ready or cancelled, otherwise <c>false</c></returns>
    public bool IsReadyOrCancelled()
    {
        return hasResult || wasCancelled;
    }

    /// <summary>
    /// Check whether the selection was cancelled instead of a result being chosen
    /// </summary>
    /// <returns><c>true</c> if the selection was cancelled, otherwise <c>false</c></returns>
    public bool WasCancelled()
    {
        return wasCancelled;
    }
}

