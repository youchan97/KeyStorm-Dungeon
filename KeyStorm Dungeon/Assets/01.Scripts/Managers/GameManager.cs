using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstValue;
public class GameManager : SingletonManager<GameManager>
{
    bool isCheatMode;

    SaveLoadManager saveLoadManager;
    StageDataManager stageDataManager;

    private bool isStart;
    public bool isPaused;

    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerRunData playerRunData;

    public PlayerRunData PlayerRunData { get => playerRunData; }
    public bool IsCheatMode { get => isCheatMode; }

    private int currentStage = 1;
    public int CurrentStage => currentStage;

    private bool isGameCleared = false;
    public bool IsGameCleared => isGameCleared; // ⭐ 추가

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        isCheatMode = true;
#endif
        InitializeRunData();
    }

    private void Start()
    {
        InitManager();
    }

    void InitializeRunData() => playerRunData = new PlayerRunData(playerData);

    void InitManager()
    {
        saveLoadManager = SaveLoadManager.Instance;
        stageDataManager = StageDataManager.Instance;
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
        InitializeRunData();
    }

    public void GameClear()
    {
        isGameCleared = true;
    }

    public void ExitGame()
    {
        saveLoadManager.SaveDatas();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoHome()
    {
        if (Time.timeScale < 1f)
            Time.timeScale = 1f;
        isPaused = false;
        isGameCleared = false; 
        currentStage = 1;
        InitializeRunData();
        SceneManager.LoadScene(StartScene);
    }

    public void RetryGame()
    {
        InitializeRunData();
        isPaused = false;
        isGameCleared = false;
        currentStage = 1; 


        stageDataManager.SelectDifficulty(stageDataManager.CurrentDifficulty);
        SceneManager.LoadScene(GameScene);
    }

    public void StageClear()
    {
        // ⭐ 이미 게임 클리어되었으면 무시
        if (isGameCleared)
        {
            return;
        }


        currentStage++;
        stageDataManager.NextStage();
        SceneManager.LoadScene(GameScene);
    }
}

[System.Serializable]
public class PlayerRunData
{
    public CharacterRunData character;
    public InventoryRunData inventory;
    public PlayerAttackRundata attackRundata;
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
        inventory = new InventoryRunData();
        attackRundata = new PlayerAttackRundata();
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
        rangeMultiple += itemData.rangeMultiple;
        shootSpeed += itemData.shotSpeed;
        maxAmmo += itemData.maxAmmo;
        useAmmo += itemData.useAmmo;
        xScale += itemData.scale;
        yScale += itemData.scale;

        if(itemData.attackChange)
        {
            switch(itemData.attackChangeType)
            {
                case AttackChangeType.ShotGun:
                    attackRundata.UpdateShotGun();
                    break;
                case AttackChangeType.Sniper:
                    attackRundata.UpdateSniper();
                    break;
                default:
                    break;
            }
        }
    }
}

[System.Serializable]
public class PlayerAttackRundata
{
    public bool isShotGun;
    public bool isSniper;

    public void UpdateShotGun() => isShotGun = true;
    public void UpdateSniper() => isSniper = true;

}

[System.Serializable]
public class CharacterRunData
{
    public string charName;
    public int id;
    public float maxHp;
    public float currentHp;
    public float damage;
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
        damage += itemData.damage;
        moveSpeed += itemData.moveSpeed;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if(currentHp >= maxHp)
        {
            currentHp = maxHp;
        }
    }

}
[System.Serializable]
public class InventoryRunData
{
    public int gold;
    public int bombCount;
    public List<ItemData> passiveItems = new();
    public ItemData activeItem;

    public void UpdateGold(int gold) => this.gold = gold;

    public void UpdateBomb(int count) => bombCount = count;

    public void ApplyInventory(ItemData itemData)
    {
        if (itemData.isActiveItem)
            activeItem = itemData;
        else
            passiveItems.Add(itemData);
    }
}

