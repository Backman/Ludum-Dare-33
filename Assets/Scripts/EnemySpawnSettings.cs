using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class EnemySpawnSettings : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawn
    {
        public float SpawnChance;
        public float SpawnValue;
        public int MinLevel;
        public int MaxLevel;
        public GameObject[] Variations;

        public int MaxY; 
        public bool HasMaxY;
        public int MinY;
        public bool HasMinY;
    }

    public EnemySpawn[] Spawns;
    public float SpawnSpeed;
    public AnimationCurve IntensityCurve;
    public float IntensityPeriod;
}
