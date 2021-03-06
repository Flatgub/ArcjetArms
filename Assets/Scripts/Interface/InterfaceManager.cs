﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button cancelCardButton;
    public Transform discardPileLocation;

    public CardRenderer activeCardRenderer;

    public DamageNumber damageNumberPrefab;

    public AudioGroup cardDrawNoises;
    public AudioGroup cardTouchNoises;
    public AudioSource audioPlayer;
    enum InterfaceState
    {
        Idle,
        BusyWithSelection
    }

    private InterfaceState state;

    public ProjectileTracer tracerPrefab;

    public void Awake()
    {
        state = InterfaceState.Idle;
        cardFactory = CardRendererFactory.GetFactory;
        manager.OnCardDrawn += VisualiseNewCard;
        manager.OnCardSelected += SelectCardFromHand;
        manager.OnCardDeselected += DeselectActiveCard;
        manager.OnCardDiscarded += DiscardCard;
        cancelCardButton.gameObject.SetActive(false);
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

        if (activeCardRenderer != null)
        {
            activeCardRenderer.UpdateDescription(); //ABSOLUTELY DON'T DO THIS EVERY TURN
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

    /// <summary>
    /// Creates a new <see cref="CardRenderer"/> to match the input card and adds it to the hand.
    /// </summary>
    /// <remarks>Used to respond to <see cref="GameManager.OnCardDrawn"/></remarks>
    /// <param name="card">The card to create a <c>CardRenderer</c> for</param>
    public void VisualiseNewCard(Card card)
    {
        CardRenderer cr = card.tiedTo;
        if (card.tiedTo is null)
        {
            cr = cardFactory.CreateCardRenderer(card);
            cr.gameObject.name = "Renderer for " + card.cardData.title;
            cr.updateCostColour = true;
        }
        hand.AddCardToHand(cr);
    }

    /// <summary>
    /// Moves the <see cref="CardRenderer"/> that matches this card into the active card location
    /// </summary>
    /// <remarks>Used to respond to <see cref="GameManager.OnCardSelected"/></remarks>
    /// <param name="card">The card that was selected</param>
    public void SelectCardFromHand(Card card)
    {
        CardRenderer cr = card.tiedTo;
        hand.RemoveCardFromHand(cr);
        cr.transform.SetParent(activeCardLocation);
        LeanTween.cancel(cr.gameObject);
        LeanTween.rotateZ(cr.gameObject, 0f, 0.2f);
        LeanTween.move(cr.gameObject, activeCardLocation.position, 0.2f);
        cancelCardButton.gameObject.SetActive(true);
        hand.HoldCardsDown = true;
        activeCardRenderer = cr;
        //PlaySoundGroup(cardTouchNoises); TODO: fix me
    }

    public void HideCancelButton()
    {
        cancelCardButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Moves the <see cref="CardRenderer"/> that matches the active card back into the hand.
    /// </summary>
    /// <remarks>Used to respond to <see cref="GameManager.OnCardDeselected"/></remarks>
    public void DeselectActiveCard()
    {
        if (activeCardRenderer != null)
        {
            hand.AddCardToHand(activeCardRenderer);
            hand.HoldCardsDown = false;
            activeCardRenderer.UpdateDescription(useStatic: true);
            activeCardRenderer = null;
            cancelCardButton.gameObject.SetActive(false);
        }
        if (state == InterfaceState.BusyWithSelection)
        {
            pendingResult = null;
            activeSelection.Cleanup();
            activeSelection = null;
            state = InterfaceState.Idle;
        }
    }

    /// <summary>
    /// Moves the <see cref="CardRenderer"/> that matches this card to the discard card location,
    /// then destroys it.
    /// </summary>
    /// <remarks>Used to respond to <see cref="GameManager.OnCardDiscarded"/></remarks>
    /// <param name="card">The card to discard</param>
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
            card.tiedTo = null;
        }
    }

    public void SpawnDamageNumber(Entity ent, int amount)
    {
        ///convert hex position to canvas position
        Vector3 worldpoint = grid.GetWorldPosition(ent.Position);
        RectTransform canvas = uiCanvas.GetComponent<RectTransform>();
        Vector2 viewportpos = Camera.main.WorldToViewportPoint(worldpoint); //TODO: cache this
        Vector2 position = viewportpos.IntoRect(canvas);

        DamageNumber num = Instantiate(damageNumberPrefab, canvas);
        num.Show(position, amount, Color.red);
    }

    private void PlaySoundGroup(AudioGroup group)
    {
        audioPlayer.clip = group.RandomClip();
        audioPlayer.Play();
    }

    
}
