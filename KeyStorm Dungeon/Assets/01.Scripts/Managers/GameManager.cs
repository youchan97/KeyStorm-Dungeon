using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstValue;
public class GameManager : SingletonManager<GameManager>
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ExitGameWeb();
#endif
    [SerializeField] bool isCheatMode;

    SaveLoadManager saveLoadManager;
    StageDataManager stageDataManager;
    AudioManager audioManager;

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

    private Room currentRoom;
    public Room CurrentRoom => currentRoom;

    [SerializeField] private RoomChangeEvent roomChangeEvent;

    protected override void Awake()
    {
        base.Awake();
#if !UNITY_EDITOR
        isCheatMode = false;
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
        audioManager = AudioManager.Instance;
    }

    public void GameStart()
    {
        isStart = true;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        audioManager.AllStopSfxLoop();
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
#elif UNITY_WEBGL
        ExitGameWeb();
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
        audioManager.AllStopSfxLoop();
        LoadingManager.LoadScene(StartScene);
    }

    public void RetryGame()
    {
        InitializeRunData();
        isPaused = false;
        isGameCleared = false;
        currentStage = 1; 


        stageDataManager.SelectDifficulty(stageDataManager.CurrentDifficulty);
        audioManager.AllStopSfxLoop();
        LoadingManager.LoadScene(GameScene);
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
        audioManager.AllStopSfxLoop();
        LoadingManager.LoadScene(GameScene);
    }

    public void InitCurrentRoom(Room room)
    {
        if (currentRoom != room)
        {
            currentRoom = room;
            roomChangeEvent?.RoomChange(currentRoom);
        }
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

    public void ApplyItemStat(ItemData itemData, PlayerLimitData limitData)
    {
        character.ApplyItemStat(itemData, limitData);
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

    public void ApplyItemStat(ItemData itemData, PlayerLimitData limitData)
    {
        maxHp = Mathf.Clamp(maxHp + itemData.maxHp,0, limitData.maxHp);
        if (itemData.maxHp > 0)
        {
            currentHp += itemData.maxHp;   
        }
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
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

[System.Serializable]
public class PlayerEffectStat
{
    PlayerRunData runData;
    PlayerLimitData limitData;
    public PlayerEffectStat(PlayerRunData runData, PlayerLimitData limitData)
    {
        this.runData = runData;
        this.limitData = limitData;
    }

    public float GetMoveSpeed => Mathf.Clamp(runData.character.moveSpeed, limitData.minMoveSpeed, limitData.maxMoveSpeed);

    public float GetDamage(float total) => Mathf.Max(total, limitData.minDamage);

    public float GetAttackSpeed(float total) => Mathf.Max(total, limitData.minAttackSpeed);
    public float GetRange(float total) => Mathf.Max(total, limitData.minRange);
    public float GetShotSpeed => Mathf.Max(runData.shootSpeed, limitData.minShotSpeed);
    public float GetScaleX => Mathf.Max(runData.xScale, limitData.minScale);
    public float GetScaleY => Mathf.Max(runData.yScale, limitData.minScale);

}

