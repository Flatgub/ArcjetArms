using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionResult : DelayedResult<bool>
{
    public void Complete()
    {
        if (isCancelled)
        {
            throw new InvalidOperationException("CardActionresult was cancelled and cannot be " +
                "completed");
        }

        isReady = true;
        result = true;
    }
}
