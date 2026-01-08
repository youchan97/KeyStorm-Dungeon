using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using static ConstValue;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    UiManager uiManager;
    GameManager gameManager;
    AudioManager audioManager;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    [SerializeField] GameObject optionPanel;
    [SerializeField] HealthUI healthUI;

    [SerializeField] Image itemImage;
    [SerializeField] Image coolTimeImage;
    Coroutine activeItemCo;

    public Player player;

    public HealthUI HealthUI { get => healthUI; }

    private void Awake()
    {
        InitManager();
    }

    private void OnEnable()
    {
        Room.OnGameCleared += GameClear;
    }

    private void OnDisable()
    {
        Room.OnGameCleared -= GameClear;
        if (player != null)
        {
            player.PlayerSkill.StartSkill -= OnActiveItemCoolDown;
            player.Inventory.OnAddActiveItem -= UpdateActiveItemSlot;
        }
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
        InitPlayerEvent(this.player);
        InitGameUi();

        // 게임 시작 시 타이머 시작
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.ResetTimer();
            GameTimeManager.Instance.StartTimer();
            Debug.Log("[GameSceneUI] 게임 타이머 시작됨");
        }
    }

    void InitPlayerEvent(Player player)
    {
        player.PlayerSkill.StartSkill += OnActiveItemCoolDown;
        player.Inventory.OnAddActiveItem += UpdateActiveItemSlot;
    }

    public void InitGameUi()
    {
        UpdateGold();
        UpdateBomb();
        UpdateAmmo();
        UpdateActiveItemSlot(player.Inventory.activeItem);
        healthUI.SetMaxHp(player.MaxHp);
        healthUI.SetHp(player.Hp);
    }

    public void UpdateGold()
    {
        coinTxt.text = player.Inventory.gold.ToString();

        // ⭐ 골드 업데이트는 PlayerInventory에서 처리하므로 여기서는 하지 않음
    }

    public void UpdateBomb()
    {
        bombTxt.text = player.Inventory.bombCount.ToString();
    }

    public void UpdateAmmo()
    {
        ammoTxt.text = string.Format("{0} / {1}", player.PlayerAttack.Ammo, player.PlayerAttack.MaxAmmo);
    }

    public void OnActiveItemCoolDown(float time)
    {
        if (activeItemCo != null)
        {
            StopCoroutine(activeItemCo);
            activeItemCo = null;
        }

        activeItemCo = StartCoroutine(ActiveItemImageFill(time));
    }

    IEnumerator ActiveItemImageFill(float coolTime)
    {
        coolTimeImage.fillAmount = 1f;
        float time = coolTime;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            coolTimeImage.fillAmount = time / coolTime;
            yield return null;
        }

        coolTimeImage.fillAmount = 0f;
        activeItemCo = null;
    }

    public void UpdateActiveItemSlot(ItemData data)
    {
        if (data == null)
        {
            return;
        }

        if (activeItemCo != null)
        {
            StopCoroutine(activeItemCo);
            coolTimeImage.fillAmount = 0f;
            activeItemCo = null;
        }

        itemImage.sprite = data.iconSprite;

        // ⭐ 아이템 추가는 PlayerInventory.SetActiveItem()에서 처리하므로 여기서는 하지 않음
    }

    public void OpenOption()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.PauseTimer();
        }

        uiManager.OpenPopup(optionPanel);
    }

    public void OpenSoundSetting()
    {
        uiManager.OpenSoundPopup();
    }

    public void ClosePopup()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.StartTimer();
        }

        uiManager.ClosePopup();
    }

    public void CloseAllPopup()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.StartTimer();
        }

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
        Debug.Log("[GameSceneUI] GameOver 호출됨");
        audioManager.PlayBgm(GameOverBgm);

        if (ResultManager.Instance != null)
        {
            ResultManager.Instance.ShowResult(false);
        }
        else
        {
            Debug.LogError("[GameSceneUI] ResultManager를 찾을 수 없습니다!");
        }
    }

    public void GameClear()
    {
        Debug.Log("[GameSceneUI] GameClear 호출됨");
        audioManager.PlayBgm(GameOverBgm);

        if (ResultManager.Instance != null)
        {
            ResultManager.Instance.ShowResult(true);
        }
        else
        {
            Debug.LogError("[GameSceneUI] ResultManager를 찾을 수 없습니다!");
        }
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
