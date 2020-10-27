using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyHealing : IAIAction
{
    public string ActionName { get { return "BasicMelee";} }

    public int healAmount;
    public event Action OnActionFinish;

    public ApplyHealing(int heal)
    {
        healAmount = heal;
    }

    public void Do(Entity with)
    {
        if (GameplayContext.ActiveEntity)
        {
            GameplayContext.ActiveEntity.Health.ApplyHealing(healAmount);
        }
        LeanTween.moveLocal(with.gameObject, GameplayContext.ActiveEntity.transform.position, 0.1f)
            .setEaseInCubic().setLoopPingPong(1);

        LeanTween.delayedCall(0.2f, () =>
        {
            //OnActionFinish?.Invoke();
            OnActionFinish = null;
        });
    }

    public bool IsDoable(Entity with)
    {
        List<Entity> adjacent= GridHelper.GetAdjacentEntities(with.Grid, with.Position);

        return (adjacent.Contains(GameplayContext.ActiveEntity));

    }
}
