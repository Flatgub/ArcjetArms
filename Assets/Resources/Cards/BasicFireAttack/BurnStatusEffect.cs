using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnStatusEffect : StatusEffect, IStatusTurnStartEventHandler
{
    public int baseDamage;
    public int turnsRemaining;
    public int damagePerTurn;

    public BurnStatusEffect(int normalDamage, int turns, int damage)
    {
        baseDamage = normalDamage;
        turnsRemaining = turns;
        damagePerTurn = damage;
    }

    public override string GetName()
    {
        return "Burning";
    }

    public override string GetDescription()
    {
        string body = "Deal {0} damage and Burning for {1} turns, for {2} damage per turn";
        return string.Format(body, baseDamage, turnsRemaining, damagePerTurn);
    }

    public void OnTurnStart(Entity subject)
    {
        //take damage
        subject.ReceiveDamage(null, damagePerTurn);

        //make sure to run out after x amount of turns
        turnsRemaining -= 1;
        if (turnsRemaining <= 0)
        {
            subject.RemoveStatusEffect(this);
        }
    }

    
}
