using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : BaseUI
{
    private Transform viewTrans;
    private Button close;
    private Transform content;
    private bool isClick = false;//ȷ�����һ��֮�󣬲���������UI
    public void SetIsClickFalse()
    {
        isClick = false;
    }
    protected override void Awake()
    {
        base.Awake();
        viewTrans = tableTransform.Find("Scroll View");
        close = tableTransform.Find("Top/Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);
        content = tableTransform.Find("Scroll View/Viewport/Content");
    }

    void Start()
    {
        int index = 0;
        AddListenersToButtons(content.transform, ref index);
        SetPageFlase();
        InitPage();
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Fade);
        InitPage();
        ViewMoveDown();
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Fade);
    }

    private void ViewMoveDown()
    {
        // ���ó�ʼλ��Ϊ��Ļ����
        viewTrans.localPosition = new Vector3(0, Screen.height, 0);
        // ��������������
        viewTrans.DOLocalMoveY(0, animationTime).SetEase(Ease.OutBack);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<CollectionUI>();
    }

    private void ShowSticker(int index)
    {
        if (isClick)
            return;

        //Debug.Log("Show : " + index.ToString());
        UIManager.Instance.HideUI<CollectionUI>();
        UIManager.Instance.ShowUI<ShowStickerUI>();
        StickerManager.Instance.ShowStikcerByIndex(index);
        isClick = true;
    }

    private void AddListenersToButtons(Transform parent, ref int index)
    {
        foreach (Transform child in parent)
        {
            // ����Ƿ��� Button ���
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                int capturedIndex = index; // ����ǰ����������հ�����
                btn.onClick.AddListener(() => 
                {
                    AudioManager.Instance.PlaySFX("Click");
                    ShowSticker(capturedIndex);
                });
                index++;
                //Debug.Log($"Button Added: {child.name}, Index: {capturedIndex}");
            }

            // �ݹ����������
            AddListenersToButtons(child, ref index);
        }
    }

    //������page���ó�δ����״̬
    private void SetPageFlase()
    {
        foreach (Transform tran in content)
        {
            tran.Find("Unlock").gameObject.SetActive(false);
            tran.Find("Lock").gameObject.SetActive(true);
            tran.Find("Unlock/Icon/Mask").gameObject.SetActive(false);
        }
    }
    //���ݴ洢�����ʾPage
    private void InitPage()
    {
        for (int i = 0; i < GameDataManager.CurrentGameData.unlockStickerCount; i++)
        {
            Transform child = content.GetChild(i);
            child.Find("Unlock").gameObject.SetActive(true);
            child.Find("Lock").gameObject.SetActive(false);
        }

        for (int i = 0; i < GameDataManager.CurrentGameData.completeStikcerCount; i++)
        {
            Transform child = content.GetChild(i);
            switch (i)
            {
                case 0:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c1");
                    break;
                case 1:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c2");
                    break;
                case 2:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c3");
                    break;
                case 3:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c4");
                    break;
                case 4:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c5");
                    break;
                case 5:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c6");
                    break;
                case 6:
                    child.Find("Unlock/Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite("c7");
                    break;
            }
            child.Find("Unlock/Icon/Mask").gameObject.SetActive(true);
        }
    }
}
