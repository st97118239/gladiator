using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyWave")]
public class Wave : ScriptableObject
{
    public List<Enemy> enemies;
    public Boss boss;
    public bool hasAbilityRoll;
    public AbilitySort[] abilitySortsToRoll;
}
