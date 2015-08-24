using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class GameSpawnSettings : ScriptableObject
{
    [System.Serializable]
    public struct SpawnPeriod
    {
        public string Name;
        public EnemySpawnSettings SpawnSettings;
        public float PeriodDuration;
    }

    public SpawnPeriod[] Periods;
    
}
