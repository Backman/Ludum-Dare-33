using UnityEngine;
using System.Collections;

public class EnemyBoat : Enemy
{
    public override EnemyType Type { get { return EnemyType.Boat; } }

    public GameObject MinePrefab;
    public Vector2 MineFireInterval;
    float _MineAttackInterval;

    protected override void Update()
    {
        base.Update();
        if (IsHit && transform.position.y < -0.1f)
        {
            Explode(true);
        }
    }

    protected override void CheckOtherCollisions(Collider2D collider)
    {
        var flying = collider.GetComponent<EnemyFlying>();
        if (flying && flying.IsHit)
        {
            GameLogic.Instance.OnRekFace.Invoke(gameObject);
            var dir = flying.transform.position - transform.position;
            Hit(dir);
            Explode(true);
        }
    }
    protected override void TryToFireProjectile()
    {
        var blokfoskPos = Blokfosk.Instance.transform.position;
        if (blokfoskPos.y > 0)
        {
            base.TryToFireProjectile();
        }
        else 
        {
            if (Mathf.Abs(transform.position.x - blokfoskPos.x) < 10f &&  _lastFire + _MineAttackInterval < Time.time)
            {
                _lastFire = Time.time;
                _MineAttackInterval = Random.Range(MineFireInterval.x, MineFireInterval.y);
                FireMine();
            }
        }
    }

    void WaterSurfaceEnter(object obj)
    {
        WaterSurface surface = obj as WaterSurface;
        if (IsHit)
        {
            Explode(true);
            surface.DoSplash(gameObject, transform.position);
        }
    }

    protected override void TentacleHit(Vector2 dir)
    {
        if (!IsHit)
        {
            Explode(false);
        }

        Hit(dir);
    }

    protected void FireMine()
    {
        var go = TrashMan.spawn(MinePrefab, transform.position + Vector3.down * 0.1f);
        go.GetComponent<Projectile>().Direction = Vector2.down;
    }
}

