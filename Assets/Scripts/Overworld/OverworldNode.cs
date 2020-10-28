using System;
using System.Collections.Generic;
using UnityEngine;

public class OverworldNode : MonoBehaviour
{
    public SpriteRenderer appearance;
    public List<OverworldNode> inwardLinks;
    public List<OverworldNode> outwardLinks;
    public event Action<OverworldNode> onClicked;
    
    void Awake()
    {
        inwardLinks = new List<OverworldNode>();
        outwardLinks = new List<OverworldNode>();
    }

    private void OnMouseDown()
    {
        onClicked?.Invoke(this);
    }
}
