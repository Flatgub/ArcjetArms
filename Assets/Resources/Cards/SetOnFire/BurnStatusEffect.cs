using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnStatusEffect : StatusEffect, IStatusTurnStartEventHandler
{
    public int turnsRemaining;
    public int damagePerTurn;

    public BurnStatusEffect(int turns, int damage)
    {
        turnsRemaining = turns;
        damagePerTurn = damage;
    }

    public override string GetName()
    {
        return "Burning";
    }

    public override string GetDescription()
    {
        string body = "Burning for {0} turns, for {1} damage per turn";
        return string.Format(body, turnsRemaining, damagePerTurn);
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
