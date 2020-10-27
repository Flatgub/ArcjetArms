using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFireArt1 : CardData
{
    public int baseDamage;
    public int turnsRemaining;
    public int damagePerTurn;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage, turnsRemaining, damagePerTurn);
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
        List<Entity> adjacentEnts = GridHelper.GetAdjacentEntities(GameplayContext.Grid, GameplayContext.Player.Position);


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

            //get all 6 spots around the player
            Hex diffVector = GameplayContext.Player.Position - victim.Position;
            List<Hex> spotsToBurn =
                GridHelper.GetHexesInRange(GameplayContext.Grid, GameplayContext.Player.Position, 1, false);

            spotsToBurn.Remove(diffVector + GameplayContext.Player.Position); //remove the spot behind the target
            spotsToBurn.Remove(GameplayContext.Player.Position); //don't hit the player

            foreach (Hex h in spotsToBurn)
            {
                if (GameplayContext.Grid.GetEntityAtHex(h) is Entity toHit)
                {
                    GameplayContext.Player.DealDamageTo(toHit, baseDamage);
                    toHit.ApplyStatusEffect(new BurnStatusEffect(baseDamage, damagePerTurn, turnsRemaining));
                    GameplayContext.Player.TriggerAttackEvent(toHit);
                }
            }

            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}