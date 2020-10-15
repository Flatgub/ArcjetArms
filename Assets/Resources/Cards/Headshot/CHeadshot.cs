using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CHeadshot: CardData
{
    //put any numbers relevant to the card here
    public int range;
    public int baseDamage;
    //generate the "basic" description, without doing any calculations.
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, range, baseDamage); // <- put stat variables in here
    }

    public override string GenerateCurrentDescription()
    {
        // replace this if any of the cards numbers are calculated, such as damage.
        return GenerateStaticDescription(); 
    }

    //what should the card do when its played, remember to do outcome.Complete or outcome.Cancel
    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        Hex pos = GameplayContext.Player.Position;

        //TODO: replace this with a line of sight check maybe?
        List<Entity> targets =
            GridHelper.GetEntitiesInRange(GameplayContext.Grid, pos, range);
        targets.Remove(GameplayContext.Player);

        SingleEntityResult target = GameplayContext.Ui.OfferSingleEntitySelection(targets);

    yield return new WaitUntil(target.IsReadyOrCancelled);

        if (!target.WasCancelled())
        {
            Entity victim = target.GetResult();
            if (victim.Health.HealthAsFraction() <= 0.5f) ;
            baseDamage = baseDamage * 2f;
    GameplayContext.Player.DealDamageTo(victim, baseDamage);
            GameplayContext.Player.TriggerAttackEvent(victim);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}

