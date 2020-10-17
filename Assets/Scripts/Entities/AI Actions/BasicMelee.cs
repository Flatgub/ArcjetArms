using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMelee : IAIAction
{
    public string ActionName { get { return "BasicMelee";} }

    public int baseDamage;
    public event Action OnActionFinish;

    public BasicMelee(int damage)
    {
        baseDamage = damage;
    }

    public void Do(Entity with)
    {
        with.DealDamageTo(GameplayContext.Player, baseDamage);
        with.TriggerAttackEvent(GameplayContext.Player);
        LeanTween.moveLocal(with.gameObject, GameplayContext.Player.transform.position, 0.1f)
            .setEaseInCubic().setLoopPingPong(1);

        LeanTween.delayedCall(0.2f, () =>
        {
            OnActionFinish?.Invoke();
            OnActionFinish = null;
        });
    }

    public bool IsDoable(Entity with)
    {
        List<Entity> adjacent= GridHelper.GetAdjacentEntities(with.Grid, with.Position);

        return (adjacent.Contains(GameplayContext.Player));

    }
}
