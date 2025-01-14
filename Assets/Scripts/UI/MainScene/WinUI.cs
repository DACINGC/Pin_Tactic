using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MainBaseUI
{
    private Transform buttonTable;
    private Button buttonNext;
    private Button buttonVideo;
    private CanvasGroup canvasGroupNext;
    private CanvasGroup canvasGroupVideo;

    private Transform topTrans;
    private Text piggyCount;
    private Text coinCount;

    private Transform future;
    private Image fillImage;
    private Image fillIcon;
    private Text fillText;
    [SerializeField] private int curCount;//��ǰ�����Ľ���
    [SerializeField] private int fillCount = 10;

    public Transform CoinTrans { get; set; }
    public Transform PiggyTrans { get; set; }
    [SerializeField] private ParticleSystem ps1;
    [SerializeField] private ParticleSystem ps2;

    [Header("��������")]
    [SerializeField] private Sprite starPin;
    [SerializeField] private Sprite rope;
    [SerializeField] private Sprite ice;
    [SerializeField] private Sprite door;
    [SerializeField] private Sprite boom;
    [SerializeField] private Sprite chain;
    [SerializeField] private Sprite key;
    [SerializeField] private Sprite clock;
    public Text PiggyText
    {
        get => piggyCount;
    }
    public Text CoinText
    {
        get => coinCount;
    }

    private bool isClick = false;//ֻ������һ��
    protected override void Awake()
    {
        base.Awake();

        // ��ȡ��ť��������
        buttonTable = tableTransform.Find("Table");
        buttonNext = buttonTable.Find("Button Next").GetComponent<Button>();
        buttonVideo = buttonTable.Find("Button Video").GetComponent<Button>();

        // ��Ӱ�ť����¼�
        buttonNext.onClick.AddListener(OnNextButtonClicked);
        buttonVideo.onClick.AddListener(OnVideoButtonClicked);

        // ��ʼ�� CanvasGroup�����ڿ���͸���ȣ�
        canvasGroupNext = buttonNext.gameObject.AddComponent<CanvasGroup>();
        canvasGroupVideo = buttonVideo.gameObject.AddComponent<CanvasGroup>();

        topTrans = tableTransform.parent.Find("Top").transform;
        piggyCount = topTrans.Find("Piggy/Count").GetComponent<Text>();
        coinCount = topTrans.Find("Total Coin/Count").GetComponent<Text>();

        PiggyTrans = topTrans.Find("Piggy/Icon Piggy Coin All").transform;
        CoinTrans = topTrans.Find("Total Coin/Icon Coin All").transform;

        future = tableTransform.Find("Contain Feature/future").transform;
        fillImage = future.Find("Image").GetComponent<Image>();
        fillIcon = future.Find("Icon").GetComponent<Image>();
        fillText = fillImage.transform.GetComponentInChildren<Text>();

        // ���ó�ʼ״̬
        ResetButtonState();
    }
    private void InitPs()
    {
        SetPsOrinPos();
        ps1.gameObject.SetActive(true);
        ps2.gameObject.SetActive(true);
        Invoke(nameof(DelaySetPSPos), 0.8f);
    }
    private void ResetButtonState()
    {
        // ��ʼ����ť��С��͸����
        buttonNext.transform.localScale = Vector3.zero;
        buttonVideo.transform.localScale = Vector3.zero;
        canvasGroupNext.alpha = 0;
        canvasGroupVideo.alpha = 0;
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(PlayShowAnimation, effectType);
        AudioManager.Instance.PlaySFX("Win");
        AudioManager.Instance.PlaySFX("WinNoti");

        isClick = false;
        piggyCount.text = GameDataManager.CurrentGameData.piggyCount.ToString();
        coinCount.text = GameDataManager.CurrentGameData.coinCount.ToString();

        InitPs();

        int curLevelNum = LevelManager.Instance.GetLevleNum();
        switch (curLevelNum)
        {
            case 1:
                //��ʼ���� ������˿
                GameDataManager.ReStartFill(10, starPin.name);
                break;
            case 11:
                //��ʼ���� ����
                GameDataManager.ReStartFill(15, rope.name);
                break;
            case 26:
                //��ʼ���� ��
                GameDataManager.ReStartFill(15, ice.name);
                break;
            case 41:
                //��ʼ���� ��
                GameDataManager.ReStartFill(15, door.name);
                break;
            case 56:
                //��ʼ���� ը��
                GameDataManager.ReStartFill(15, boom.name);
                break;
            case 71:
                //��ʼ���� ����
                GameDataManager.ReStartFill(15, chain.name);
                break;
            case 86:
                //��ʼ����Կ��
                GameDataManager.ReStartFill(15, key.name);
                break;
            case 101:
                //��ʼ��������
                GameDataManager.ReStartFill(15, clock.name);
                break;
            case 116:
                //�����Ѿ��������
                future.parent.gameObject.SetActive(false);
                break;
        }

        if (curLevelNum >= 10 && GameDataManager.CurrentGameData.isStreaklocked == false)
        {
            //11�ؿ�ʼ��ʼ����Streak
            GameDataManager.AddStreak();
        }

        if (curLevelNum >= 20 && GameDataManager.CurrentGameData.isLuckySpinlocked == false)
        {
            //��20�ؿ�ʼ���ӳ齱Ʊ����
            GameDataManager.AddSpinProgress();
        }

        //�õ���ǰ����ͼƬ
        Sprite curSprite = GetSpriteByName(GameDataManager.CurrentGameData.fillSpriteName);
        fillIcon.sprite = curSprite;
        LevelManager.Instance.UnlockSprite = curSprite;

        //���ӵ�ǰ�������
        GameDataManager.AddCurFillCount();
        curCount = GameDataManager.CurrentGameData.curFillCount;
        fillCount = GameDataManager.CurrentGameData.allfillCount;

        fillImage.fillAmount = curCount / (float)fillCount;
        fillText.text = $"{curCount}/{fillCount}";

        //���ӵ�ǰ�Ĺؿ���
        LevelManager.Instance.AddLevelNum();
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        //��ť��ʧ
        PlayHideAnimation();

        float hideDelayTime = 1.2f;

        if (LevelManager.Instance.GetLevleNum() <= 5)
        {
            //��ӽ�ң��������ݴ洢
            int prePiggyCount = GameDataManager.CurrentGameData.piggyCount;
            int preCoinCount = GameDataManager.CurrentGameData.coinCount;
            GameDataManager.AddCoinCount(10, 30);
            int curPiggyCount = GameDataManager.CurrentGameData.piggyCount;
            int curCoinCount = GameDataManager.CurrentGameData.coinCount;


            ItemMoveManager.Instance.MoveCoin(preCoinCount, curCoinCount, prePiggyCount, curPiggyCount);
        }
        else
        {
            hideDelayTime = 0;
        }

        base.HideUI(hideDelayTime, () =>
        {
            ps1.gameObject.SetActive(false);
            ps2.gameObject.SetActive(false);
            HideCallBack();
        });
    }

    public Sprite GetSpriteByName(string name)
    {
        // �������е�Sprite���Ա����ǵ�name
        if (starPin.name == name) return starPin;
        else if (rope.name == name) return rope;
        else if (ice.name == name) return ice;
        else if (door.name == name) return door;
        else if (boom.name == name) return boom;
        else if (chain.name == name) return chain;
        else if (key.name == name) return key;
        else if (clock.name == name) return clock;

        // ���û���ҵ�ƥ���Sprite
        Debug.LogWarning($"Sprite with name '{name}' not found.");
        return null;
    }
    /// <summary>
    /// ��ť��С����Ķ���
    /// </summary>
    private void PlayShowAnimation()
    {
        // ��ť��С���󲢽���
        buttonNext.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        canvasGroupNext.DOFade(1, 0.5f);

        buttonVideo.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        canvasGroupVideo.DOFade(1, 0.5f);
    }

    private void PlayHideAnimation()
    {
        // ��ť�Ӵ�С������
        buttonNext.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        canvasGroupNext.DOFade(0, 0.5f);

        buttonVideo.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        canvasGroupVideo.DOFade(0, 0.5f);
    }

    private void OnNextButtonClicked()
    {
        if (isClick)
            return;
        isClick = true;

        LevelManager.Instance.ClearLevel();
        UIManager.Instance.HideUI<WinUI>();
    }

    private static void HideCallBack()
    {
        //�ؿ�С��5�����next��ťֱ��ȥ����һ��
        if (LevelManager.Instance.GetLevleNum() <= 5)
        {
            LevelManager.Instance.InitLevel();
        }
        else
        {
            //���ص�������

            GameManager.Instance.EnterHomeScene();

            //��ӽ�ң��������ݴ洢
            int prePiggyCount = GameDataManager.CurrentGameData.piggyCount;
            int preCoinCount = GameDataManager.CurrentGameData.coinCount;
            GameDataManager.AddCoinCount(10, 30);
            int curPiggyCount = GameDataManager.CurrentGameData.piggyCount;
            int curCoinCount = GameDataManager.CurrentGameData.coinCount;


            ItemMoveManager.Instance.MoveCoin(preCoinCount, curCoinCount, prePiggyCount, curPiggyCount);
            //���������ƶ�����
            ItemMoveManager.Instance.MoveAndRotateStar();

            //��11�ؿ�ʼ���ƶ�Arrow
            if (LevelManager.Instance.GetLevleNum() >= 11 && GameDataManager.CurrentGameData.isStreaklocked == false)
            {
                HomeSceneUI.Instance.homeUI.HomeItemMove.MoveArrowAnim();
            }

            //��20�ؿ����ƶ�sipn
            if (LevelManager.Instance.GetLevleNum() >= 20 && GameDataManager.CurrentGameData.isLuckySpinlocked == false)
            {
                HomeSceneUI.Instance.homeUI.HomeItemMove.MoveSpinAnim();
            }
            HomeSceneUI.Instance.homeUI.UpdateHomePlayText();
        }
    }
    private void OnVideoButtonClicked()
    {
        // ���������ﶨ����Ƶ��ť�Ĺ���
        Debug.Log("Video button clicked!");
    }
    private void SetPsOrinPos()
    {
        ps1.transform.position = new Vector3(-6, -6, 0);
        ps2.transform.position = new Vector3(6, -6, 0);
    }
    private void DelaySetPSPos()
    {
        ps1.transform.position = new Vector3(-6, 12, 0);
        ps2.transform.position = new Vector3(6, 12, 0);
    }
}
