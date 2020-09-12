﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rename CardDataTemplate to the name of the card, like CStep or CSalvage
public class CardDataTemplate : CardData
{
    //put any numbers relevant to the card here

    //generate the "basic" description, without doing any calculations.
    public override string GenerateStaticDescription()
    {
        return string.Format(descriptionTemplate); // <- put stat numbers in here
    }

    public override string GenerateCurrentDescription(GameplayContext context)
    {
        // replace this if any of the cards numbers are calculated, such as damage.
        return GenerateStaticDescription(); 
    }

    //what should the card do when its played, remember to do outcome.Complete or outcome.Cancel
    public override IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome)
    {
        throw new System.NotImplementedException();
    }

}
