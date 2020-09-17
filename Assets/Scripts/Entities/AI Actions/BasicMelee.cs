using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMelee : IAIAction
{
    public int baseDamage = 5;

    public void Do(GameplayContext context, Entity with)
    {
        with.DealDamageTo(context.Player, baseDamage);
        with.TriggerAttackEvent(context.Player, context);
    }

    public bool IsDoable(GameplayContext context, Entity with)
    {
        List<Entity> adjacent= GridHelper.GetAdjacentEntities(with.Grid, with.Position);

        return (adjacent.Contains(context.Player));

    }
}
