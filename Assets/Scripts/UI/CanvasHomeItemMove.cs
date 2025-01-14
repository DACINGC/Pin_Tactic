using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHomeItemMove : MonoBehaviour
{
    private RectTransform Arrow;
    private RectTransform Spin;
    private Vector3 originArrowPos;
    private Vector3 originSpinPos;
    private Image canClickImage;

    private void Awake()
    {
        Arrow = transform.Find("Arrow").GetComponent<RectTransform>();
        Spin = transform.Find("Spin").GetComponent<RectTransform>();
        canClickImage = transform.Find("CanClickImage").GetComponent<Image>();
        canClickImage.enabled = false;

        originArrowPos = Arrow.position;
        originSpinPos = Spin.position;
        Arrow.localScale = Vector3.one;
    }

    public void PlayMoveAnim()
    {
        // 传入目标位置的Transform，分别为Arrow和Spin调用移动动画方法
        MoveItemWithFx(Arrow, originArrowPos, HomeSceneUI.Instance.homeUI.StreakTransfom);
        MoveItemWithFx(Spin, originSpinPos, HomeSceneUI.Instance.homeUI.LuckySpinTrans);
    }
    //移动汽油
    public void MoveArrowAnim()
    {
        MoveItemWithFx(Arrow, originArrowPos, HomeSceneUI.Instance.homeUI.StreakTransfom, () =>
        {
            //更新文本
            EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);

            //如果已经解锁了相应的物品
            if (GameDataManager.CurrentGameData.curStreakIndex <= HomeSceneUI.Instance.homeUI.winStreakUI.GetCurMoveTargetValue())
            {
                Debug.Log("已经解锁过宝箱 CurMoveIndex : " + GameDataManager.CurrentGameData.curStreakIndex);
                return;
            }

            //如果当前的连胜数达到了下一个奖励目标,显示StreakUI
            if (GameDataManager.CurrentGameData.curStreakIndex == HomeSceneUI.Instance.homeUI.winStreakUI.GetNextMoveTargetValue())
            {
                Debug.Log("显示奖励物体");
                HomeSceneUI.Instance.homeUI.winStreakUI.ShowWinStreakUI(() =>
                {
                    HomeSceneUI.Instance.homeUI.winStreakUI.HideWinStreakUI();

                    //热气球移动完毕之后，显示宝箱
                    int moveTargetVal = HomeSceneUI.Instance.homeUI.winStreakUI.GetCurMoveTargetValue();
                    UIManager.Instance.ShowUI<StickerChestUI>();
                    switch (moveTargetVal)
                    {
                        case 2:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("1open"), 
                                ResourceLoader.Instance.GetUnlockImageSprite("1close"),
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList
                                );
                            break;
                        case 5:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("2open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("2close"),
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 10:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("3open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("3close"),
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 15:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("4open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("4close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 20:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("5open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("5close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 25:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("6open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("6close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 30:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("7open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("7close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 35:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("8open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("8close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 40:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("9open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("9close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 45:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("10open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("10close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 50:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("11open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("11close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 55:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("12open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("12close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                        case 60:
                            UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
                                ResourceLoader.Instance.GetUnlockImageSprite("13open"),
                                ResourceLoader.Instance.GetUnlockImageSprite("13close"), 
                                HomeSceneUI.Instance.homeUI.winStreakUI.CurRewardList);
                            break;
                    }

                });
            }
        });
    }
    //移动抽奖票
    public void MoveSpinAnim()
    {
        MoveItemWithFx(Spin, originSpinPos, HomeSceneUI.Instance.homeUI.LuckySpinTrans);
    }
    private void MoveItemWithFx(RectTransform itemToMove, Vector3 originPos, Transform targetTrans, System.Action callback = null)
    {
        //移动时不可以点击
        canClickImage.enabled = true;

        GameObject apreaFx = ResourceLoader.Instance.GetFxGameObject("Effect Appear");

        // 获取目标位置
        Vector3 targetPos = targetTrans.position;

        itemToMove.position = originPos; // 这里可以设置物体的初始位置
        itemToMove.gameObject.SetActive(true);

        GameObject curFx = Instantiate(apreaFx);
        curFx.transform.localScale = Vector3.one * 0.6f;

        // 处理特效的UI位置
        TransFormUtility.MoveGameObjectToUIPosition(curFx.transform, itemToMove);
        curFx.GetComponent<ParticleSystem>().Play();
        Destroy(curFx, 0.8f);

        // 先放大到原来的原来的大小
        AnimationUtility.ScaleUpAndFadeIn(itemToMove, 0.5f, () =>
        {
            // 移动物体到目标位置
            AnimationUtility.MoveUIObjectToTarget(itemToMove, targetTrans, 0.5f, () =>
            {
                GameObject curFx = Instantiate(apreaFx);
                curFx.transform.localScale = Vector3.one * 0.6f;
                TransFormUtility.MoveGameObjectToUIPosition(curFx.transform, itemToMove);
                curFx.GetComponent<ParticleSystem>().Play();

                Destroy(curFx, 0.8f);
                itemToMove.gameObject.SetActive(false);

                //移动完成之后，可以点击
                callback?.Invoke();
                canClickImage.enabled = false;
            });
        });
    }
}
