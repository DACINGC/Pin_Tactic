using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    private Button ClickButton;
    private Transform iconGrid;
    private GameObject lockObj;
    private Text buttonText;

    [SerializeField] private RewardItemGroup rewardList;
    [SerializeField] private GameObject rewardPrefab;
    private void Init()
    {
        ClickButton = transform.Find("Button").GetComponent<Button>();
        iconGrid = transform.Find("IconGrid");
        buttonText = ClickButton.transform.Find("Text").GetComponent<Text>();

        lockObj = transform.Find("Lock").gameObject;
        lockObj.SetActive(true);
        ClickButton.interactable = false;
    }

    public void InitDailyRewardItem(RewardItemGroup item, GameObject rewardObj)
    {
        Init();
        rewardList = item;
        rewardPrefab = rewardObj;

        for (int i = 0; i < rewardList.rewardItems.Count; i++)
        {
            GameObject curReward = Instantiate(rewardPrefab);
            curReward.transform.SetParent(iconGrid);
            curReward.transform.localScale = Vector3.one;

            curReward.GetComponent<Image>().sprite = rewardList.rewardItems[i].icon;
            curReward.transform.GetChild(0).GetComponent<Text>().text = rewardList.rewardItems[i].count.ToString() 
                + NameUtility.GetStringByName(rewardList.rewardItems[i].icon.name);

        }

        ClickButton.onClick.AddListener(ClickEvent);

        if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
        {
            buttonText.text = "FREE";
        }
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
        { 
            buttonText.text = "免费";
        }
    }

    public void UnlockReward(bool needFx)
    {
        lockObj.SetActive(false);
        ClickButton.interactable = true;

        if (needFx == false)
            return;

        Invoke(nameof(DelayShowFx), 0.1f);
    }

    private void ClickEvent()
    {
        ClickButton.interactable = false;

        //增加物品
        for (int i = 0; i < rewardList.rewardItems.Count; i++)
        {
            GameDataManager.AddItemCountBySpriteName(rewardList.rewardItems[i].icon.name, rewardList.rewardItems[i].count);
        }
        //更新UI，并增加天数索引
        EventManager.Instance.TriggerEvent(GameEvent.UpdateDailyEvent);


        //奖励物体上移
        AnimationUtility.MoveUpAndFadeOut(iconGrid.GetComponent<RectTransform>(), 0.5f, 200, () =>
        {
            //选项缩小消失
            AnimationUtility.ScaleDownAndFadeOut(transform, 0.3f, () =>
            {
                gameObject.SetActive(false);
                //解锁下一个
                UIManager.Instance.GetUI<DailyBonusUI>().ChangeDailyReward();
            });
        });
    }
    private void DelayShowFx()
    {
        GameObject newBoom = Instantiate(ResourceLoader.Instance.GetFxGameObject("Expolosion"));

        TransFormUtility.SetEffectPosition(newBoom.transform, lockObj.GetComponent<RectTransform>());
    }
}
