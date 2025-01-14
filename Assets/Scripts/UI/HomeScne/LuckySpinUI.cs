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
    private Button spinButton; // �齱��ť
    private float spinDuration = 5f; // ת����תʱ��
    //private float spinSpeed = 360f; // ת�̻�����ת�ٶ�
    private bool isSpinning = false; // �Ƿ�������ת
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
            timeText.text = $"{luckSpinTime.remainingTime.Days}��{luckSpinTime.remainingTime.Hours}ʱ";
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
    /// �齱
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
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("û���㹻�ĳ齱Ʊ��");
            }
            Debug.Log("�齱������");
            return;
        }

        if (isSpinning)
        {
            return; // ������ת��δ����ת�̣�ֱ�ӷ���
        }

        //���ٳ齱Ʊ
        GameDataManager.DecreaseSpinCount();
        //���³齱���
        HomeSceneUI.Instance.homeUI.UpdateLuckyPinNoti();
        isSpinning = true;

        spinCountText.text = GameDataManager.CurrentGameData.curSpinCount.ToString();
        // ��ȡת�̵���������������Ʒ������
        int childCount = spinItems.childCount;

        // ���ѡ��һ����Ʒ����
        int chosenIndex = UnityEngine.Random.Range(0, childCount);

        // ÿ����Ʒ�ĽǶ�
        float sectorAngle = 360f / childCount;

        // Ŀ��Ƕȣ�ת����Ȧ + Ŀ�꽱Ʒλ��
        float targetAngle = 360f * 5 + chosenIndex * sectorAngle + 20;

        // ִ����ת����
        spin.DORotate(new Vector3(0, 0, -targetAngle), spinDuration, RotateMode.FastBeyond360)
             .SetEase(Ease.OutQuad)
             .OnComplete(() =>
             {
                 isSpinning = false;
                 //spinButton.interactable = true;

                 // ��ʾ�н����
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

        // ��ȡ�н���Ʒ������
        Transform prize = spinItems.GetChild(index);
        string prizeName = prize.name;
        // ��ʾ���

        Debug.Log($"�н���Ʒ��{prizeName}");
        AddRewardItem(prizeName);
        HandlePrize(prize);
    }
    /// <summary>
    /// �����н��߼������ƽ�Ʒ���Ŵ���С�������ƶ���������
    /// </summary>
    /// <param name="prize">�н����� Transform</param>
    /// <param name="scaleDuration">�Ŵ���С��ʱ��</param>
    /// <param name="moveDistance">�����ƶ��ľ���</param>
    /// <param name="moveDuration">�ƶ��ͽ�����ʱ��</param>
    public void HandlePrize(Transform prize, float scaleDuration = 0.5f, float moveDistance = 8f, float moveDuration = 1.8f)
    {
        // �����н�����
        Transform prizeCopy = Instantiate(prize, prize.position, prize.rotation, prize.parent.parent.parent);
        prizeCopy.name = prize.name + "_Copy";
        GameObject fx = Instantiate(appearFx, prize.position + Vector3.up * 0.2f, prize.rotation, prize.parent.parent.parent);

        // ȷ�����Ƶ������� CanvasGroup ��������ڽ�����
        CanvasGroup canvasGroup = prizeCopy.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = prizeCopy.gameObject.AddComponent<CanvasGroup>();
        }

        // ���ó�ʼ͸����Ϊ 1
        canvasGroup.alpha = 1f;

        // Ŀ��λ��
        Vector3 targetPosition = prizeCopy.position + Vector3.up * moveDistance;

        // ������������
        Sequence sequence = DOTween.Sequence();

        // �������Ŵ�����С
        sequence.Append(prizeCopy.DOScale(1.2f, scaleDuration / 2).SetEase(Ease.OutQuad)) // �Ŵ�
            .OnStart(() =>
            {
                fx.SetActive(true);
                fx.GetComponent<ParticleSystem>().Play();
            })
            .Append(prizeCopy.DOScale(1f, scaleDuration / 2).SetEase(Ease.InQuad));   // ��С

        // �����������ƶ�������
        sequence.Append(prizeCopy.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad)) // �����ƶ�
                .Join(canvasGroup.DOFade(0f, moveDuration).SetEase(Ease.Linear));            // ����

        // ������ɺ����ٸ�������
        sequence.OnComplete(() =>
        {
            Destroy(prizeCopy.gameObject);
            Destroy(fx);
        });

        // ���Ŷ���
        sequence.Play();
    }

    private void AddRewardItem(string input)
    {
        // ������ʽ����ȡ��ĸ����
        string letters = Regex.Replace(input, @"[^a-zA-Z]", "");

        // ������ʽ����ȡ���ֲ���
        string numbers = Regex.Replace(input, @"[^0-9]", "");

        int count = int.Parse(numbers);

        GameDataManager.AddItemCountBySpriteName(letters, count);
        HomeSceneUI.Instance.homeUI.UpdateResourceText();
    }
}
