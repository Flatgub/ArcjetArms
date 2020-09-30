using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private CardActionResult currentCardAction;

    public Text playerText;
    public Text playerEnergyText;
    public Text enemyText;

    private DeckTemplate basicDeck;
    private Deck drawPile;
    private Deck discardPile;

    private int cardsLeftToDraw = 0;
    private float cardDrawTimer = 0f;
    private float cardDrawPause = 0.1f;

    public int HandSize = 1;
    private int energy;

    private List<Card> playerHand;
    private Card activeCard;

    private List<Card> allExistingCards = null; //TODO: REMOVE

    public InfoPanelStack playerStatusEffectPanel;

    public Button endTurnButton;

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
        EnemyTurn,

        DrawingCards,
    }

    private Stack<GameState> stateStack;

    // Start is called before the first frame update
    void Start()
    {
        stateStack = new Stack<GameState>();
        stateStack.Push(GameState.PlayerIdle);
        
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
        player.OnStatusEffectsChanged += UpdatePlayerStatusEventPanel;

        GameplayContext.InitializeForEncounter(this, player, worldGrid, interfaceManager);

        allEnemies = new List<Entity>();

        for (int i = 0; i <= 3; i++)
        {
            Entity e = entFactory.CreateEntity(10);
            e.AddToGrid(worldGrid, new Hex(3-i, i));
            e.entityName = "enemy " + i;
            //e.ApplyStatusEffect(new DebugStatusEffect());
            entFactory.AddAIController(e);
            allEnemies.Add(e);
        }

        if (GameplayContext.CurrentLoadout != null)
        {
            basicDeck = GameplayContext.CurrentLoadout.LoadoutToDeckTemplate();
        }
        else
        {
            //fudge a deck together so we can start on the combat scene without crashing
            CardDatabase.LoadAllCards();
            basicDeck = new DeckTemplate();
            basicDeck.AddCardID(CardDatabase.GetCardDataByName("Shoot").cardID, 6);
            basicDeck.AddCardID(CardDatabase.GetCardDataByName("Step").cardID, 4);
        }
        

        drawPile = basicDeck.ConvertToDeck();
        drawPile.PrintContents("drawpile");

        allExistingCards = new List<Card>();
        foreach (Card c in drawPile)
        {
            allExistingCards.Add(c);
        }

        discardPile = new Deck();
        playerHand = new List<Card>();

        drawPile.Shuffle();

        energy = 5;

        Invoke("StartNewTurn", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (stateStack.Peek())
        {
            case GameState.PlayerIdle:
            {
                endTurnButton.interactable = true;
            }
            ;break;

            case GameState.PlayerCardPending:
            {
                endTurnButton.interactable = false;
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
                        energy -= GameplayContext.ActiveCard.cardData.energyCost;
                        UpdatePlayerStatusEventPanel();
                    }

                    CleanDeadEnemies();

                    stateStack.Pop();
                    currentCardAction = null;
                    GameplayContext.ActiveCard = null;

                }
            };
            break;

            case GameState.EnemyTurn:
            {
                //each entity takes a turn
                foreach (Entity enemy in allEnemies)
                {
                    enemy.StartTurn();
                    enemy.AIController.DoRandomAction();
                    enemy.EndTurn();
                }

                StartNewTurn();
            };break;

            case GameState.DrawingCards:
            {
                endTurnButton.interactable = false;
                if (cardDrawTimer <= 0)
                {
                    DrawCard();
                    cardsLeftToDraw--;
                    cardDrawTimer = cardDrawPause;
                    Debug.Log("Drew 1, " + cardsLeftToDraw + "cards remain");
                }
                else
                {
                    cardDrawTimer -= Time.deltaTime;
                }

                //stop drawing cards if empty or finished
                if ((drawPile.Count == 0 && discardPile.Count == 0) || cardsLeftToDraw == 0)
                {
                    cardDrawTimer = 0;
                    stateStack.Pop();
                }
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
            drawPile.PrintContents("draw pile");
            discardPile.PrintContents("discard pile");
        }
    }

    public void StartNewTurn()
    {
        energy = 5;
        DiscardHand();
        player.StartTurn();
        stateStack.Pop();
        stateStack.Push(GameState.PlayerIdle);
        DrawHand();
    }

    public void EndPlayerTurn()
    {
        player.EndTurn();
        stateStack.Pop();
        stateStack.Push(GameState.EnemyTurn);
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
        if (stateStack.Peek() == GameState.PlayerIdle)
        {
            if (energy >= card.cardData.energyCost)
            {
                activeCard = card;
                GameplayContext.ActiveCard = card;

                OnCardSelected?.Invoke(activeCard);

                playerHand.Remove(card);
                
                currentCardAction = card.AttemptToPlay();
                stateStack.Push(GameState.PlayerCardPending);
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
        AttemptDrawCard(HandSize);
    }

    /// <summary>
    /// Draw a number of cards from the top of the draw pile at once. 
    /// </summary>
    /// <remarks>n should never be more than the number of total cards in both the draw and discard 
    /// piles</remarks>
    /// <param name="n">How many cards to draw</param>
    /// <returns>A list of all the cards that were drawn</returns>
    private List<Card> DrawCards(int n)
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
    private Card DrawCard()
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

    public void AttemptDrawCard(int n = 1)
    {
        if (drawPile.Count == 0 && discardPile.Count == 0)
        {
            return;
        }
        cardsLeftToDraw = n;
        stateStack.Push(GameState.DrawingCards);
        //DrawCards(n);
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

    private void UpdatePlayerStatusEventPanel()
    {
        playerStatusEffectPanel.Clear();
        foreach (StatusEffect s in player.GetStatusEffects())
        {
            playerStatusEffectPanel.AddPanel(s.GetName(), s.GetDescription());
        }
        
    }

}
