using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using static ConstValue;

public class GameSceneUI : MonoBehaviour
{
    UiManager uiManager;
    GameManager gameManager;

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttack attack;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    [SerializeField] GameObject optionPanel;


    [SerializeField] GameObject gameoverPanel;
    [SerializeField] TextMeshProUGUI gameoverText;
    [SerializeField] float typingInterval;
    [SerializeField] GameObject gameoverMenu;

    Player player;

    private void Awake()
    {
        uiManager = UiManager.Instance;
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        coinTxt.text = inventory.gold.ToString();
        bombTxt.text = inventory.bombCount.ToString();
        ammoTxt.text = attack.Ammo.ToString() + " / " + attack.MaxAmmo.ToString();
    }
    private void OnDisable()
    {
        player.OnDie -= GameOverCanvas;
    }

    public void InitPlayerData(Player player)
    {
        this.player = player;
        inventory = player.Inventory;
        attack = player.PlayerAttack;
        player.OnDie += GameOverCanvas;
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

    public void OnClickHomeButton()
    {
        gameManager.GoHome();
    }

    public void OnClickExitButton()
    {
        gameManager.ExitGame();
    }

    public void OnClickRetryButton()
    {
        gameManager.RetryGame();
    }

    public void GameOverCanvas()
    {
        gameoverPanel.SetActive(true);
        StartCoroutine(StartGameOver(GameOverText));
    }

    IEnumerator StartGameOver(string message)
    {
        gameoverText.text = "";

        foreach (char c in message)
        {
            gameoverText.text += c;
            yield return new WaitForSeconds(typingInterval);
        }

        gameoverMenu.SetActive(true);
        yield return null;
    }
}
