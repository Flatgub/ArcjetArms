using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleStep : IAIAction
{
    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position);

        with.MoveTo(path[0]);
    }

    public bool IsDoable(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position);

        return (path != null && path.Count > 1); //count 1 is adjacent
    }
}
