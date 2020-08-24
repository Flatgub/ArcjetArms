﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entityFactory is used to instanciate entities based on given parameters
/// </summary>
public class EntityFactory : MonoBehaviour
{
    private static EntityFactory factoryInstance;
    private GameObject entityPrefab;


    public static EntityFactory GetFactory
    {
        get
        {
            if (factoryInstance == null)
            {
                GameObject obj = new GameObject();
                factoryInstance = obj.AddComponent<EntityFactory>();
            }
            return factoryInstance;
        }
    }

    public void Awake()
    {
        entityPrefab = Resources.Load<GameObject>("Prefabs/BasicEntity");
    }

    //TODO: make this method more modular to accept unique constructors for unique entities
    //      or different prefabs
    /// <summary>
    /// Used to create an entity at a given location
    /// </summary>
    /// <param name="grid">The <see cref="HexGrid"/> the entity exists within</param>
    /// <param name="position">The hex position of the entity within the grid</param>
    /// <returns>An Entity Component, attached to the BasicEntity Prefab</returns>
    public Entity CreateEntity(HexGrid grid, Hex position)
    {
        //construct new entity prefab
        GameObject entityObj = Instantiate(entityPrefab);
        Entity entComponent = entityObj.GetComponent<Entity>();
        //set relevant properties on the prefab
        entComponent.Initialize(grid, position);
        //add the prefab to the grid
        grid.AddEntityToGrid(entComponent);
        return entComponent;
    }
}
