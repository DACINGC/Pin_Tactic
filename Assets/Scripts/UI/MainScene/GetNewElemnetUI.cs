using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ItemType
{
    Hole, Rocket, DoubleBox
}

public class GetNewElemnetUI : MainBaseUI
{
    private Button buttonclaim;
    private Image iconImage;
    [SerializeField] private List<Sprite> icon;

    private Text ItemCount;
    private Text contentText;

    protected override void Awake()
    {
        base.Awake();
        buttonclaim = tableTransform.Find("Button CLAIM").GetComponent<Button>();
        iconImage = tableTransform.Find("refill/icon").GetComponent<Image>();
        ItemCount = tableTransform.Find("Text Count").GetComponent<Text>();
        contentText = tableTransform.Find("ContentText").GetComponent<Text>();

        buttonclaim.onClick.AddListener(ClaimEvent);
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, effectType);
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, effectType);
    }
    //���ȷ����ť֮���ƶ�Ӳ��
    private void ClaimEvent()
    {
        UIManager.Instance.HideUI<GetNewElemnetUI>(0, () =>
        {
            if (LevelManager.Instance.GetLevleNum() == 5)
            {
                //������ť
                MainSceneUI.Instance._GamePlayUI.InitButon();
                GameDataManager.AddItemCount(ItemType.Hole, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.Hole, () =>
                {
                    //�ƶ����֮�����ӵ��ߵ�����
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Hole);
                });
            }
            else if (LevelManager.Instance.GetLevleNum() == 10)
            {
                //�����������
                MainSceneUI.Instance._GamePlayUI.InitButon();
                GameDataManager.AddItemCount(ItemType.Rocket, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.Rocket, () =>
                {
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Rocket);
                });
            }
            else if (LevelManager.Instance.GetLevleNum() == 15)
            {
                MainSceneUI.Instance._GamePlayUI.InitButon();
                GameDataManager.AddItemCount(ItemType.DoubleBox, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.DoubleBox, () =>
                {
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.DoubleBox);
                });
            }

        });
    }

    /// <summary>
    /// ����UI��ͼƬ�͵�������
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void SetMoveIcon(ItemType type, int count)
    {
        if (type == ItemType.Hole)
        {
            iconImage.sprite = icon[0];
        }
        else if (type == ItemType.Rocket)
        {
            iconImage.sprite = icon[1];
        }
        else if (type == ItemType.DoubleBox)
        {
            iconImage.sprite = icon[2];
        }
        ItemCount.text = count.ToString();
    }

    public void SetContextText(string s)
    {
        contentText.text = s;
    }
}
