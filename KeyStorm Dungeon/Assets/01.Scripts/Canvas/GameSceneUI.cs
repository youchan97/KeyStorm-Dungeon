using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameSceneUI : MonoBehaviour
{
    UiManager uiManager;

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttack attack;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    [SerializeField] GameObject optionPanel;

    Player player;

    private void Awake()
    {
        uiManager = UiManager.Instance;
    }

    void Update()
    {
        coinTxt.text = inventory.gold.ToString();
        bombTxt.text = inventory.bombCount.ToString();
        ammoTxt.text = attack.Ammo.ToString() + " / " + attack.MaxAmmo.ToString();
    }

    public void InitPlayerData(Player player)
    {
        this.player = player;
        inventory = player.Inventory;
        attack = player.PlayerAttack;
    }

    public void OpenOption()
    {
        uiManager.OpenPopup(optionPanel);
    }

    public void OpenSoundSetting()
    {
        uiManager.OpenSoundPopup();
    }

    public void ClosePopup()
    {
        uiManager.ClosePopup();
    }

    public void CloseAllPopup()
    {
        uiManager.CloseAllPopup();
    }

    public void OnClickExitButton()
    {
        GameManager.Instance.ExitGame();
    }
}
