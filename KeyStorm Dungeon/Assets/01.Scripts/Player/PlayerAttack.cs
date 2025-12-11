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

    [SerializeField] bool canSniper;
    bool canShotGun;

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
    public int Damage { get; private set; }
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
    #endregion

    private void Start()
    {
        player = GetComponent<Player>();

        InitPlayerAttack(player, player.Data);

        InitialDic();
        InitSniperDic();
        ShuffleKey();

        SetAttackUi();
    }


    void InitPlayerAttack(Player player, PlayerData data)
    {
        Damage = player.Damage;
        DamageMultiple = data.damageMultiple;
        SpecialDamageMultiple = data.specialDamageMultiple;
        AttackSpeed = data.attackSpeed;
        AttackSpeedMultiple = data.attackSpeedMultiple;
        Range = data.range;
        RangeMultiple = data.rangeMultiple;
        ShootSpeed = data.shootSpeed;
        MaxAmmo = data.maxAmmo;
        UseAmmo = data.useAmmo;
        Ammo = data.ammo;

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
        /*DirType type = SwitchType(vec);
        switch(type)
        {
            case DirType.LeftUp:
                dirTexts[((int)DirType.LeftUp)].text = text;
                break;
            case DirType.Up:
                dirTexts[((int)DirType.Up)].text = text;
                break;
            case DirType.RightUp:
                dirTexts[((int)DirType.RightUp)].text = text;
                break;
            case DirType.Left:
                dirTexts[((int)DirType.Left)].text = text;
                break;
            case DirType.Right:
                dirTexts[((int)DirType.Right)].text = text;
                break;
            case DirType.LeftDown:
                dirTexts[((int)DirType.LeftDown)].text = text;
                break;
            case DirType.Down:
                dirTexts[((int)DirType.Down)].text = text;
                break;
            case DirType.RightDown:
                dirTexts[((int)DirType.RightDown)].text = text;
                break;
        }*/

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

    public void Shoot(string keyName, bool isSpecial = false)
    {
        if (keyCoolDic[keyName] || isReloading) return;

        if (!keyDic.ContainsKey(keyName)) return;

        AttackObj obj = player.AttackPoolManager.GetAttack();

        Sprite sprite = isSpecial ? player.SBullet : player.Bullet;
        int damage = isSpecial ? (int)(Damage * SpecialDamageMultiple) : Damage;
        Vector2 dir = keyDic[keyName].normalized;
        obj.transform.position = player.transform.position + (Vector3)dir * ShootOffset;

        obj.InitData(sprite, damage, dir, ShootSpeed, Range, player.AttackPoolManager);

        Debug.Log(keyName + dir);

        DirType type = SwitchType(keyDic[keyName]);

        KeyCool(keyName, type);

        Ammo--;
        if(Ammo == 0)
        {
            Reload();
        }
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
        yield return new WaitForSeconds(ShootSpeed);
        keyCoolDic[key] = false;
    }

    IEnumerator ImageFiil(DirType type, int slot = 0)
    {
        Image image;
        if(canSniper)
            image = sniperDirImageDic[type][slot];
        else
            image = coolImage[(int)type];
        float timer = ShootSpeed;

        image.fillAmount = 1f;
        image.gameObject.SetActive(true);
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            float fill = timer / ShootSpeed;
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
        yield return new WaitForSeconds(ShootSpeed);
        Ammo = MaxAmmo;
        isReloading = false;
    }

    IEnumerator ReloadImage()
    {
        float timer = ShootSpeed;

        for(int i = 0; i<coolImage.Length; i++)
        {
            coolImage[i].fillAmount = 1f;
            coolImage[i].gameObject.SetActive(true);
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float fill = timer / ShootSpeed;

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
}
