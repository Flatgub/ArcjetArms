using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory entFactory;
    private CardRendererFactory cardFactory;
    private Entity player;
    private List<Entity> allEnemies;
    public InterfaceManager interfaceManager;
    public GameplayContext currentContext;

    private CardActionResult currentCardAction;

    public Text playerText;
    public Text playerEnergyText;
    public Text enemyText;

    public int HandSize = 5;
    private int energy;

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

        player = entFactory.CreateEntity(40);
        player.AddToGrid(worldGrid, new Hex(1, 0));
        player.entityName = "Player";
        player.appearance.sprite = Resources.Load<Sprite>("Sprites/PlayerArt");

        currentContext = new GameplayContext(this, player, worldGrid, interfaceManager);
        allEnemies = new List<Entity>();

        for (int i = 0; i <= 3; i++)
        {
            Entity e = entFactory.CreateEntity(10);
            e.AddToGrid(worldGrid, new Hex(3-i, i));
            e.entityName = "enemy";
            entFactory.AddAIController(e);
            allEnemies.Add(e);
        }
       

        CardDatabase.LoadAllCards();

        energy = 5;

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
            }
            ;break;

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
                        interfaceManager.DiscardCard(cr);
                        interfaceManager.hand.HoldCardsDown = false;
                        energy -= currentContext.ActiveCard.cardData.energyCost;

                        
                    }

                    CleanDeadEnemies();

                    state = GameState.PlayerIdle;
                    currentCardAction = null;
                    currentContext.ActiveCard = null;

                }
            };
            break;
        }

        playerText.text = "PLAYER HEALTH: " + player.Health;
        playerEnergyText.text = "ENERGY: " + energy.ToString();

        if (worldGrid.GetHexUnderMouse() is Hex mousehex  
            && worldGrid.GetEntityAtHex(mousehex) is Entity entUnderMouse
            && entUnderMouse != player)
        {
            enemyText.enabled = true;
            enemyText.text = "ENEMY HEALTH: " + entUnderMouse.Health;
        }
        else
        {
            enemyText.enabled = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            StartNewTurn();
        }

    }

    public void StartNewTurn()
    {
        //each entity takes a turn
        foreach (Entity enemy in allEnemies)
        {
            enemy.AIController.DoRandomAction(currentContext);
        }
        
        energy = 5;
        interfaceManager.DiscardHand();
        DrawHand();
    }

    public void AttemptPlayingCard(Card card)
    {
        //TODO: put energy cost restriction checking in here
        if (state == GameState.PlayerIdle)
        {
            if (energy >= card.cardData.energyCost)
            {
                interfaceManager.SelectCardFromHand(card);
                currentContext.ActiveCard = card;
                currentCardAction = card.AttemptToPlay(currentContext);
                state = GameState.PlayerCardPending;
            }
        }
    }

    public void DrawHand()
    {
        //force the drawing of a step and a punch, then randomly fill the rest
        DrawCard(0);
        DrawCard(1);
        DrawCards(HandSize - 2);
    }

    public List<Card> DrawCards(int n)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < n; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }

    public Card DrawCard(int id = -1)
    {
        //pick a random card if ID isn't defined
        if (id == -1)
        {
            id = CardDatabase.GetAllIDs().GetRandom();
        }
        
        Card card = CardDatabase.CreateCardFromID(id);
        CardRenderer cr = cardFactory.CreateCardRenderer(card);
        interfaceManager.hand.AddCardToHand(cr);
        bop = !bop;
        return card;
    }

    private void CleanDeadEnemies()
    {
        for (int i = allEnemies.Count - 1; i >= 0; i--)
        {
            Entity ent = allEnemies[i];
            if (ent.Health.IsDead)
            {
                allEnemies.RemoveAt(i);
                Destroy(ent.gameObject);
            }
        }
    }
}
