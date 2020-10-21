using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetStatusEffect : StatusEffect, IStatusTurnStartEventHandler
{
    public int baseDamage;
    public int turnsRemaining;


    public BurnStatusEffect(int normalDamage, int turns)
    {
        baseDamage = normalDamage;
        turnsRemaining = turns;
    }

    public override string GetName()
    {
        return "Wet";
    }

    public override string GetDescription()
    {
        string body = "Become wet for {0} turns";
        return string.Format(body, baseDamage, turnsRemaining);
    }

    public void OnTurnStart(Entity subject)
    {

        //make sure to run out after x amount of turns
        turnsRemaining -= 1;
        if (turnsRemaining <= 0)
        {
            subject.RemoveStatusEffect(this);
        }
    }


}
