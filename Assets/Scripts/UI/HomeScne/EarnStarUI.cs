using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnStarUI : BaseUI
{
    private Button close;
    private Button playButton;
    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        playButton = tableTransform.Find("Button Play").GetComponent<Button>();

        close.onClick.AddListener(CloseEvent);
        playButton.onClick.AddListener(PlayEvent);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<EarnStarUI>();
    }
    private void PlayEvent()
    {
        UIManager.Instance.HideUI<EarnStarUI>(0, () => LevelManager.Instance.InitLevel());
    }
}
