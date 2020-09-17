using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStatusEffect : StatusEffect, IStatusAttackEventHandler, 
    IStatusCalculateDamageEventHandler
{
    public void OnAttack(Entity subject, Entity target, GameplayContext gc)
    {
        if (gc.ActiveCard is Card card && card.cardData.title == "Punch")
        {
            Debug.Log("that was a punch!");
            subject.RemoveStatusEffect(this);
        }
    }

    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }

    public float OnCalculateDamageMultiplicative(float damage)
    {
        return damage * 2f;
    }
}
