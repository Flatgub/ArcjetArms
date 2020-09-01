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

    public override string GenerateCurrentDescription(GameplayContext context)
    {
        return GenerateStaticDescription();
    }

    public override IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome)
    {
        List<Hex> movementCandidates = new List<Hex>();

        //cast out a line in each direction from the player to determine movement spots
        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line = GridHelper.CastLineInDirection(context.Grid,
                context.Player.GetPosition(), dir, moveDistance, includeStart: false);

            movementCandidates.AddRange(line);
        }

        // Show the locations to the player and let them pick one
        SelectionResult moveLocation = context.Ui.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            // Move to the location they selected
            context.Player.MoveTo(moveLocation.GetResult());
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
