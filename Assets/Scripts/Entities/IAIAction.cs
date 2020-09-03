using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAction
{
    bool IsDoable(GameplayContext context, Entity with);

    void Do(GameplayContext context, Entity with);
}
