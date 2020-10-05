using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CFireArt2: CardData
{
    //put any numbers relevant to the card here
    public int 6
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
        throw new System.NotImplementedException();
    }

}
