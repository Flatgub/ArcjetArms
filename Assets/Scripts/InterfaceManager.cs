using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public HexGrid grid;
    public GameManager manager;

    public GameObject selectionPrefab;
    public Color selectionIdleColour;
    public Color selectionMouseOverColour;

    private HashSet<SelectionResponder> activeSelectionHexes;

    //public event Action<Hex> OnSelectionMade;
    private SelectionResult activeSelection;

    public HandContainer hand;
    public Transform activeCardLocation;

    public CardRenderer activeCardRenderer;

    public void Awake()
    {
        activeSelectionHexes = new HashSet<SelectionResponder>();
    }

    public void Update()
    {
        Hex mousehex = grid.GetHexUnderMouse();
        if (!(mousehex is null))
        {
            foreach (SelectionResponder hex in activeSelectionHexes)
            {
                if (hex.position == mousehex)
                {
                    hex.appearance.color = selectionMouseOverColour;
                }
                else
                {
                    hex.appearance.color = selectionIdleColour;
                }
            }
        }

        if (activeSelection != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ClearSelectionHexes();
                activeSelection.Cancel();
            }
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

    public void DeselectActiveCard()
    {
        if (activeCardRenderer != null)
        {
            hand.AddCardToHand(activeCardRenderer);
            hand.HoldCardsDown = false;
            activeCardRenderer = null;
        }
    }

    public SelectionResult OfferSingleHexSelection(ICollection<Hex> options)
    {
        foreach (Hex hex in options)
        {
            if (grid.Contains(hex))
            {
                GenerateSelectionHex(hex);
            }
        }

        activeSelection = new SelectionResult();
        return activeSelection;
    }

    /// <summary>
    /// Instantiate a <see cref="SelectionResponder"/> at the given hex position
    /// </summary>
    /// <param name="pos"></param>
    private void GenerateSelectionHex(Hex pos)
    {
        GameObject hex = Instantiate(selectionPrefab);

        hex.transform.parent = transform;
        hex.transform.position = grid.GetWorldPosition(pos);

        SelectionResponder responder = hex.GetComponent<SelectionResponder>();
        responder.Initialize(this, pos);
        responder.appearance.color = selectionIdleColour;

        activeSelectionHexes.Add(responder);
    }

    /// <summary>
    /// Destroy all active selection hexes and clear the ActiveSelectionHexes list
    /// </summary>
    private void ClearSelectionHexes()
    {
        foreach (SelectionResponder hex in activeSelectionHexes)
        {
            Destroy(hex.gameObject);
        };
        activeSelectionHexes.Clear();
    }

    /// <summary>
    /// The event callback method that's triggered when a <see cref="SelectionResponder"/> is 
    /// clicked
    /// </summary>
    /// <param name="hex">The hex represented by the clicked SelectionResponder</param>
    public void HexSelected(Hex hex)
    {
        ClearSelectionHexes();

        activeSelection.AddSelection(hex);
        activeSelection = null;

        //OnSelectionMade?.Invoke(hex);
    }
}
