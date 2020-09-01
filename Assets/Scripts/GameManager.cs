using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory entFactory;
    private CardRendererFactory cardFactory;
    private Entity player;
    public InterfaceManager interfaceManager;
    public GameplayContext currentContext;

    // Start is called before the first frame update
    void Start()
    {
        if (worldGrid == null)
        {
            worldGrid = GameObject.FindObjectOfType<HexGrid>();
        }
        worldGrid.GenerateMap(mapRadius);
        entFactory = EntityFactory.GetFactory;
        cardFactory = CardRendererFactory.GetFactory;
        player = entFactory.CreateEntity(worldGrid, new Hex(0, 0));

        currentContext = new GameplayContext(this, player, worldGrid, interfaceManager);

        CardDatabase.LoadAllCards();

        bop = true;
    }

    private bool bop;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Card card = CardDatabase.CreateCardFromID(bop ? 0 : 9);
            CardRenderer cr = cardFactory.CreateCardRenderer(card);
            interfaceManager.hand.AddCardToHand(cr);
            bop = !bop;
        }

    }

}
