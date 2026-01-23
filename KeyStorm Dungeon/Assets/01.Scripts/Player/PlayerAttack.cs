using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public enum DirType
{
    LeftUp,
    Up,
    RightUp,
    Left,
    Right,
    LeftDown,
    Down,
    RightDown
}

public class PlayerAttack : MonoBehaviour
{
    Player player;
    bool isReloading;

    [SerializeField] string specialAttackKey;

    [SerializeField] bool canSniper;
    [SerializeField] bool canShotGun;
    [SerializeField] bool canThrough;
    [SerializeField] bool canBomb;

    [SerializeField] int shotGunBulletCount;
    [SerializeField] float shotSpreadAngle;

    [SerializeField] Vector2 projectileColliderOffset;    // 투사체 중심점
    [SerializeField] float projectileColliderRadius;      // 투사체 크기

    [SerializeField] AttackData normalAttack;
    [SerializeField] AttackData specialAttack;

    #region 폭탄
    [SerializeField] ThrownBomb bomb;
    bool isHoldingBomb;
    ThrownBomb curBomb;
    #endregion


    #region readOnly
    private readonly Vector2[] defaultUi =
        {new Vector2(-1.125f, 1.125f), new Vector2(0, 1.125f), new Vector2(1.125f, 1.125f), new Vector2(-1.125f, 0),
    new Vector2(1.125f, 0), new Vector2(-1.125f, -1.125f),new Vector2(0, -1.125f),new Vector2(1.125f, -1.125f)};
    private readonly Vector2[] sniperUi =
        {new Vector2(0, 2.125f), new Vector2(0, 1.125f), new Vector2(-2.125f, 0), new Vector2(-1.125f, 0),
    new Vector2(2.125f, 0), new Vector2(1.125f, 0),new Vector2(0, -1.125f),new Vector2(0, -2.125f)};
    private readonly string[] keyNames =
    { "Q", "W", "E", "R", "A", "S","D", "F" };

