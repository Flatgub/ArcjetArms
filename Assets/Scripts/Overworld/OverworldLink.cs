using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldLink : MonoBehaviour
{
    public LineRenderer line;
    public OverworldNode from;
    public OverworldNode to;

    // Start is called before the first frame update
    void Start()
    {
        line.textureMode = LineTextureMode.Tile;
    }

    public void Link(OverworldNode to, OverworldNode from)
    {
        this.to = to;
        this.from = from;
        to.outwardLinks.Add(from);
        from.inwardLinks.Add(to);
    }

    // Update is called once per frame
    void Update()
    {
        if (to is OverworldNode && from is OverworldNode)
        {
            line.SetPosition(0, from.transform.position);
            line.SetPosition(1, to.transform.position);
        }
    }
}
