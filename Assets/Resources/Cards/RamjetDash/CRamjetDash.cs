using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CRamjetDash : CardData
{

    public int moveDistance;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, moveDistance);
    }

    public override string GenerateCurrentDescription()
    {
        return GenerateStaticDescription();
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
            GameplayContext.Player.MoveTo(moveLocation.GetResult());
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
