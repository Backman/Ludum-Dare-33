using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public EnemySpawnSettings Settings;
    public float LayerDepth = 30;
    public float GridSize = 1;


    float _SpawnAccumulator;
    List<EnemySpawnData> _SpawnedEnemies = new List<EnemySpawnData>();

    Dictionary<GameObject, Queue<GameObject>> _EnemyPool = new Dictionary<GameObject, Queue<GameObject>>();

    void Update()
    {
        IntRect currentRect = CloudSpawner.GetCurrentRect(LayerDepth, GridSize);


        _SpawnAccumulator += Settings.SpawnSpeed * Settings.IntensityCurve.Evaluate(Time.time / Settings.IntensityPeriod) * Time.deltaTime;
        if (_SpawnAccumulator > 0)
        {
            float totalSpawnChance = 0;
            for (int i = 0; i < Settings.Spawns.Length; i++)
            {
                var spawn = Settings.Spawns[i];
                if (IsValidSpawn(currentRect, spawn))
                {
                    totalSpawnChance += spawn.SpawnChance;
                }
            }

            float randomVal = Random.Range(0, totalSpawnChance);

            for (int i = 0; i < Settings.Spawns.Length; i++)
            {
                var spawn = Settings.Spawns[i];
                if (IsValidSpawn(currentRect, spawn))
                {
                    if (spawn.SpawnChance > randomVal)
                    {
                        AttemptSpawn(currentRect, spawn);
                        break;
                    }
                    randomVal -= spawn.SpawnChance;
                }
            }

        }
    }

    bool IsValidSpawn(IntRect rect, EnemySpawnSettings.EnemySpawn spawn)
    {

        //if(spawn.MinLevel >= _CurrentLevel && spawn.MaxLevel <= _CurrentLevel)

        return (spawn.HasMinY == false || spawn.MinY <= rect.MaxY) && (spawn.HasMaxY == false || spawn.MaxY > rect.MinY);
    }
    struct SpawnCoords
    {
        public int X;
        public int Y;
    }

    void AttemptSpawn(IntRect rect, EnemySpawnSettings.EnemySpawn spawn)
    {
        List<SpawnCoords> validCoords = new List<SpawnCoords>();
        int minY = rect.MinY;
        if (spawn.HasMinY)
            minY = Mathf.Max(minY, spawn.MinY);
        int maxY = rect.MaxY;
        if (spawn.HasMaxY)
            maxY = Mathf.Min(maxY, spawn.MaxY + 1);
        for (int x = rect.MinX; x < rect.MaxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                if (x != rect.MinX
                    && x != rect.MaxX - 1
                    && y != minY
                    && y != maxY - 1)
                    continue;
                if (IsSpawnValid(x, y) == false)
                    continue;
                validCoords.Add(new SpawnCoords()
                {
                    X = x,
                    Y = y,
                });
            }
        }

        if (validCoords.Count > 0)
        {
            var coord = validCoords[Random.Range(0, validCoords.Count)];
            SpawnEnemy(spawn, coord);
        }
    }

    void SpawnEnemy(EnemySpawnSettings.EnemySpawn spawn, SpawnCoords coord)
    {
        Vector3 position = new Vector3(coord.X / GridSize, coord.Y / GridSize, LayerDepth);

        var prefab = spawn.Variations[Random.Range(0, spawn.Variations.Length)];

        GameObject obj;
        Queue<GameObject> pool;
        if (_EnemyPool.TryGetValue(prefab, out pool) && pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(prefab) as GameObject;
        }
        var transform = obj.transform;
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.position = position;
            rb.rotation = 0f;
        }
        else
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
        }
        _SpawnAccumulator -= spawn.SpawnValue;
    }

    bool IsSpawnValid(int x, int y)
    {
        for (int i = 0; i < _SpawnedEnemies.Count; i++)
        {
            var enemy = _SpawnedEnemies[i];
            Vector3 pos = enemy.transform.position;
            int enemyX = Mathf.RoundToInt(pos.x * GridSize);
            int enemyY = Mathf.RoundToInt(pos.x * GridSize);
            if (enemyX == x && enemyY == y)
                return false;
        }
        return true;
    }
}
