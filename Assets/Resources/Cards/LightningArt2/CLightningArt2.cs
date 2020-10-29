using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CLightningArt2 : CardData
{

    public int moveDistance;
    public int baseDamage;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, moveDistance, baseDamage);
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
        return string.Format(descriptionTemplate, moveDistance, dmgstring);
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        List<Hex> movementCandidates = new List<Hex>();

        //cast out a line in each direction from the player to determine movement spots
        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line = GridHelper.CastLineInDirection(GameplayContext.Grid,
                GameplayContext.Player.Position, dir, moveDistance, includeStart: false);

            movementCandidates.AddRange(line);
        }



        // Show the locations to the player and let them pick one
        SingleHexResult moveLocation
            = GameplayContext.Ui.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            // Move to the location they selected
            Hex moveDirection = GameplayContext.Player.Position.GetDirectionTo(moveLocation.GetResult());
            Hex moveFrom = GameplayContext.Player.Position;
            Hex movePosition = moveLocation.GetResult();
            GameplayContext.Player.MoveTo(moveLocation.GetResult());
            int spotsToHit = moveFrom.DistanceTo(GameplayContext.Player.Position);
            List<Hex> spotsInPath = GridHelper.CastLineInDirection(GameplayContext.Grid, moveFrom, moveDirection, spotsToHit, false, false, true);
            spotsInPath.Remove(GameplayContext.Player.Position);
            foreach (Hex spot in spotsInPath)
            {
                if (GameplayContext.Grid.GetEntityAtHex(spot) is Entity hit)
                {
                    if (hit.HasStatusEffect(typeof(WetStatusEffect)))
                    {
                        GameplayContext.Player.DealDamageTo(hit, baseDamage * 2);
                        hit.ApplyStatusEffect(new StunStatusEffect());
                        GameplayContext.Player.TriggerAttackEvent(hit);

                    }
                    else
                    {
                        GameplayContext.Player.DealDamageTo(hit, baseDamage);
                        outcome.Complete();
                    }
                }
            }

            
        }
        else
        {
            outcome.Cancel();
        }
    }

}
