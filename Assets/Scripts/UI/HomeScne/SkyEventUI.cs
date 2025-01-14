using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyEventUI : BaseUI
{
    private Button close;
    private Button infoButton;
    private InfoUI info;

    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        infoButton = tableTransform.Find("Button Infomation").GetComponent<Button>();
        info = tableTransform.Find("UI Infomation").GetComponent<InfoUI>();

        infoButton.onClick.AddListener(ShowInfo);
        close.onClick.AddListener(Close);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Fade);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Fade);
    }

    private void Close()
    {
        UIManager.Instance.HideUI<SkyEventUI>();
    }
    private void ShowInfo()
    {
        info.ShowInfo();
    }
}
