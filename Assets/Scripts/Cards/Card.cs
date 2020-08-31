using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image FrameArt;
    public Image CardArt;
    public Image EnergyCostArt;
    public Text TitleText;
    public Text BodyText;

    public CardData cardData;

    public void LoadDataFrom(CardData data)
    {
        cardData = data;
        FrameArt.sprite = cardData.cardFrame;
        CardArt.sprite = cardData.cardArt;
        TitleText.text = cardData.title;       
        BodyText.text = cardData.description;
    }

    public CardActionResult AttemptToPlay(GameplayContext gc)
    {
        if (cardData == null)
        {
            throw new InvalidOperationException("Attempted to play a card with no data");
        }

        CardActionResult returnResult = new CardActionResult();

        StartCoroutine(cardData.CardBehaviour(gc, returnResult));

        return returnResult;
    }
}
