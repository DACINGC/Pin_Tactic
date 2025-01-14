using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : BaseUI
{
    private Button close;
    private Button bgmButton; // 控制背景音乐的按钮
    private Button sfxButton; // 控制音效的按钮
    private bool isBGMPlaying = true; // 当前背景音乐是否播放
    private bool areSFXEnabled = true; // 当前音效是否启用
    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);

        bgmButton = tableTransform.Find("Contain/OptionMusic/Button/Slider").GetComponent<Button>();
        sfxButton = tableTransform.Find("Contain/OptionSound/Button/Slider").GetComponent<Button>();
        // 背景音乐按钮点击事件
        bgmButton.onClick.AddListener(ToggleBGM);

        // 音效按钮点击事件
        sfxButton.onClick.AddListener(ToggleSFX);
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, effectType);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, effectType);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<SettingUI>();
    }

    // 切换背景音乐播放/暂停
    private void ToggleBGM()
    {
        if (isBGMPlaying)
        {
            AudioManager.Instance.PauseBGM();
            isBGMPlaying = false;
        }
        else
        {
            AudioManager.Instance.PlayBGM();
            isBGMPlaying = true;
        }
    }

    // 切换音效播放启用/禁用
    private void ToggleSFX()
    {
        if (areSFXEnabled)
        {
            AudioManager.Instance.DisableSFX(); // 禁用音效播放
            areSFXEnabled = false;
        }
        else
        {
            AudioManager.Instance.EnableSFX(); // 启用音效播放
            areSFXEnabled = true;
        }
    }
}
