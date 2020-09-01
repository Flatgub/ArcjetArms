using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HandContainer : MonoBehaviour
{
    private List<CardRenderer> cardsInHand;
    private RectTransform rect;
    private float width;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        width = rect.rect.width;
        cardsInHand = new List<CardRenderer>();
        foreach (CardRenderer cr in transform.GetComponentsInChildren<CardRenderer>())
        {
            cardsInHand.Add(cr);
        }

        UpdatePositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePositions()
    {
        foreach (CardRenderer cr in cardsInHand)
        {
            Debug.Log("found " + cr.gameObject.name);
        }
    }
}
