using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CSteadyAim: CardData
{
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription();
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Player.ApplyStatusEffect(new SteadyAimStatusEffect());
        outcome.Complete();
        yield break;
    }

}