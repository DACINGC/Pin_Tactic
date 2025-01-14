using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public static readonly string HeartName = "SHOP_INFINITY_MEB";
[System.Serializable]
public class RewardItem
{
    public Sprite icon;
    public int count;
}
public class ChestReward : MonoBehaviour
{
    [SerializeField] private GameObject rewardItem;
    public void InitRewardItems(List<RewardItem> items, System.Action callBack = null)
    {
        //����֮ǰ�Ľ�������
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        transform.GetComponent<GridLayoutGroup>().enabled = true;

        //ʵ����������Ʒ
        for (int i = 0; i < items.Count; i++)
        {

            Transform child = Instantiate(rewardItem).transform;
            child.SetParent(transform);
            child.localScale = Vector3.one;

            child.GetComponent<Image>().sprite = items[i].icon;
            child.GetChild(0).GetComponent<Text>().text = items[i].count.ToString() + NameUtility.GetStringByName(items[i].icon.name);

            child.name = items[i].icon.name;
        }

        //���ñ����ͬʱ��������Ʒ
        
        foreach (RewardItem reward in items)
        {
            GameDataManager.AddItemCountBySpriteName(reward.icon.name, reward.count);
        }

        callBack?.Invoke();
        //�Ѿ��򿪱���
        //GameDataManager.SetStikcerChestOpen(true);
    }
    public void MoveRewardToTarget()
    {
        transform.GetComponent<GridLayoutGroup>().enabled = false;

        ChestReward moveReward = Instantiate(this);
        moveReward.transform.SetParent(ItemMoveManager.Instance.transform);
        moveReward.transform.localScale = Vector3.one;
        moveReward.transform.position = transform.position;

        //��ʱ�б�洢Ҫ�ƶ�����Ʒ
        List<Transform> moveList = new List<Transform>();
        for (int i = 0; i < moveReward.transform.childCount; i++)
            moveList.Add(moveReward.transform.GetChild(i));

        for (int i = 0; i < moveList.Count; i++)
        {
            Transform t = moveList[i];
            Transform moveTarget = HomeSceneUI.Instance.homeUI.GetTargetTrans(t.name);

            AnimationUtility.MoveToPositionWithParentChange(t, moveTarget.parent, moveTarget.position, 0.8f, () =>
            {
                t.gameObject.SetActive(false);
                Destroy(t.gameObject);

                //���������ı����¼�
                EventManager.Instance.TriggerEvent(GameEvent.OpenChestEvent);

                if (moveReward.transform.childCount == 0)
                    moveList.Clear();
            });
        }
    }
}
