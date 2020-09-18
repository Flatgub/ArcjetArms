using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIgnite: CardData
{
    public int increasePrecentage = 20;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, increasePrecentage); 
    }

    public override string GenerateCurrentDescription()
    {

        return GenerateStaticDescription(); 
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Player.ApplyStatusEffect(new IgniteStatusEffect(increasePrecentage));
        outcome.Complete();
        yield break;
    }

}
