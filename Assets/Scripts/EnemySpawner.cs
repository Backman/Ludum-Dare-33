using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
	public GameSpawnSettings GameSettings;
	public float LayerDepth = 30;
	public float GridSize = 1;


	float _SpawnAccumulator;
	List<Enemy> _SpawnedEnemies = new List<Enemy> ();

	Dictionary<GameObject, Queue<GameObject>> _EnemyPool = new Dictionary<GameObject, Queue<GameObject>> ();


    EnemySpawnSettings GetCurrentSpawnSettings(float time, out float periodT)
    {
        float t = 0f;
        EnemySpawnSettings settings = null;
        periodT = 0f;
        for (int i = 0; i < GameSettings.Periods.Length; i++)
        {
            var period = GameSettings.Periods[i];
            if (time + period.PeriodDuration >= time)
            {
                settings = period.SpawnSettings;
                periodT = Mathf.Repeat((time - t) / period.PeriodDuration, 1);
                break;
            }
            t += period.PeriodDuration;
        }
        if (settings == null)
        {
            settings = GameSettings.Periods[0].SpawnSettings;
            periodT = Mathf.Repeat((time - t) / GameSettings.Periods[0].PeriodDuration, 1);
        }
        return settings;
    }

	void Update ()
	{
		IntRect currentRect = CloudSpawner.GetCurrentRect (LayerDepth, GridSize);

        float periodT;
        var spawnSettings = GetCurrentSpawnSettings(Time.timeSinceLevelLoad, out periodT);

		_SpawnAccumulator += spawnSettings.SpawnSpeed * spawnSettings.IntensityCurve.Evaluate (periodT) * Time.deltaTime;
		if (_SpawnAccumulator > 0) {
			float totalSpawnChance = 0;
			for (int i = 0; i < spawnSettings.Spawns.Length; i++) {
				var spawn = spawnSettings.Spawns [i];
				if (IsValidSpawn (currentRect, spawn)) {
					totalSpawnChance += spawn.SpawnChance;
				}
			}

			float randomVal = Random.Range (0, totalSpawnChance);

			for (int i = 0; i < spawnSettings.Spawns.Length; i++) {
				var spawn = spawnSettings.Spawns [i];
				if (IsValidSpawn (currentRect, spawn)) {
					if (spawn.SpawnChance > randomVal) {
						AttemptSpawn (currentRect, spawn);
						break;
					}
					randomVal -= spawn.SpawnChance;
				}
			}

		}
	}

	bool IsValidSpawn (IntRect rect, EnemySpawnSettings.EnemySpawn spawn)
	{

		//if(spawn.MinLevel >= _CurrentLevel && spawn.MaxLevel <= _CurrentLevel)

		return (spawn.HasMinY == false || spawn.MinY <= rect.MaxY) && (spawn.HasMaxY == false || spawn.MaxY > rect.MinY);
	}

	struct SpawnCoords
	{
		public int X;
		public int Y;
	}

	void AttemptSpawn (IntRect rect, EnemySpawnSettings.EnemySpawn spawn)
	{
		List<SpawnCoords> validCoords = new List<SpawnCoords> ();
		int minY = rect.MinY;
		if (spawn.HasMinY)
			minY = Mathf.Max (minY, spawn.MinY);
		int maxY = rect.MaxY;
		if (spawn.HasMaxY)
			maxY = Mathf.Min (maxY, spawn.MaxY + 1);
		for (int x = rect.MinX; x < rect.MaxX; x++) {
			for (int y = minY; y < maxY; y++) {
				if (x != rect.MinX
				    && x != rect.MaxX - 1
				    && y != rect.MinY
				    && y != rect.MaxY - 1)
					continue;
				if (IsSpawnValid (spawn, x, y) == false)
					continue;
				validCoords.Add (new SpawnCoords () {
					X = x,
					Y = y,
				});
			}
		}

		if (validCoords.Count > 0) {
			var coord = validCoords [Random.Range (0, validCoords.Count)];
			SpawnEnemy (spawn, coord);
		}
	}

	void SpawnEnemy (EnemySpawnSettings.EnemySpawn spawn, SpawnCoords coord)
	{
		Vector3 position = new Vector3 (coord.X / GridSize, coord.Y / GridSize, LayerDepth);

		var prefab = spawn.Variations [Random.Range (0, spawn.Variations.Length)];

		GameObject obj;
		Queue<GameObject> pool;
		if (_EnemyPool.TryGetValue (prefab, out pool) && pool.Count > 0) {
			obj = pool.Dequeue ();
		} else {
			obj = GameObject.Instantiate (prefab) as GameObject;
		}
		var transform = obj.transform;
		var rb = obj.GetComponent<Rigidbody2D> ();
		if (rb != null) {
			rb.velocity = Vector3.zero;
			rb.angularVelocity = 0f;
			rb.position = position;
			rb.rotation = 0f;
		}
		if (transform != null) {
			transform.position = position;
			transform.rotation = Quaternion.identity;
		}
		var enemy = obj.GetComponent<Enemy> ();
		enemy.FromPrefab = prefab;
		var explodable = obj.GetComponent<Explodable> ();
		explodable.Spawner = this;
		var foskPos = Blokfosk.Instance.transform.position;

		var dir = (foskPos - position);
		dir.z = 0f;
		dir.y = 0f;

		dir.x = dir.x < 0f ? -1f : 1f;
		enemy.Direction = dir;

		var scale = enemy.transform.localScale;
		scale = Vector3.one * Random.Range (spawn.ScaleRange.x, spawn.ScaleRange.y);
		if (dir.x > 0) {
			scale.x = Mathf.Abs (scale.x);
		} else {
			scale.x = -Mathf.Abs (scale.x);
		}
		enemy.transform.localScale = scale;

		enemy.MovementSpeed = Random.Range (spawn.MovementSpeedRange.x, spawn.MovementSpeedRange.y);
		enemy.Reset ();

		_SpawnedEnemies.Add (enemy);
		_SpawnAccumulator -= spawn.SpawnValue;
	}

	public void ReturnSpawnValue (GameObject obj)
	{
        float t;
        var spawnSettings = GetCurrentSpawnSettings(Time.timeSinceLevelLoad, out t);
		for (int i = 0; i < spawnSettings.Spawns.Length; i++) {
			var spawn = spawnSettings.Spawns [i];
			for (int j = 0; j < spawn.Variations.Length; j++) {
				var variation = spawn.Variations [j];
				if (variation == obj) {
					_SpawnAccumulator += spawn.SpawnValue;
					break;
				}
			}
		}
	}

	public void Despawn (GameObject obj)
	{
		var enemy = obj.GetComponent<Enemy> ();

		_SpawnedEnemies.RemoveAll (x => x == enemy);
		Queue<GameObject> pool;
		if (_EnemyPool.TryGetValue (enemy.FromPrefab, out pool) == false)
			pool = _EnemyPool [enemy.FromPrefab] = new Queue<GameObject> ();
		pool.Enqueue (obj);
		obj.SetActive (false);
	}

	bool IsSpawnValid (EnemySpawnSettings.EnemySpawn spawn, int x, int y)
	{
		if ((spawn.HasMinY && spawn.MinY > y) || (spawn.HasMaxY && spawn.MaxY < y)) {
			return false;
		}
		/*for (int i = 0; i < _SpawnedEnemies.Count; i++) {
			var enemy = _SpawnedEnemies [i];
			Vector3 pos = enemy.transform.position;
			int enemyX = Mathf.RoundToInt (pos.x * GridSize);
			int enemyY = Mathf.RoundToInt (pos.x * GridSize);
			if (enemyX == x && enemyY == y)
				return false;
		}*/
		return true;
	}
}
