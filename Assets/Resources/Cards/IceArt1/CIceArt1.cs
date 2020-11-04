using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIceArt1 : CardData
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
        List<Hex> movementCandidates = new List<Hex>();



        //cast out a line in each direction from the player to determine movement spots
        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line = GridHelper.CastLineInDirection(GameplayContext.Grid,
                GameplayContext.Player.Position, dir, range, includeStart: false);

            movementCandidates.AddRange(line);
            movementCandidates.Remove(GameplayContext.Player.Position);
        }

       

        // Show the locations to the player and let them pick one
        SingleHexResult moveLocation
            = GameplayContext.Ui.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            Hex targetpoint = moveLocation.GetResult();

            //get all 6 hexes around that point
            List<Hex> pointsAroundTarget =
                GridHelper.GetHexesInRange(GameplayContext.Grid, targetpoint, 1, false);

            //add the point we clicked to that list, so we now have all 7 hexes
            pointsAroundTarget.Add(targetpoint);
            pointsAroundTarget.Remove(GameplayContext.Player.Position);

            //for each hex
            foreach (Hex spot in pointsAroundTarget)
            {
                //find who's standing there
                Entity victim = GameplayContext.Grid.GetEntityAtHex(spot);
                if (victim != null)
                {
                    //hurt them
                    GameplayContext.Player.DealDamageTo(victim, 2);
                    victim.ApplyStatusEffect(new StunStatusEffect());
                    GameplayContext.Player.TriggerAttackEvent(victim); 
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
