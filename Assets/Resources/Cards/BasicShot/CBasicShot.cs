using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBasicShot: CardData
{
    public int range;
    public int baseDamage;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, range, baseDamage);
    }

    public override string GenerateCurrentDescription()
    {
        if (GameplayContext.Player == null)
        {
            return GenerateStaticDescription();
        }

        int damage = baseDamage;
        if (GameplayContext.EntityUnderMouse is Entity target)
        {
            damage = Entity.CalculateDamage(GameplayContext.Player, target, damage);
        }
        else
        {
            damage = GameplayContext.Player.CalculateDamage(damage);
        }

        string dmgstring = damage.Colored(Color.red);
        return string.Format(descriptionTemplate, range, dmgstring);
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
            GameplayContext.Player.DealDamageTo(victim, baseDamage);
            GameplayContext.Player.TriggerAttackEvent(victim);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
