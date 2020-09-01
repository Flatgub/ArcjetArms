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

    private CardActionResult currentCardAction;

    //TODO: replace this enum with a different system
    enum GameState
    {
        PlayerIdle,
        PlayerCardPending,
    }

    private GameState state;

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.PlayerIdle;
        
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
        switch (state)
        {
            case GameState.PlayerIdle:
            {
                
            };break;

            case GameState.PlayerCardPending:
            {
                if (currentCardAction.IsReadyOrCancelled())
                {
                    if (currentCardAction.WasCancelled())
                    {
                        //card was cancelled, return it to the hand
                        interfaceManager.DeselectActiveCard();
                    }
                    else
                    {
                        //card was played, put it in the discard pile
                        CardRenderer cr = interfaceManager.activeCardRenderer;
                        Card card = cr.tiedTo;
                        Destroy(cr.gameObject);
                        card.tiedTo = null;
                        interfaceManager.activeCardRenderer = null;
                        interfaceManager.hand.HoldCardsDown = false;
                    }

                    state = GameState.PlayerIdle;
                    currentCardAction = null;
                }
            };
            break;
        }


        if (Input.GetButtonDown("Jump"))
        {
            Card card = CardDatabase.CreateCardFromID(bop ? 0 : 9);
            CardRenderer cr = cardFactory.CreateCardRenderer(card);
            interfaceManager.hand.AddCardToHand(cr);
            bop = !bop;
        }

    }

    public void AttemptPlayingCard(Card card)
    {
        //TODO: put energy cost restriction checking in here
        if (state == GameState.PlayerIdle)
        {
            interfaceManager.SelectCardFromHand(card);
            currentCardAction = card.AttemptToPlay(currentContext);
            state = GameState.PlayerCardPending;
        }
    }

}
