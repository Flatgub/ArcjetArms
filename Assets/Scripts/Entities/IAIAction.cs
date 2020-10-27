using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAction
{
    string ActionName { get; }

    bool IsDoable(Entity with);

    void Do(Entity with);

    event Action OnActionFinish;
}
