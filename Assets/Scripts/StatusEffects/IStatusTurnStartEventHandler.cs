using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusTurnStartEventHandler
{
    void OnTurnStart(Entity subject, GameplayContext gc);
}
