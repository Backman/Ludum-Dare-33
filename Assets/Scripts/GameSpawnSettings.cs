using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class GameSpawnSettings : ScriptableObject
{
    [System.Serializable]
    public struct SpawnPeriod
    {
        public EnemySpawnSettings SpawnSettings;
        public float PeriodDuration;
    }

    public SpawnPeriod[] Periods;
    
}
