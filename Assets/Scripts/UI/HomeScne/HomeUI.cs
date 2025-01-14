using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    private GameObject BG;
    public GameObject Bg { get => BG; }

    #region ��
    private Transform panelDevice;
    private Transform buttonHolder;
    private Button buttonPlay;
    private Text buttonPlayText;
    private Button buttonStick;
    private Text pageText;
    private Image stickerSlider;
    private Text stickerProgress;
    private GameObject stickerNoti;

    private Button buttonNewSitcker;
    private GameObject newStickerFx;
    private Transform flashTrans;
    private Transform flash;
    #endregion

    #region ��
    private Transform topTrans;
    private Button buttonCoin;
    private Button buttonHeart;
    private Button buttonStar;
    private Button buttonSetting;
    public Transform CoinTrans { get; set; }
    public Transform PiggyTrans { get; set; }
    public Transform StarTrans { get; set; }
    public Transform HeartTrans { get; set; }

    private Text coinText;
    private Text heartText;
    private Image heartImage;
    private Text starText;
    public bool IsWireLessHeart { get; set; }//�Ƿ�����������״̬
    #endregion

    #region ��
    private Transform rightTrans;
    private Button buttonNoAds;
    private Button buttonPiggyBank;
    private GameObject piggyFullObj;
    private Button buttonLuckySpin;
    private Button buttonDailyBonus;
    private GameObject dailyNoti;
    public Transform LuckySpinTrans { get; private set; }
    private GameObject luckSpinNoti;

    #endregion

    #region ��
    private Transform leftTrans;
    private Button shopButton;
    private Button collectionButton;
    private Button streakButton;
    private Button skyRaceButton;
    private Text streakText;
    public Transform StreakTransfom { get; private set; }
    private Text streakTimeText;

    private TimedObject streakTime;
    #endregion

    private Vector3 originLeftPos;
    private Vector3 originRightPos;

    public WinStreakUI winStreakUI;
    public CanvasHomeItemMove HomeItemMove;
    public Text GetCoinText()
    {
        return coinText;
    }
    public Text GetHeartText()
    {
        return heartText;
    }

    private TimedObject wirelessHeartTime;
    private void Awake()
    {
        BG = transform.Find("Bg").gameObject;
        panelDevice = transform.Find("Bg/Save Area/Panel Device").transform;

        #region �²�
        buttonHolder = panelDevice.Find("Botton").transform;
        buttonPlay = buttonHolder.Find("Button Play").GetComponent<Button>();
        buttonPlay.onClick.AddListener(() =>
        {
            GamePlay();
            AudioManager.Instance.PlaySFX("Click");
        });
        buttonPlayText = buttonPlay.transform.Find("Text Play Level").GetComponent<Text>();
        buttonStick = buttonHolder.Find("Button Stick").GetComponent<Button>();
        buttonStick.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("Click");
            StickEvent();
        });
        pageText = buttonStick.transform.Find("Content/Page").GetComponent<Text>();
        stickerSlider = buttonStick.transform.Find("Content/slider/slider").GetComponent<Image>();
        stickerProgress = buttonStick.transform.Find("Content/slider/Text").GetComponent<Text>();
        stickerNoti = buttonStick.transform.Find("Content/Noti").gameObject;

        buttonNewSitcker = buttonHolder.Find("Button NewSticker").GetComponent<Button>();
        buttonNewSitcker.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("Click");
            NewStickerEvent();
        });
        newStickerFx = buttonHolder.Find("ButtonNewStickFX").gameObject;
        newStickerFx.SetActive(false);
        flashTrans = buttonPlay.transform.Find("FlashButton");
        flash = flashTrans.Find("Flash");
        #endregion

        #region ��
        topTrans = panelDevice.Find("Top").transform;
        buttonCoin = topTrans.Find("Button Coin").GetComponent<Button>();
        buttonHeart = topTrans.Find("Button Heart").GetComponent<Button>();
        buttonStar = topTrans.Find("Button Star").GetComponent<Button>();
        buttonSetting = topTrans.Find("Button Setting").GetComponent<Button>();

        coinText = buttonCoin.transform.Find("Text Coin").GetComponent<Text>();
        heartText = buttonHeart.transform.Find("Text Heart").GetComponent<Text>();
        starText = buttonStar.transform.Find("Text Star").GetComponent<Text>();

        buttonStar.onClick.AddListener(StarEvent);
        buttonHeart.onClick.AddListener(HeartEvent);
        buttonCoin.onClick.AddListener(CoinEvent);
        buttonSetting.onClick.AddListener(SettingEvent);

        CoinTrans = buttonCoin.transform.Find("Icon Coin");
        StarTrans = buttonStar.transform.Find("Icon Heart");
        HeartTrans = buttonHeart.transform.Find("Icon Heart");
        heartImage = HeartTrans.GetComponent<Image>();
        #endregion

        #region ��
        rightTrans = panelDevice.Find("Contain Right").transform;
        buttonPiggyBank = rightTrans.Find("Button Piggy Bank").GetComponent<Button>();
        piggyFullObj = buttonPiggyBank.transform.Find("Icon Full").gameObject;

        buttonNoAds = rightTrans.Find("Button No Ads").GetComponent<Button>();
        buttonLuckySpin = rightTrans.Find("Button LuckySpin").GetComponent<Button>();
        buttonDailyBonus = rightTrans.Find("Button Daily Bonus").GetComponent<Button>();
        PiggyTrans = buttonPiggyBank.transform.Find("Icon");
        LuckySpinTrans = buttonLuckySpin.transform;
        dailyNoti = buttonDailyBonus.transform.Find("Noti").gameObject;
        luckSpinNoti = LuckySpinTrans.Find("Noti").gameObject;

        buttonNoAds.onClick.AddListener(NoAdsEvent);
        buttonLuckySpin.onClick.AddListener(LuckySpinEvent);
        buttonDailyBonus.onClick.AddListener(DailyBonusEvent);
        #endregion


        #region ��
        leftTrans = panelDevice.Find("Contain Left").transform;
        shopButton = leftTrans.Find("Button Shop").GetComponent<Button>();
        collectionButton = leftTrans.Find("Button Collection").GetComponent<Button>();
        streakButton = leftTrans.Find("Button Streak").GetComponent<Button>();
        skyRaceButton = leftTrans.Find("Button Sky Race").GetComponent<Button>();
        shopButton.onClick.AddListener(ShopEvent);
        collectionButton.onClick.AddListener(CollectionEvent);
        streakButton.onClick.AddListener(ShowWinStreakUI);
        skyRaceButton.onClick.AddListener(SkyRaceEvent);

        StreakTransfom = streakButton.transform;
        streakText = StreakTransfom.Find("Image/count").GetComponent<Text>();

        streakTimeText = StreakTransfom.Find("TimeText").GetComponent<Text>();
        #endregion

        originLeftPos = leftTrans.position;
        originRightPos = rightTrans.position;
    }
    private void Start()
    {
        AnimationUtility.RepeatMoveToRight(flash);
        FreshHomeButton();

        //ע���¼�
        EventManager.Instance.RegisterEvent(GameEvent.UpdateDailyEvent, UpdateResourceText);
        EventManager.Instance.RegisterEvent(GameEvent.UpdateStreakEvent, UpdateStreakText);
        EventManager.Instance.RegisterEvent(GameEvent.OpenChestEvent, UpdateResourceText);
        EventManager.Instance.RegisterEvent(GameEvent.AddHearEvent, AddHeartCount);
        UpdateHomeUI();

        streakTime = TimeManager.Instance.GetTimeObj(TimeEventType.Streak);
        wirelessHeartTime = TimeManager.Instance.GetTimeObj(TimeEventType.WireLessHeart);

        UpdateWirelessHeartIcon();
    }
    private void OnEnable()
    {
        UpdateHomeUI();
    }
    private void Update()
    {
        //���������������Ҫ���и���
        if (wirelessHeartTime != null && wirelessHeartTime.currentState != ActivityState.Stop)
        {
            heartText.transform.localScale = Vector3.one * 0.7f;
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            { 
                heartText.text = $"{wirelessHeartTime.remainingTime.Minutes}m{wirelessHeartTime.remainingTime.Seconds}";
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                heartText.text = $"{wirelessHeartTime.remainingTime.Minutes}��{wirelessHeartTime.remainingTime.Seconds}";
            }
        }

    }
    /// <summary>
    /// ��ʼ��Ϸ��ť���õ�ʱ��
    /// </summary>
    private void GamePlay()
    {
        //�����ؿ���
        if (LevelManager.Instance.GetLevleNum() > 200)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("YOU WIN ALL THE GAME!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("���Ѿ������������Ϸ!");
            }

            return;
        }

        //����ֵ����
        if (GameDataManager.CurrentGameData.heartCount <= 0)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            UIManager.Instance.ShowUI<OutOfLiveUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("NO ENOUGH LIFE TO PLAY GAME!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("û���㹻������!");
            }

            return;
        }
        LevelManager.Instance.InitLevel();
    }

    public void UpdateHomeUI()
    {
        UpdateResourceText();
        UpdateStreakText();
        UpdateHomePlayText();

        //�Ƿ�Ϊ����ֽ
        if (GameDataManager.CurrentGameData.isNewSticker)
        {
            //���ð�ťΪ����ֽ
            SetStickerButton(false);
        }
        UpdateHomeStickerSlider();
        UpdateNoticeState();

        //Debug.Log(LevelManager.Instance.GetLevleNum().ToString());
    }

    #region ���
    /// <summary>
    /// ���º����ʾ״̬
    /// </summary>
    private void UpdateNoticeState()
    {
        UpdateDailyNoti();
        UpdatePiggyFull();
        UpdateLuckyPinNoti();
        UpdateStickerNoti();
    }
    /// <summary>
    /// ����ÿ�պ��
    /// </summary>
    public void UpdateDailyNoti()
    {
        if (GameDataManager.CurrentGameData.curDailyIndex < 5)
        {
            dailyNoti.SetActive(true);
        }
        else
        {
            dailyNoti.SetActive(false);
        }
        dailyNoti.transform.Find("Text").GetComponent<Text>().text = (5 - GameDataManager.CurrentGameData.curDailyIndex).ToString();
    }
    /// <summary>
    /// ���´�Ǯ�Ƿ����˵ı�ʶ
    /// </summary>
    public void UpdatePiggyFull()
    {
        if (GameDataManager.CurrentGameData.piggyCount >= 750)
        {
            piggyFullObj.SetActive(true);
        }
        else
        {
            piggyFullObj.SetActive(false);
        }
    }
    /// <summary>
    /// ���³齱���
    /// </summary>
    public void UpdateLuckyPinNoti()
    {
        if (GameDataManager.CurrentGameData.curSpinCount > 0)
        {
            luckSpinNoti.SetActive(true);
        }
        else
        {
            luckSpinNoti.SetActive(false);
        }

        luckSpinNoti.transform.Find("Text").GetComponent<Text>().text = GameDataManager.CurrentGameData.curSpinCount.ToString();
    }

    /// <summary>
    /// ������ֽ���
    /// </summary>
    public void UpdateStickerNoti()
    {
        if (!StickerManager.Instance.IsStickerCompeleted() && StickerManager.Instance.CurSticker.CanNoti())
        {
            stickerNoti.SetActive(true);
        }
        else
        {
            stickerNoti.SetActive(false);
        }
    }
    #endregion

    #region ʱ�����������

    public void SetStreakButton(bool val)
    {
        StreakTransfom.gameObject.SetActive(val);
    }

    public void SetLuckSpinButton(bool val)
    {
        LuckySpinTrans.gameObject.SetActive(val);
    }

    private void AddHeartCount()
    {
        GameDataManager.AddHeartCount(1);
        UpdateResourceText();

        if (GameDataManager.CurrentGameData.heartCount >= 5)
        {
            //��������Ѿ����ˣ���ֹͣ��ʱ
            TimedObject timeObj = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
            timeObj.currentState = ActivityState.Stop;
            timeObj.remainingTime = timeObj.startTime.ToTimeSpan();
        }
    }

    /// <summary>
    /// ����������ͼƬ
    /// </summary>
    public void UpdateWirelessHeartIcon()
    {
        if (wirelessHeartTime != null && wirelessHeartTime.currentState != ActivityState.Stop)
        {
            heartImage.sprite = ResourceLoader.Instance.GetUnlockImageSprite("wlheart");
            buttonHeart.interactable = false;
            IsWireLessHeart = true;
        }
        else
        {
            heartImage.sprite = ResourceLoader.Instance.GetUnlockImageSprite("norheart");
            buttonHeart.interactable = true;
            IsWireLessHeart = false;
            int heartCount = GameDataManager.CurrentGameData.heartCount;

            if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
                heartText.text = heartCount < 5 ? heartCount.ToString() : "FULL";
            else if(YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                heartText.text = heartCount < 5 ? heartCount.ToString() : "����";

        }
    }

    #endregion

    /// <summary>
    /// ������ֽ����
    /// </summary>
    public void UpdateHomeStickerSlider()
    {
        pageText.text = $"PAGE 0{GameDataManager.CurrentGameData.curStickerIndex + 1}";
        stickerSlider.fillAmount = (GameDataManager.GetUnlockStickerList().Count) / 9.0f;
        stickerProgress.text = $"{GameDataManager.GetUnlockStickerList().Count} / 9";
    }
    /// <summary>
    /// ���½�����Ϸ�ı�
    /// </summary>
    public void UpdateHomePlayText()
    {
        if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
        { 
            buttonPlayText.text = "LEVEL " + LevelManager.Instance.GetLevleNum().ToString();
        }
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
        {
            buttonPlayText.text = "�ؿ� " + LevelManager.Instance.GetLevleNum().ToString();
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    private void UpdateStreakText()
    {
        if (GameDataManager.CurrentGameData.isStreaklocked == true)
            return;

        int curStreak = GameDataManager.CurrentGameData.curStreakIndex;
        if (curStreak == 0)
        {
            streakText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            streakText.transform.parent.gameObject.SetActive(true);
        }
        //Debug.Log(GameDataManager.CurrentGameData.curStreakIndex);

        streakText.text = GameDataManager.CurrentGameData.curStreakIndex.ToString();
    }

    #region ����
    /// <summary>
    /// ���¿�ʼ�����¼�
    /// </summary>
    /// <param name="callBack"></param>
    public void ResetStreakEvent(System.Action callBack = null)
    {
        winStreakUI.RestartStreakEvent(callBack);
    }

    public void UpdateStreakTimeText()
    {
        if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
            streakTimeText.text = $"{streakTime.remainingTime.Days}d{streakTime.remainingTime.Hours}h{streakTime.remainingTime.Minutes}m";
        else if(YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            streakTimeText.text = $"{streakTime.remainingTime.Days}��{streakTime.remainingTime.Hours}ʱ{streakTime.remainingTime.Minutes}��";
    }
    #endregion
    /// <summary>
    /// ������������Դ�ı�
    /// </summary>
    public void UpdateResourceText()
    {
        coinText.text = GameDataManager.CurrentGameData.coinCount.ToString();
        heartText.transform.localScale = Vector3.one;

        int heartCount = GameDataManager.CurrentGameData.heartCount;

        if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            heartText.text = heartCount < 5 ? heartCount.ToString() : "FULL";
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            heartText.text = heartCount < 5 ? heartCount.ToString() : "����";

        starText.text = GameDataManager.CurrentGameData.starCount.ToString();
    }

    private void ShopEvent()
    {
        UIManager.Instance.ShowUI<ShopUI>();
        UIManager.Instance.GetUI<ShopUI>().EnterShop();
    }
    private void CollectionEvent()
    {
        UIManager.Instance.ShowUI<CollectionUI>();
        UIManager.Instance.GetUI<CollectionUI>().SetIsClickFalse();
    }
    private void NoAdsEvent()
    {
        UIManager.Instance.ShowUI<NoAdsUI>();
    }
    private void HeartEvent()
    {
        UIManager.Instance.ShowUI<OutOfLiveUI>();
    }
    private void StarEvent()
    {
        UIManager.Instance.ShowUI<EarnStarUI>();
    }
    private void CoinEvent()
    {
        UIManager.Instance.ShowUI<ShopUI>();
        UIManager.Instance.GetUI<ShopUI>().EnterCoin();
    }
    private void StickEvent()
    {
        HomeSceneUI.Instance.EnterSticker();
    }
    public void EnterSticker()
    {
        buttonStick.interactable = false;
        buttonPlay.interactable = false;
        AnimationUtility.PlayExitToRight(rightTrans);
        AnimationUtility.PlayExitToLeft(leftTrans);
        AnimationUtility.FadeOut(topTrans);
        AnimationUtility.FadeOut(buttonHolder, 0.5f, () => BG.SetActive(false));
    }
    public void ExitSticker()
    {
        BG.SetActive(true);
        AnimationUtility.PlayEnterFromLeft(leftTrans);
        AnimationUtility.PlayEnterFromRight(rightTrans);
        AnimationUtility.FadeIn(topTrans);
        AnimationUtility.FadeIn(buttonHolder);
        buttonStick.interactable = true;
        buttonPlay.interactable = true;


        UpdateStickerNoti();
        UpdateHomePlayText();
        UpdateResourceText();
    }
    public void ShowWinStreakUI()
    {
        winStreakUI.ShowWinStreakUI();
    }
    //��һ�ν���streak
    public void ShowWinStreakUIFirst()
    {
        winStreakUI.PlayScrollAnimation();
    }
    private void LuckySpinEvent()
    {
        UIManager.Instance.ShowUI<LuckySpinUI>();
    }
    private void SkyRaceEvent()
    {
        UIManager.Instance.ShowUI<SkyEventUI>();
    }
    private void DailyBonusEvent()
    {
        UIManager.Instance.ShowUI<DailyBonusUI>();
    }
    private void SettingEvent()
    {
        UIManager.Instance.ShowUI<SettingUI>();
    }
    private void NewStickerEvent()
    {
        UIManager.Instance.ShowUI<UnlockStickerUI>();
    }

    /// <summary>
    /// �л���ֽ��ť(trueΪ��ͨ��falseΪ����ֽ)
    /// </summary>
    /// <param name="val"></param>
    public void SetStickerButton(bool val)
    {
        buttonStick.gameObject.SetActive(val);
        buttonNewSitcker.gameObject.SetActive(!val);

        if (buttonNewSitcker.gameObject.activeSelf)
            newStickerFx.gameObject.SetActive(true);
        else
        { 
            newStickerFx.gameObject.SetActive(false);
            //��������ֽ�������ı�
            UpdateHomeStickerSlider();
        }

        //�洢�Ƿ�Ϊ����ֽ��ť
        GameDataManager.SetNewStickerButton(!val);
    }
    /// <summary>
    /// ���ݴ�������ַ���Ŀ��
    /// </summary>
    public Transform GetTargetTrans(string name)
    {
        string lowerName = name.ToLower();
        if (lowerName.Contains("heart"))
        {
            return HeartTrans;
        }
        else if (lowerName.Contains("coin"))
        {
            return CoinTrans;
        }
        else
        {
            Debug.Log("��������");
            return flashTrans;
        }
    }

    //����HomeUI��button
    public void FreshHomeButton()
    {
        streakButton.gameObject.SetActive(GameDataManager.CurrentGameData.isStreaklocked == false);
        skyRaceButton.gameObject.SetActive(GameDataManager.CurrentGameData.isSkyRacelocked == false);
        buttonLuckySpin.gameObject.SetActive(GameDataManager.CurrentGameData.isLuckySpinlocked == false);
    }

    public void UnlockAllButton()
    {
        streakButton.gameObject.SetActive(true);
        skyRaceButton.gameObject.SetActive(true);
        buttonLuckySpin.gameObject.SetActive(true);
    }
}
