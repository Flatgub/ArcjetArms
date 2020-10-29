using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReducedDrawStatusEffect : StatusEffect, IStatusApplyEventHandler, IStatusTurnEndEventHandler
{
    public override string GetDescription()
    {
        return "You only draw 4 cards per turn";
    }

    public override string GetName()
    {
        return "Disrupted";
    }

    public void OnApply(Entity subject)
    {
        if (subject == GameplayContext.Player)
        {
            GameplayContext.Manager.HandSize = 4;
        }
    }

    public void OnTurnEnd(Entity subject)
    {
        GameplayContext.Manager.HandSize = 5;
        subject.RemoveStatusEffect(this);
    }
}
