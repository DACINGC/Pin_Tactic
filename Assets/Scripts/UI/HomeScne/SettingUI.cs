using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : BaseUI
{
    private Button close;
    private Button bgmButton; // ���Ʊ������ֵİ�ť
    private Button sfxButton; // ������Ч�İ�ť
    private bool isBGMPlaying = true; // ��ǰ���������Ƿ񲥷�
    private bool areSFXEnabled = true; // ��ǰ��Ч�Ƿ�����
    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);

        bgmButton = tableTransform.Find("Contain/OptionMusic/Button/Slider").GetComponent<Button>();
        sfxButton = tableTransform.Find("Contain/OptionSound/Button/Slider").GetComponent<Button>();
        // �������ְ�ť����¼�
        bgmButton.onClick.AddListener(ToggleBGM);

        // ��Ч��ť����¼�
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

    // �л��������ֲ���/��ͣ
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

    // �л���Ч��������/����
    private void ToggleSFX()
    {
        if (areSFXEnabled)
        {
            AudioManager.Instance.DisableSFX(); // ������Ч����
            areSFXEnabled = false;
        }
        else
        {
            AudioManager.Instance.EnableSFX(); // ������Ч����
            areSFXEnabled = true;
        }
    }
}
