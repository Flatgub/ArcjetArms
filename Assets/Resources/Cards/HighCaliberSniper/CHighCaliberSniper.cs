using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CHighCaliberSniper: CardData
{
    public int range;
    public int damageOverDistance;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate,range,damageOverDistance);
    }

    public override string GenerateCurrentDescription()
    {
        if (GameplayContext.Player == null || GameplayContext.EntityUnderMouse == null)
        {
            return GenerateStaticDescription();
        }

        Entity player = GameplayContext.Player;
        Entity target = GameplayContext.EntityUnderMouse;
        int damage = target.Position.DistanceTo(player.Position) * damageOverDistance;
        damage = Entity.CalculateDamage(player, target, damage);
        

        string dmgstring = damage.Colored(Color.red);
        return string.Format(descriptionTemplate + " ({2})", range, damageOverDistance, dmgstring);
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        Hex pos = GameplayContext.Player.Position;

        //TODO: replace this with a line of sight check maybe?
        List<Entity> targets =
            GridHelper.GetEntitiesInRange(GameplayContext.Grid, pos, range);

        //don't let the player shoot themselves
        targets.Remove(GameplayContext.Player);

        SingleEntityResult target = GameplayContext.Ui.OfferSingleEntitySelection(targets);

        yield return new WaitUntil(target.IsReadyOrCancelled);

        if (!target.WasCancelled())
        {
            Entity victim = target.GetResult();
            int damage = victim.Position.DistanceTo(pos) * damageOverDistance;
            GameplayContext.Player.DealDamageTo(victim, damage);
            GameplayContext.Player.TriggerAttackEvent(victim);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
