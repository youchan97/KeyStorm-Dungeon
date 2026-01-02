using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using static ConstValue;

public class GameSceneUI : MonoBehaviour
{
    UiManager uiManager;
    GameManager gameManager;
    AudioManager audioManager;

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttack attack;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    [SerializeField] GameObject optionPanel;


    [SerializeField] GameObject gameResultPanel;
    /*[SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] float typingInterval;
    [SerializeField] GameObject gameOverMenu;

    [SerializeField] GameObject gameClearPanel;
    [SerializeField] TextMeshProUGUI gameClearText;
    [SerializeField] GameObject gameClearMenu;*/

    Player player;

    private void Awake()
    {
        InitManager();     
    }

    private void OnEnable()
    {
        Room.OnGameCleared += GameClear;
    }

    public void UpdateGold(int amount)
    {
        coinTxt.text = amount.ToString();
    }
    public void UpdateBomb(int amount)
    {
        bombTxt.text = amount.ToString();
    }
    public void UpdateAmmo(int ammo, int maxAmmo)
    {
        ammoTxt.text = string.Format("{0} / {1}", ammo, maxAmmo);
    }

    private void OnDisable()
    {
        Room.OnGameCleared -= GameClear;
    }

    void InitManager()
    {
        uiManager = UiManager.Instance;
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
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

    public void OnClickHomeButton()
    {
        uiManager.ClearStack();
        audioManager.PlayButton();
        gameManager.GoHome();
    }

    public void OnClickExitButton()
    {
        audioManager.PlayButton();
        gameManager.ExitGame();
    }

    public void OnClickRetryButton()
    {
        audioManager.PlayButton();
        gameManager.RetryGame();
    }

    public void GameOver()
    {
        audioManager.PlayBgm(GameOverBgm);
        gameResultPanel.SetActive(true);
    }
    public void GameClear()
    {
        audioManager.PlayBgm(GameOverBgm);
        gameResultPanel.SetActive(true);
    }

    /*IEnumerator StartResult(string message, GameObject menu)
    {
        gameOverText.text = "";

        foreach (char c in message)
        {
            gameOverText.text += c;
            yield return new WaitForSeconds(typingInterval);
        }

        gameOverMenu.SetActive(true);
        yield return null;
    }*/

}
