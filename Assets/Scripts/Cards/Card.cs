using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image FrameArt;
    public Image CardArt;
    public Image EnergyCostArt;
    public Text TitleText;
    public Text BodyText;
    //public Button selector;
    private Sprite[] EnergyCostFrames;

    public CardData cardData;

    public void Awake()
    {
        EnergyCostFrames = Resources.LoadAll<Sprite>("Cards/CardPriceSprites");
        //selector = GetComponent<Button>();
        //selector.onClick.AddListener(whatever);
        //selector.on
        //selector.onPointerExit.AddListener(whateverOut);
    }

    public void LoadDataFrom(CardData data)
    {
        cardData = data;
        FrameArt.sprite = cardData.cardFrame;
        CardArt.sprite = cardData.cardArt;
        TitleText.text = cardData.title;
        BodyText.text = cardData.GenerateStaticDescription();
        EnergyCostArt.sprite = EnergyCostToSprite(cardData.energyCost);
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

    public Sprite EnergyCostToSprite(int ec)
    {
        ec = Math.Max(Math.Min(3, ec), 0);
        return EnergyCostFrames[ec];
    }

    // HANDLE THESE LATER
    /*
    public void whatever()
    {
        Debug.Log("onPointer: " + gameObject.name);
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        Debug.Log("onPointerEnter: " + gameObject.name);
    }
    public void whateverOut(PointerEventData ped)
    {
        Debug.Log("onPointerExit: " + gameObject.name);
    }*/
}
