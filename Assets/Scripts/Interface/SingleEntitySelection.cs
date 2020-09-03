using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEntitySelection : MonoBehaviour, ISelectionPrompt
{
    public SingleEntityResult result;
    public InterfaceManager manager;

    public SelectionResponder selectionPrefab;
    public Color selectionIdleColour;
    public Color selectionMouseOverColour;

    private HashSet<SelectionResponder> activeSelectables;
    private Dictionary<Hex, Entity> whoIsWhere;

    public void Initialize(InterfaceManager manager, ICollection<Entity> candidates,
        SingleEntityResult result)
    {
        this.manager = manager;
        this.result = result;
        foreach (Entity ent in candidates)
        {
            Hex pos = ent.Position;
            whoIsWhere.Add(pos, ent);
            GenerateSelectionHex(pos);
        }
    }

    public void Awake()
    {
        activeSelectables = new HashSet<SelectionResponder>();
        whoIsWhere = new Dictionary<Hex, Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectionVisuals();

        if (Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    /// <summary>
    /// The event callback method that's triggered when a <see cref="SelectionResponder"/> is 
    /// clicked
    /// </summary>
    /// <param name="hex">The hex represented by the clicked SelectionResponder</param>
    public void OnCandidateSelected(Hex hex)
    {
        result.AddSelection(whoIsWhere[hex]);
        ClearSelectables();
        //and then we die
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

        activeSelectables.Add(responder);
    }

    /// <summary>
    /// Destroy all active selectables and clear the activeSelectables list
    /// </summary>
    public void ClearSelectables()
    {
        foreach (SelectionResponder hex in activeSelectables)
        {
            Destroy(hex.gameObject);
        };
        activeSelectables.Clear();
    }

    public void UpdateSelectionVisuals()
    {
        Hex mousehex = manager.grid.GetHexUnderMouse();
        if (!(mousehex is null))
        {
            foreach (SelectionResponder hex in activeSelectables)
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
        ClearSelectables();
        //and then we die
    }

    public void Cleanup()
    {
        Destroy(gameObject);
    }
}
