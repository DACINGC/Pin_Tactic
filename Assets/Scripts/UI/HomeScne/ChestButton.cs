using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class ChestButton : MonoBehaviour
{
    private Transform tip;
    private Button chest;

    // ��ʾʱ��
    private float displayDuration = 3f;

    private bool isAnimating = false; // ��ֹ�ظ�����
    private void Awake()
    {
        tip = transform.Find("Tip");
        chest = transform.Find("Button").GetComponent<Button>();
        chest.onClick.AddListener(ChestClick);
        tip.gameObject.SetActive(false);
    }

    private void ChestClick()
    {
        AudioManager.Instance.PlaySFX("Click");
        // ����������ڽ����У�ֱ�ӷ���
        if (isAnimating)
            return;
        // ��ʼ����
        ShowAndHideObject();
    }

    private void ShowAndHideObject()
    {
        // ����Ϊ�ɼ�����ʼ��͸����
        tip.gameObject.SetActive(true);
        isAnimating = true;

        AnimationUtility.FadeIn(tip, 0.25f, () =>
        {
            DOVirtual.DelayedCall(displayDuration, () =>
            {
                AnimationUtility.FadeOut(tip, 0.5f, () => isAnimating = false);
            });

        });

    }
}
