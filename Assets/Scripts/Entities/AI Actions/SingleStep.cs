using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleStep : IAIAction
{
    public void Do(GameplayContext context, Entity with)
    {
        List<Hex> canditates = with.Position.GetAllNeighbours();
        GridHelper.RemoveOccupiedHexes(with.Grid, canditates);
        with.MoveTo(canditates.GetRandom());
    }

    public bool IsDoable(GameplayContext context, Entity with)
    {

        List<Hex> canditates = with.Position.GetAllNeighbours();
        GridHelper.RemoveOccupiedHexes(with.Grid, canditates);

        return (canditates.Count != 0);

    }
}
