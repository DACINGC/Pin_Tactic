using DG.Tweening;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpinUI : BaseUI
{
    private Transform top;
    private Button close;
    private Button infoButton;
    private InfoUI info;
    private Transform processTrans;
    private Image processImage;
    private Text processText;
    private Text spinCountText;
    private Text timeText;
    private TimedObject luckSpinTime;

    private Transform spin;
    private Transform spinItems;
    private Button spinButton; // 抽奖按钮
    private float spinDuration = 5f; // 转盘旋转时长
    //private float spinSpeed = 360f; // 转盘基础旋转速度
    private bool isSpinning = false; // 是否正在旋转
    [SerializeField] private GameObject appearFx;

    protected override void Awake()
    {
        base.Awake();
        top = tableTransform.Find("Top");
        close = top.Find("Button Close").GetComponent<Button>();
        infoButton = top.Find("Button Infomation").GetComponent<Button>();
        info = transform.Find("UI Infomation").GetComponent<InfoUI>();
        processTrans = tableTransform.Find("process");
        processImage = processTrans.Find("ProcessImage").GetComponent<Image>();
        processText = processTrans.Find("ProcessText").GetComponent<Text>();
        spinCountText = tableTransform.Find("Contain Card/Text").GetComponent<Text>();
        timeText = processTrans.Find("Text time").GetComponent<Text>();

        spin = tableTransform.Find("FrameSpin/Spin");
        spinItems = spin.Find("Spin Item");
        spinButton = tableTransform.Find("Contain Button Spin/Button").GetComponent<Button>();

        infoButton.onClick.AddListener(ShowInfo);
        close.onClick.AddListener(CloseEvent);
        spinButton.onClick.AddListener(StartSpin);
    }
    private void Start()
    {
        luckSpinTime = TimeManager.Instance.GetTimeObj(TimeEventType.LuckySpin);
    }
    private void Update()
    {
        if (luckSpinTime != null)
        {
            UpdateLuckSpinTimeText();
        }
    }

    private void UpdateLuckSpinTimeText()
    {
        if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
            timeText.text = $"{luckSpinTime.remainingTime.Days}d{luckSpinTime.remainingTime.Hours}h";
        else if(YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            timeText.text = $"{luckSpinTime.remainingTime.Days}天{luckSpinTime.remainingTime.Hours}时";
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Fade);
        UpdateLuckSpinProcess();
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Fade);
    }

    private void ShowInfo()
    {
        info.ShowInfo();
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<LuckySpinUI>();
    }

    private void UpdateLuckSpinProcess()
    {
        processImage.fillAmount = GameDataManager.CurrentGameData.curSpinProgress / 10.0f;
        processText.text = $"{GameDataManager.CurrentGameData.curSpinProgress} / 10";
        spinCountText.text = GameDataManager.CurrentGameData.curSpinCount.ToString();
    }

    /// <summary>
    /// 抽奖
    /// </summary>
    private void StartSpin()
    {
        AudioManager.Instance.PlaySFX("Click");
        if (GameDataManager.CurrentGameData.curSpinCount <= 0)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("THERE AREN'T ENOUGH DRAWS!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("没有足够的抽奖票！");
            }
            Debug.Log("抽奖数不够");
            return;
        }

        if (isSpinning)
        {
            return; // 正在旋转或未设置转盘，直接返回
        }

        //减少抽奖票
        GameDataManager.DecreaseSpinCount();
        //更新抽奖红点
        HomeSceneUI.Instance.homeUI.UpdateLuckyPinNoti();
        isSpinning = true;

        spinCountText.text = GameDataManager.CurrentGameData.curSpinCount.ToString();
        // 获取转盘的子物体数量（奖品数量）
        int childCount = spinItems.childCount;

        // 随机选择一个奖品索引
        int chosenIndex = UnityEngine.Random.Range(0, childCount);

        // 每个奖品的角度
        float sectorAngle = 360f / childCount;

        // 目标角度：转若干圈 + 目标奖品位置
        float targetAngle = 360f * 5 + chosenIndex * sectorAngle + 20;

        // 执行旋转动画
        spin.DORotate(new Vector3(0, 0, -targetAngle), spinDuration, RotateMode.FastBeyond360)
             .SetEase(Ease.OutQuad)
             .OnComplete(() =>
             {
                 isSpinning = false;
                 //spinButton.interactable = true;

                 // 显示中奖结果
                 ShowPrize(chosenIndex);
             });
    }

    private void ShowPrize(int index)
    {
        if (index < 0 || index >= spinItems.childCount)
        {
            Debug.LogError("Invalid prize index!");
            return;
        }

        // 获取中奖奖品的名称
        Transform prize = spinItems.GetChild(index);
        string prizeName = prize.name;
        // 显示结果

        Debug.Log($"中奖奖品：{prizeName}");
        AddRewardItem(prizeName);
        HandlePrize(prize);
    }
    /// <summary>
    /// 处理中奖逻辑：复制奖品，放大缩小后向上移动并渐隐。
    /// </summary>
    /// <param name="prize">中奖物体 Transform</param>
    /// <param name="scaleDuration">放大缩小的时长</param>
    /// <param name="moveDistance">向上移动的距离</param>
    /// <param name="moveDuration">移动和渐隐的时长</param>
    public void HandlePrize(Transform prize, float scaleDuration = 0.5f, float moveDistance = 8f, float moveDuration = 1.8f)
    {
        // 复制中奖物体
        Transform prizeCopy = Instantiate(prize, prize.position, prize.rotation, prize.parent.parent.parent);
        prizeCopy.name = prize.name + "_Copy";
        GameObject fx = Instantiate(appearFx, prize.position + Vector3.up * 0.2f, prize.rotation, prize.parent.parent.parent);

        // 确保复制的物体有 CanvasGroup 组件（用于渐隐）
        CanvasGroup canvasGroup = prizeCopy.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = prizeCopy.gameObject.AddComponent<CanvasGroup>();
        }

        // 设置初始透明度为 1
        canvasGroup.alpha = 1f;

        // 目标位置
        Vector3 targetPosition = prizeCopy.position + Vector3.up * moveDistance;

        // 创建动画序列
        Sequence sequence = DOTween.Sequence();

        // 动画：放大再缩小
        sequence.Append(prizeCopy.DOScale(1.2f, scaleDuration / 2).SetEase(Ease.OutQuad)) // 放大
            .OnStart(() =>
            {
                fx.SetActive(true);
                fx.GetComponent<ParticleSystem>().Play();
            })
            .Append(prizeCopy.DOScale(1f, scaleDuration / 2).SetEase(Ease.InQuad));   // 缩小

        // 动画：向上移动并渐隐
        sequence.Append(prizeCopy.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad)) // 向上移动
                .Join(canvasGroup.DOFade(0f, moveDuration).SetEase(Ease.Linear));            // 渐隐

        // 动画完成后销毁复制物体
        sequence.OnComplete(() =>
        {
            Destroy(prizeCopy.gameObject);
            Destroy(fx);
        });

        // 播放动画
        sequence.Play();
    }

    private void AddRewardItem(string input)
    {
        // 正则表达式：提取字母部分
        string letters = Regex.Replace(input, @"[^a-zA-Z]", "");

        // 正则表达式：提取数字部分
        string numbers = Regex.Replace(input, @"[^0-9]", "");

        int count = int.Parse(numbers);

        GameDataManager.AddItemCountBySpriteName(letters, count);
        HomeSceneUI.Instance.homeUI.UpdateResourceText();
    }
}
