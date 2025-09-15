using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyWave")]
public class Wave : ScriptableObject
{
    public int waveCount;
    public List<Enemy> enemies;
}
