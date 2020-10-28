using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;

public class AI_Scavva : IAiTemplate
{
    public int MaxHealth = 30;
    public int moveSpeed = 4;
    public int attackDamage = 5;
    public int attackRange = 1;
    public int DifficultyScore { get { return 3; } }
    // Start is called before the first frame update

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Scavva";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        // entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new MoveAwayFromPlayer(moveSpeed), 1);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);

    }
}

class Scavange : IAIAction
{
    public string ActionName { get { return "Scavange"; } }
    public event Action OnActionFinish;

    void IAIAction.Do(Entity with)
    {
        if (!with.IsDead)
        {
            GameplayContext.Manager.HandSize == 4;
            //GameplayContext.Player.ApplyStatusEffect(asdg)

            LeanTween.delayedCall(0.2f, () =>
            {
                OnActionFinish?.Invoke();
                OnActionFinish = null;
            });
        }

    }

    bool IAIAction.IsDoable(Entity with)
    {
        if (with.IsDead)
        {
            return false;
        }

        else
        {
            return true;
        }
    }
}

