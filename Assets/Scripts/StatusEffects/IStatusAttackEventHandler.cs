using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusAttackEventHandler
{
    void OnAttack(Entity subject, Entity target);
}
