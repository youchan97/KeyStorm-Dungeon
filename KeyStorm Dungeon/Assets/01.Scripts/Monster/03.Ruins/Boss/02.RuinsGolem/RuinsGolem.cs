using System;
using UnityEngine;

public class RuinsGolem : MeleeMonster
{
    [Header("골렘의 핵")]
    [SerializeField] private GolemsCore golemsCore;

    [Header("골렘 공통 수치")]
    [SerializeField] private float idleTime;
    [SerializeField] private float attackDelay;

    [Header("바닥치기 패턴 수치")]
    [SerializeField] private GameObject slamEffect;
    [SerializeField] private float[] slamInnerRadius;
    [SerializeField] private float[] slamOuterRadius;
    [SerializeField] private float slamDelay;   // 각 바닥치기 사이 딜레이

    [Header("낙석 패턴 수치")]
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private float allRockFallDuration;
    [SerializeField] private float singleRockFallDuration;
    [SerializeField] private float shadowDuration;
    [SerializeField] private float minShadowScale;
    [SerializeField] private float maxShadowScale;
    [SerializeField] private float rockSpawnHeight;
    [SerializeField] private float minRockSpawnTime;
    [SerializeField] private float maxRockSpawnTime;
    [SerializeField] private float rockFallShakeCameraPower;
    [SerializeField] private float rockFallShakeCameraDuration;
    [SerializeField] private float damageRadius;

    public float IdleTime => idleTime;
    public float AttackDelay => attackDelay;
    public GameObject SlamEffect => slamEffect;
    public float[] SlamInnerRadius => slamInnerRadius;
    public float[] SlamOuterRadius => slamOuterRadius;
    public float SlamDelay => slamDelay;
    public GameObject ShadowPrefab => shadowPrefab;
    public GameObject RockPrefab => rockPrefab;
    public float AllRockFallDuration => allRockFallDuration;
    public float SingleRockFallDuration => singleRockFallDuration;
    public float ShadowDuration => shadowDuration;
    public float MinShadowScale => minShadowScale;
    public float MaxShadowScale => maxShadowScale;
    public float RockSpawnHeight => rockSpawnHeight;
    public float MinRockSpawnTime => minRockSpawnTime;
    public float MaxRockSpawnTime => maxRockSpawnTime;
    public float RockFallShakeCameraPower => rockFallShakeCameraPower;
    public float RockFallShakeCameraDuration => rockFallShakeCameraDuration;
    public float DamageRadius => damageRadius;

    public event Action OnSlamAnimation;
    public event Action OnRockFallAnimation;

    private RuinsGolemIdleState _idleState;
    private RuinsGolemAttackState _attackState;
    private RuinsGolemDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new RuinsGolemIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        return null;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new RuinsGolemAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new RuinsGolemDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
        golemsCore.OnMonsterDied += Die;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        golemsCore.OnMonsterDied -= Die;
    }

    protected override void Start()
    {
        base.Start();

        golemsCore.SetMyRoom(MyRoom);
        MyRoom.AddMonster(golemsCore);
    }
    public override void TakeDamage(float damage)
    {
        return;
    }

    public Vector2 GetRandomTilemapInRoom()
    {
        if (MyRoom != null)
        {
            return MyRoom.GetRandomWalkableTilemap();
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;
        if (slamOuterRadius == null || slamInnerRadius == null)
            return;
        if (slamOuterRadius.Length == 0 || slamInnerRadius.Length == 0)
            return;
        Vector3 attackCenter = transform.position;

        Color[] outerColors = { Color.red, Color.yellow, Color.green };
        Color[] innerColors = { Color.magenta, Color.cyan, Color.blue };

        int numSlamStages = Mathf.Min(slamOuterRadius.Length, slamInnerRadius.Length, 3); // 최대 3단계만 고려

        for (int i = 0; i < numSlamStages; i++)
        {
            float outerRadius = slamOuterRadius[i];
            float innerRadius = slamInnerRadius[i];

            Gizmos.color = outerColors[i];
            Gizmos.DrawWireSphere(attackCenter, outerRadius);

            Gizmos.color = innerColors[i];
            Gizmos.DrawWireSphere(attackCenter, innerRadius);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(attackCenter, 0.1f);
    }

    #region 애니메이션이 끝남을 알리는 메서드
    public void OnSlam()
    {
        OnSlamAnimation?.Invoke();
    }

    public void OnRockFall()
    {
        OnRockFallAnimation?.Invoke();
    }
    #endregion
}
