using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public class InventoryUi : MonoBehaviour
{
    [Header("Components 관련")]
    PlayerInventory inventory;
    [SerializeField] InventoryController controller;
    [SerializeField] WorldItemUIWidget widget;

    [Header("Slot 관련")]
    [SerializeField] Transform itemSlots;
    [SerializeField] InventoryItemSlot itemSlot;
    [SerializeField] int columnCount;

    [Header("ItemInfo 관련")]
    [SerializeField] Image itemBackGround;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameKor;
    [SerializeField] TextMeshProUGUI itemNameEng;
    [SerializeField] TextMeshProUGUI itemTier;
    [SerializeField] TextMeshProUGUI itemStatus;
    [SerializeField] TextMeshProUGUI itemDes;

    [Header("Status 관련")]
    [SerializeField] TextMeshProUGUI moveSpeedText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI specialDamageMultipleText;
    [SerializeField] TextMeshProUGUI attackSpeedText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI shotSpeedText;

    List<InventoryItemSlot> inventoryItemSlots = new List<InventoryItemSlot>();

    List<ItemData> passiveItemDatas;
    ItemData activeItemData;
    bool isFirst;

    int currentIndex;

    private void OnEnable()
    {
        if (inventoryItemSlots != null && inventoryItemSlots.Count > 0)
        {
            SelectItem();
        }
        else
        {
            if (itemBackGround != null)
                itemBackGround.gameObject.SetActive(false);
        }

        if (controller != null)
        {
            controller.OnSelect += InputIndex;
        }
    }

    private void OnDisable()
    {
        if (controller != null)
        {
            controller.OnSelect -= InputIndex;
        }
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    public void SetInventoryUi(PlayerInventory inventory)
    {
        this.inventory = inventory;
        InitializeInventory();
        InitEvent();
    }

    void InitEvent()
    {
        inventory.OnAddPassiveItem += AddPassiveItem;
        inventory.OnAddActiveItem += AddActiveItem;
    }

    void RemoveEvent()
    {
        inventory.OnAddPassiveItem -= AddPassiveItem;
        inventory.OnAddActiveItem -= AddActiveItem;
    }

    void InitializeInventory()
    {
        passiveItemDatas = inventory.passiveItems;
        activeItemData = inventory.activeItem;
        isFirst = true;
        if (passiveItemDatas.Count > 0)
        {
            for(int i = 0; i < passiveItemDatas.Count; i++)
            {
                ItemData itemData = passiveItemDatas[i];
                AddPassiveItem(itemData);
            }
        }

        if (activeItemData != null)
            AddActiveItem(activeItemData);

        isFirst = false;
    }


    void AddPassiveItem(ItemData data)
    {
        if (passiveItemDatas.Contains(data) && !isFirst)
            return;

        InventoryItemSlot slot = Instantiate(itemSlot, itemSlots);
        slot.InitData(data);

        inventoryItemSlots.Add(slot);
    }

    void AddActiveItem(ItemData data)
    {
        if (activeItemData == data && !isFirst)
            return;

        InventoryItemSlot slot = Instantiate(itemSlot, itemSlots);
        slot.InitData(data);
        slot.transform.SetAsFirstSibling();
        inventoryItemSlots.Insert(0, slot);
    }

    void SelectItem(int index = 0)
    {
        /*if (passiveItemDatas.Count == 0 && activeItemData == null)
            return;
        else if (passiveItemDatas.Count == 0)
        {
            ViewItemInfo(activeItemData);
            return;
        }*/

        if (inventoryItemSlots == null || inventoryItemSlots.Count == 0)
        {
            if (itemBackGround != null)
                itemBackGround.gameObject.SetActive(false);
            return;
        }

        if (!itemBackGround.gameObject.activeSelf)
            itemBackGround.gameObject.SetActive(true);

        for(int i = 0; i < inventoryItemSlots.Count; i++)
        {
            InventoryItemSlot slot = inventoryItemSlots[i];
            if (i == index)
            {
                slot.SetSelected(true);
                ViewItemInfo(inventoryItemSlots[index].ItemData);
                currentIndex = index;
            }
            else
                slot.SetSelected(false);    
        }    
    }

    void ViewItemInfo(ItemData data)
    {
        if (data == null) return;
        if (widget == null)
        {
            Debug.LogWarning("[InventoryUi] widget이 null! Inspector에서 연결하세요.");
            return;
        }
        if (itemBackGround == null || itemImage == null) return;
        if (itemNameKor == null || itemNameEng == null) return;
        if (itemTier == null || itemStatus == null) return;

        ItemTier tier = data.tier;

        itemBackGround.sprite = widget.GetTierBackground(tier);
        itemImage.sprite = data.iconSprite;
        TextColorSetting(tier);
        itemNameKor.text = data.name;
        itemNameEng.text = data.itemEnglishName;
        itemTier.text = string.Format("Tier : {0}", widget.GetTierText(tier));
        itemStatus.text = widget.BuildStatsText(data);
        if (data.description != null || data.description != "")
            itemDes.text = data.description;
    }

    void TextColorSetting(ItemTier tier)
    {
        if (widget == null) return;

        Color color = widget.GetTierColor(tier);
        itemNameKor.color = color;
        itemNameEng.color = color;
        itemTier.color = color;
    }

    void InputIndex(Vector2 input)
    {
        if (inventoryItemSlots.Count == 0)
            return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > CenterValue)
                MoveSelect(DefalueMoveIndex);
            else if (input.x < -CenterValue)
                MoveSelect(-DefalueMoveIndex);
        }
        else
        {
            if (input.y > CenterValue)
                MoveSelect(-columnCount);
            else if (input.y < -CenterValue)
                MoveSelect(columnCount);
        }
    }


    void MoveSelect(int index)
    {
        int targetIndex = currentIndex + index;

        if (targetIndex < 0 || targetIndex >= inventoryItemSlots.Count)
            return;

        SelectItem(targetIndex);
    }


    public void SetStatus(Player player)
    {
        PlayerAttack attack = player.PlayerAttack;
        PlayerRunData runData = player.PlayerRunData;

        moveSpeedText.color = GetStautsTextColot(runData.character.moveSpeed, player.PlayerEffectStat.GetMoveSpeed);
        moveSpeedText.text = player.PlayerEffectStat.GetMoveSpeed.ToString("0.##");

        float totalDamage = attack.Damage * attack.DamageMultiple;
        damageText.color = GetStautsTextColot(totalDamage, player.PlayerEffectStat.GetDamage(totalDamage));
        damageText.text = string.Format("{0:0.##}(x{1:0.##})", attack.Damage, attack.DamageMultiple);

        specialDamageMultipleText.text = attack.SpecialDamageMultiple.ToString("0.##");

        float totalAttackSpeed = attack.AttackSpeed * attack.AttackSpeedMultiple;
        attackSpeedText.color = GetStautsTextColot(totalAttackSpeed, player.PlayerEffectStat.GetAttackSpeed(totalAttackSpeed));
        attackSpeedText.text = string.Format("{0:0.##}(x{1:0.##})", attack.AttackSpeed, attack.AttackSpeedMultiple);

        float totalRange = attack.Range * attack.RangeMultiple;
        rangeText.color = GetStautsTextColot(totalRange, player.PlayerEffectStat.GetRange(totalRange));
        rangeText.text = string.Format("{0:0.##}(x{1:0.##})", attack.Range, attack.RangeMultiple);

        shotSpeedText.color = GetStautsTextColot(attack.ShootSpeed, player.PlayerEffectStat.GetShotSpeed);
        shotSpeedText.text = player.PlayerEffectStat.GetShotSpeed.ToString("0.##");
    }


    Color GetStautsTextColot(float realData, float gameData)
    {
        return (realData >= gameData) ? Color.white : Color.red;
    }
}
