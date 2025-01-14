using System;
using UnityEngine;
using UnityEngine.UI;

public class UnlockFuture : MainBaseUI
{
    private Button playbutton;
    private Image icon;
    protected override void Awake()
    {
        base.Awake();
        playbutton = tableTransform.Find("Button").GetComponent<Button>();
        playbutton.onClick.AddListener(PlayEvent);

        icon = tableTransform.Find("Icon").GetComponent<Image>();
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Scale);
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Scale);
    }

    public void SetUnlockIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
    private void PlayEvent()
    {
        UIManager.Instance.HideUI<UnlockFuture>();
    }
}
