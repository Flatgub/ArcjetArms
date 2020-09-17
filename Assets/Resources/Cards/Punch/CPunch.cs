using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPunch : CardData
{
    public int baseDamage;

    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage);
    }

    public override string GenerateCurrentDescription(GameplayContext context)
    {
        return GenerateStaticDescription();
    }

    public override IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome)
    {
        //get a list of adjacent entities
        List<Entity> adjacentEnts = GridHelper.GetAdjacentEntities(context.Grid,
            context.Player.Position);

        //let the player select one of the adjacent enties
        SingleEntityResult target = 
            context.Ui.OfferSingleEntitySelection(adjacentEnts);

        //wait for the player to make a selection
        yield return new WaitUntil(target.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!target.WasCancelled())
        {
            //hit 'em
            Entity victim = target.GetResult();
            context.Player.DealDamageTo(victim, baseDamage);
            context.Player.TriggerAttackEvent(victim, context);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
