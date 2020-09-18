using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBlock: CardData
{
    public float blockPercentage;

    public override string GenerateStaticDescription()
    {
        int blockAsPercentage = Mathf.CeilToInt(100 * blockPercentage);
        return string.Format(descriptionTemplate, blockAsPercentage);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription(); 
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Player.ApplyStatusEffect(new BlockStatusEffect(blockPercentage));
        outcome.Complete();
        yield break;
    }

}
