using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterStateManager characterStateManager;


    #region Property
    public string CharName {  get; protected set; }
    public int Id { get; protected set; }
    public int MaxHp { get; protected set; }
    public int Hp { get; protected set; }
    public int Damage { get; protected set; }
    public float MoveSpeed { get; protected set; }
    #endregion

    private void Awake()
    {
        characterStateManager = new CharacterStateManager(this);
    }


    private void Update()
    {
        characterStateManager?.Update();
    }

    private void FixedUpdate()
    {
        characterStateManager?.FixedUpdate();
    }
    /// <summary>
    /// 캐릭터 공통 정보 초기화
    /// </summary>
    /// <param name="data"></param>
    protected void InitCharData(CharacterData data)
    {
        CharName = data.charName;
        Id = data.id;
        MaxHp = data.maxHp;
        Hp = data.maxHp;
        Damage = data.damage;
        MoveSpeed = data.moveSpeed;
    }

    /// <summary>
    /// 공격
    /// </summary>
    /// <param name="character"></param>
    public virtual void Attack(Character character)
    {
        if (character == null) return;

        character.TakeDamage(Damage);
    }
    /// <summary>
    /// 피격
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        Hp -= damage;
        if(Hp <= 0)
        {
            Hp = 0;
            Die();
        }
    }
    /// <summary>
    /// 죽음(아마 스테이트 머신 연결하시면 될 거에요)
    /// </summary>
    public virtual void Die() { }
}
