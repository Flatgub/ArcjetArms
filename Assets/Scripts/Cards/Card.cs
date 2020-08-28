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
}
