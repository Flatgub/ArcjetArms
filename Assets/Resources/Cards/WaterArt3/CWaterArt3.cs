using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CWaterArt3: CardData
{
    public float blockPercentage;
    public int turns = 1;


    public override string GenerateStaticDescription()
    {
        int blockAsPercentage = Mathf.CeilToInt(100 * blockPercentage);
        return string.Format(descriptionTemplate, turns, blockAsPercentage);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription();
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Player.ApplyStatusEffect(new WaterWallStatusEffect(blockPercentage, turns));
        outcome.Complete();
        yield break;
    }

}

