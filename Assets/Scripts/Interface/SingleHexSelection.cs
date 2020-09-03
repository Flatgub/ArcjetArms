using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHexSelection: MonoBehaviour, ISelectionPrompt
{
    public SingleHexResult result;
    public InterfaceManager manager;

    public SelectionResponder selectionPrefab;
    public Color selectionIdleColour;
    public Color selectionMouseOverColour;

    private HashSet<SelectionResponder> activeSelectionHexes;

    public void Initialize(InterfaceManager manager, ICollection<Hex> candidates,
        SingleHexResult result)
    {
        this.manager = manager;
        this.result = result;
        foreach (Hex hex in candidates)
        {
            if (manager.grid.Contains(hex))
            {
                // add a selection hex to the interfacemanager
                GenerateSelectionHex(hex);
            }
        }
    }

    public void Awake()
    {
        activeSelectionHexes = new HashSet<SelectionResponder>();
    }

    public void Update()
    {
        UpdateSelectionVisuals();

        if (Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    /// <summary>
    /// Instantiate a <see cref="SelectionResponder"/> at the given hex position
    /// </summary>
    /// <param name="pos"></param>
    public void GenerateSelectionHex(Hex pos)
    {
        SelectionResponder responder = Instantiate(selectionPrefab);

        responder.transform.parent = transform;
        responder.transform.position = manager.grid.GetWorldPosition(pos);

        responder.Initialize(pos, OnCandidateSelected);
        responder.appearance.color = selectionIdleColour;

        activeSelectionHexes.Add(responder);
    }

    /// <summary>
    /// Destroy all active selection hexes and clear the ActiveSelectionHexes list
    /// </summary>
    public void ClearSelectionHexes()
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
    public void OnCandidateSelected(Hex hex)
    {
        result.AddSelection(hex);
        ClearSelectionHexes();
        //and then we die
    }

    public void UpdateSelectionVisuals()
    {
        Hex mousehex = manager.grid.GetHexUnderMouse();
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

    public void Cancel()
    {
        result.Cancel();
        ClearSelectionHexes();
        //and then we die
    }

    public void Cleanup()
    {
        Destroy(gameObject);
    }
}
