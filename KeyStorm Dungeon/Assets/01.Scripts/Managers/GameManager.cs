using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstValue;
public class GameManager : SingletonManager<GameManager>
{
    private bool isStart;
    private bool isPaused;

    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerRunData playerRunData;

    public PlayerRunData PlayerRunData { get => playerRunData;}

    protected override void Awake()
    {
        base.Awake();
        //스타트가 아니라 캐릭터 커스텀마이징 선택 후 게임 시작 때 불러와야함
        playerRunData = new PlayerRunData(playerData); 
    }

    public void GameStart()
    {
        isStart = true;
        isPaused = false;
        Time.timeScale = 1f;

    }
    public void Pause()
    {
        isStart = false;
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    public void GameOver()
    {
        isStart = false;
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void StageClear()
    {
        StageDataManager.Instance.NextStage();
        LoadingManager.LoadScene(GameScene);
    }
}

[System.Serializable]
public class PlayerRunData
{
    public CharacterRunData character;
    public float damageMultiple;
    public float specialDamageMultiple;
    public float attackSpeed;
    public float attackSpeedMultiple;
    public float range;
    public float rangeMultiple;
    public float shootSpeed;
    public int maxAmmo;
    public int useAmmo;
    public int ammo;
    public float xScale;
    public float yScale;
    

    public PlayerRunData(PlayerData playerData)
    {
        character = new CharacterRunData(playerData.characterData);
        damageMultiple = playerData.damageMultiple;
        specialDamageMultiple = playerData.specialDamageMultiple;
        attackSpeed = playerData.attackSpeed;
        attackSpeedMultiple = playerData.attackSpeedMultiple;
        range = playerData.range;
        rangeMultiple = playerData.rangeMultiple;
        shootSpeed = playerData.shootSpeed;
        maxAmmo = playerData.maxAmmo;
        useAmmo = playerData.useAmmo;
        ammo = playerData.maxAmmo;
        xScale = playerData.xScale;
        yScale = playerData.yScale;
    }

    public void ApplyItemStat(ItemData itemData)
    {
        character.ApplyItemStat(itemData);
        damageMultiple += itemData.damageMultiple;
        specialDamageMultiple += itemData.specialDamageMultiple;
        attackSpeed += itemData.attackSpeed;
        attackSpeedMultiple += itemData.attackSpeedMultiple;
        range += itemData.range;
        shootSpeed += itemData.shotSpeed;
        maxAmmo += itemData.maxAmmo;
        useAmmo += itemData.useAmmo;
        xScale = itemData.scale;
        yScale = itemData.scale;
    }
}

[System.Serializable]
public class CharacterRunData
{
    public string charName;
    public int id;
    public int maxHp;
    public int currentHp;
    public int damage;
    public float moveSpeed;

    public CharacterRunData(CharacterData characterData)
    {
        charName = characterData.charName;
        id = characterData.id;
        maxHp = characterData.maxHp;
        currentHp = maxHp;
        damage = characterData.damage;
        moveSpeed = characterData.moveSpeed;
    }

    public void ApplyItemStat(ItemData itemData)
    {
        maxHp += itemData.maxHp;
        if (itemData.maxHp > 0)
        {
            currentHp += itemData.maxHp;
            currentHp = Mathf.Min(currentHp, maxHp);
        }
        damage += (int)itemData.damage;
        moveSpeed += itemData.moveSpeed;
    }
}

