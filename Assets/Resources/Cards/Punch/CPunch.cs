using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPunch : CardData
{
    public int baseDamage;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage);
    }

    public override string GenerateCurrentDescription(GameplayContext context)
    {
        return GenerateStaticDescription();
    }

    public override IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome)
    {
        List<CombatEntity> adjacentEnts = GridHelper.GetAdjacentEntities(context.Grid,
            context.Player.Position);



        //context.Player.DealDamageTo(target, baseDamage);

        outcome.Cancel();
        yield break;
    }

}
