using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> unlockList = new List<GameObject>();

    private Image icon;
    private Button clickButton;
    private GameObject compeletedText;
    private Text count;

    private Sticker sticker;
    private StickerUI ui;
    private bool isInit = false;
    private void Awake()
    {
        if(!isInit)
            InitFindItem();
    }
    private void InitFindItem()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        clickButton = transform.Find("Button").GetComponent<Button>();
        compeletedText = transform.Find("Text").gameObject;
        count = clickButton.transform.Find("Count").GetComponent<Text>();

        clickButton.onClick.AddListener(UnlockEvent);

        isInit = true;
    }
    public void FreshUnlockStickerList(List<GameObject> lis, StickerUI _ui)
    {
        unlockList.Clear();
        unlockList.AddRange(lis);
        ui = _ui;
    }
    public void FreshButton(ButtonItem item)
    {
        if (count == null)
        {
            InitFindItem();
        }
        ImageUtility.SetImageSpriteWithAspect(icon, item.GetSprite(), 180, 180);
        count.text = item.GetCount().ToString();
    }
    private void UnlockEvent()
    {
        AudioManager.Instance.PlaySFX("Click");
        int curStarCount = int.Parse(count.text);

        if (curStarCount > GameDataManager.CurrentGameData.starCount)
        {
            //Debug.Log("û���㹻�������ˣ�");
            UIManager.Instance.ShowUI<EarnStarUI>();
            return;
        }


        SetButton(false);
        //��������
        sticker = StickerManager.Instance.CurSticker;
        int curindex = GameDataManager.CurrentGameData.curButtonIndex;
        //����������
        GameDataManager.DecreaseStarCount(curStarCount);
        //�����ı�
        HomeSceneUI.Instance.stickerUI.UpdateStarText();

        StartCoroutine(InitStarCoroutine(0.1f, curStarCount, curindex));

        //������Ӧ����ֽ
        GameDataManager.UnlockSticker(unlockList[curindex].name);

       ui.UpdateStikcerProgress();
    }

    /// <summary>
    /// �������ǵ�Э��
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator InitStarCoroutine(float delayTime, int curStarCount, int curindex)
    {
        bool canCallBack = true;
        for (int i = 0; i < curStarCount; i++)
        {
            StickerManager.Instance.InitStar(unlockList[curindex].transform.position, () =>
            {
                if (canCallBack)
                {
                    //Debug.Log("�ص�");
                    canCallBack = false;
                    sticker.UnlockObj(unlockList[curindex].name);
                    Invoke(nameof(DelayCheckComplete), 0.5f);
                }
            });

            yield return new WaitForSeconds(delayTime);
        }
    }
    private void DelayCheckComplete()
    {
        if (sticker.CanChange)
        {
            ui.FreshStickerButton();
            sticker.CompleteChange();
        }
    }
    //�Ƿ������ť
    public void SetButton(bool val)
    {
        clickButton.gameObject.SetActive(val);
        compeletedText.SetActive(!val);
    }


}
