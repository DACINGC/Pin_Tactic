using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsUI : BaseUI
{
    private Button closeButton;
    private Button buyButton;
    protected override void Awake()
    {
        base.Awake();
        buyButton = tableTransform.Find("Buy Button").GetComponent<Button>();
        closeButton = tableTransform.Find("Close").GetComponent<Button>();

        closeButton.onClick.AddListener(CloseEvent);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<NoAdsUI>();
    }
}
