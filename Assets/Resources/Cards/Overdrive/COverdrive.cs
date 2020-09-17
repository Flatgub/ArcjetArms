using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COverdrive : CardData
{
    public int turns = 2;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, turns);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription(); 
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        GameplayContext.Player.ApplyStatusEffect(new OverdriveStatusEffect(turns));
        outcome.Complete();
        yield break;
    }

}
