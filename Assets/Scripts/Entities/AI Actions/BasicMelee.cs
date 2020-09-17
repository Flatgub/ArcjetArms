using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMelee : IAIAction
{
    public int baseDamage = 5;

    public void Do(Entity with)
    {
        with.DealDamageTo(GameplayContext.Player, baseDamage);
        with.TriggerAttackEvent(GameplayContext.Player);
    }

    public bool IsDoable(Entity with)
    {
        List<Entity> adjacent= GridHelper.GetAdjacentEntities(with.Grid, with.Position);

        return (adjacent.Contains(GameplayContext.Player));

    }
}
