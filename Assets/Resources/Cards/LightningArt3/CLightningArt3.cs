using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLightningArt3 : CardData
{
    public int baseDamage;



    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage);
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
        foreach (Entity victim in GameplayContext.Manager.allEnemies)
        {

            //find who's standing there
            if (victim.HasStatusEffect(typeof(WetStatusEffect)))
            {
                //hurt them
                GameplayContext.Player.DealDamageTo(victim, baseDamage * 2);
                victim.ApplyStatusEffect(new StunStatusEffect());
                GameplayContext.Player.ApplyStatusEffect(new StunStatusEffect());
                GameplayContext.Player.TriggerAttackEvent(victim);
            }

            else
            {
                GameplayContext.Player.DealDamageTo(victim, baseDamage);
                GameplayContext.Player.ApplyStatusEffect(new StunStatusEffect());

            }
        }
        outcome.Complete();
        yield break;

    }
}
    

    




            
       


