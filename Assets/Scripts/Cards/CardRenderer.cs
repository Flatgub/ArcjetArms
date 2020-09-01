using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image frameArt;
    public Image cardArt;
    public Image energyCostArt;
    public Text titleText;
    public Text bodyText;
    public CardData cardData;
    public Card tiedTo;
    public HandContainer inHand;

    public void TieTo(Card card)
    {
        tiedTo = card;
        ShowCardData(tiedTo.cardData);
    }

    public void ShowCardData(CardData data)
    {
        cardData = data;
        frameArt.sprite = cardData.cardFrame;
        cardArt.sprite = cardData.cardArt;
        titleText.text = cardData.title;
        bodyText.text = cardData.GenerateStaticDescription();
        energyCostArt.sprite = EnergyCostToSprite(cardData.energyCost);
    }

    public static Sprite EnergyCostToSprite(int ec)
    {
        ec = Math.Max(Math.Min(3, ec), 0);
        return CardDatabase.EnergyCostFrames[ec];
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(cardData.title);
    }*/

    public void OnPointerEnter(PointerEventData ped)
    {
        if (inHand != null)
        {
            inHand.OnCardMouseOver(this);
        }
    }

    public void OnPointerExit(PointerEventData ped)
    {
        if (inHand != null)
        {
            inHand.OnCardMouseLeave(this);
        }
    }

    // HANDLE THESE LATER
    /*
    public void whatever()
    {
        Debug.Log("onPointer: " + gameObject.name);
    }

    
    public void whateverOut(PointerEventData ped)
    {
        Debug.Log("onPointerExit: " + gameObject.name);
    }*/
}
