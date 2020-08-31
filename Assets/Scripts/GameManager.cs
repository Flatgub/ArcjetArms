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

        currentContext = new GameplayContext(player, worldGrid, interfaceManager);

        exampleCard.LoadDataFrom(exampleData);
        exampleCard.AttemptToPlay(currentContext);
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }

}
