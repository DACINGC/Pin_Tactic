using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    // ��̬ʵ��������ȫ�ַ���
    public static StickerManager Instance { get; private set; }
    [SerializeField] private GameObject buildingFx;
    [SerializeField] private GameObject smokeFx;
    [SerializeField] private Transform starTrans;
    [SerializeField] private GameObject star;
    [SerializeField] private List<Sticker> stickerList = new List<Sticker>();
    [SerializeField] private int curStickIndex;
    public Sticker CurSticker
    {
        get
        {

            if (stickerList.Count == 0)
            {
                InitStickerList();
            }
            return stickerList[GameDataManager.CurrentGameData.curStickerIndex];
        }
            
    }
    public Sticker NextSticker
    {
        get => stickerList[GameDataManager.CurrentGameData.curStickerIndex + 1];
    }
    public GameObject GetBuildingFx()
    {
        return buildingFx;
    }
    private ObjectPool starPool;

    /// <summary>
    /// �Ѿ��������һ����ֽ
    /// </summary>
    /// <returns></returns>
    public bool IsLastSticker()
    {
        return GameDataManager.CurrentGameData.curStickerIndex >= stickerList.Count - 1;
    }
    public void ChangeNextSticker()
    {
        if (IsLastSticker())
        {
            Debug.Log("��û����һ����ֽ");
            return;
        }
        CurSticker.gameObject.SetActive(false);
        GameDataManager.AddStickerIndex();
        CurSticker.gameObject.SetActive(true);
        //��ʼ����ֽ�б�
        CurSticker.InitSticker(GameDataManager.GetUnlockStickerList());
        
        curStickIndex = GameDataManager.CurrentGameData.curStickerIndex;//������ʾ
    }
    private void Awake()
    {
        // ȷ��ֻ��һ�� StickerManager ʵ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // �����ظ���ʵ��
        }
        else
        {
            Instance = this;  // ���õ�ǰʵ��Ϊ����
        }
    }

    /// <summary>
    /// ��ʼ����ֽ�б�
    /// </summary>
    private void InitStickerList()
    {
        foreach (Transform c in transform)
        {
            stickerList.Add(c.GetComponent<Sticker>());
        }
        starPool = new ObjectPool(star, 6, starTrans);

        for (int i = 0; i < stickerList.Count; i++)
        {
            if (i == GameDataManager.CurrentGameData.curStickerIndex)
            {
                stickerList[i].InitSticker(GameDataManager.GetUnlockStickerList());
                stickerList[i].gameObject.SetActive(true);
            }
            else
                stickerList[i].gameObject.SetActive(false);
        }

        curStickIndex = GameDataManager.CurrentGameData.curStickerIndex;//������ʾ
    }

    // ����star��ʹ��DOTween�ƶ�
    public void InitStar(Vector3 targetPosition, System.Action callback = null)
    {
        // �Ӷ�����л�ȡһ���µ�starʵ��
        GameObject newStar = starPool.GetObj();

        RectTransform newStarRect = newStar.GetComponent<RectTransform>();
        RectTransform starTransRect = HomeSceneUI.Instance.homeUI.StarTrans.GetComponent<RectTransform>();

        // �� StarTrans ����������ת��Ϊ newStar ���ڸ�����ľֲ�����
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            newStarRect.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, starTransRect.position),
            null,
            out localPoint
        );

        // ���� newStar �� anchoredPosition
        newStarRect.anchoredPosition = localPoint;

        // ʹ��DOTween��ʵ���ƶ��������ȿ����
        newStar.transform.DOMove(targetPosition, 1.2f)  // �ƶ�ʱ��2��
            .SetEase(Ease.InOutSine)// �ȿ������Ч����InOutSine����
            .OnComplete(() =>
            {
                //��ʾ������
                GameObject smoke = Instantiate(smokeFx, starTrans);
                smoke.transform.position = newStar.transform.position;

                starPool.ReturnToPool(newStar);
                Destroy(smoke, 1f);
                callback?.Invoke();
            });

    }

    //�ռ�����
    /// <summary>
    /// չʾ��ֽ
    /// </summary>
    /// <param name="index"></param>
    public void ShowStikcerByIndex(int index)
    {
        for (int i = 0; i < stickerList.Count; i++)
        {
            if (i == index)
            {
                stickerList[i].gameObject.SetActive(true);
            }
            else
                stickerList[i].gameObject.SetActive(false);
        }

        stickerList[index].ShowUnlockedItem();
    }

    /// <summary>
    /// ��ʾ��ǰ��ֽ
    /// </summary>
    public void ShowCurSticker()
    {
        CurSticker.gameObject.SetActive(true);
        //��ʼ����ֽ�б�
        CurSticker.InitSticker(GameDataManager.GetUnlockStickerList());
    }

    public bool IsStickerCompeleted()
    {
        if (curStickIndex == stickerList.Count - 1 && CurSticker.IsCompleted())
        {
            return true;
        }

        return false;
    }
}
