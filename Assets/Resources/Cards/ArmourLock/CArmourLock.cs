using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArmourLock : CardData
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
        GameplayContext.Player.ApplyStatusEffect(new ArmourLockStatusEffect(blockPercentage));
        GameplayContext.Manager.EndPlayerTurn();
        outcome.Complete();
        yield break;
    }

}
