using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class WinStreakUI : MonoBehaviour
{
    private Transform BG;
    private Transform top;
    private Button infoButton;
    private Button close;
    private Text timeText;

    private ScrollRect scrollRect;
    private RectTransform contentRect;

    private RectTransform balloon;
    private Text balloonText;
    private InfoUI streakInfo;


    [SerializeField] private List<int> targetVal = new List<int> { 0, 2, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65 }; // 所有目标位置
    [SerializeField] private List<Transform> chestObjects = new List<Transform>();
    private List<Vector3> chestPosList = new List<Vector3>();

    [SerializeField] private int preStreakIndex;
    [SerializeField] private int curStreakIndex;
    [SerializeField] private int curMoveIndex = 0;  // 当前索引


    [Header("奖励")]
    [SerializeField] private List<RewardItemGroup> rewardItemList = new List<RewardItemGroup>();
    //得到当前奖励物
    public List<RewardItem> CurRewardList
    {
        get
        {
            return rewardItemList[curMoveIndex].rewardItems;
        }
    }

    private string chestNameContains = "chest"; // 子物体名称包含chest

    private TimedObject streakTime;
    //得到下一个moveindex的目标值
    public int GetNextMoveTargetValue()
    {
        return targetVal[GameDataManager.CurrentGameData.curStreakChestIndex + 1];
    }

    //得到当前已经解锁的目标值
    public int GetCurMoveTargetValue()
    {
        return targetVal[GameDataManager.CurrentGameData.curStreakChestIndex];
    }
    private void Awake()
    {
        BG = transform.Find("BG");
        top = BG.Find("Top");
        scrollRect = BG.Find("Scroll View").GetComponent<ScrollRect>();
        contentRect = scrollRect.content;
        timeText = top.Find("time/time").GetComponent<Text>();

        infoButton = top.Find("Button Infomation").GetComponent<Button>();
        infoButton.onClick.AddListener(ShowStreakIfo);
        close = top.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(HideWinStreakUI);

        balloon = contentRect.Find("Balloon").GetComponent<RectTransform>();
        balloonText = balloon.Find("Image/count").GetComponent<Text>();
        streakInfo = transform.Find("UI Infomation").GetComponent<InfoUI>();

    }
    private void Start()
    {
        streakTime = TimeManager.Instance.GetTimeObj(TimeEventType.Streak);

        foreach (Transform t in contentRect)
        {
            if (t.name.Contains(chestNameContains))
            {
                chestObjects.Add(t);
                chestPosList.Add(t.GetComponent<RectTransform>().position);
            }
        }
        // 按名称中的数字部分升序排序chestObjects
        chestObjects.Sort((x, y) => ExtractNumberFromName(x.name).CompareTo(ExtractNumberFromName(y.name)));

        curStreakIndex = GameDataManager.CurrentGameData.curStreakIndex;
        curMoveIndex = GameDataManager.CurrentGameData.curStreakChestIndex;

        EventManager.Instance.RegisterEvent(GameEvent.UpdateStreakEvent, UpdateBallonText);

        UpdateBallonText();

        InitStreakPos();
        InitChestState();
    }

    private void Update()
    {
        if (streakTime != null)
        {
            HomeSceneUI.Instance.homeUI.UpdateStreakTimeText();
            UpdateTimeText();
        }

    }

    /// <summary>
    /// 更新时间文本
    /// </summary>
    private void UpdateTimeText()
    {
        if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
            timeText.text = $"{streakTime.remainingTime.Days}d{streakTime.remainingTime.Hours}h{streakTime.remainingTime.Minutes}m";
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            timeText.text = $"{streakTime.remainingTime.Days}天{streakTime.remainingTime.Hours}时{streakTime.remainingTime.Minutes}秒";

    }

    /// <summary>
    /// 更新气球的文本
    /// </summary>
    private void UpdateBallonText()
    {
        balloonText.text = curStreakIndex.ToString();
    }
    /// <summary>
    /// 初始化，按名字计算物体排名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private int ExtractNumberFromName(string name)
    {
        // 使用正则表达式提取名称中的第一个数字
        Regex regex = new Regex(@"\d+");
        Match match = regex.Match(name);
        return match.Success ? int.Parse(match.Value) : 0; // 如果没有找到数字，返回0
    }
    private void ShowStreakIfo()
    {
        streakInfo.ShowInfo();
    }
    /// <summary>
    /// 显示StreakUI
    /// </summary>
    /// <param name="callback"></param>
    public void ShowWinStreakUI(System.Action callback = null)
    {
        AudioManager.Instance.PlaySFX("Click");
        AnimationUtility.FadeIn(BG);
        //MoveBalloon();
        UpdateBallonText();
        MoveToNextTarget(callback);
    }
    /// <summary>
    /// 隐藏StreakUI
    /// </summary>
    public void HideWinStreakUI()
    {
        AudioManager.Instance.PlaySFX("Click");
        //EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);
        AnimationUtility.FadeOut(BG);
    }

    /// <summary>
    /// 将内容从底部移动到顶部（入场动画）
    /// </summary>
    public void PlayScrollAnimation()
    {
        AnimationUtility.FadeIn(BG);
        UpdateBallonText();
        // 设置起始位置为最底部
        SetContentToBottom();
        EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);
        // 动画移动到顶部
        scrollRect.enabled = false;
        contentRect.DOAnchorPosY(0, 3f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                balloon.transform.SetParent(contentRect);
                balloon.DOAnchorPosY(chestObjects[0].GetComponent<RectTransform>().anchoredPosition.y, 1.5f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    scrollRect.enabled = true;
                    close.gameObject.SetActive(true);
                });
               
            });
    }

    /// <summary>
    /// 设置 Content 初始位置到最底部
    /// </summary>
    private void SetContentToBottom()
    {
        close.gameObject.SetActive(false);
        // 获取 Content 的高度
        float contentHeight = contentRect.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        balloon.transform.SetParent(contentRect.parent);
        Vector3 oriBalloonPos = balloon.anchoredPosition;

        balloon.anchoredPosition = new Vector3(oriBalloonPos.x, oriBalloonPos.y + 800, oriBalloonPos.z);

        contentRect.anchoredPosition = new Vector2(0, viewportHeight - contentHeight);
    }

    /// <summary>
    /// 移动 Content 使热气球对齐到奖励位置
    /// </summary>
    public void MoveToNextTarget(System.Action callback = null)
    {
        //获取数据
        curStreakIndex = GameDataManager.CurrentGameData.curStreakIndex;
        preStreakIndex = GameDataManager.CurrentGameData.preStreakIndex;
        curMoveIndex = GameDataManager.CurrentGameData.curStreakChestIndex;

        // 如果当前索引与之前索引相同，则不执行操作
        if (curStreakIndex == preStreakIndex)
        {
            Debug.LogWarning("当前奖励位置与之前相同，跳过移动操作！");
            return;
        }

        if (curMoveIndex >= targetVal.Count - 1)
        {
            Debug.LogWarning("已经到达最后一个目标位置！");
            return;
        }


        RectTransform content = scrollRect.content;

        // 找到当前的目标值范围索引
        int lowerIndex = 0;
        int upperIndex = 0;
        GetRangeIndices(out lowerIndex,out upperIndex);

        // 确定插值比例
        float rangeStart = targetVal[lowerIndex];
        float rangeEnd = targetVal[upperIndex];
        float t = Mathf.Clamp01((curStreakIndex - rangeStart) / (rangeEnd - rangeStart));

        // 获取奖励物体的 Y 坐标
        RectTransform lowerChest = chestObjects[lowerIndex] as RectTransform;
        RectTransform upperChest = chestObjects[upperIndex] as RectTransform;

        if (lowerChest == null || upperChest == null)
        {
            Debug.LogError("奖励物体的 RectTransform 无效！");
            return;
        }

        float lowerY = lowerChest.localPosition.y;
        float upperY = upperChest.localPosition.y;

        // 计算目标 Y 坐标
        float targetY = Mathf.Lerp(lowerY, upperY, t);

        // 计算 Content 的移动偏移量
        float balloonY = balloon.localPosition.y;
        float deltaY = balloonY - targetY;

        Vector2 targetAnchorPos = content.anchoredPosition + new Vector2(0, deltaY);

        // 禁用 ScrollRect 的拖动
        scrollRect.enabled = false;

        // 将热气球暂时移到 Content 的父级
        balloon.SetParent(content.parent);

        // 使用 DoTween 平滑移动 Content
        content.DOAnchorPos(targetAnchorPos, 1).SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                // 在动画过程中检查 y 值
                if (content.position.y <= -129.44f)
                {
                    Debug.Log("Content已经移动到最底部");
                    // 如果已经达到限制，直接停止动画
                    content.DOKill(); // 停止当前动画
                    content.position = new Vector2(content.position.x, -129.44f);

                    balloon.SetParent(content);
                    scrollRect.enabled = true;

                    if (curStreakIndex == targetVal[curMoveIndex + 1])
                    {
                        GameDataManager.AddStreakChestIndex();
                        InitChestState();
                    }
                    GameDataManager.ChangePreStreakIndex();
                    //preStreakIndex = curMoveIndex;
                    callback?.Invoke();
                }
            })
            .OnComplete(() =>
            {
                // 其他完成后的逻辑
                balloon.SetParent(content);
                scrollRect.enabled = true;

                if (curStreakIndex == targetVal[curMoveIndex + 1])
                {
                    //增加宝箱索引
                    GameDataManager.AddStreakChestIndex();
                    InitChestState();
                }
                GameDataManager.ChangePreStreakIndex();
                //preStreakIndex = curMoveIndex;
                callback?.Invoke();
            });

    }
    /// <summary>
    /// 初始化宝箱是否为开启状态
    /// </summary>
    private void InitChestState()
    {
        int unlockChestCount = GameDataManager.CurrentGameData.unlockStreakChestCount;
        for (int i = 0; i < unlockChestCount; i++)
        {
            Sprite openSprite = null;
            //找到对应的图片
            switch (i)
            {
                case 1:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("1open");
                    break;
                case 2:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("2open");
                    break;
                case 3:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("3open");
                    break;
                case 4:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("4open");
                    break;
                case 5:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("5open");
                    break;
                case 6:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("6open");
                    break;
                case 7:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("7open");
                    break;
                case 8:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("8open");
                    break;
                case 9:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("9open");
                    break;
                case 10:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("10open");
                    break;
                case 11:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("11open");
                    break;
                case 12:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("12open");
                    break;
                case 13:
                    openSprite = ResourceLoader.Instance.GetUnlockImageSprite("13open");
                    break;
            }
            if (i > 0 && i < 13)
            {
                //Debug.Log(chestObjects[i].name);
                chestObjects[i].Find("Button").GetComponent<Image>().sprite = openSprite;
            }
            else if (i == 13)
                chestObjects[i].Find("chestStreak/Button").GetComponent<Image>().sprite = openSprite;
        }
    }
    /// <summary>
    /// 得到当前curStreakIndex的所处位置
    /// </summary>
    /// <param name="lowerIndex"></param>
    /// <param name="upperIndex"></param>
    public void GetRangeIndices(out int lowerIndex, out int upperIndex)
    {
        // 检查 curStreakIndex 是否在列表中有效

        if (curStreakIndex < targetVal[0] || curStreakIndex > targetVal[targetVal.Count - 1])
        {
            Debug.LogWarning("curStreakIndex is out of range of the targetVal list.");
            lowerIndex = -1;
            upperIndex = -1;
            return;
        }

        // 遍历 targetVal，找到 curStreakIndex 的位置

        for (int i = 0; i < targetVal.Count - 1; i++)
        {
            // 找到值所在范围
            if (curStreakIndex >= targetVal[i] && curStreakIndex < targetVal[i + 1])
            {
                lowerIndex = i;
                upperIndex = i + 1;
                return;
            }
        }

        Debug.Log("当前curStreakIndex的值，不在目标列表中");
        lowerIndex = -1;
        upperIndex = -1;
    }
    //初始化位置
    public void InitStreakPos()
    {
        preStreakIndex = GameDataManager.CurrentGameData.preStreakIndex;

        if (curMoveIndex >= targetVal.Count - 1)
        {
            Debug.LogWarning("已经到达最后一个目标位置！");
            return;
        }

        if (scrollRect == null || balloon == null || chestObjects == null || chestObjects.Count < 2)
        {
            Debug.LogError("ScrollRect 或 Balloon 或奖励物体未正确设置！");
            return;
        }

        RectTransform content = scrollRect.content;

        // 找到当前的目标值范围索引
        int lowerIndex = 0;
        int upperIndex = 0;
        GetRangeIndices(out lowerIndex, out upperIndex);

        // 确定插值比例
        float rangeStart = targetVal[lowerIndex];
        float rangeEnd = targetVal[upperIndex];
        float t = Mathf.Clamp01((preStreakIndex - rangeStart) / (rangeEnd - rangeStart));

        //Debug.Log($"CurStreakIndex: {curStreakIndex}, Range: [{rangeStart}, {rangeEnd}], t: {t}");

        // 获取奖励物体的 Y 坐标
        RectTransform lowerChest = chestObjects[lowerIndex] as RectTransform;
        RectTransform upperChest = chestObjects[upperIndex] as RectTransform;

        if (lowerChest == null || upperChest == null)
        {
            Debug.LogError("奖励物体的 RectTransform 无效！");
            return;
        }

        float lowerY = lowerChest.localPosition.y;
        float upperY = upperChest.localPosition.y;

        // 计算目标 Y 坐标
        float targetY = Mathf.Lerp(lowerY, upperY, t);

        // 计算 Content 的移动偏移量
        float balloonY = balloon.localPosition.y;
        float deltaY = balloonY - targetY;

        Vector2 targetAnchorPos = content.anchoredPosition + new Vector2(0, deltaY);

        // 禁用 ScrollRect 的拖动
        scrollRect.enabled = false;

        // 将热气球暂时移到 Content 的父级
        balloon.SetParent(content.parent);

        // 使用 DoTween 平滑移动 Content
        content.DOAnchorPos(targetAnchorPos, 0).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 移动完成后，将热气球放回 Content
                balloon.SetParent(content);

                // 恢复 ScrollRect 的拖动
                scrollRect.enabled = true;
            });
    }

    /// <summary>
    /// 初始化宝箱的状态
    /// </summary>
    // 将 Content 移回初始位置
    public void RestartStreakEvent(System.Action callback = null)
    {
        AnimationUtility.FadeIn(BG);
        if (scrollRect == null || balloon == null)
        {
            Debug.LogError("ScrollRect 或 Balloon 未正确设置！");
            return;
        }

        //GameDataManager.ResetStreak();

        RectTransform content = scrollRect.content;

        // 禁用 ScrollRect 的拖动
        scrollRect.enabled = false;

        // 将热气球暂时移到 Content 的父级
        balloon.SetParent(content.parent);

        // 移动到初始位置
        content.DOAnchorPos(Vector2.zero, 1).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 移动完成后，将热气球放回 Content
                balloon.SetParent(content);

                // 恢复 ScrollRect 的拖动
                scrollRect.enabled = true;

                callback?.Invoke();

                //渐隐
                AnimationUtility.FadeOut(BG);
            });
    }

    public void AddStickerIndex()
    {
        curStreakIndex++;
        if (curStreakIndex == targetVal[curMoveIndex + 1])
        {
            ShowWinStreakUI();
        }
    }
    public void DebuggContentPos()
    {
        Debug.Log(contentRect.position);
    }

}
