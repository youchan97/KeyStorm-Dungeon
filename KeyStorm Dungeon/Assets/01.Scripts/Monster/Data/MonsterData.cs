using UnityEngine;

public enum MonsterTier
{
    Normal,
    Rare,
    Epic,
    Boss,
}

[System.Serializable]
[CreateAssetMenu(fileName = "Monster", menuName = "NewMonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 정보")]
    public CharacterData characterData;
    public UnitType type;       // 지상, 공중 몬스터 타입
    public float detectRange;   // 플레이어 탐지 거리
    public float targetDistance;// 공격 사거리
    public MonsterTier tier;    // 몬스터의 등급
    public float shotSpeed;     // 투사체 속도
    public float attackSpeed;   // 공격과 공격 사이의 속도 (공격 쿨타임)
    public Vector2 projectileColliderOffset;    // 투사체 중심점
    public float projectileColliderRadius;      // 투사체 크기

    /*[Header("스티어링 비헤이비어 설정")]
    public float separationRaduis;  // 자신 이외 다른 몬스터 감지 반경(분리)
    public float separationStrength;// 분리 힘의 강도
    public float pathfindingWeight; // A* 경로 힘의 가중치
    public float separationWeight;  // 분리 힘의 가중치
    public float maxSteeringForce;  // 몬스터 최대 스티어링 힘
    public float mass;              // 몬스터 질량*/

    [Header("골드 드랍 설정")]
    public int minDropGold;
    public int maxDropGold;

    [Header("몬스터 등장 시기")]
    public int minStage;
    public int maxStage;

    [Header("스프라이트 크기")]
    public float xScale;
    public float yScale;
}
