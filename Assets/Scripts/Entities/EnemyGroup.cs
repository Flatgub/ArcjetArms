using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "EnemyGrouping")]
public class EnemyGroup : ScriptableObject
{
    public int difficulty = 1;
    public List<string> enemies;
}
