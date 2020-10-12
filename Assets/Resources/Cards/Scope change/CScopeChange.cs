using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CScopeChange : CardData
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
        GameplayContext.Player.ApplyStatusEffect(new copeChangeEffect());
        outcome.Complete();
        yield break;
    }

}
