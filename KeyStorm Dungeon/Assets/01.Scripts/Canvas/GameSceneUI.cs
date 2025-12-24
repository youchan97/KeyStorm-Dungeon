using UnityEngine;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttack attack;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    private void Start()
    {
        if (inventory == null || attack == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (inventory == null) inventory = player.GetComponent<PlayerInventory>();
                if (attack == null) attack = player.GetComponent<PlayerAttack>();
            }
        }
    }
    void Update()
    {
        coinTxt.text = inventory.gold.ToString();
        bombTxt.text = inventory.bombCount.ToString();
        ammoTxt.text = attack.Ammo.ToString() + " / " + attack.MaxAmmo.ToString();
    }
}
