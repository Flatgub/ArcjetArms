using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public HexGrid grid;

    public GameObject selectionPrefab;
    public Color selectionIdleColour;
    public Color selectionMouseOverColour;

    private HashSet<SelectionResponder> activeSelectionHexes;

    public event Action<Hex> OnSelectionMade;
    private SelectionResult activeSelection;

    public HandContainer hand;

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
    }

    public SelectionResult OfferSingleHexSelection(ICollection<Hex> options)
    {
        foreach (Hex hex in options)
        {
            GenerateSelectionHex(hex);
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

        OnSelectionMade?.Invoke(hex);
    }
}
