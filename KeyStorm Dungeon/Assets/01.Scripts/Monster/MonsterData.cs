using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Monster", menuName = "NewMonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 정보")]
    public CharacterData characterData;
    public float detectRange;   // 플레이어 탐지 거리
    public float attackRange;   // 공격 사거리
    public int tier;            // 몬스터의 등급
    public float targetDistance;// 플레이어와의 거리
    public float shotSpeed;     // 투사체 속도
    public float attackSpeed;   // 공격과 공격 사이의 속도

    [Header("골드 드랍 설정")]
    public int dropGold;

    [Header("몬스터 등장 시기")]
    public int minStage;
    public int maxStage;

    [Header("스프라이트 크기")]
    public float xScale;
    public float yScale;
}
