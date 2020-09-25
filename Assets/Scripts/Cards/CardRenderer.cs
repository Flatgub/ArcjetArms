using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// CardRenderer is a component which manages the UI element that represents a card. It hands both
/// updating subcomponents in its prefab for things like cart and title text, as well as handling
/// user input events.
/// </summary>
public class CardRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    public Image frameArt;
    public Image cardArt;
    public Image energyCostArt;
    public Text titleText;
    public Text bodyText;
    public CardData cardData;
    public Card tiedTo;

    public event Action<CardRenderer> OnClick;
    public event Action<CardRenderer> OnMouseEnter;
    public event Action<CardRenderer> OnMouseExit;

    /// <summary>
    /// Link this CardRenderer with a Card object, matching the visuals to that cards stats and
    /// allowing the the two to reference one another.
    /// </summary>
    /// <param name="card"></param>
    public void TieTo(Card card)
    {
        tiedTo = card;
        card.tiedTo = this;
        ShowCardData(tiedTo.cardData);
    }

    /// <summary>
    /// Update the visuals of this card by copying information from a CardData statblock.
    /// </summary>
    /// <param name="data">The CardData to copy data from</param>
    public void ShowCardData(CardData data)
    {
        cardData = data;
        frameArt.sprite = cardData.cardFrame;
        cardArt.sprite = cardData.cardArt;
        titleText.text = cardData.title;
        bodyText.text = cardData.GenerateStaticDescription();
        energyCostArt.sprite = EnergyCostToSprite(cardData.energyCost);
    }

    /// <summary>
    /// Get the sprite which matches a given energy cost
    /// </summary>
    /// <param name="ec">the energy cost</param>
    /// <returns>A sprite</returns>
    public static Sprite EnergyCostToSprite(int ec)
    {
        ec = Math.Max(Math.Min(3, ec), 0);
        return CardDatabase.EnergyCostFrames[ec];
    }

    public void OnDestroy()
    {
        if (tiedTo?.tiedTo == this)
        {
            tiedTo.tiedTo = null;
        }
    }

    /// <summary>
    /// The event triggered when a card is clicked on
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == 0)
        {
            OnClick?.Invoke(this);
        }
    }

    /// <summary>
    /// The event triggered when a card is moused over
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData ped)
    {
        OnMouseEnter?.Invoke(this);
    }

    /// <summary>
    /// The event triggered when the mouse leaves the card
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData ped)
    {
        OnMouseExit?.Invoke(this);
    }
}
