using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerUI : MonoBehaviour
{
    private GameObject BG;

    private Transform panelDevice;
    private Transform topTrans;
    private Image sliderImage;
    private Text sliderText;
    private Button starButton;
    private Text starText;

    private Transform ButtonHolder;
    private StickerButton b1;
    private StickerButton b2;
    private StickerButton b3;

    private List<Vector3> originPosList = new List<Vector3>();
    private Transform process;
    private Button close;
    [SerializeField] private Sticker curSticker;
    private void Awake()
    {
        BG = transform.Find("BG").gameObject;
        panelDevice = transform.Find("BG/Save Area/Panel Device");
        topTrans = panelDevice.Find("Top");
        sliderImage = topTrans.Find("Process/slider").GetComponent<Image>();
        sliderText = topTrans.Find("Process/slider/Text").GetComponent<Text>();
        starButton = topTrans.Find("Button Star").GetComponent<Button>();
        starText = starButton.transform.Find("Text").GetComponent<Text>();
        starButton.onClick.AddListener(StarEvent);

        ButtonHolder = panelDevice.Find("Botton");
        b1 = ButtonHolder.Find("B1").GetComponent<StickerButton>();
        b2 = ButtonHolder.Find("B2").GetComponent<StickerButton>();
        b3 = ButtonHolder.Find("B3").GetComponent<StickerButton>();

        process = topTrans.Find("Process");
        close = topTrans.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("Click");
            CloseEvent();
        } );

        for (int i = 0; i < ButtonHolder.childCount; i++)
        {
            originPosList.Add(ButtonHolder.GetChild(i).position);
        }

    }
    private void Start()
    {
        FreshStickerButton();
    }
    //���°�ťͼ��
    public void FreshStickerButton()
    {
        curSticker = StickerManager.Instance.CurSticker;

        if (curSticker.IsCompleted())
        {
            Debug.Log("��ǰ��ͼ�Ѿ����");
            CompeleteEvent();
            return;
        }

        FreshUnlockStickerEvent();
        b1.FreshButton(curSticker.GetB1Item());
        b2.FreshButton(curSticker.GetB2Item());
        b3.FreshButton(curSticker.GetB3Item());

        SetButtonClick();
    }
    private void FreshUnlockStickerEvent()
    {
        b1.FreshUnlockStickerList(curSticker.GetB1List(), this);
        b2.FreshUnlockStickerList(curSticker.GetB2List(), this);
        b3.FreshUnlockStickerList(curSticker.GetB3List(), this);
    }

    /// <summary>
    /// ���֮����õ��¼�
    /// </summary>
    private  void CompeleteEvent()
    {
        //����Ѿ��򿪹����䣬����
        if (GameDataManager.CurrentGameData.isStikcerChestOpen)
        { 
            return;
        }
        //�˳�����
        HomeSceneUI.Instance.ExitSticker();
        //�������UI
        UIManager.Instance.ShowUI<StickerCompletedUI>();
        //�����ռ���ϵ��б�
        GameDataManager.AddCompleteStickerList();
        //���ñ���Ϊδ����״̬
        GameDataManager.SetStikcerChestOpen(false);

        if (StickerManager.Instance.IsLastSticker())
        {
            Debug.Log("�Ѿ������һ����ֽ��");
            return;
        }
        //�л���ť
        HomeSceneUI.Instance.homeUI.SetStickerButton(false);
    }

    //����ť����Ϊ���Ե��
    public void SetButtonClick()
    {
        b1.SetButton(GameDataManager.CurrentGameData.curB1Index <= GameDataManager.CurrentGameData.curButtonIndex);
        b2.SetButton(GameDataManager.CurrentGameData.curB2Index <= GameDataManager.CurrentGameData.curButtonIndex);
        b3.SetButton(GameDataManager.CurrentGameData.curB3Index <= GameDataManager.CurrentGameData.curButtonIndex);
    }
    /// <summary>
    /// ������ֽ����
    /// </summary>
    public void UpdateStikcerProgress()
    {
        sliderImage.fillAmount = GameDataManager.GetUnlockStickerList().Count / 9.0f;
        sliderText.text = $"{GameDataManager.GetUnlockStickerList().Count} / 9";
    }

    //����ui���˳��ĺ���
    public void EnterSticker()
    {
        BG.SetActive(true);
        if (StickerManager.Instance.IsLastSticker() && GameDataManager.CurrentGameData.curButtonIndex == 3)
        {
            ButtonHolder.gameObject.SetActive(false);   
        }
        AnimationUtility.ChildrenDownToUp(ButtonHolder, originPosList);
        AnimationUtility.PlayEnterFromRight(close.transform);
        AnimationUtility.FadeIn(process);

        //������ֽ����
        UpdateStarText();
        UpdateStikcerProgress();
    }

    public void ExitSticker()
    {
        AnimationUtility.ChildrenUpToDown(ButtonHolder, originPosList);
        AnimationUtility.PlayExitToRight(close.transform);
        AnimationUtility.FadeOut(process, 0.5f, () => BG.SetActive(false));
        HomeSceneUI.Instance.homeUI.UpdateHomeStickerSlider();
    }
    private void CloseEvent()
    {
        HomeSceneUI.Instance.ExitSticker();
    }

    private void StarEvent()
    {
        UIManager.Instance.ShowUI<EarnStarUI>();
    }

    /// <summary>
    /// ������ֽUI�е��ı�
    /// </summary>
    public void UpdateStarText()
    {
        starText.text = GameDataManager.CurrentGameData.starCount.ToString();
    }

}
