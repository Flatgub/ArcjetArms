using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStatusEffect : StatusEffect, IStatusTurnEndEventHandler, IStatusApplyEventHandler
{
    public override string GetDescription()
    {
        return "No actions can be taken until the end of next turn";
    }

    public override string GetName()
    {
        return "Stunned";
    }

    public void OnApply(Entity subject)
    {
        subject.isStunned = true;
    }

    public void OnTurnEnd(Entity subject)
    {
        subject.isStunned = false;
        subject.RemoveStatusEffect(this);
    }
}
