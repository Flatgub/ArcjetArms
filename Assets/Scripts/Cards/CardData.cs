using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public string title;
    public Sprite cardArt;
    public Sprite cardFrame;
    [TextArea]
    public string description;
    public int energyCost;
    public CardData upgradesTo = null;

    public abstract IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome);
}
