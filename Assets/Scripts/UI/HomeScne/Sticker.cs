using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sticker : MonoBehaviour
{
    private Dictionary<string, GameObject> lockedObjectsDic = new Dictionary<string, GameObject>();
    [Header("����������")]
    [SerializeField] private List<GameObject> lockedObjList = new List<GameObject>();

    [Header("���ð�ťͼƬ")]
    [SerializeField] private List<ButtonItem> B1Obj = new List<ButtonItem>();
    [SerializeField] private List<ButtonItem> B2Obj = new List<ButtonItem>();
    [SerializeField] private List<ButtonItem> B3Obj = new List<ButtonItem>();

    [Header("���ð�ť��������")]
    [SerializeField] private List<GameObject> b1GameObj = new List<GameObject>();
    [SerializeField] private List<GameObject> b2GameObj = new List<GameObject>();
    [SerializeField] private List<GameObject> b3GameObj = new List<GameObject>();

    [Header("���ý���֮��Ľ���")]
    [SerializeField] private List<RewardItem> rewardList;

    [Header("����ͼƬ")]
    [SerializeField] private Sprite bg;
    public Sprite BG { get => bg; }
    public List<RewardItem> RewardList { get => rewardList; }
    [Header("��ǰ��ť������")]
    [SerializeField]private int curB1Index;
    [SerializeField]private int curB2Index;
    [SerializeField]private int curB3Index;
    [Header("��ǰ���ڵڼ���")]
    [SerializeField]private int curIndex;


    private void InitlockedDic()
    {
        foreach (GameObject obj in lockedObjList)
        {
            //obj.name = obj.GetComponent<Image>().sprite.name;
            if (!lockedObjectsDic.ContainsKey(obj.name))
            {
                lockedObjectsDic.Add(obj.name, obj);
                obj.SetActive(false);
            }
        }
    }
    /// <summary>
    /// ���ݴ洢�����ʼ����ֽ
    /// </summary>
    public void InitSticker(List<string> nameList)
    {
        InitlockedDic();
        foreach (string name in nameList)
        {
            if (lockedObjectsDic.ContainsKey(name))
            {
                lockedObjectsDic[name].SetActive(true);
            }
        }
    }
    /// <summary>
    /// ����ָ�����Ƶ����塣
    /// </summary>
    /// <param name="name">��������ơ�</param>
    /// <returns>��������� GameObject������������򷵻� null��</returns>
    public Transform UnlockObj(string name)
    {
        if (lockedObjectsDic.TryGetValue(name, out GameObject obj))
        {
            obj.SetActive(true);
            //�������壬��ʾ����
            AnimationUtility.PlayScaleBounce(obj.transform, 1.1f);
            GameObject fx = Instantiate(StickerManager.Instance.GetBuildingFx(), obj.transform.parent);
            fx.transform.position = obj.transform.position;
            Destroy(fx, 1f);

            IncreaseCurItemIndex(name);
            return obj.transform;
        }
        else
        {
            Debug.LogWarning($"Object with name '{name}' not found in locked objects.");
            return null;
        }
    }
    //���ڰ�ť�л�ͼƬ
    private bool canChange = false;
    public bool CanChange { get => canChange; }
    public void CompleteChange()
    {
        canChange = false;
    }

    /// <summary>
    /// ���ӵ�ǰ��ť������
    /// </summary>
    /// <param name="name"></param>
    private void IncreaseCurItemIndex(string name)
    {
        foreach (GameObject g in b1GameObj)
        {
            if (g.name == name)
            {
                GameDataManager.AddB1Index();
                CheckNext();
                return;
            }
        }
        foreach (GameObject g in b2GameObj)
        {
            if (g.name == name)
            {
                GameDataManager.AddB2Index();
                CheckNext();
                return;
            }
        }
        foreach (GameObject g in b3GameObj)
        {
            if (g.name == name)
            {
                GameDataManager.AddB3Index();
                CheckNext();
                return;
            }
        }
    }
    private void CheckNext()
    {
        //������еİ�ť��������һ�׶Σ��л���ǰ����
        if (GameDataManager.CurrentGameData.curB1Index == GameDataManager.CurrentGameData.curB2Index 
            && GameDataManager.CurrentGameData.curB2Index == GameDataManager.CurrentGameData.curB3Index)
        {
            GameDataManager.ChangeStickerButtonIndex();
            canChange = true;
        }
    }
    public ButtonItem GetB1Item()
    {
        return B1Obj[GameDataManager.CurrentGameData.curButtonIndex];
    }
    public ButtonItem GetB2Item()
    {
        return B2Obj[GameDataManager.CurrentGameData.curButtonIndex];
    }
    public ButtonItem GetB3Item()
    {
        return B3Obj[GameDataManager.CurrentGameData.curButtonIndex];
    }
    public List<GameObject> GetB1List()
    {
        return b1GameObj;
    }   
    public List<GameObject> GetB2List()
    {
        return b2GameObj;
    }
    public List<GameObject> GetB3List()
    {
        return b3GameObj;
    }
    //��鵱ǰ��ͼ�Ƿ����
    public bool IsCompleted()
    {
        return GameDataManager.CurrentGameData.curButtonIndex >= 3;
    }

    //չʾ��������
    public void ShowUnlockedItem()
    {
        foreach (GameObject obj in lockedObjList)
        {
            obj.SetActive(true);
        }

        AnimationUtility.ShowObjectsOneByOneWithScale(lockedObjList);
    }
    public bool CanNoti()
    {
        int starCount = GameDataManager.CurrentGameData.starCount;

        //����п��Խ��������壬��չʾ���
        if (B1Obj.Count <= starCount && curB1Index == curIndex)
            return true;
        
        if (B2Obj.Count <= starCount && curB2Index == curIndex)
            return true;
        
        if (B3Obj.Count <= starCount && curB3Index == curIndex)
            return true;


        return false;
    }
}


[System.Serializable]
public class ButtonItem
{
    [SerializeField] private Sprite icon;
    [SerializeField] private int count;

    public Sprite GetSprite()
    {
        return icon;
    }

    public int GetCount()
    {
        return count;
    }
}
