﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWallStatusEffect : StatusEffect, IStatusReceiveDamageEventHandler, IStatusTurnStartEventHandler
{
    private float blockAmount;
    private int remainingTurns;

    public WaterWallStatusEffect(float amount, int turns)
    {
        blockAmount = amount;
        remainingTurns = turns;
    }

    public override string GetName()
    {
        return "Blocked";
    }

    public override string GetDescription()
    {
        string phrase = "";
        if (remainingTurns == 1)
        {
            phrase = "Till the end of turn ";
        }
        else
        {
            phrase = "For the next " + remainingTurns.Colored(Color.blue) + " turns ";
        }
        return phrase + "all attacks deal " + "0".Colored(Color.blue) + " damage to you";

        string amount = Mathf.CeilToInt((1f - blockAmount) * 100).ToString();
        return string.Format("Resist {0} incoming damage from the next attack.",
            (amount + "%").Colored(Color.blue));
    }

    public void OnTurnStart(Entity subject)
    {
        remainingTurns--;
        if (remainingTurns == 0)
        {
            subject.RemoveStatusEffect(this);
        }
    }
    public void OnAttacked(Entity subject, Entity attacker)
    {
        subject.RemoveStatusEffect(this);
    }

    public int OnReceivingDamage(int baseDamage)
    {
        return Mathf.CeilToInt(baseDamage * blockAmount);
    }
}
