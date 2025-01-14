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


    [SerializeField] private List<int> targetVal = new List<int> { 0, 2, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65 }; // ����Ŀ��λ��
    [SerializeField] private List<Transform> chestObjects = new List<Transform>();
    private List<Vector3> chestPosList = new List<Vector3>();

    [SerializeField] private int preStreakIndex;
    [SerializeField] private int curStreakIndex;
    [SerializeField] private int curMoveIndex = 0;  // ��ǰ����


    [Header("����")]
    [SerializeField] private List<RewardItemGroup> rewardItemList = new List<RewardItemGroup>();
    //�õ���ǰ������
    public List<RewardItem> CurRewardList
    {
        get
        {
            return rewardItemList[curMoveIndex].rewardItems;
        }
    }

    private string chestNameContains = "chest"; // ���������ư���chest

    private TimedObject streakTime;
    //�õ���һ��moveindex��Ŀ��ֵ
    public int GetNextMoveTargetValue()
    {
        return targetVal[GameDataManager.CurrentGameData.curStreakChestIndex + 1];
    }

    //�õ���ǰ�Ѿ�������Ŀ��ֵ
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
        // �������е����ֲ�����������chestObjects
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
    /// ����ʱ���ı�
    /// </summary>
    private void UpdateTimeText()
    {
        if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
            timeText.text = $"{streakTime.remainingTime.Days}d{streakTime.remainingTime.Hours}h{streakTime.remainingTime.Minutes}m";
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            timeText.text = $"{streakTime.remainingTime.Days}��{streakTime.remainingTime.Hours}ʱ{streakTime.remainingTime.Minutes}��";

    }

    /// <summary>
    /// ����������ı�
    /// </summary>
    private void UpdateBallonText()
    {
        balloonText.text = curStreakIndex.ToString();
    }
    /// <summary>
    /// ��ʼ���������ּ�����������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private int ExtractNumberFromName(string name)
    {
        // ʹ��������ʽ��ȡ�����еĵ�һ������
        Regex regex = new Regex(@"\d+");
        Match match = regex.Match(name);
        return match.Success ? int.Parse(match.Value) : 0; // ���û���ҵ����֣�����0
    }
    private void ShowStreakIfo()
    {
        streakInfo.ShowInfo();
    }
    /// <summary>
    /// ��ʾStreakUI
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
    /// ����StreakUI
    /// </summary>
    public void HideWinStreakUI()
    {
        AudioManager.Instance.PlaySFX("Click");
        //EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);
        AnimationUtility.FadeOut(BG);
    }

    /// <summary>
    /// �����ݴӵײ��ƶ����������볡������
    /// </summary>
    public void PlayScrollAnimation()
    {
        AnimationUtility.FadeIn(BG);
        UpdateBallonText();
        // ������ʼλ��Ϊ��ײ�
        SetContentToBottom();
        EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);
        // �����ƶ�������
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
    /// ���� Content ��ʼλ�õ���ײ�
    /// </summary>
    private void SetContentToBottom()
    {
        close.gameObject.SetActive(false);
        // ��ȡ Content �ĸ߶�
        float contentHeight = contentRect.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        balloon.transform.SetParent(contentRect.parent);
        Vector3 oriBalloonPos = balloon.anchoredPosition;

        balloon.anchoredPosition = new Vector3(oriBalloonPos.x, oriBalloonPos.y + 800, oriBalloonPos.z);

        contentRect.anchoredPosition = new Vector2(0, viewportHeight - contentHeight);
    }

    /// <summary>
    /// �ƶ� Content ʹ��������뵽����λ��
    /// </summary>
    public void MoveToNextTarget(System.Action callback = null)
    {
        //��ȡ����
        curStreakIndex = GameDataManager.CurrentGameData.curStreakIndex;
        preStreakIndex = GameDataManager.CurrentGameData.preStreakIndex;
        curMoveIndex = GameDataManager.CurrentGameData.curStreakChestIndex;

        // �����ǰ������֮ǰ������ͬ����ִ�в���
        if (curStreakIndex == preStreakIndex)
        {
            Debug.LogWarning("��ǰ����λ����֮ǰ��ͬ�������ƶ�������");
            return;
        }

        if (curMoveIndex >= targetVal.Count - 1)
        {
            Debug.LogWarning("�Ѿ��������һ��Ŀ��λ�ã�");
            return;
        }


        RectTransform content = scrollRect.content;

        // �ҵ���ǰ��Ŀ��ֵ��Χ����
        int lowerIndex = 0;
        int upperIndex = 0;
        GetRangeIndices(out lowerIndex,out upperIndex);

        // ȷ����ֵ����
        float rangeStart = targetVal[lowerIndex];
        float rangeEnd = targetVal[upperIndex];
        float t = Mathf.Clamp01((curStreakIndex - rangeStart) / (rangeEnd - rangeStart));

        // ��ȡ��������� Y ����
        RectTransform lowerChest = chestObjects[lowerIndex] as RectTransform;
        RectTransform upperChest = chestObjects[upperIndex] as RectTransform;

        if (lowerChest == null || upperChest == null)
        {
            Debug.LogError("��������� RectTransform ��Ч��");
            return;
        }

        float lowerY = lowerChest.localPosition.y;
        float upperY = upperChest.localPosition.y;

        // ����Ŀ�� Y ����
        float targetY = Mathf.Lerp(lowerY, upperY, t);

        // ���� Content ���ƶ�ƫ����
        float balloonY = balloon.localPosition.y;
        float deltaY = balloonY - targetY;

        Vector2 targetAnchorPos = content.anchoredPosition + new Vector2(0, deltaY);

        // ���� ScrollRect ���϶�
        scrollRect.enabled = false;

        // ����������ʱ�Ƶ� Content �ĸ���
        balloon.SetParent(content.parent);

        // ʹ�� DoTween ƽ���ƶ� Content
        content.DOAnchorPos(targetAnchorPos, 1).SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                // �ڶ��������м�� y ֵ
                if (content.position.y <= -129.44f)
                {
                    Debug.Log("Content�Ѿ��ƶ�����ײ�");
                    // ����Ѿ��ﵽ���ƣ�ֱ��ֹͣ����
                    content.DOKill(); // ֹͣ��ǰ����
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
                // ������ɺ���߼�
                balloon.SetParent(content);
                scrollRect.enabled = true;

                if (curStreakIndex == targetVal[curMoveIndex + 1])
                {
                    //���ӱ�������
                    GameDataManager.AddStreakChestIndex();
                    InitChestState();
                }
                GameDataManager.ChangePreStreakIndex();
                //preStreakIndex = curMoveIndex;
                callback?.Invoke();
            });

    }
    /// <summary>
    /// ��ʼ�������Ƿ�Ϊ����״̬
    /// </summary>
    private void InitChestState()
    {
        int unlockChestCount = GameDataManager.CurrentGameData.unlockStreakChestCount;
        for (int i = 0; i < unlockChestCount; i++)
        {
            Sprite openSprite = null;
            //�ҵ���Ӧ��ͼƬ
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
    /// �õ���ǰcurStreakIndex������λ��
    /// </summary>
    /// <param name="lowerIndex"></param>
    /// <param name="upperIndex"></param>
    public void GetRangeIndices(out int lowerIndex, out int upperIndex)
    {
        // ��� curStreakIndex �Ƿ����б�����Ч

        if (curStreakIndex < targetVal[0] || curStreakIndex > targetVal[targetVal.Count - 1])
        {
            Debug.LogWarning("curStreakIndex is out of range of the targetVal list.");
            lowerIndex = -1;
            upperIndex = -1;
            return;
        }

        // ���� targetVal���ҵ� curStreakIndex ��λ��

        for (int i = 0; i < targetVal.Count - 1; i++)
        {
            // �ҵ�ֵ���ڷ�Χ
            if (curStreakIndex >= targetVal[i] && curStreakIndex < targetVal[i + 1])
            {
                lowerIndex = i;
                upperIndex = i + 1;
                return;
            }
        }

        Debug.Log("��ǰcurStreakIndex��ֵ������Ŀ���б���");
        lowerIndex = -1;
        upperIndex = -1;
    }
    //��ʼ��λ��
    public void InitStreakPos()
    {
        preStreakIndex = GameDataManager.CurrentGameData.preStreakIndex;

        if (curMoveIndex >= targetVal.Count - 1)
        {
            Debug.LogWarning("�Ѿ��������һ��Ŀ��λ�ã�");
            return;
        }

        if (scrollRect == null || balloon == null || chestObjects == null || chestObjects.Count < 2)
        {
            Debug.LogError("ScrollRect �� Balloon ��������δ��ȷ���ã�");
            return;
        }

        RectTransform content = scrollRect.content;

        // �ҵ���ǰ��Ŀ��ֵ��Χ����
        int lowerIndex = 0;
        int upperIndex = 0;
        GetRangeIndices(out lowerIndex, out upperIndex);

        // ȷ����ֵ����
        float rangeStart = targetVal[lowerIndex];
        float rangeEnd = targetVal[upperIndex];
        float t = Mathf.Clamp01((preStreakIndex - rangeStart) / (rangeEnd - rangeStart));

        //Debug.Log($"CurStreakIndex: {curStreakIndex}, Range: [{rangeStart}, {rangeEnd}], t: {t}");

        // ��ȡ��������� Y ����
        RectTransform lowerChest = chestObjects[lowerIndex] as RectTransform;
        RectTransform upperChest = chestObjects[upperIndex] as RectTransform;

        if (lowerChest == null || upperChest == null)
        {
            Debug.LogError("��������� RectTransform ��Ч��");
            return;
        }

        float lowerY = lowerChest.localPosition.y;
        float upperY = upperChest.localPosition.y;

        // ����Ŀ�� Y ����
        float targetY = Mathf.Lerp(lowerY, upperY, t);

        // ���� Content ���ƶ�ƫ����
        float balloonY = balloon.localPosition.y;
        float deltaY = balloonY - targetY;

        Vector2 targetAnchorPos = content.anchoredPosition + new Vector2(0, deltaY);

        // ���� ScrollRect ���϶�
        scrollRect.enabled = false;

        // ����������ʱ�Ƶ� Content �ĸ���
        balloon.SetParent(content.parent);

        // ʹ�� DoTween ƽ���ƶ� Content
        content.DOAnchorPos(targetAnchorPos, 0).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // �ƶ���ɺ󣬽�������Ż� Content
                balloon.SetParent(content);

                // �ָ� ScrollRect ���϶�
                scrollRect.enabled = true;
            });
    }

    /// <summary>
    /// ��ʼ�������״̬
    /// </summary>
    // �� Content �ƻس�ʼλ��
    public void RestartStreakEvent(System.Action callback = null)
    {
        AnimationUtility.FadeIn(BG);
        if (scrollRect == null || balloon == null)
        {
            Debug.LogError("ScrollRect �� Balloon δ��ȷ���ã�");
            return;
        }

        //GameDataManager.ResetStreak();

        RectTransform content = scrollRect.content;

        // ���� ScrollRect ���϶�
        scrollRect.enabled = false;

        // ����������ʱ�Ƶ� Content �ĸ���
        balloon.SetParent(content.parent);

        // �ƶ�����ʼλ��
        content.DOAnchorPos(Vector2.zero, 1).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // �ƶ���ɺ󣬽�������Ż� Content
                balloon.SetParent(content);

                // �ָ� ScrollRect ���϶�
                scrollRect.enabled = true;

                callback?.Invoke();

                //����
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
