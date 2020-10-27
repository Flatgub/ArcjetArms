using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTracer : MonoBehaviour
{
    public TrailRenderer tail;
    public float trailSpeed;

    public void GoTo(Vector2 pos)
    {
        transform.LeanMove(new Vector3(pos.x, pos.y, -1), trailSpeed);//;.setEaseOutCirc();
    }

    public void GoTo(HexGrid grid, Hex pos)
    {
        Vector2 realpos = grid.GetWorldPosition(pos);
        transform.LeanMove(new Vector3(realpos.x, realpos.y, -1), trailSpeed);//.setEaseOutCirc();
    }

    
}
