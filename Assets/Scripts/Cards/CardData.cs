using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardData : ScriptableObject
{
    public string title;
    public Sprite cardArt;
    public Sprite cardFrame;
    
    [TextArea]
    public string descriptionTemplate;

    public int energyCost;
    public CardData upgradesTo = null;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract string GenerateStaticDescription();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract string GenerateCurrentDescription(GameplayContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="outcome"></param>
    /// <returns></returns>
    public abstract IEnumerator CardBehaviour(GameplayContext context, CardActionResult outcome);


}
