using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory entFactory;
    public EncounterTemplate defaultEncounterTemplate;
    
    private Entity player;
    public List<Entity> allEntities;
    [HideInInspector]
    public List<Entity> allEnemies;
    public InterfaceManager interfaceManager;

    private CardActionResult currentCardAction;

    public Text playerText;
    public Text enemyText;

    public TextMeshProUGUI energyNumber;
    public TextMeshProUGUI drawPileNumber;
    public TextMeshProUGUI discardPileNumber;

    private DeckTemplate basicDeck;
    private Deck drawPile;
    private Deck discardPile;

    private int cardsLeftToDraw = 0;
    private float cardDrawTimer = 0f;
    private float cardDrawPause = 0.1f;

    private float turnTimer = 0;
    public float timeBetweenTurns = 1f;
    private List<Entity> enemiesWhoNeedTurns;
    private Entity enemyTakingTurn;
    private bool enemyTurnFinished;

    public int HandSize = 1;
    public int energy;

    private List<Card> playerHand;
    private Card activeCard;

    public HealthBar playerHealthBar;
    public HealthBar enemyHealthBar;

    private List<Card> allExistingCards = null; //TODO: REMOVE

    public InfoPanelStack playerStatusEffectPanel;
    public InfoPanelStack enemyStatusEffectPanel;

    public Button endTurnButton;

    public Transform GameoverPanel;
    public Transform VictoryPanel;
    public CanvasGroup gameplayElements;
    public RewardMenu rewardMenu;

    public TerrainType rockTerrain;
    public int difficultyThreshold = 3; //how much easier than the current difficulty is allowed

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
        GameOver,

        DrawingCards,
    }

    private Stack<GameState> stateStack;

    // Start is called before the first frame update
    void Start()
    {
        FXHelper.Initialize();

        ColorUtility.TryParseHtmlString("#793434", out Color col);
        enemyHealthBar.Colour = col;

        stateStack = new Stack<GameState>();
        stateStack.Push(GameState.PlayerIdle);
        
        if (worldGrid == null)
        {
            worldGrid = GameObject.FindObjectOfType<HexGrid>();
        }
        worldGrid.GenerateMap(mapRadius);
        entFactory = EntityFactory.GetFactory;

        allEntities = new List<Entity>();
        allEnemies = new List<Entity>();
        enemiesWhoNeedTurns = new List<Entity>();

        if (GameplayContext.ChosenTemplate != null)
        {
            GenerateEncounter(GameplayContext.ChosenTemplate);
        }
        else
        {
            GenerateEncounter(defaultEncounterTemplate);
        }

        GameplayContext.InitializeForEncounter(this, player, worldGrid, interfaceManager);

        if (GameplayContext.CurrentLoadout != null)
        {
            basicDeck = GameplayContext.CurrentLoadout.ToDeckTemplate();
        }
        else
        {
            //fudge a deck together so we can start on the combat scene without crashing
            CardDatabase.LoadAllCards();
            basicDeck = new DeckTemplate();
            basicDeck.AddCardID(CardDatabase.GetCardDataByName("Shoot").cardID, 6);
            basicDeck.AddCardID(CardDatabase.GetCardDataByName("Step").cardID, 4);
        }

        if (!GearDatabase.IsLoaded)
        {
            GearDatabase.LoadAllGear();
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

        if (!GameplayContext.InDebugMode)
        {
            GameObject endbutton = GameObject.Find("EndGameButton");
            endbutton.SetActive(false);
        }
        

        Invoke("StartNewTurn", 0.2f);
    }

    private void GenerateEncounter(EncounterTemplate template)
    {
        //spawn terrain
        foreach (Hex rockpos in template.terrainPieces)
        {
            Entity rock = entFactory.CreateTerrain(rockTerrain);
            rock.AddToGrid(worldGrid, rockpos);
            allEntities.Add(rock);
        }

        //spawn player
        Hex playerspawn = template.playerSpawnPoints.GetRandom();

        player = entFactory.CreateEntity(40);
        player.AddToGrid(worldGrid, playerspawn);
        player.EnableStatusEffects(true);
        player.entityName = "Player";
        if (GameplayContext.LastPlayerHealth > -1)
        {
            player.Health.SetHealth(GameplayContext.LastPlayerHealth);
        }
        player.appearance.sprite = Resources.Load<Sprite>("Sprites/PlayerArt");
        player.OnStatusEffectsChanged += UpdatePlayerStatusEffectPanel;
        ColorUtility.TryParseHtmlString("#46537d", out Color col );
        playerHealthBar.Colour = col;
        playerHealthBar.MaxValue = player.Health.MaxHealth;


        //select enemygroup, ignoring difficulty when in debug mode
        EnemyGroup enemyGroup;
        if (GameplayContext.InDebugMode)
        {
            enemyGroup = entFactory.GetEnemyGroup(template.minEnemies, template.maxEnemies);
        }
        else
        {
            int maxDif = GameplayContext.CurrentDifficulty;
            int minDif = Math.Max(1, maxDif - difficultyThreshold);
            enemyGroup = entFactory.GetEnemyGroup(template.minEnemies, template.maxEnemies, minDif, maxDif);
            Debug.Log("making encounter using " + GameplayContext.CurrentDifficulty);
        }
        
        List<PODHex> enemySpots = new List<PODHex>(template.enemySpawnPoints);
        //spawn enemies
        foreach (string enemyType in enemyGroup.enemies)
        {
            if (enemySpots.Count == 0)
            {
                break;
            }

            Hex spawnpoint = enemySpots.PopRandom();
            Entity e = entFactory.CreateEntity(10);

            e.AddToGrid(worldGrid, spawnpoint);
            e.EnableStatusEffects(true);
            entFactory.AddAIController(e, enemyType);
            Debug.Log("spawned: " + e.entityName);
            allEntities.Add(e);
            allEnemies.Add(e);
        }
        //
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
                        UpdatePlayerStatusEffectPanel();
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
                endTurnButton.interactable = false;
                if (turnTimer > 0)
                {
                    turnTimer -= Time.deltaTime;
                }
                else
                {
                    if (enemyTakingTurn is null)
                    {
                        StartNextEnemyTurn();
                    }
                    else if (enemyTurnFinished)
                    {
                        turnTimer = timeBetweenTurns;
                    }

                }
            };break;

            case GameState.DrawingCards:
            {
                endTurnButton.interactable = false;
                if (cardDrawTimer <= 0)
                {
                    DrawCard();
                    cardsLeftToDraw--;
                    cardDrawTimer = cardDrawPause;
                    //Debug.Log("Drew 1, " + cardsLeftToDraw + "cards remain");
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
        
        playerHealthBar.Value = player.Health.Current;
        

        drawPileNumber.text = drawPile.Count.ToString();
        discardPileNumber.text = discardPile.Count.ToString();
        energyNumber.text = energy.ToString();

        if (worldGrid.GetHexUnderMouse() is Hex mousehex  
            && worldGrid.GetEntityAtHex(mousehex) is Entity entUnderMouse
            && entUnderMouse != player)
        {
            enemyText.enabled = true;
            enemyText.text = entUnderMouse.entityName;
            enemyHealthBar.gameObject.SetActive(true);
            enemyHealthBar.MaxValue = entUnderMouse.Health.MaxHealth;
            enemyHealthBar.Value = entUnderMouse.Health.Current;
            enemyStatusEffectPanel.gameObject.SetActive(true);
            UpdateEnemyStatusEffectPanel(entUnderMouse);
            GameplayContext.EntityUnderMouse = entUnderMouse;
        }
        else
        {
            enemyText.enabled = false;
            enemyHealthBar.gameObject.SetActive(false);
            enemyStatusEffectPanel.gameObject.SetActive(false);
            GameplayContext.EntityUnderMouse = null;
        }

        if (stateStack.Peek() != GameState.GameOver && allEnemies.Count == 0)
        {
            WinEncounter();
        }
    }

    public void StartNewTurn()
    {
        if (!player.Health.IsDead)
        {
            
            player.StartTurn();
            stateStack.Pop();
            stateStack.Push(GameState.PlayerIdle);
            if (!player.isStunned)
            {
                energy = 5;
                DrawHand();
            }
            else
            {
                energy = 0;
            }
            
        }
        else
        {
            LoseEncounter();
        }
        UpdatePlayerStatusEffectPanel();


    }

    public void EndPlayerTurn()
    {
        player.EndTurn();
        DiscardHand();

        stateStack.Pop();
        //each entity takes a turn
        foreach (Entity enemy in allEntities)
        {
            if (enemy.AIController != null)
            {
                enemiesWhoNeedTurns.Add(enemy);
            }
        }
        turnTimer = timeBetweenTurns * 0.5f;
        stateStack.Push(GameState.EnemyTurn);
        UpdatePlayerStatusEffectPanel();
    }

    public void StartNextEnemyTurn()
    {
        //Do a turn with one enemy
        turnTimer = timeBetweenTurns;
        enemyTurnFinished = false;
        enemyTakingTurn = enemiesWhoNeedTurns[0];
        enemiesWhoNeedTurns.RemoveAt(0);
        enemyTakingTurn.StartTurn();
        enemyTakingTurn.AIController.DoRandomAction(EndEnemyTurn);
    }

    public void EndEnemyTurn()
    {
        enemyTakingTurn.EndTurn();
        enemyTurnFinished = true;
        enemyTakingTurn = null;
        if (enemiesWhoNeedTurns.Count == 0)
        {
            StartNewTurn();
        }
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
        for (int i = allEntities.Count - 1; i >= 0; i--)
        {
            Entity ent = allEntities[i];
            if (ent.Health.IsDead)
            {
                allEntities.RemoveAt(i);
                Destroy(ent.gameObject);
                if (allEnemies.Contains(ent))
                {
                    allEnemies.Remove(ent);
                }
            }
        }
    }

    private void UpdatePlayerStatusEffectPanel()
    {
        playerStatusEffectPanel.Clear();
        foreach (StatusEffect s in player.GetStatusEffects())
        {
            playerStatusEffectPanel.AddPanel(s.GetName(), s.GetDescription());
        }
        
    }

    private void UpdateEnemyStatusEffectPanel(Entity enemy)
    {
        enemyStatusEffectPanel.Clear();
        if (enemy.AcceptsStatusEffects)
        {
            foreach (StatusEffect s in enemy.GetStatusEffects())
            {
                enemyStatusEffectPanel.AddPanel(s.GetName(), s.GetDescription());
            }
        }
    }

    public void ShowRewardMenu()
    {
        LootPool masterpool = GearDatabase.GenerateMasterLootPool();

        if (GameplayContext.CurrentLoadout is GearLoadout load)
        {
            masterpool.SubtractLoadout(load);
        }

        if (GameplayContext.CurrentInventory is InventoryCollection inv)
        {
            masterpool.SubtractInventory(inv);
        }

        masterpool.MakeActive();

        //bail if there isn't enough loot, cuz we crash otherwise
        if (masterpool.activePool.Count < 3)
        {
            masterpool.Finish();
            ReturnToLoadoutScreen();
            return;
        }

        gameplayElements.interactable = false;
        gameplayElements.blocksRaycasts = false;
        gameplayElements.LeanAlpha(0, rewardMenu.appearSpeed);
        rewardMenu.Init();
        rewardMenu.ShowRewardMenu();

        
        for (int i = 0; i < 3; i++)
        {
            rewardMenu.AddRewardOption(masterpool.Pop());
        }
        masterpool.Finish();
    }

    public void WinEncounter()
    {
        stateStack.Pop();
        stateStack.Push(GameState.GameOver);
        DiscardHand();
        VictoryPanel.LeanMoveLocal(Vector3.zero, 1.0f).setEaseOutElastic();
        GameplayContext.CurrentDifficulty += 2;
        GameplayContext.LastPlayerHealth = player.Health.Current;
    }

    public void LoseEncounter()
    {
        player.appearance.enabled = false;
        stateStack.Pop();
        stateStack.Push(GameState.GameOver);
        GameoverPanel.LeanMoveLocal(Vector3.zero, 1.0f).setEaseOutElastic();
    }

    public void ReturnToLoadoutScreen()
    {
        GameplayContext.Clear();
        SceneManager.LoadScene("InventoryMenu");
    }
}
