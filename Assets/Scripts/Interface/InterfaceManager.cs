﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public HexGrid grid;
    public GameManager manager;

    public SingleHexSelection singleHexPromptPrefab;
    public SingleEntitySelection singleEntityPromptPrefab;

    private ISelectionPrompt activeSelection;
    private IDelayable pendingResult;

    public HandContainer hand;
    public Transform activeCardLocation;
    public Transform discardPileLocation;

    public CardRenderer activeCardRenderer;

    enum InterfaceState
    {
        Idle,
        BusyWithSelection
    }

    private InterfaceState state;

    public void Awake()
    {
        state = InterfaceState.Idle;
    }

    public void Update()
    {
        switch (state)
        {
            case InterfaceState.BusyWithSelection:
            {

                if (pendingResult.IsReadyOrCancelled())
                {
                    pendingResult = null;
                    activeSelection.Cleanup();
                    activeSelection = null;
                    state = InterfaceState.Idle;
                }

            };break;
        }
    }

    public void OnPlayerSelectCard(CardRenderer cr)
    {
        manager.AttemptPlayingCard(cr.tiedTo);
    }

    public void SelectCardFromHand(Card card)
    {
        CardRenderer cr = card.tiedTo;
        hand.RemoveCardFromHand(cr);
        cr.transform.SetParent(activeCardLocation);
        LeanTween.cancel(cr.gameObject);
        LeanTween.rotateZ(cr.gameObject, 0f, 0.2f);
        LeanTween.move(cr.gameObject, activeCardLocation.position, 0.2f);
        hand.HoldCardsDown = true;
        activeCardRenderer = cr;
    }

    public void DiscardCard(CardRenderer cr)
    {
        if (activeCardRenderer == cr)
        {
            activeCardRenderer = null;
        }
        if (hand.Contains(cr))
        {
            hand.RemoveCardFromHand(cr);
        }
        cr.transform.SetParent(discardPileLocation);
        LeanTween.cancel(cr.gameObject);
        LeanTween.rotateZ(cr.gameObject, 0f, 0.2f);
        LeanTween.move(cr.gameObject, discardPileLocation.position, 0.2f).destroyOnComplete = true;
    }

    public void DiscardHand()
    {
        for (int i = hand.cardsInHand.Count - 1; i >= 0; i--)
        {
            CardRenderer cr = hand.cardsInHand[i];
            DiscardCard(cr);
        }
    }

    public void DeselectActiveCard()
    {
        if (activeCardRenderer != null)
        {
            hand.AddCardToHand(activeCardRenderer);
            hand.HoldCardsDown = false;
            activeCardRenderer = null;
        }
    }

    public SingleHexResult OfferSingleHexSelection(ICollection<Hex> options)
    {

        SingleHexResult result = new SingleHexResult();

        SingleHexSelection shs = Instantiate(singleHexPromptPrefab);
        shs.Initialize(this, options, result);
        shs.transform.parent = transform;

        activeSelection = shs;
        pendingResult = result;

        state = InterfaceState.BusyWithSelection;

        return result;
    }

    public SingleEntityResult OfferSingleEntitySelection(ICollection<Entity> options)
    {
        SingleEntityResult result = new SingleEntityResult();

        SingleEntitySelection ses = Instantiate(singleEntityPromptPrefab);
        ses.Initialize(this, options, result);
        ses.transform.parent = transform;

        activeSelection = ses;
        pendingResult = result;

        state = InterfaceState.BusyWithSelection;

        return result;
    }
}