    private readonly Vector2[] defaultShotDir =
        {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1),
    new Vector2(-1, 1), new Vector2(0, -1),new Vector2(-1, -1),new Vector2(-1, 0)};

    private readonly Vector2[] sniperShotDir =
        {new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0),
    new Vector2(-1, 0), new Vector2(-1, 0),new Vector2(0, -1),new Vector2(0, -1)};

    private readonly DirType[] sniperDirType = { DirType.Up, DirType.Left, DirType.Right, DirType.Down };

    private readonly Color normalColor = Color.white;
    private readonly Color specialColor = Color.white;
    #endregion

    #region UI 관련
    [SerializeField] Image[] dirImages;
    [SerializeField] TextMeshProUGUI[] dirTexts;
    [SerializeField] Image[] coolImage;

    Dictionary<DirType, List<string>> dirTextDic = new Dictionary<DirType, List<string>>();

    Dictionary<DirType, List<TextMeshProUGUI>> sniperDirTmpDic = new Dictionary<DirType, List<TextMeshProUGUI>>();

    Dictionary<DirType, List<Image>> sniperDirImageDic = new Dictionary<DirType, List<Image>>();

    Dictionary<string, Coroutine> keyImageCoroutine = new Dictionary<string, Coroutine>();
    Coroutine reloadImageCoroutine;
    #endregion

    #region 키 입력 관련

    Dictionary<string, Vector2> keyDic = new Dictionary<string, Vector2>();

    Dictionary<string, bool> keyCoolDic = new Dictionary<string, bool>();

    Dictionary<string, int> sniperKeySlotIndex = new Dictionary<string, int>();

    Dictionary<string, Coroutine> keyCoroutine = new Dictionary<string, Coroutine>();
    Coroutine reloadCoroutine;
    #endregion

    #region Property
    public float Damage { get; private set; }
    public float DamageMultiple { get; private set; }
    public float SpecialDamageMultiple { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackSpeedMultiple { get; private set; }
    public float Range { get; private set; }
    public float RangeMultiple { get; private set; }
    public float ShootSpeed { get; private set; }
    public int MaxAmmo { get; private set; }
    public int UseAmmo { get; private set; }
    public int Ammo { get; private set; }
    public Dictionary<string, bool> KeyCoolDic { get => keyCoolDic; }
    public ThrownBomb Bomb { get => bomb; set => bomb = value; }

    public bool CanBomb { get => canBomb; }
    #endregion

    private void Start()
    {
        player = GetComponent<Player>();

        InitialDic();
        InitSniperDic();
        ShuffleKey();

        SetAttackUi();
    }


    public void InitPlayerAttack(PlayerRunData data)
    {
        Damage = data.character.damage;
        DamageMultiple = data.damageMultiple;
        SpecialDamageMultiple = data.specialDamageMultiple;
        AttackSpeedMultiple = data.attackSpeedMultiple;
        AttackSpeed = 1/(data.attackSpeed*AttackSpeedMultiple);
        Range = data.range;
        RangeMultiple = data.rangeMultiple;
        ShootSpeed = data.shootSpeed;
        MaxAmmo = data.maxAmmo;
        UseAmmo = data.useAmmo;
        Ammo = data.maxAmmo;

        canShotGun = data.attackRundata.isShotGun;
        canSniper = data.attackRundata.isSniper;
        canThrough = data.attackRundata.isThrough;
        canBomb = data.attackRundata.isBomb;
    }

    void InitialDic()
    {
        foreach (string key in keyNames)
        {
            keyCoroutine[key] = null;
            keyImageCoroutine[key] = null;
        }

        dirTextDic[DirType.Up] = new List<string>();
        dirTextDic[DirType.Down] = new List<string>();
        dirTextDic[DirType.Left] = new List<string>();
        dirTextDic[DirType.Right] = new List<string>();
        dirTextDic[DirType.LeftUp] = new List<string>();
        dirTextDic[DirType.RightUp] = new List<string>();
        dirTextDic[DirType.LeftDown] = new List<string>();
        dirTextDic[DirType.RightDown] = new List<string>();
    }

    void InitSniperDic()
    {
        sniperDirTmpDic.Clear();

        int index = 0;
        


        foreach (var type in sniperDirType)
        {
            sniperDirTmpDic[type] = new List<TextMeshProUGUI>()
            {
                dirTexts[index],
                dirTexts[index + 1]
            };
            sniperDirImageDic[type] = new List<Image>()
            {
                coolImage[index],
                coolImage[index + 1]
            };
            index += 2;
        }
    }

    void SetAttackUi()
    {
        Vector2[] uiVec = canSniper ? sniperUi : defaultUi;

        for (int i = 0; i < dirImages.Length; i++)
            dirImages[i].rectTransform.anchoredPosition = uiVec[i];
    }

    #region 플레이어 능력치 변화
    public void InCreaseDamage(int value) => Damage += value;
    public void InCreaseDamageMultiple(float value) => SpecialDamageMultiple += value;
    public void InCreaseSpecialDamageMultiple(float value) => SpecialDamageMultiple += value;
    public void InCreaseAttackSpeed(float value) => AttackSpeed += value;
    public void InCreaseAttackSpeedMultiple(float value) => AttackSpeedMultiple += value;
    public void InCreaseRange(float value) => Range += value;
    public void InCreaseRangeMultiple(float value) => RangeMultiple += value;
    public void InCreaseShootSpeed(float value) => ShootSpeed += value;
    public void InCreaseMaxAmmo(int value) => MaxAmmo += value;
    public void InCreaseUseAmmo(int value) => UseAmmo += value;
    public void InCreaseAmmo(int value) => Ammo += value;
    #endregion

    void InitialKeyCoolTime()
    {
        for(int i = 0; i < keyNames.Length; i++)
        {
            keyCoolDic[keyNames[i]] = false;
        }


    }

    void ShuffleKey()
    {
        keyDic.Clear();

        foreach(var list in dirTextDic.Keys.ToList())
        {
            dirTextDic[list].Clear();
        }
        sniperKeySlotIndex.Clear();
        Vector2[] veces = canSniper ? sniperShotDir : defaultShotDir;

        Vector2[] randVec = veces.OrderBy(x => Random.Range(0f, 1f)).ToArray();

        for(int i = 0; i < keyNames.Length; i++)
        {
            keyDic[keyNames[i]] = randVec[i];
            DirType type = SwitchType(keyDic[keyNames[i]]);
            dirTextDic[type].Add(keyNames[i]);
        }
        AttackDirUiUpdate();
        SetSpecialAttackKey();
        InitialKeyCoolTime();
    }

    DirType SwitchType(Vector2 vec)
    {
        if (vec == Vector2.up) return DirType.Up;
        else if (vec == Vector2.down) return DirType.Down;
        else if (vec == Vector2.right) return DirType.Right;
        else if (vec == Vector2.left) return DirType.Left;
        else if (vec == new Vector2(1, 1)) return DirType.RightUp;
        else if (vec == new Vector2(1, -1)) return DirType.RightDown;
        else if (vec == new Vector2(-1, 1)) return DirType.LeftUp;
        else if (vec == new Vector2(-1, -1)) return DirType.LeftDown;

        return default;
    }

    void AttackDirUiUpdate()
    {
        foreach(DirType list in dirTextDic.Keys.ToList())
        {
            string[] keys = dirTextDic[list].OrderBy(x => Random.Range(0f, 1f)).ToArray();

            int num = (int)list;

            if (keys == null || keys.Length == 0)
            {
                continue;
            }

            if(canSniper)
            {
                for(int i = 0; i < keys.Length; i++)
                {
                    sniperDirTmpDic[list][i].text = keys[i];
                    sniperKeySlotIndex[keys[i]] = i;
                }
            }
            else
            {
                dirTexts[(int)list].text = keys[0];
            }
        } 
    }

    public void Shoot(string keyName)
    {
        if (keyCoolDic[keyName] || isReloading) return;

        if (!keyDic.ContainsKey(keyName)) return;

        if (player == null || player.AttackPoolManager == null)
        {
            Debug.LogWarning("[PlayerAttack] AttackPoolManager가 없습니다!");
            return;
        }

        if (curBomb != null)
        {
            if (player.Inventory.bombCount <= 0 && curBomb == null)
                return;

            ThrowBomb(keyName);
        }
        else
        {
            UseAmmo = canShotGun ? shotGunBulletCount : DefaultBulletCount;
            SetBullet(keyName);
        }
        player.AudioManager.PlayEffect(ShootSfx);


        DirType type = SwitchType(keyDic[keyName]);

        KeyCool(keyName, type);

        
        if(Ammo <= 0)
        {
            Ammo = 0;
            TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
            hook?.ReportMagazineEmpty();
            Reload();
        }
    }

    void SetSpecialAttackKey()
    {
        if (keyNames == null || keyNames.Length <= 1) return;

        string prevKey = specialAttackKey;
        string resultKey = prevKey;

        while(prevKey == resultKey)
        {
            int a = Random.Range(0, keyNames.Length);
            resultKey = keyNames[a];
        }

        specialAttackKey = resultKey;
        UpdateSpecialAttackUI();
    }

    void UpdateSpecialAttackUI()
    {
        ResetKeyUI();

        if (string.IsNullOrEmpty(specialAttackKey)) return;

        DirType type = SwitchType(keyDic[specialAttackKey]);

        Image image;

        if (!canSniper)
        {
            image = dirTexts[(int)type].transform.parent.GetComponent<Image>();
            image.color = Color.yellow;
            return;
        }

        int slot = sniperKeySlotIndex[specialAttackKey];
        image = sniperDirTmpDic[type][slot].transform.parent.GetComponent<Image>();
        image.color = Color.yellow;
    }

    void ResetKeyUI()
    {
        if(!canSniper)
        {
            foreach (var txt in dirTexts)
            {
                Image image = txt.transform.parent.GetComponent<Image>();
                image.color = Color.white;
            }
            return;
        }

        foreach (var list in sniperDirTmpDic.Values)
            foreach (var txt in list)
            {
                Image image = txt.transform.parent.GetComponent<Image>();
                image.color = Color.white;
            }
    }

    void SetBullet(string keyName)
    {
        bool isSpecial = keyName == specialAttackKey;

        int totalUseAmmo = isSpecial ? UseAmmo * 2 : UseAmmo;

        int useAmmo = GetUseAmmo(totalUseAmmo, isSpecial);

        for (int i = 0; i < useAmmo; i++)
        {
            float normalDamage = Damage * DamageMultiple;
            float totalDamage = isSpecial ? (Damage * SpecialDamageMultiple) : Damage;
            totalDamage = player.PlayerEffectStat.GetDamage(totalDamage);
            Vector2 dir = keyDic[keyName].normalized;
            if(canShotGun)
            {
                float angle = Random.Range(-shotSpreadAngle,shotSpreadAngle);
                dir = Quaternion.Euler(0,0,angle) * dir; 
            }
            if(!canBomb)
            {
                AttackObj obj = player.AttackPoolManager.GetObj();
                //Sprite sprite = isSpecial ? player.SBullet : player.Bullet;
                AttackData attackData = isSpecial ? specialAttack : normalAttack;
                obj.transform.position = player.transform.position + (Vector3)dir * ShootOffset;

                obj.InitData(null, totalDamage, dir, ShootSpeed, Range, player.AttackPoolManager, true, projectileColliderOffset, projectileColliderRadius, attackData, canThrough);
            }
            else
            {
                ShootBomb(dir, totalDamage);
            }
        }

        int consumeAmmo = isSpecial ? useAmmo * SpecialBulletConsume : useAmmo;
        Ammo -= Mathf.Min(consumeAmmo, Ammo);

        if(isSpecial)
            SetSpecialAttackKey();
    }

    int GetUseAmmo(int total, bool isSpecial)
    {
        int useAmmo = Mathf.Min(total, Ammo);
        if (isSpecial)
        {
            if (total > Ammo)
            {
                useAmmo = (Ammo + 1) / 2;
            }
            else
                useAmmo = total / 2;
        }
        return useAmmo;
    }

    void KeyCool(string keyName, DirType type)
    {
        if (keyCoroutine[keyName] != null)
            StopCoroutine(keyCoroutine[keyName]);

        keyCoroutine[keyName] = StartCoroutine(StartKeyCool(keyName));
        if (canSniper)
        {
            int slot = sniperKeySlotIndex[keyName];
            keyImageCoroutine[keyName] = StartCoroutine(ImageFiil(type, slot));
        }
        else
        {
            keyImageCoroutine[keyName] = StartCoroutine(ImageFiil(type));
        }
    }

    IEnumerator StartKeyCool(string key)
    {
        keyCoolDic[key] = true;
        yield return new WaitForSeconds(AttackSpeed);
        keyCoolDic[key] = false;
    }

    IEnumerator ImageFiil(DirType type, int slot = 0)
    {
        Image image;
        if(canSniper)
            image = sniperDirImageDic[type][slot];
        else
            image = coolImage[(int)type];

        float timer = AttackSpeed;

        image.fillAmount = 1f;
        image.gameObject.SetActive(true);
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            float fill = timer / AttackSpeed;
            image.fillAmount = fill;

            yield return null;
        }

        image.fillAmount = 0f;
        image.gameObject.SetActive(false);
    }

    void Reload()
    {
        CoroutineStop();
        StartCoroutine(StartReload());
        StartCoroutine(ReloadImage());
    }

    void CoroutineStop()
    {
        foreach(string key in keyNames)
        {
            if (keyCoroutine.ContainsKey(key) && keyCoroutine[key] != null)
            {
                StopCoroutine(keyCoroutine[key]);
                keyCoroutine[key] = null;
            }
            if (keyImageCoroutine.ContainsKey(key) && keyImageCoroutine[key] != null)
            {
                StopCoroutine(keyImageCoroutine[key]);
                keyImageCoroutine[key] = null;
            }
        }
    }

    IEnumerator StartReload()
    {
        isReloading = true;
        ShuffleKey();
        yield return new WaitForSeconds(AttackSpeed);
        Ammo = MaxAmmo;
        isReloading = false;

        if (player != null && player.GameSceneUI != null)
            player.GameSceneUI.UpdateAmmo();
    }

    IEnumerator ReloadImage()
    {
        float timer = AttackSpeed;

        for(int i = 0; i<coolImage.Length; i++)
        {
            coolImage[i].fillAmount = 1f;
            coolImage[i].gameObject.SetActive(true);
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float fill = timer / AttackSpeed;

            for (int i = 0; i < coolImage.Length; i++)
            {
                coolImage[i].fillAmount = fill;
            }

            yield return null;
        }

        for (int i = 0; i < coolImage.Length; i++)
        {
            coolImage[i].fillAmount = 0f;
            coolImage[i].gameObject.SetActive(false);
        }
    }

    #region Bomb

    public void ActiveItemBomb()
    {
        if (curBomb != null) return;
        InstantiateBoob();
    }
    public void HoldBomb()
    {
        PlayerInventory inven = player.Inventory;
        if (curBomb != null || inven.bombCount <= 0) return;

        InstantiateBoob();
        inven.bombCount--;
        inven.runData.UpdateBomb(inven.bombCount);
    }

    void InstantiateBoob()
    {
        ThrownBomb bomb = Instantiate(Bomb);
        bomb.Hold(transform);
        curBomb = bomb;
        bomb.OnExplode += () => { ExplodingInPlayer(bomb); };
    }

    void ExplodingInPlayer(ThrownBomb bomb)
    {
        if (curBomb == bomb) curBomb = null;
    }

    void ThrowBomb(string keyName)
    {
        if (curBomb == null) return;

        Vector2 dir = keyDic[keyName];
        curBomb.transform.position = player.transform.position + (Vector3)dir * ShootOffset;
        curBomb.Throw(dir, ShootSpeed);

        curBomb = null;
    }

    void ShootBomb(Vector2 dir, float damage)
    {
        ThrownBomb bomb = Instantiate(Bomb);
        bomb.OnExplode += () => { ExplodingInPlayer(bomb); };
        bomb.transform.position = player.transform.position + (Vector3)dir * ShootOffset;
        bomb.InitData(damage, canThrough);
        bomb.Throw(dir, ShootSpeed);
    }
    #endregion

    #region StatUpdate
    public void SyncPlayerAttackStat(PlayerRunData data)
    {
        Damage = data.character.damage;
        SpecialDamageMultiple = data.specialDamageMultiple;
        DamageMultiple = data.damageMultiple;
        AttackSpeedMultiple = data.attackSpeedMultiple;
        AttackSpeed = player.PlayerEffectStat.GetAttackSpeed(1 / (data.attackSpeed*AttackSpeedMultiple));
        RangeMultiple = data.rangeMultiple;
        Range = player.PlayerEffectStat.GetRange(data.range * RangeMultiple);
        ShootSpeed = player.PlayerEffectStat.GetShotSpeed;
        MaxAmmo = data.maxAmmo;
        UseAmmo = data.useAmmo;
        if (MaxAmmo < Ammo)
            Ammo = MaxAmmo;
        else
            Ammo = data.ammo;
        if(data.attackRundata.isShotGun)
            canShotGun = data.attackRundata.isShotGun;

        if(data.attackRundata.isSniper)
        {
            canSniper = data.attackRundata.isSniper;
            ShuffleKey();
            SetAttackUi();
        }

        if(data.attackRundata.isThrough)
            canThrough = data.attackRundata.isThrough;

        if(data.attackRundata.isBomb)
            canBomb = data.attackRundata.isBomb;

    }
    #endregion
}
