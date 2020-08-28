using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory factory;
    private Entity player;
    public InterfaceManager interfaceManager;

    public Card exampleCard;
    [SerializeField]
    public CardData exampleData;

    // Start is called before the first frame update
    void Start()
    {
        if (worldGrid == null)
        {
            worldGrid = GameObject.FindObjectOfType<HexGrid>();
        }
        worldGrid.GenerateMap(mapRadius);
        factory = EntityFactory.GetFactory;
        player = factory.CreateEntity(worldGrid, new Hex(0, 0));

        exampleCard.LoadDataFrom(exampleData);
        /*
        interfaceManager.OfferSingleHexSelection(player.GetPosition().GetAllNeighbours()
            .FindAll((_) => !worldGrid.IsHexOccupied(_)));

        interfaceManager.OnSelectionMade += (pos) =>
        {
            player.MoveTo(pos);
            interfaceManager.OfferSingleHexSelection(player.GetPosition().GetAllNeighbours()
                .FindAll((_) => !worldGrid.IsHexOccupied(_)));
        };*/

        StartCoroutine(MovementCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Hex mouseHex = worldGrid.GetHexUnderMouse();
            if (!(mouseHex is null))
            {
                if (!worldGrid.IsHexOccupied(mouseHex))
                {
                    player.MoveTo(mouseHex);
                }
            }
        }

    }

    // Very basic example of the step action
    IEnumerator MovementCoroutine()
    {
        // Get the list of possible locations to move to
        List<Hex> movementCandidates = player.GetPosition().GetAllNeighbours();

        // Show the locations to the player and let them pick one
        SelectionResult moveLocation = interfaceManager.OfferSingleHexSelection(movementCandidates);

        // Wait until the player has made a selection or cancels the action
        yield return new WaitUntil(moveLocation.IsReadyOrCancelled);

        // If the player didn't cancel the selection
        if (!moveLocation.WasCancelled())
        {
            // Move to the location they selected
            player.MoveTo(moveLocation.GetResult());
        }
        
    }
}
