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

    private Deck drawPile;
    private Deck discardPile;

    public int HandSize = 5;
    private int energy;

    private List<Card> playerHand;
    private Card activeCard;

    public event Action<Card> OnCardDrawn;
    public event Action<Card> OnCardSelected;
    public event Action       OnCardDeselected;
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

        drawPile = new Deck();
        discardPile = new Deck();
        playerHand = new List<Card>();

        CardDatabase.LoadAllCards();

        for (int i = 0; i < 4; i++) //four steps
        {
            drawPile.AddToTop(CardDatabase.CreateCardFromID(0));
        }
        for (int i = 0; i < 4; i++) //four punches
        {
            drawPile.AddToTop(CardDatabase.CreateCardFromID(1)); 
        }
        for (int i = 0; i < 2; i++) //two dashes
        {
            drawPile.AddToTop(CardDatabase.CreateCardFromID(9)); 
        }

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
                        //CardRenderer cr = interfaceManager.activeCardRenderer;
                        DiscardCard(activeCard);
                        //interfaceManager.DiscardCard(cr);
                        //interfaceManager.hand.HoldCardsDown = false;
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
        //interfaceManager.DeselectActiveCard();
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
                //interfaceManager.SelectCardFromHand(card);
                
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
            //playerHand.RemoveAt(0);
            //discardPile.AddToTop(card);
            //interfaceManager.DiscardCard(card.tiedTo);
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

    public void DrawHand()
    {
        //force the drawing of a step and a punch, then randomly fill the rest
        //DrawCard(0);
        //DrawCard(1);
        DrawCards(HandSize);
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
        if (drawPile.Count == 0)
        {
            discardPile.MergeAllInto(drawPile);
            drawPile.Shuffle();
        }

        Card card = drawPile.TakeFromTop();
        playerHand.Add(card);
        OnCardDrawn?.Invoke(card);
        //CardRenderer cr = cardFactory.CreateCardRenderer(card);
        //interfaceManager.hand.AddCardToHand(cr);
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
