using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStep : CardData
{
    public int moveDistance;

    public override IEnumerator CardBehaviour(GameplayContext gc, CardActionResult outcome)
    {
        
        // Get the list of possible locations to move to
        List<Hex> movementCandidates = gc.player.GetPosition().GetAllNeighbours();

        // Show the locations to the player and let them pick one
        SelectionResult moveLocation = gc.ui.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            // Move to the location they selected
            gc.player.MoveTo(moveLocation.GetResult());
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        } 
    }
}
