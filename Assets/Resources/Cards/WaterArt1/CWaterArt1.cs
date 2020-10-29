using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWaterArt1 : CardData
{
    public int baseDamage;
    public int turnsRemaining;



    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage, turnsRemaining);
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
        return string.Format(descriptionTemplate, dmgstring);
    }

    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {

       
           

            //get all 6 spots around the player
            Hex diffVector = GameplayContext.Player.Position - victim.Position;
            List<Hex> spotsToWet =
                GridHelper.GetHexesInRange(GameplayContext.Grid, GameplayContext.Player.Position, 1, false);

            spotsToWet.Remove(GameplayContext.Player.Position); //don't hit the player
        List<Hex> allpositions = GridHelper.GetHexesInRange(GameplayContext.Grid, GameplayContext.Player.Position, 999);
            foreach (Hex h in spotsToWet)
            {
                if (GameplayContext.Grid.GetEntityAtHex(h) is Entity toHit)
                {
                    GameplayContext.Player.DealDamageTo(toHit, baseDamage);
                    toHit.MoveTo(allpositions.PopRandom());
                    toHit.ApplyStatusEffect(new WetStatusEffect(baseDamage, turnsRemaining));
                    GameplayContext.Player.TriggerAttackEvent(toHit);
                }
            }

            outcome.Complete();

    }

}