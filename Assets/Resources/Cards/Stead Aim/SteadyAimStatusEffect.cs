using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyAimStatusEffect : StatusEffect, IStatusTurnEndEventHandler
, IStatusCalculateDamageEventHandler
{
    public override string GetName()
    {
        return "SteadyAim";
    }
    public override string GetDescription()
    {

        return string.Format("All attacks deal double damage for the rest of turn", 2f);
           
    }
    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }
    public float OnCalculateDamageMultiplicative(float damage)
    {
        return damage * 2f;
    }
    public void OnTurnEnd(Entity subject)
    {
        subject.RemoveStatusEffect(this);
    }
}
