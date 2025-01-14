using UnityEngine;
using DG.Tweening;

public class BaseUI : MonoBehaviour
{
    public float animationTime = 0.5f; // ����ʱ��
    private CanvasGroup canvasGroup;
    protected RectTransform bg;
    protected RectTransform tableTransform;
    public enum UIEffectType
    {
        Slide, // ��������
        Scale,    // ��С����
        Fade   //����
    }
    protected virtual void Awake()
    {
        bg = transform.Find("BG").GetComponent<RectTransform>();
        tableTransform = transform.Find("BG/Table").GetComponent<RectTransform>();
        canvasGroup = tableTransform.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = tableTransform.gameObject.AddComponent<CanvasGroup>();
    }
    public virtual void ShowUI(System.Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        //������Ч
        AudioManager.Instance.PlaySFX("Click");

        bg.gameObject.SetActive(true);
        canvasGroup.alpha = 0;

        if (effectType == UIEffectType.Slide)
        {
            // ���ó�ʼλ��Ϊ��Ļ����
            tableTransform.localPosition = new Vector3(0, Screen.height, 0);
            // ��������������
            tableTransform.DOLocalMoveY(0, animationTime).SetEase(Ease.OutBack);
        }
        else if (effectType == UIEffectType.Scale)
        {
            // ���ó�ʼΪ��С״̬
            tableTransform.localScale = Vector3.zero;
            // ��������С����
            tableTransform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutBack);
        }
        else if (effectType == UIEffectType.Fade)
        { 
            
        }
        // ����
        canvasGroup.DOFade(1, animationTime).OnComplete(() => callback?.Invoke());
    }
    public virtual void HideUI(float delaytime = 0, System.Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        AudioManager.Instance.PlaySFX("Click");

        if (effectType == UIEffectType.Slide)
        {
            // ��������������
            tableTransform.DOLocalMoveY(Screen.height, animationTime).SetEase(Ease.InBack).SetDelay(delaytime);
        }
        else if (effectType == UIEffectType.Scale)
        {
            // �������Ӵ�С
            tableTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InBack).SetDelay(delaytime);
        }
        else if (effectType == UIEffectType.Fade)
        { 
            
        }
        // ����
        canvasGroup.DOFade(0, animationTime).SetDelay(delaytime).OnComplete(() =>
        {
            callback?.Invoke();
        });

        // ������ɺ����UI
        Invoke(nameof(DisableUI), animationTime + delaytime);
    }
    private void DisableUI()
    {
        bg.gameObject.SetActive(false);
    }
}
