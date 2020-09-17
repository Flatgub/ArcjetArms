using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusTurnEndEventHandler
{
    void OnTurnEnd(Entity subject, GameplayContext gc);
}
