using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entityFactory is used to instanciate entities based on given parameters
/// </summary>
public class EntityFactory : MonoBehaviour
{
    private static EntityFactory factoryInstance;
    private GameObject entityPrefab;
    private static Sprite defaultEntitySprite;
    private Dictionary<string, IAiTemplate> allAITemplates;
    private static Dictionary<string, Sprite> enemySprites;
    private static List<EnemyGroup> allEnemyGroups;
    private static Dictionary<EnemyGroup, int> enemyGroupDifficulties;

    /// <summary>
    /// Get the singleton instance of the entityFactory, or make one if it doesn't yet exist
    /// </summary>
    public static EntityFactory GetFactory
    {
        get
        {
            if (factoryInstance == null)
            {
                GameObject obj = new GameObject("Entity Factory");
                factoryInstance = obj.AddComponent<EntityFactory>();
            }
            return factoryInstance;
        }
    }

    public void Awake()
    {
        entityPrefab = Resources.Load<GameObject>("Prefabs/BasicEntity");
        defaultEntitySprite = Resources.Load<Sprite>("Sprites/NoArtEntity");
        allAITemplates = new Dictionary<string, IAiTemplate>
        {
            ["LightAttacker"] = new AI_LightAttacker(),
            ["Sniper"] = new AI_Sniper(),
            ["HookThrower"] = new AI_HookThrower(),
            ["Lancer"] = new AI_Lancer(),
            ["Armoured"] = new AI_ArmoredEnemy(),
            ["Mortar"] = new AI_Mortar(),
            ["Mechanic"] = new AI_Mechanic()
        };
        enemySprites = new Dictionary<string, Sprite>();
        allEnemyGroups = new List<EnemyGroup>(Resources.LoadAll<EnemyGroup>("EnemyGroups"));

        enemyGroupDifficulties = new Dictionary<EnemyGroup, int>();
        foreach (EnemyGroup group in allEnemyGroups)
        {
            int score = 0;
            foreach (string enemy in group.enemies)
            {
                score += allAITemplates[enemy].DifficultyScore;
            }
            enemyGroupDifficulties.Add(group, score);
        }
        
    }

    //TODO: make this method more modular to accept unique constructors for unique entities
    //      or different prefabs
    //
    /// <summary>
    /// Used to create an entity at a given location
    /// </summary>
    /// <param name="grid">The <see cref="HexGrid"/> the entity exists within</param>
    /// <param name="position">The hex position of the entity within the grid</param>
    /// <returns>An Entity Component, attached to the BasicEntity Prefab</returns>
    public Entity CreateEntity(int maxHealth)
    {
        //construct new entity prefab
        GameObject entityObj = new GameObject("Entity");//Instantiate(entityPrefab);
        AddRenderComponent(entityObj);
        HealthComponent.AddHealthComponent(entityObj, maxHealth);
        Entity entComponent = entityObj.AddComponent<Entity>();
        entComponent.Initialize();
        return entComponent;
    }

    private SpriteRenderer AddRenderComponent(GameObject go)
    {
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = defaultEntitySprite;
        return sr;
    }

    public EntityAIController AddAIController(Entity ent, string templateName)
    {
        EntityAIController ai = ent.gameObject.AddComponent<EntityAIController>();
        if (templateName != "random" && !allAITemplates.ContainsKey(templateName))
        {
            throw new ArgumentException("No such AI template '" + templateName + "'");
        }
        else if (templateName == "random")
        {
            allAITemplates.GetRandomValue().ApplyTo(ent);
        }
        else
        {
            allAITemplates[templateName].ApplyTo(ent);
        }
        return ai;
    }

    public Entity CreateTerrain(TerrainType type)
    {
        Entity obstruction = CreateEntity(type.health);
        obstruction.entityName = type.name;
        obstruction.gameObject.name = type.name;
        obstruction.appearance.sprite = type.images.GetRandom();
        return obstruction;
    }

    public static Sprite GetEnemySprite(string spritename)
    {
        if (enemySprites.TryGetValue(spritename, out Sprite spr))
        {
            return spr;
        }
        else
        {
            Sprite newspr = Resources.Load<Sprite>("Sprites/" + spritename);
            if (newspr is Sprite)
            {
                enemySprites.Add(spritename, newspr);
                return newspr;
            }
            else
            {
                Debug.LogWarning("Cannot find enemy sprite with name '" + spritename + "'");
                return defaultEntitySprite;
            }
        }
    }

    public EnemyGroup GetEnemyGroup(int minEnemies, int maxEnemies,
        int minDifficulty = 1, int maxDifficulty = 10)
    {
        List<EnemyGroup> candidates = new List<EnemyGroup>();
        foreach (EnemyGroup group in allEnemyGroups)
        {
            int difficulty = enemyGroupDifficulties[group];
            if (group.enemies.Count >= minEnemies && group.enemies.Count <= maxEnemies
                && difficulty >= minDifficulty && difficulty <= maxDifficulty)
            {
                candidates.Add(group);
            }
        }
        if (candidates.Count == 0)
        {
            Debug.LogWarning(
                String.Format("Could not find enemy group within restrictions, " +
                              "enemies: [{0}-{1}], difficulty: [{2}-{3}]",
                minEnemies, maxEnemies, minDifficulty, maxDifficulty));
            return allEnemyGroups[0];
        }
        else
        {
            return candidates.GetRandom();
        }
    }

}

