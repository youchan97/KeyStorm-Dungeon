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

    #region UI 관련
    [SerializeField] TextMeshProUGUI[] dirTexts;
    [SerializeField] Image[] coolImage;
    Coroutine[] keyImageCoroutine = new Coroutine[8];
    Coroutine reloadImageCoroutine;
    #endregion

    #region 키 입력 관련
    private readonly string[] keyNames =
    { "Q", "W", "E", "R", "A", "S","D", "F" };

    private readonly Vector2[] shotDir =
        {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1),
    new Vector2(-1, 1), new Vector2(0, -1),new Vector2(-1, -1),new Vector2(-1, 0)};

    Dictionary<string, Vector2> keyDic = new Dictionary<string, Vector2>();

    Dictionary<string, bool> keyCoolDic = new Dictionary<string, bool>();

    Coroutine[] keyCoroutine = new Coroutine[8];
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
    }


    public void InitPlayerAttack(Player player, PlayerData data)
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

        ShuffleKey();
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

        Vector2[] randVec = shotDir.OrderBy(x => Random.Range(0f, 1f)).ToArray();

        for(int i = 0; i < keyNames.Length; i++)
        {
            keyDic[keyNames[i]] = randVec[i];
            AttackDirUiUpdate(keyNames[i], randVec[i]);
        }
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

    void AttackDirUiUpdate(string text, Vector2 vec)
    {
        DirType type = SwitchType(vec);
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
        keyCoroutine[(int)type] = StartCoroutine(StartKeyCool(keyName));
        keyImageCoroutine[(int)type] = StartCoroutine(ImageFiil(type));
    }

    IEnumerator StartKeyCool(string key)
    {
        keyCoolDic[key] = true;
        yield return new WaitForSeconds(ShootSpeed);
        keyCoolDic[key] = false;
    }

    IEnumerator ImageFiil(DirType type)
    {
        Image image = coolImage[(int)type];
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
        for (int i = 0; i < keyCoroutine.Length; i++)
        {
            if (keyCoroutine[i] != null)
            {
                StopCoroutine(keyCoroutine[i]);
                keyCoroutine[i] = null;
            }

            if (keyImageCoroutine[i] != null)
            {
                StopCoroutine(keyImageCoroutine[i]);
                keyImageCoroutine[i] = null;
                coolImage[i].fillAmount = 1f;
                coolImage[i].gameObject.SetActive(false);
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
