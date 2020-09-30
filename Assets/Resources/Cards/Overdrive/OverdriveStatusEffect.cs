using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverdriveStatusEffect : StatusEffect, IStatusCalculateDamageEventHandler,
    IStatusTurnStartEventHandler, IStackableStatus
{
    private int remainingTurns;

    public OverdriveStatusEffect(int turns)
    {
        remainingTurns = turns;
    }

    //add the number of turns that would've been added to the number we have so far
    public void GainStack(IStackableStatus other)
    {
        OverdriveStatusEffect o = other as OverdriveStatusEffect;
        remainingTurns += o.remainingTurns;
    }

    public override string GetName()
    {
        return "In Overdrive";
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
            phrase = "For the next " + remainingTurns.Colored(Color.yellow) + " turns ";
        }
        return phrase + "all attacks deal "+"+5".Colored(Color.yellow)+" damage";
    }

    public int OnCalculateDamageAdditive(int damage)
    {
        return damage + 5;
    }

    public float OnCalculateDamageMultiplicative(float damage)
    {
        return damage;
    }

    public void OnTurnStart(Entity subject)
    {
        remainingTurns--;
        if (remainingTurns == 0)
        {
            Debug.Log("overdrive wore off");
            subject.RemoveStatusEffect(this);
        }
    }
}
