using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : BaseUI
{
    private Transform baseContent;
    private Transform coinContent;
    private Button moreButton;
    private Button coinMoreButton;

    private Transform moreScrollView;
    private Transform moreContent;
    private Vector3 originPos;
    private Button closeButton;



    protected override void Awake()
    {
        base.Awake();
        baseContent = tableTransform.Find("BaseContent").transform;
        coinContent = tableTransform.Find("CoinContent");

        moreButton = baseContent.Find("MoreButton").GetComponent<Button>();
        coinMoreButton = coinContent.Find("MoreButton").GetComponent<Button>();
        moreButton.onClick.AddListener(MoreEvent);
        coinMoreButton.onClick.AddListener(CoinMoreEvent);

        moreScrollView = tableTransform.Find("MoreScrollView").transform;
        moreContent = moreScrollView.Find("Viewport/MoreContent").transform;
        originPos = moreContent.position;

        closeButton = tableTransform.Find("Top/Button Close").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseEvent);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Fade);
    }

    public void EnterShop()
    {
        baseContent.gameObject.SetActive(true);
        moreScrollView.gameObject.SetActive(false);
        coinContent.gameObject.SetActive(false);
        PlayEnterAnimation(baseContent);
    }

    public void EnterCoin()
    {
        coinContent.gameObject.SetActive(true);
        moreScrollView.gameObject.SetActive(false);
        baseContent.gameObject.SetActive(false);
        PlayEnterAnimation(coinContent);
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Fade);
    }
    private void MoreEvent()
    {
        baseContent.gameObject.SetActive(false);
        coinContent.gameObject.SetActive(false);
        moreScrollView.gameObject.SetActive(true);
        moreContent.position = originPos;

        PlayEnterAnimation(moreContent);
    }

    private void CoinMoreEvent()
    {
        coinContent.gameObject.SetActive(false);
        baseContent.gameObject.SetActive(false);
        moreScrollView.gameObject.SetActive(true);
        moreContent.position = originPos;

        PlayEnterAnimation(moreContent);
    }
    private void CloseEvent()
    {
        UIManager.Instance.HideUI<ShopUI>();
    }
    // ÿ����������ӳ�ʱ��
    private float delayBetweenChildren = 0.08f;
    // ����ʱ��
    private float duration = 0.5f;
    // �ص�Ч����ƫ����
    private float bounceOffset = -1f;

    public void PlayEnterAnimation(Transform parent, float screenOffsetX = 0f, System.Action onComplete = null)
    {
        int childCount = parent.childCount;
        Vector3[] targetPositions = new Vector3[childCount];

        // ��ȡÿ��������Ŀ��λ��
        for (int i = 0; i < childCount; i++)
        {
            targetPositions[i] = parent.GetChild(i).position;
        }

        // ����һ����������
        Sequence sequence = DOTween.Sequence();

        // ��ȡ��Ļ���
        float screenWidth = Screen.width;

        // Ϊÿ�����������ö���
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // ��ȡ������� RectTransform
            RectTransform childRect = child.GetComponent<RectTransform>();
            float childWidth = childRect.rect.width;

            // �����ʼλ�ã�ʹ�������Ļ�Ҳ��ⲿ����
            Vector3 offScreenRight = new Vector3(screenWidth + childWidth / 2 + screenOffsetX, child.localPosition.y, child.localPosition.z);

            // ���ó�ʼλ��Ϊ��Ļ���Ҳ�
            child.localPosition = offScreenRight;

            // �ֲ�������ֹ�հ���������
            int index = i;
            sequence.Insert(
                index * delayBetweenChildren,
                child.DOMoveX(targetPositions[index].x + bounceOffset, duration * 0.7f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                        // ʹ�þֲ�����ȷ��������ȷ������Ŀ��λ��
                        child.DOMoveX(targetPositions[index].x, duration * 0.3f).SetEase(Ease.OutBack);
                     })
            );
        }

        // ������ɺ���ûص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }


}
