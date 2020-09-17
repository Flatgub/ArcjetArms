using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAction
{
    bool IsDoable(Entity with);

    void Do(Entity with);
}
