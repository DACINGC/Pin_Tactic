using System;
using UnityEngine.UI;

public class AlertUI : BaseUI
{
    private Text alertText;
    private bool isShow = false;
    protected override void Awake()
    {
        base.Awake();
        alertText = tableTransform.Find("Image/Text").GetComponent<Text>();
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        if (isShow == false)
        {
            isShow = true;
            base.ShowUI(callback, UIEffectType.Fade);
            Invoke(nameof(DelayHideAlert), 1f);
        }
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, () =>
        {
            isShow = false;
        }, UIEffectType.Fade);
    }

    public void SetAlertText(string s)
    {
        alertText.text = s;
    }

    private void DelayHideAlert()
    {
        UIManager.Instance.HideUI<AlertUI>();
    }
}
