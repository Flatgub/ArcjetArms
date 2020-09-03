using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public HexGrid grid;
    public GameManager manager;
    private CardRendererFactory cardFactory;

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
        cardFactory = CardRendererFactory.GetFactory;
        manager.OnCardDrawn += VisualiseNewCard;
        manager.OnCardSelected += SelectCardFromHand;
        manager.OnCardDeselected += DeselectActiveCard;
        manager.OnCardDiscarded += DiscardCard;
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

    // GameManager event responders

    //responds to OnCardDrawn
    public void VisualiseNewCard(Card card)
    {
        CardRenderer cr = cardFactory.CreateCardRenderer(card);
        hand.AddCardToHand(cr);
    }

    //responds to OnCardSelected
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

    //responds to OnCardDeselected
    public void DeselectActiveCard()
    {
        if (activeCardRenderer != null)
        {
            hand.AddCardToHand(activeCardRenderer);
            hand.HoldCardsDown = false;
            activeCardRenderer = null;
        }
    }

    //responds to OnCardDiscarded
    public void DiscardCard(Card card)
    {
        if (card.tiedTo is CardRenderer cr)
        {
            if (activeCardRenderer == cr)
            {
                activeCardRenderer = null;
                hand.HoldCardsDown = false;
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
    }
}
