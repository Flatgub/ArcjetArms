using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COverclock: CardData
{

    public float healthPenalty;
    public float damageMultiplier;

    public override string GenerateStaticDescription()
    {
        int penaltyAsPercentage = Mathf.CeilToInt(healthPenalty * 100);
        return string.Format(descriptionTemplate, penaltyAsPercentage, damageMultiplier); 
    }

    public override string GenerateCurrentDescription()
    {
        
        return GenerateStaticDescription(); 
    }

    //TODO: come back and fix this once resistance exists
    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        OverclockStatusEffect effect = new OverclockStatusEffect(healthPenalty, damageMultiplier);
        GameplayContext.Player.ApplyStatusEffect(effect);
        outcome.Complete();
        yield break;
    }

}
