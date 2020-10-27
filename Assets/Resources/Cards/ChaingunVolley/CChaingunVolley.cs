using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CChaingunVolley : CardData
{
    //put any numbers relevant to the card here
    public int baseDamage;
    public int range;

    //generate the "basic" description, without doing any calculations.
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage, range); // <- put stat variables in here
    }

    public override string GenerateCurrentDescription()
    {
        // replace this if any of the cards numbers are calculated, such as damage.
        return GenerateStaticDescription();
    }

    //what should the card do when its played, remember to do outcome.Complete or outcome.Cancel
    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        //get a list of ranged entities
        List<Entity> rangedEnts = GridHelper.GetEntitiesInRange(GameplayContext.Grid, GameplayContext.Player.Position, 2);

        //let the player select one of the enties
        SingleEntityResult target = GameplayContext.Ui.OfferSingleEntitySelection(rangedEnts);

        //wait for the player to make a selection
        yield return new WaitUntil(target.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!target.WasCancelled())
        {
            //hit 'em
            Entity victim = target.GetResult();
            for (int i = 1; i <= 10; i++)
            { // 10 attacks
                if (Random.value <= 0.5f) // 50% chance to hit
                    GameplayContext.Player.DealDamageTo(victim, baseDamage);
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
