using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class HandContainer : MonoBehaviour
{
    private List<CardRenderer> cardsInHand;
    private RectTransform rect;
    private float width;
    private int highlightedCard;
    private int lastHighlightedCard;

    public float highlightedY;
    public float hiddenY;
    public float verticalStray;
    public float rotationalStray;

    private float cardMoveTime = 0.2f;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        width = rect.rect.width;
        cardsInHand = new List<CardRenderer>();
        highlightedCard = -1;
        lastHighlightedCard = 0;
    
    }


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
            if (highlightedCard != -1)
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

    public void AddCardToHand(CardRenderer card)
    {
        if (!cardsInHand.Contains(card))
        {
            cardsInHand.Add(card);
            card.inHand = this;
            card.transform.SetParent(transform);
            card.transform.localScale = Vector3.one*1.2f;
            UpdatePositions();
        }
    }

    public void RemoveCardFromHand(CardRenderer card)
    {
        if (cardsInHand.Contains(card))
        {
            int index = cardsInHand.IndexOf(card);
            cardsInHand.RemoveAt(index);
            card.transform.SetParent(null);
            card.inHand = null;
            if (highlightedCard == index)
            {
                highlightedCard = -1;
                lastHighlightedCard = cardsInHand.Count / 2;
            }
            UpdatePositions();
        }
    }

    //TODO: make this order correct
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

    public void OnCardMouseOver(CardRenderer cr)
    {
        SetHighlightedCard(cr);
    }

    public void OnCardMouseLeave(CardRenderer cr)
    {
        if (highlightedCard == cardsInHand.IndexOf(cr))
        {
            highlightedCard = -1;
            UpdatePositions();
        }
    }

}
