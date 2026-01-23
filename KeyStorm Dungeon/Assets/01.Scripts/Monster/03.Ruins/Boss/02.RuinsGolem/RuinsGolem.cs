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
        // 게임 플레이 중에는 Gizmo를 그리지 않거나, 씬 뷰에 방해되지 않도록 처리
        if (Application.isPlaying)
            return;

        // 필요한 데이터가 없는 경우 그리지 않음
        if (slamOuterRadius == null || slamInnerRadius == null)
            return;
        if (slamOuterRadius.Length == 0 || slamInnerRadius.Length == 0)
            return;

        // 공격의 중심점 계산 (골렘 위치 + 오프셋)
        Vector3 attackCenter = transform.position;

        // 도넛 공격은 여러 단계로 이루어지므로, 각 단계별로 다른 색상을 사용하여 구분하기 용이하게 합니다.
        Color[] outerColors = { Color.red, Color.yellow, Color.green }; // 1, 2, 3번째 공격의 외부 원 색상
        Color[] innerColors = { Color.magenta, Color.cyan, Color.blue }; // 1, 2, 3번째 공격의 내부 원 색상

        int numSlamStages = Mathf.Min(slamOuterRadius.Length, slamInnerRadius.Length, 3); // 최대 3단계만 고려

        for (int i = 0; i < numSlamStages; i++)
        {
            float outerRadius = slamOuterRadius[i];
            float innerRadius = slamInnerRadius[i];

            // 외부 원 그리기
            Gizmos.color = outerColors[i]; // 각 단계별 색상 적용
            Gizmos.DrawWireSphere(attackCenter, outerRadius);

            // 내부 원 그리기
            Gizmos.color = innerColors[i]; // 각 단계별 색상 적용
            Gizmos.DrawWireSphere(attackCenter, innerRadius);
        }

        // 공격 중심점을 나타내는 작은 구 그리기
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
