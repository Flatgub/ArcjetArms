using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "EnemyGrouping")]
public class EnemyGroup : ScriptableObject
{
    public List<string> enemies;
}
