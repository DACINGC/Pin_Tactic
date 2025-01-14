using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockStickerUI : BaseUI
{
    private Image unlockImage;
    private Button unlockButton;
    private Button close;
    private GameObject fx;
    protected override void Awake()
    {
        base.Awake();
        unlockImage = tableTransform.Find("Content/Mask/Hide").GetComponent<Image>();
        unlockButton = tableTransform.Find("Button").GetComponent<Button>();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        fx = transform.Find("New area effect").gameObject;
        fx.gameObject.SetActive(false);

        unlockButton.onClick.AddListener(UnlockEvent);
        close.onClick.AddListener(CloseEvent);
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        unlockImage.sprite = StickerManager.Instance.NextSticker.BG;
        base.ShowUI(callback, effectType);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, effectType);
    }

    private void UnlockEvent()
    {
        //���ð�ť
        GameDataManager.ResetStickerButton();

        fx.SetActive(true);
        Invoke(nameof(DelaySetFxFalse), 2);

        //������ֽ��ť��Ϊfalse
        HomeSceneUI.Instance.homeUI.SetStickerButton(true);
        StickerManager.Instance.ChangeNextSticker();

        //������ֽ����
        HomeSceneUI.Instance.homeUI.UpdateHomeStickerSlider();
        //����������Ϊ��δ��ʼ״̬
        GameDataManager.SetStikcerChestOpen(false);

        //���°�ť
        HomeSceneUI.Instance.stickerUI.FreshStickerButton();

        //�����½������ռ���ֽ
        GameDataManager.AddUnlockStikcerList();
        CloseEvent();

    }

    private void DelaySetFxFalse()
    {
        fx.SetActive(false);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<UnlockStickerUI>();
    }
}
