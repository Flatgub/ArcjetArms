using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card
{
    public CardData cardData;
    public CardRenderer tiedTo;

    public Card(CardData data)
    {
        cardData = data;
    }

    public CardActionResult AttemptToPlay(GameplayContext gc)
    {
        if (cardData == null)
        {
            throw new InvalidOperationException("Attempted to play a card with no data");
        }

        CardActionResult returnResult = new CardActionResult();

        gc.StartCoroutineOnManager(cardData.CardBehaviour(gc, returnResult));

        return returnResult;
    }

}
