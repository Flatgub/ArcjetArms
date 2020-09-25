using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// HandContainer is a UI component used for visually arranging the player's hand of cards and
/// facilitating player interaction with those cards for selection purposes.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HandContainer : MonoBehaviour
{
    [HideInInspector]
    public List<CardRenderer> cardsInHand;
    private RectTransform rect;
    private float width;
    private int highlightedCard;
    private int lastHighlightedCard;

    public float highlightedY;
    public float hiddenY;
    public float verticalStray;
    public float rotationalStray;

    private bool holdCardsDown;
    public bool HoldCardsDown {
        get
        {
            return holdCardsDown;
        }
        set
        {
            bool old = holdCardsDown;
            holdCardsDown = value;
            if (holdCardsDown != old)
            {
                UpdatePositions();
            }
        } 
    }
    
    public InterfaceManager manager;

    private float cardMoveTime = 0.2f;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        width = rect.rect.width;
        cardsInHand = new List<CardRenderer>();
        highlightedCard = -1;
        lastHighlightedCard = 0;
        holdCardsDown = false;
    }

    /// <summary>
    /// Update the transforms of all the cards in the hand, as well as the render order
    /// </summary>
    public void UpdatePositions()
    {
        int numberOfCards = cardsInHand.Count;
        float distBetween = width / (numberOfCards + 1);
        float totalDistance = distBetween * numberOfCards;
        float currentX = (-rect.rect.width/2.0f) + distBetween;

        for (int i = 0; i < numberOfCards; i++)
        {
            CardRenderer card = cardsInHand[i];
            LeanTween.cancel(card.gameObject);

            float desiredY = 0;
            if (holdCardsDown)
            {
                desiredY = hiddenY;
            }
            else if (highlightedCard != -1)
            {
                desiredY = (highlightedCard == i) ? highlightedY : hiddenY;
            }

            int dist = Math.Abs(lastHighlightedCard - i);

            LeanTween.moveLocal(card.gameObject,
                new Vector2(currentX, desiredY - verticalStray*dist), cardMoveTime);

            LeanTween.rotateZ(card.gameObject, rotationalStray * (lastHighlightedCard - i)
                , cardMoveTime);

            currentX += distBetween;
        }

        UpdateCardOrder();
    }

    /// <summary>
    /// Add a card to hand
    /// </summary>
    /// <param name="card">The CardRenderer tied to the card to add</param>
    public void AddCardToHand(CardRenderer card)
    {
        if (!cardsInHand.Contains(card))
        {
            cardsInHand.Add(card);
            card.OnClick += OnCardMouseClick;
            card.OnMouseEnter += OnCardMouseOver;
            card.OnMouseExit += OnCardMouseLeave;
            card.transform.SetParent(transform);
            card.transform.localScale = Vector3.one*1.2f; //TODO: found out why this is happening
            UpdatePositions();
        }
    }

    /// <summary>
    /// Remove a card from the hand
    /// </summary>
    /// <param name="card">The CardRenderer tied to the card to remove</param>
    public void RemoveCardFromHand(CardRenderer card)
    {
        if (cardsInHand.Contains(card))
        {
            int index = cardsInHand.IndexOf(card);
            cardsInHand.RemoveAt(index);
            card.transform.SetParent(null);

            card.OnClick -= OnCardMouseClick;
            card.OnMouseEnter -= OnCardMouseOver;
            card.OnMouseExit -= OnCardMouseLeave;

            if (highlightedCard == index)
            {
                highlightedCard = -1;
                lastHighlightedCard = cardsInHand.Count / 2;
            }
            UpdatePositions();
        }
    }

    //TODO: make this order correct
    /// <summary>
    /// Sort the render order of the cards in the hand from back to front, so the highlighted card
    /// is at the front
    /// </summary>
    private void UpdateCardOrder()
    {
        int cardToUse = (highlightedCard == -1) ? lastHighlightedCard : highlightedCard;
        int numberOfCards = cardsInHand.Count;
        for (int i = 0; i < numberOfCards; i++)
        {
            CardRenderer card = cardsInHand[i];
            int dist = Math.Abs(cardToUse - i);
            card.transform.SetSiblingIndex(numberOfCards - 1 - dist);
        }

    }

    private void SetHighlightedCard(CardRenderer cr)
    {
        highlightedCard = cardsInHand.IndexOf(cr);
        lastHighlightedCard = highlightedCard;
        UpdatePositions();
    }

    public bool Contains(CardRenderer cr)
    {
        return cardsInHand.Contains(cr);
    }

    /// <summary>
    /// The event cards trigger when they are moused over
    /// </summary>
    /// <param name="cr"></param>
    public void OnCardMouseOver(CardRenderer cr)
    {
        SetHighlightedCard(cr);
    }

    /// <summary>
    /// The event cards trigger when the mouse leaves a card
    /// </summary>
    /// <param name="cr"></param>
    public void OnCardMouseLeave(CardRenderer cr)
    {
        if (highlightedCard == cardsInHand.IndexOf(cr))
        {
            highlightedCard = -1;
            UpdatePositions();
        }
    }


    /// <summary>
    /// The event cards trigger when they're clicked on
    /// </summary>
    /// <param name="cr"></param>
    public void OnCardMouseClick(CardRenderer cr)
    {
        if (!holdCardsDown)
        {
            manager.OnPlayerSelectCard(cr);
        }
    }

}
