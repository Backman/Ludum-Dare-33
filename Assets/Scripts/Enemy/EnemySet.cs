using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemySet : ScriptableObject
{
	public EnemyFlying[] AirEnemies;
	public EnemyBoat[] ShipEnemies;
	public EnemySubmarine[] SubmarineEnemies;
}
