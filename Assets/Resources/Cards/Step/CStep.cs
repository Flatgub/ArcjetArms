using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStep : CardData
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

    public override IEnumerator CardBehaviour(GameplayContext gc, CardActionResult outcome)
    {

        // Get the list of possible locations to move to
        List<Hex> movementCandidates = 
            GridHelper.GetHexesInRange(gc.Grid, gc.Player.Position, moveDistance);
        movementCandidates.Remove(gc.Player.Position);

        // Show the locations to the player and let them pick one
        SingleHexResult moveLocation = gc.Ui.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            // Move to the location they selected
            gc.Player.MoveTo(moveLocation.GetResult());
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        } 
    }

  
}
