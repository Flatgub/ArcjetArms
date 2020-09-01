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
    public GameplayContext currentContext;

    public CardRenderer exampleRender;

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

        currentContext = new GameplayContext(this, player, worldGrid, interfaceManager);

        CardDatabase.LoadAllCards();

        Card card = CardDatabase.CreateCardFromID(0);

        exampleRender.TieTo(card);
        card.AttemptToPlay(currentContext);

        
    }

    // Update is called once per frame
    void Update()
    {
       

    }

}
