using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleStep : IAIAction
{
    public void Do(GameplayContext context, Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(context.Grid, with.Position,
            context.Player.Position);

        with.MoveTo(path[0]);
    }

    public bool IsDoable(GameplayContext context, Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(context.Grid, with.Position,
            context.Player.Position);

        return (path != null && path.Count > 1); //count 1 is adjacent
    }
}
