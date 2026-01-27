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
    [SerializeField] InventoryUi inventoryUi;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI bombTxt;
    [SerializeField] private TextMeshProUGUI ammoTxt;

    [SerializeField] GameObject optionPanel;
    [SerializeField] HealthUI healthUI;

    [SerializeField] GameObject activeItemSlot;
    [SerializeField] Image itemImage;
    [SerializeField] Image coolTimeImage;
    [SerializeField] private GameObject bossHpBarUI;
    [SerializeField] private Transform bossHpBarLayout;
    
    private List<BossHpBar> bossHpBars = new List<BossHpBar>();
    private Room currentRoom;

    Coroutine activeItemCo;

    public Player player;

    public HealthUI HealthUI { get => healthUI; }
    public InventoryUi InventoryUi { get => inventoryUi;}

    [SerializeField] private RoomChangeEvent roomChangeEvent;

    [SerializeField] GameObject minimapPanel;

    private void Awake()
    {
        InitManager();
    }

    void Start()
    {
        CheckGameClearedState();
    }

    private void CheckGameClearedState()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameCleared)
        {
            StartCoroutine(ShowVictoryAfterInit());
        }
    }

    private IEnumerator ShowVictoryAfterInit()
    {
        yield return new WaitForSeconds(0.1f);


        if (ResultManager.Instance != null)
        {
            ResultManager.Instance.ShowResult(true);
        }
        else
        {

        }
    }

    private void OnEnable()
    {
        Room.OnGameCleared += GameClear;
        roomChangeEvent.OnRoomChange += CurrentRoomChange;
    }

    private void OnDisable()
    {
        Room.OnGameCleared -= GameClear;

        if (roomChangeEvent != null)
        {
            roomChangeEvent.OnRoomChange -= CurrentRoomChange;
        }

        if (currentRoom != null)
        {
            currentRoom.OnBossSpawn -= CreateBossHpBar;
        }

        foreach (BossHpBar bossHpBar in bossHpBars)
        {
            if (bossHpBar != null)
            {
                bossHpBar.OnRemoveUI -= RemoveBossHpBar;
                Destroy(bossHpBar);
            }
        }
        bossHpBars.Clear();

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
        inventoryUi.SetInventoryUi(player.Inventory);
        inventoryUi.SetStatus(player);
        InitGameUi();

        if (GameTimeManager.Instance != null && !GameManager.Instance.IsGameCleared)
        {
            if (GameManager.Instance.CurrentStage == 1)
            {
                GameTimeManager.Instance.ResetTimer();
            }
            else
            {

            }

            GameTimeManager.Instance.StartTimer();
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

        if (!activeItemSlot.activeSelf)
            activeItemSlot.SetActive(true);

        if (activeItemCo != null)
        {
            StopCoroutine(activeItemCo);
            coolTimeImage.fillAmount = 0f;
            activeItemCo = null;
        }

        itemImage.sprite = data.iconSprite;
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

        if (inventoryUi.gameObject.activeSelf)
            inventoryUi.gameObject.SetActive(false);

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
        audioManager.AllStopSfxLoop();
        audioManager.PlayBgm(GameOverBgm);

        if (ResultManager.Instance != null)
        {
            ResultManager.Instance.ShowResult(false);
        }
        else
        {

        }
    }

    public void GameClear()
    {
        audioManager.AllStopSfxLoop();
        audioManager.PlayBgm(ClearBgm);

        if (ResultManager.Instance != null)
        {
            ResultManager.Instance.ShowResult(true);
        }
        else
        {

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

    private void CurrentRoomChange(Room room)
    {
        foreach (BossHpBar bossHpBar in bossHpBars)
        {
            Destroy(bossHpBar);
        }
        bossHpBars.Clear();

        if (currentRoom != null)
        {
            currentRoom.OnBossSpawn -= CreateBossHpBar;
        }

        currentRoom = room;
        currentRoom.OnBossSpawn += CreateBossHpBar;
    }

    private void CreateBossHpBar(Monster boss)
    {
        if (boss != null)
        {
            GameObject bossHpBarGO = Instantiate(bossHpBarUI, bossHpBarLayout);
            BossHpBar bossHpBar = bossHpBarGO.GetComponent<BossHpBar>();

            if (bossHpBar != null)
            {
                bossHpBar.InitBossInfo(boss);
                bossHpBar.OnRemoveUI += RemoveBossHpBar;
                bossHpBars.Add(bossHpBar);
            }
        }
    }

    private void RemoveBossHpBar(BossHpBar bossHpBar)
    {
        if (bossHpBar == null) return;

        bossHpBar.OnRemoveUI -= RemoveBossHpBar;
        bossHpBars.Remove(bossHpBar);
        Destroy(bossHpBar.gameObject);
    }

    public void SetMinimap(bool isOpen)
    {
        minimapPanel.SetActive(isOpen);
    }
}
