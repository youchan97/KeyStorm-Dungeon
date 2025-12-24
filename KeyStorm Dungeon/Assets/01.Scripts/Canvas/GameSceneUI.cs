using UnityEngine;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttack attack;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    public void InitPlayerData(Player player)
    {
        inventory = player.Inventory;
        attack = player.PlayerAttack;
    }
    void Update()
    {
        coinTxt.text = inventory.gold.ToString();
        bombTxt.text = inventory.bombCount.ToString();
        ammoTxt.text = attack.Ammo.ToString() + " / " + attack.MaxAmmo.ToString();
    }
}
