using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLightningArt1 : CardData
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
            Entity victim = target.GetResult();
            if (victim.Health.HealthAsFraction() <= 0.5f)
            {
                baseDamage = baseDamage * 2;
            }
            GameplayContext.Player.DealDamageTo(victim, baseDamage);
            GameplayContext.Player.TriggerAttackEvent(victim);
            if (adjacentEnts.Contains(GameplayContext.Player))
            {
                adjacentEnts.Remove(GameplayContext.Player);

            }
            foreach (Entity toZap in adjacentEnts)
            {
                //hit 'em
                Entity victims = target.GetResult();
                if (toZap.HasStatusEffect(typeof(WetStatusEffect)))
                {
                    GameplayContext.Player.DealDamageTo(victims, baseDamage * 2);
                    toZap.ApplyStatusEffect(new StunStatusEffect());
                }
                else
                {
                    GameplayContext.Player.DealDamageTo(victims, baseDamage);
                }

               
                outcome.Complete();
            }
        }
        else
        {
            outcome.Cancel();
        }
        

    }
}