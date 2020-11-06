using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBasicLightningAttack : CardData
{
    public int baseDamage;


    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage);
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
        return string.Format(descriptionTemplate, dmgstring);
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {

        //get a list of adjacent entities
        List<Entity> adjacentEnts = GridHelper.GetAdjacentEntities(GameplayContext.Grid,
            GameplayContext.Player.Position);



        //let the player select one of the adjacent entities
        SingleEntityResult target =
            GameplayContext.Ui.OfferSingleEntitySelection(adjacentEnts);

        //wait for the player to make a selection
        yield return new WaitUntil(target.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!target.WasCancelled())
        {
            //hit 'em
            Entity victim = target.GetResult();      
            if (adjacentEnts.Contains(GameplayContext.Player))
        {
            adjacentEnts.Remove(GameplayContext.Player);

        }
        foreach (Entity toZap in adjacentEnts)
        {
            if (toZap.HasStatusEffect(typeof(WetStatusEffect)))
            {
                GameplayContext.Player.DealDamageTo(victim, baseDamage * 2);
                toZap.ApplyStatusEffect(new StunStatusEffect());
            }
            else
            {
                GameplayContext.Player.DealDamageTo(victim, baseDamage);
            }
        }
            GameplayContext.Player.TriggerAttackEvent(victim);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}