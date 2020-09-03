using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entityFactory is used to instanciate entities based on given parameters
/// </summary>
public class EntityFactory : MonoBehaviour
{
    private static EntityFactory factoryInstance;
    private GameObject entityPrefab;
    private Sprite defaultEntitySprite;

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
        //FIXME: perhaps don't hardcode these
        entityPrefab = Resources.Load<GameObject>("Prefabs/BasicEntity");
        defaultEntitySprite = Resources.Load<Sprite>("Sprites/NoArtEntity");
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

    public EntityAIController AddAIController(Entity ent)
    {
        EntityAIController ai = ent.gameObject.AddComponent<EntityAIController>();
        ai.AddAction(new SingleStep());
        //twice as likely to attack, don't do this in future, make this weighted
        ai.AddAction(new BasicMelee());
        ai.AddAction(new BasicMelee());
        return ai;
    }
}

