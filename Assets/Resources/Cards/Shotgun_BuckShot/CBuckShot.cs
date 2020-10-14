using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CBuckShot: CardData
{
    //put any numbers relevant to the card here
    public int range;
    public int maxDamage;
    public int minDamage;
    //generate the "basic" description, without doing any calculations.
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate); // <- put stat variables in here
    }

    public override string GenerateCurrentDescription()
    {
        // replace this if any of the cards numbers are calculated, such as damage.
        return GenerateStaticDescription(); 
    }

    //what should the card do when its played, remember to do outcome.Complete or outcome.Cancel
    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        //throw new System.NotImplementedException();
        Hex pos = GameplayContext.Player.Position;

        //TODO: replace this with a line of sight check maybe?
        List<Entity> targets =
            GridHelper.GetEntitiesInRange(GameplayContext.Grid, pos, range);

        //don't let the player shoot themselves
        targets.Remove(GameplayContext.Player);

        SingleEntityResult target = GameplayContext.Ui.OfferSingleEntitySelection(targets);

        yield return new WaitUntil(target.IsReadyOrCancelled);

        if (!target.WasCancelled())
        {
            Entity victim = target.GetResult();

            int damage = 0;

            if(victim.Position.DistanceTo(pos) >= 2)
            {
                 damage = minDamage;
            }
            else
            {
                 damage = maxDamage;
            }
            
            GameplayContext.Player.DealDamageTo(victim, damage);
            GameplayContext.Player.TriggerAttackEvent(victim);
            outcome.Complete();
        }
        else
        {
            outcome.Cancel();
        }
    }

}
