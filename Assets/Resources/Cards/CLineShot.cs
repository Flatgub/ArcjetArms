using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CLineShot : CardData
{
    //put any numbers relevant to the card here
    public int baseDamage;
    public int damageDistance;
    public int AfterDamage;
    //generate the "basic" description, without doing any calculations.
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate, baseDamage, damageDistance, AfterDamage); // <- put stat variables in here

        {
            if (GameplayContext.Player == null)
                return GenerateStaticDescription();
        }//todo add after damage of line shot
    }
    //what should the card do when its played, remember to do outcome.Complete or outcome.Cancel
    public override IEnumerator CardBehaviour(CardActionResult outcome)
    {
        List<Hex> targetCandidates = new List<Hex>();
        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line = GridHelper.CastLineInDirection(GameplayContext.Grid,
                GameplayContext.Player.Position, dir, damageDistance, includeStart: false);
            List<Entity> targets =
                GridHelper.GetEntitiesInRange(GameplayContext.Grid, dir, range);
            targetCandidates.AddRange(line);
        }
        targets.Remove(GameplayContext.Player);


    }
}