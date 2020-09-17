using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSalvage : CardData
{
    public int cardsToDraw;

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Manager.AttemptDrawCard(n: cardsToDraw);
        outcome.Complete();
        yield break;
    }

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, cardsToDraw);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription();
    }
}
