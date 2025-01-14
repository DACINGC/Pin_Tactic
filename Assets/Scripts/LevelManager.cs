using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Obi;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Dictionary<string, GameObject> levels = new Dictionary<string, GameObject>();
    [SerializeField] private Dictionary<string, GameObject> boxs = new Dictionary<string, GameObject>();
    [SerializeField] private int levelNum;
    [Header("����")]
    [SerializeField] private ObiRopeChainCreator obiropeCrearer;
    [Header("��")]
    [SerializeField] private GameObject icePrefab;
    [Header("��")]
    [SerializeField] private GameObject doorPrefab;
    [Header("ը��")]
    [SerializeField] private GameObject boomPrefab;
    [Header("����")]
    [SerializeField] private GameObject chainFxPrefab;
    [Header("����")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject lockPrefab;
    public ObiRopeChainCreator ObiRopeCreater
    {
        get => obiropeCrearer;
    }
    private static LevelManager _instance;
    private GameObject boxlevel;//��¼�ϴεĹؿ�������ɾ��
    private GameObject level;//��¼�ϴεĹؿ�������ɾ��
    private List<GameObject> ropes = new List<GameObject>();
    private List<ObiRope> chainList = new List<ObiRope>();
    private List<GameObject> keys = new List<GameObject>();
    private List<GameObject> locks = new List<GameObject>();
    public Level CurLevel
    { get; private set; }
    public int GetLevleNum()
    {
        return GameDataManager.CurrentGameData.levelNum;
    }
    public Sprite UnlockSprite { get; set; }
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(LevelManager).ToString());
                    _instance = singleton.AddComponent<LevelManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        levels = ResourceLoader.Instance.Levels;
        boxs = ResourceLoader.Instance.BoxLevels;
    }
    private void Start()
    {
        // �����ǰ�ؿ��źͽ����
        levelNum = GameDataManager.CurrentGameData.levelNum;
    }
    // ��ȡ�ļ����е����ֲ���
    private int ExtractNumber(string name)
    {
        string number = new string(name.Where(char.IsDigit).ToArray());
        return string.IsNullOrEmpty(number) ? 0 : int.Parse(number);
    }

    /// <summary>
    /// ���ؿ�ʵ��������
    /// </summary>
    public async void InitLevel()
    {
        GameManager.Instance.EnterMainScene();
        ClearLevel();
        //ʵ�����ؿ�
        InitLevelGameObj();

        GlobalInit();

        UniqueInitObj();
        Invoke(nameof(DelayInitBoxLevel), 0.005f);

        await LoadLevelAsync();
    }
    /// <summary>
    /// ȫ������
    /// </summary>
    private void GlobalInit()
    {
        Invoke(nameof(CreateRope), 0.05f);//�ӳ��������ӣ����ùؿ���ʼ�����
        Invoke(nameof(CreateIce), 0.05f);//�ӳ����ɱ�
        Invoke(nameof(CreateDoor), 0.05f);
        Invoke(nameof(CreateBoom), 0.05f);
        Invoke(nameof(CreateChain), 0.05f);
        Invoke(nameof(CreateKey), 0.05f);
        Invoke(nameof(CreateLock), 0.05f);
    }
    /// <summary>
    /// �ض��ؿ�����
    /// </summary>
    private void UniqueInitObj()
    {
        MainSceneUI.Instance.SetLevelTip(false);
        Sprite icon = null;

        switch (levelNum)
        {
            case 1:
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Fill toolboxes with \nmatching pins");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "��ƥ���ͼ����乤����");
                }
                break;
            case 5:
                //�������ڵ���
                if (GameDataManager.CurrentGameData.isHoleLocked == false)
                {
                    Debug.Log("�Ѿ�������������");
                    break;
                }

                GameDataManager.UnlockItem(ItemType.Hole);
                UIManager.Instance.ShowUI<GetNewElemnetUI>();
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("GET A NEW HOLE!");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("����µ��򶴵���!");
                }
                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.Hole, 2);
                break;
            case 10:
                //�����������
                if (GameDataManager.CurrentGameData.isRocketLocked == false)
                {
                    Debug.Log("�Ѿ��������������");
                    break;
                }

                GameDataManager.UnlockItem(ItemType.Rocket);
                UIManager.Instance.ShowUI<GetNewElemnetUI>();
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("GET A NEW ROCKET!");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("����µĻ������!");
                }
                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.Rocket, 2);
                break;
            case 15:
                //�������ڵ���
                if (GameDataManager.CurrentGameData.isDoubleBoxLocked == false)
                {
                    Debug.Log("�Ѿ�������˫�����ӵ���");
                    break;
                }

                GameDataManager.UnlockItem(ItemType.DoubleBox);
                UIManager.Instance.ShowUI<GetNewElemnetUI>();
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("GET A NEW BOX!");
                }
                else
                {
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("����µĺ��ӵ���!");
                }

                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.DoubleBox, 2);
                break;
            case 11:
                //��ʮһ�أ�����������˿
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Use star shaped pins \nwhen needed");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "��Ҫʱʹ��������˿");
                }

                if (GameDataManager.CurrentGameData.isStarStrewLocked == false)
                {
                    Debug.Log("�Ѿ�������������˿");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("star");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("star");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 26:
                //�ڶ�ʮ���أ���������
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Connected pins move \ntogether");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "���ӵ���˿��һ���ƶ�");
                }

                if (GameDataManager.CurrentGameData.isRopeLocked == false)
                {
                    Debug.Log("�Ѿ�����������");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("rope");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("rope");
                UIManager.Instance.ShowUI<UnlockFuture>(); 
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 41:
                //����ʮһ�أ�������
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Break the ice by \ncollecting any 3 pins");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "�����ռ�������˿\n���Ʊ���");
                }

                if (GameDataManager.CurrentGameData.isIceLocked == false)
                {
                    Debug.Log("�Ѿ���������");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("ice");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("ice");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 56:
                //����ʮ���أ�������
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Shutter opens or closes \nwith each move");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "������ÿ���ƶ��򿪻�ر�");
                }

                if (GameDataManager.CurrentGameData.isDoorLocked == false)
                {
                    Debug.Log("�Ѿ���������");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("door");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("door");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 71:
                //����ʮһ�أ�����ը��
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Clear the bomb before \nrunning out of its moves");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "��ը�����ֽ���֮ǰ���ը��");
                }

                if (GameDataManager.CurrentGameData.isBoomLocked == false)
                {
                    Debug.Log("�Ѿ�������ը��");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("boom");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("boom");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 86:
                //�ڰ�ʮ���أ���������
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Remove connected \nchains to free the\nchained pin");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "�Ƴ����ӵ��������ͷ���ʽ��˿");
                }

                if (GameDataManager.CurrentGameData.ischainLocked == false)
                {
                    Debug.Log("�Ѿ�������������");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("chain");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("chain");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 101:
                //��101�أ�����Կ��
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Free the key to unlock \nthe lock");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "�ͷ�Կ���Կ���");
                }

                if (GameDataManager.CurrentGameData.isKeyLocked == false)
                {
                    Debug.Log("�Ѿ�������Կ��");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("key");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("key");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 116:
                //��116�أ���������

                if (GameDataManager.CurrentGameData.isClockLocked == false)
                {
                    Debug.Log("�Ѿ�����������");
                    break;
                }
                icon = ResourceLoader.Instance.GetUnlockImageSprite("clock");
                GameDataManager.UnlockGameSpecialItem("clock");

                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            default:
                MainSceneUI.Instance.SetLevelTip(false);
                break;
        }

        //������
        if (CurLevel.HasClock)
        {
            MainSceneUI.Instance._GamePlayUI.ShowClockUI(CurLevel.GetMinutes, CurLevel.GetSeconds);
            Debug.Log("��ʾ����");
        }
    }

    private async Task LoadLevelAsync()
    {
        if (levelNum % 10 == 0)
            await LoadLevelResources();

        ReleaseLevelResources();
    }

    private void InitLevelGameObj()
    {
        Transform gameParent = GameManager.Instance.transform;
        Debug.Log(levels["Level " + levelNum.ToString()]);
        level = Instantiate(levels["Level " + levelNum.ToString()]);;
        level.gameObject.SetActive(true);
        level.name = "Level";
        level.transform.position = new Vector3(0, -1, 0);
        level.transform.SetParent(gameParent);

        CurLevel = level.GetComponent<Level>();

        boxlevel = Instantiate(boxs["BoxLevel_" + levelNum.ToString()]);
        boxlevel.name = "BoxLevel";
        boxlevel.transform.SetParent(gameParent);

        if (CurLevel.IsHard)
            UIManager.Instance.ShowUI<HardLevelUI>();
    }
    public void ClearLevel()
    {
        if (boxlevel != null)
            Destroy(boxlevel);

        if (level != null)
            Destroy(level);

        for (int i = ropes.Count - 1; i >= 0; i--)
        {
            obiropeCrearer.ReturnRopeOrChain(ropes[i].GetComponent<ObiRope>());
        }
        ropes.Clear();

        foreach (ObiRope chain in chainList)
        {
            obiropeCrearer.ReturnRopeOrChain(chain);
        }
        chainList.Clear();

        foreach (GameObject key in keys)
        {
            Destroy(key);
        }
        keys.Clear();

        foreach (GameObject loc in locks)
        {
            Destroy(loc);
        }
        locks.Clear();

    }
    /// <summary>
    /// ��������ʱ�����ܹ�����������ݣ���Ҫ�ӳٵ���
    /// </summary>
    private void DelayInitBoxLevel()
    {
        GameManager.Instance.InitBoxLevel();
        //��ʼ���ؿ�����
        MainSceneUI.Instance.SetLevelNum(levelNum);
    }

    //���ӹؿ���
    public void AddLevelNum()
    {
        GameDataManager.AddLevelNum();
        levelNum = GameDataManager.CurrentGameData.levelNum;
    }
    public void ReStartGmae()
    {
        InitLevel();
    }

    #region ȫ������
    /// <summary>
    /// ��������
    /// </summary>
    private void CreateRope()
    {
        foreach (Transform trans in level.transform)
        {
            Layer layer = trans.GetComponent<Layer>();
            //�ò㼶�����ӵ���˿
            if (layer != null && layer.HasConnected == true)
            {
                List<Glass> curglassList = layer.GlassList;
                foreach (Glass glass in curglassList)
                {
                    //�ò����������ӵ���˿
                    if (glass.HasConnect)
                    {
                        List<Screw> curScrewList = glass.ScrewList;
                        foreach (Screw screw in curScrewList)
                        {
                            //����˿������
                            if (screw.ConnectedScrew != null && screw.rope == null)
                            {
                                ObiRope curRope = obiropeCrearer.CreateRopeOrChain(screw.transform, screw.ConnectedScrew.transform, false);
                                screw.SetObiRope(curRope);
                                screw.ConnectedScrew.SetObiRope(curRope);
                                ropes.Add(curRope.gameObject);
                            }
                        }
                    }
                }
            }

        }
    }
    /// <summary>
    /// ���ɱ�
    /// </summary>
    private void CreateIce()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasIceCovered)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasIceCoverd)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasIceCovered)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //���ɱ�
                                if (curScrew.IsIceCovered)
                                {
                                    GameObject curIce = Instantiate(icePrefab, curScrew.transform.parent);
                                    curIce.transform.position = curScrew.transform.position;
                                    curIce.GetComponent<Ice>().SetSortingLayer(curScrew.LayerName);

                                    curScrew.ScrewIce = curIce.GetComponent<Ice>();
                                }
                            }
                        }

                    }
                }

            }
        }
    }
    /// <summary>
    /// ������
    /// </summary>
    private void CreateDoor()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasDoor)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasDoor)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasDoor)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //������
                                if (curScrew.HasDoor)
                                {
                                    GameObject newDoor = Instantiate(doorPrefab, curScrew.transform.parent);
                                    newDoor.transform.position = curScrew.transform.position;

                                    Door doorScript = newDoor.GetComponent<Door>();
                                    curScrew.ScrewDoor = doorScript;

                                    doorScript.SetClose(curScrew.IsDoorClose);
                                    doorScript.SetLayer(curScrew.LayerName, curScrew.LayerOrder);
                                }
                            }
                        }

                    }
                }

            }
        }
    }
    /// <summary>
    /// ����ը��
    /// </summary>
    private void CreateBoom()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasBoom)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasBoom)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasBoom)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //����ը��
                                if (curScrew.HasBoom)
                                {
                                    GameObject newBoom = Instantiate(boomPrefab, curScrew.transform.parent);
                                    newBoom.transform.position = curScrew.transform.position;
                                    newBoom.gameObject.SetActive(true);

                                    Boom boomScript = newBoom.GetComponent<Boom>();
                                    boomScript.InitBoom(curScrew.ScrewColor, curScrew.BoomCount, curScrew.LayerName, curScrew.LayerOrder);
                                    curScrew.ScrewBoom = boomScript;
                                }
                            }
                        }

                    }
                }

            }
        }
    }
    /// <summary>
    /// ������
    /// </summary>
    private void CreateChain()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasChain)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasChain)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasChain)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //������
                                if (curScrew.HasChain)
                                {
                                    //�����������뵽���б���
                                    chainList.AddRange(curScrew.CreateChain(obiropeCrearer));
                                    GameObject curChainFx = Instantiate(chainFxPrefab, curScrew.transform.parent);
                                    curChainFx.transform.position = curScrew.transform.position;

                                    curScrew.SetChain(curChainFx.GetComponent<ChainFx>());

                                }
                            }
                        }

                    }
                }

            }
        }
    }

    /// <summary>
    /// ����Կ��
    /// </summary>
    private void CreateKey()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasKey)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasKey)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasKey)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //����Կ��
                                if (curScrew.HasKey)
                                {
                                    GameObject newKey = Instantiate(keyPrefab, curScrew.transform.parent);
                                    newKey.transform.position = curScrew.transform.position + new Vector3(0, -0.4f, 0);
                                    newKey.SetActive(true);

                                    Key curKey = newKey.GetComponent<Key>();
                                    curScrew.ScrewKey = curKey;
                                    curKey.InitKeyLayer(curScrew.LayerName, curScrew.LayerOrder - 1);

                                    keys.Add(newKey);
                                }
                            }
                        }

                    }
                }

            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void CreateLock()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasLock)
        {
            //�����ؿ��㼶
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasLock)
                {
                    //��������
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasLock)
                        {
                            //����С��
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //����Կ��
                                if (curScrew.HasLock)
                                {
                                    GameObject newLock = Instantiate(lockPrefab, curScrew.transform.parent);
                                    newLock.transform.position = curScrew.transform.position;
                                    newLock.SetActive(true);

                                    Lock curLock = newLock.GetComponent<Lock>();
                                    curScrew.ScrewLock = curLock;
                                    curLock.InitLockLayer(curScrew.LayerName, curScrew.LayerOrder + 1);

                                    locks.Add(newLock);
                                }
                            }
                        }

                    }
                }

            }
        }
    }
    #endregion

    /// <summary>
    /// �첽������Դ(Ԥ����)
    /// </summary>
    /// <returns></returns>
    private async Task LoadLevelResources()
    {
        int num = levelNum + 10;

        if (num >= 210)
            return;

        await ResourceLoader.Instance.LoadLevelResources(num);
        levels = ResourceLoader.Instance.Levels;
        boxs = ResourceLoader.Instance.BoxLevels;
    }

    private void ReleaseLevelResources()
    {
        int num = levelNum % 10;
        //�ͷ�֮ǰ����Դ
        if (num == 1)
        {
            ResourceLoader.Instance.ReleaseLevelResources(NameUtility.SetLastDigitToZero(levelNum));
        }
    }
}
