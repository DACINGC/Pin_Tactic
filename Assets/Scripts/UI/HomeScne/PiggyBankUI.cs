using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiggyBankUI : BaseUI
{
    private Button CloseButton;
    private GameObject fullObj;
    private Image processImage;
    private Text processText;

    private Button BreakButton;
    private GameObject breakObj;
    private GameObject playObj;
    protected override void Awake()
    {
        base.Awake();
        CloseButton = tableTransform.Find("Button Close").GetComponent<Button>();
        BreakButton = tableTransform.Find("Button Break").GetComponent<Button>();
        fullObj = tableTransform.Find("Icon Full").gameObject;
        processImage = tableTransform.Find("Process").GetComponent<Image>();
        processText = tableTransform.Find("Text Process").GetComponent<Text>();
        breakObj = BreakButton.transform.Find("Text Break").gameObject;
        playObj = BreakButton.transform.Find("Text Play").gameObject;

        CloseButton.onClick.AddListener(CloseEvent);
        BreakButton.onClick.AddListener(BreakEvent);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, effectType);
        UpdatePiggyProgcess();
    }
    private void CloseEvent()
    {
        UIManager.Instance.HideUI<PiggyBankUI>();
    }
    private void UpdatePiggyProgcess()
    {
        processImage.fillAmount = GameDataManager.CurrentGameData.piggyCount / 750.0f;
        processText.text = $"{GameDataManager.CurrentGameData.piggyCount} / 750";

        if (GameDataManager.CurrentGameData.piggyCount >= 750)
        {
            fullObj.SetActive(true);
            breakObj.SetActive(true);
            playObj.SetActive(false);
        }
        else
        {
            fullObj.SetActive(false);
            breakObj.SetActive(false);
            playObj.SetActive(true);
        }
    }

    private void BreakEvent()
    {
        if (fullObj.activeSelf)
        {
            Debug.Log("µÃµ½½ð±Ò");
            GameDataManager.AddPiggyCount();
            UpdatePiggyProgcess();
            HomeSceneUI.Instance.homeUI.UpdateResourceText();
        }
        else
        {
            GameManager.Instance.EnterMainScene();
            LevelManager.Instance.InitLevel();
            UIManager.Instance.HideUI<PiggyBankUI>();

        }
    }
}
