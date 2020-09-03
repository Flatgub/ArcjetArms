using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory entFactory;
    
    private Entity player;
    private List<Entity> allEnemies;
    public InterfaceManager interfaceManager;
    public GameplayContext currentContext;

    private CardActionResult currentCardAction;

    public Text playerText;
    public Text playerEnergyText;
    public Text enemyText;

    private DeckTemplate basicDeck;
    private Deck drawPile;
    private Deck discardPile;

    public int HandSize = 5;
    private int energy;

    private List<Card> playerHand;
    private Card activeCard;

    /// <summary>
    /// The event triggered when a card is added from the draw pile into the hand
    /// </summary>
    public event Action<Card> OnCardDrawn;
    /// <summary>
    /// The event triggered when a card is selected to be the active card
    /// </summary>
    public event Action<Card> OnCardSelected;
    /// <summary>
    /// The event triggered when the active card is deselected
    /// </summary>
    public event Action       OnCardDeselected;
    /// <summary>
    /// The event triggered when a card is discarded (either from the hand or as the active card)
    /// </summary>
    public event Action<Card> OnCardDiscarded;

    //TODO: replace this enum with a different system
    enum GameState
    {
        PlayerIdle,
        PlayerCardPending,
        EnemyTurn
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

        basicDeck = new DeckTemplate();
        basicDeck.AddCardID(0, 4); //four steps
        basicDeck.AddCardID(1, 4); //four punches
        basicDeck.AddCardID(9, 2); //two dashes

        drawPile = basicDeck.ConvertToDeck();
        discardPile = new Deck();
        playerHand = new List<Card>();

        drawPile.Shuffle();

        energy = 5;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.PlayerIdle:
            {
                if (Input.GetButtonDown("Jump"))
                {
                    state = GameState.EnemyTurn;
                }
            }
            ;break;

            case GameState.PlayerCardPending:
            {
                if (currentCardAction.IsReadyOrCancelled())
                {
                    if (currentCardAction.WasCancelled())
                    {
                        //card was cancelled, return it to the hand
                        DeselectActiveCard();
                    }
                    else
                    {
                        //card was played, put it in the discard pile
                        DiscardCard(activeCard);
                        energy -= currentContext.ActiveCard.cardData.energyCost;
                    }

                    CleanDeadEnemies();

                    state = GameState.PlayerIdle;
                    currentCardAction = null;
                    currentContext.ActiveCard = null;

                }
            };
            break;

            case GameState.EnemyTurn:
            {
                //each entity takes a turn
                foreach (Entity enemy in allEnemies)
                {
                    enemy.AIController.DoRandomAction(currentContext);
                }

                StartNewTurn();
            };break;
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

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("draw pile");
            drawPile.PrintContents();
            Debug.Log("discard pile");
            discardPile.PrintContents();
        }
    }

    public void StartNewTurn()
    {
        energy = 5;
        DiscardHand();
        DrawHand();
        state = GameState.PlayerIdle;
    }

    public void DeselectActiveCard()
    {
        playerHand.Add(activeCard);
        OnCardDeselected?.Invoke();
        activeCard = null;
    }

    public void AttemptPlayingCard(Card card)
    {
        //TODO: put energy cost restriction checking in here
        if (state == GameState.PlayerIdle)
        {
            if (energy >= card.cardData.energyCost)
            {
                activeCard = card;
                currentContext.ActiveCard = card;

                OnCardSelected?.Invoke(activeCard);

                playerHand.Remove(card);
                
                currentCardAction = card.AttemptToPlay(currentContext);
                state = GameState.PlayerCardPending;
            }
        }
    }

    public void DiscardHand()
    {
        while (playerHand.Count != 0)
        {
            Card card = playerHand[0];
            DiscardCard(card);
        }
    }

    public void DiscardCard(Card card)
    {
        if (activeCard == card)
        {
            activeCard = null;
        }
        playerHand.Remove(card);
        OnCardDiscarded?.Invoke(card);
        discardPile.AddToTop(card);
    }

    /// <summary>
    /// Draws a full hand of cards. Will eventually 
    /// </summary>
    public void DrawHand()
    {
        //TODO: force the drawing of a step and a punch, then randomly fill the rest
        //NOTE: this requires a way to ensure certain cards are deleted instead of discarded
        //      otherwise the deck will get bigger every turn as we add cards
        DrawCards(HandSize);
    }

    /// <summary>
    /// Draw a number of cards from the top of the draw pile at once. 
    /// </summary>
    /// <remarks>n should never be more than the number of total cards in both the draw and discard 
    /// piles</remarks>
    /// <param name="n">How many cards to draw</param>
    /// <returns>A list of all the cards that were drawn</returns>
    public List<Card> DrawCards(int n)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < n; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }

    /// <summary>
    /// Draws a card from the draw pile. If the draw pile is empty, the discard pile will be merged
    /// into the draw pile and then the draw pile will be shuffled before a card is drawn.
    /// </summary>
    /// <returns>The card from the top of the draw pile</returns>
    public Card DrawCard()
    {
        if (drawPile.Count == 0)
        {
            discardPile.MergeAllInto(drawPile);
            drawPile.Shuffle();
        }

        Card card = drawPile.TakeFromTop();
        playerHand.Add(card);
        OnCardDrawn?.Invoke(card);
        return card;
    }

    ///<summary>Sweep and remove any entities from the entities list that are no longer alive.</summary>
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
