using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DelayedResult is used as asynchronous container for getting a result at a later date.
/// Methods which require some process that may take real time should immediately return a
/// DelayedResult, so that the data can be extracted when <see cref="IsReadyOrCancelled"/> returns
/// true.</summary>
/// <remarks>The primary use case for DelayedResult is in coroutines, where you request some 
/// data and then <see cref="WaitUntil"/>(<see cref="IsReadyOrCancelled"/>)</remarks>,
/// at which point the coroutine will resume execution when the choice has been made
public abstract class DelayedResult<T>
{
    protected bool isReady;
    protected bool isCancelled;
    protected T result;

    /// <summary>
    /// Check whether 
    /// </summary>
    /// <remarks>Primarily useful for waiting until the result is available, such as 
    /// in a coroutine or a update loop. </remarks>
    /// <returns><c>true</c> when</returns>
    public virtual bool IsReadyOrCancelled()
    {
        return isReady || isCancelled;
    }

    /// <summary>
    /// Get the result if the result is ready.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown when GetResult is called before the
    /// a result is ready or if the result was cancelled</exception>
    public virtual T GetResult()
    {
        if (!isReady)
        {
            throw new InvalidOperationException(this + " attempted to get result while not ready");
        }
        else if (isCancelled)
        {
            throw new InvalidOperationException(this + " attempted to get result while cancelled");
        }

        return result;
    }

    /// <summary>
    /// Mark the delayedResult as cancelled, allowing for the completion signal without requring a 
    /// result
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual void Cancel()
    {
        if (isReady)
        {
            throw new InvalidOperationException(this + " is ready and cannot be cancelled");
        }

        isCancelled = true;
    }

    /// <summary>
    /// Check whether the delayedResult has been cancelled and contains no result;
    /// </summary>
    /// <returns><c>true</c> if the selection was cancelled, otherwise <c>false</c></returns>
    public virtual bool WasCancelled()
    {
        return isCancelled;
    }
}
