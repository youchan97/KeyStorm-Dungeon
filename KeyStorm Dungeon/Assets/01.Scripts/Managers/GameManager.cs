using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstValue;
public class GameManager : SingletonManager<GameManager>
{
    public bool isCheatMode;

    SaveLoadManager saveLoadManager;
    StageDataManager stageDataManager;

    private bool isStart;
    public bool isPaused;

    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerRunData playerRunData;

    public PlayerRunData PlayerRunData { get => playerRunData;}
    protected override void Awake()
    {
        base.Awake();
        //스타트가 아니라 캐릭터 커스텀마이징 선택 후 게임 시작 때 불러와야함
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
        InitializeRunData();
        LoadingManager.LoadScene(StartScene);
    }

    public void RetryGame()
    {
        InitializeRunData();
        isPaused = false;
        stageDataManager.SelectDifficulty(stageDataManager.CurrentDifficulty);
        LoadingManager.LoadScene(GameScene);
    }

    public void StageClear()
    {
        stageDataManager.NextStage();
        LoadingManager.LoadScene(GameScene);
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

