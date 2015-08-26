using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Flying,
        Boat,
        Submarine
    }

    [System.NonSerialized]
    public GameObject FromPrefab;
    public GameObject ProjectilePrefab;
    public float MovementSpeed = 2f;
    public int Score = 5;

    public SoundSourceType SoundSource;

    public GameObject Explosion;

    public Vector2 AttackIntervall;

    public AudioClip OnHitClip;

    public Blokfosk Blokfosk { get; set; }

    public Vector2 Direction { get; set; }

    public bool IsHit { get; set; }

    public abstract EnemyType Type { get; }

    protected float _attackTimer;
    protected float _lastFire;

    protected Rigidbody2D _rb;

    private float _LastVisibleTime = 0f;

    Renderer[] _Renderers;

    protected virtual void Awake()
    {
        _Renderers = GetComponentsInChildren<Renderer>();
        _rb = GetComponent<Rigidbody2D>();
        Direction = transform.right;
        Blokfosk = Blokfosk.Instance;
    }

    protected virtual void Update()
    {
        if (IsHit)
        {
            return;
        }

        TryToFireProjectile();
    }


    void OnEnable()
    {
        _LastVisibleTime = Time.time;
    }

    protected virtual void TryToFireProjectile()
    {
        if (_lastFire + _attackTimer < Time.time)
        {
            BasicFire();
            _lastFire = Time.time;
            RandomizeAttackTimer();
        }
    }

    protected virtual void FixedUpdate()
    {
        _rb.position += Direction * MovementSpeed * Time.deltaTime;
        bool anythingVisible = false;
        for (int i = 0; i < _Renderers.Length; i++)
        {
            if (_Renderers[i].isVisible)
            {
                anythingVisible = true;
                break;
            }
        }

        if (anythingVisible)
        {
            _LastVisibleTime = Time.time;
        }
        else
        {
            Vector3 pos = transform.position;
            Vector2 centerPos;
            Vector2 frustumSize = CloudSpawner.GetFrustumSize(pos.z, out centerPos);

            float distance = Vector2.Distance(pos, Blokfosk.Instance.transform.position);
            if (_LastVisibleTime + 5f < Time.time || distance > frustumSize.x * 2.5f)
            {
                var explode = GetComponent<Explodable>();
                if (explode)
                {
                    explode.Spawner.ReturnSpawnValue(FromPrefab);
                    explode.Spawner.Despawn(gameObject);
                }
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Blokfosk_Tentacle")
        {
            if (!IsHit)
            {
                if (OnHitClip)
                {
                    Music.PlayClipAtPoint(OnHitClip, transform.position, Music.instance.sfxv, 1f, SoundSource);
                }
                GameLogic.Instance.AddScore(Score);
            }

            var tentaclePos = new Vector2(collider.transform.position.x, collider.transform.position.y);
            TentacleHit(_rb.position - tentaclePos);

            GameLogic.Instance.OnRekFace.Invoke(gameObject);
            IsHit = true;
        }

        CheckOtherCollisions(collider);
    }

    protected virtual void Hit(Vector2 dir)
    {
        IsHit = true;
        _rb.AddForce(dir.normalized * 5f, ForceMode2D.Impulse);
        _rb.AddTorque(360f, ForceMode2D.Impulse);
        _rb.gravityScale = 1f;
    }

    protected virtual void TentacleHit(Vector2 dir)
    {
        Explode(true);
    }

    protected virtual void CheckOtherCollisions(Collider2D collider)
    {

    }

    public void Explode(bool shouldDestroy)
    {
        var explode = GetComponent<Explodable>();
        if (explode)
        {
            GameLogic.Instance.OnEnemyKilled.Invoke();
            explode.Explode(transform.position, shouldDestroy);
        }
    }

    protected void BasicFire()
    {
        var dir = Blokfosk == null ? transform.right : Blokfosk.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rot = Quaternion.AngleAxis(angle, Vector3.forward);

        var go = TrashMan.spawn(ProjectilePrefab, transform.position, rot);
        var projectile = go.GetComponent<Projectile>();


        projectile.Direction = dir.normalized;
    }

    private void RandomizeAttackTimer()
    {
        _attackTimer = Random.Range(AttackIntervall.x, AttackIntervall.y);
    }

    public virtual void Reset()
    {
        IsHit = false;
    }
}
