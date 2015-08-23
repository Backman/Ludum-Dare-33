﻿using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public EnemySpawnSettings Settings;
    public float LayerDepth = 30;
    public float GridSize = 1;


    float _SpawnAccumulator;
    List<Enemy> _SpawnedEnemies = new List<Enemy>();

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
                if (IsSpawnValid(spawn, x, y) == false)
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
        if (transform != null)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
        }
        var enemy = obj.GetComponent<Enemy>();
        enemy.FromPrefab = prefab;
        var explodable = obj.GetComponent<Explodable>();
        explodable.Spawner = this;
        var foskPos = Blokfosk.Instance.transform.position;

        var dir = (foskPos - position);
        dir.z = 0f;
        dir.y = 0f;
        enemy.Direction = dir.normalized;
        var scale = enemy.transform.localScale;
        if (dir.x > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        enemy.transform.localScale = scale;

        enemy.MovementSpeed = Random.Range(spawn.MovementSpeedRange.x, spawn.MovementSpeedRange.y);

        _SpawnedEnemies.Add(enemy);
        _SpawnAccumulator -= spawn.SpawnValue;
    }

    public void Despawn(GameObject obj)
    {
        var enemy = obj.GetComponent<Enemy>();

        _SpawnedEnemies.RemoveAll(x => x == enemy);
        Queue<GameObject> pool;
        if (_EnemyPool.TryGetValue(enemy.FromPrefab, out pool) == false)
            pool = _EnemyPool[enemy.FromPrefab] = new Queue<GameObject>();
        pool.Enqueue(obj);
        obj.SetActive(false);
    }

    bool IsSpawnValid(EnemySpawnSettings.EnemySpawn spawn, int x, int y)
    {
        if ((spawn.HasMinY && spawn.MinY < y) || (spawn.HasMaxY && spawn.MaxY > spawn.MaxY))
        {
            return false;
        }
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