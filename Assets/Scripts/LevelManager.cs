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
    [Header("绳子")]
    [SerializeField] private ObiRopeChainCreator obiropeCrearer;
    [Header("冰")]
    [SerializeField] private GameObject icePrefab;
    [Header("门")]
    [SerializeField] private GameObject doorPrefab;
    [Header("炸弹")]
    [SerializeField] private GameObject boomPrefab;
    [Header("锁链")]
    [SerializeField] private GameObject chainFxPrefab;
    [Header("门锁")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject lockPrefab;
    public ObiRopeChainCreator ObiRopeCreater
    {
        get => obiropeCrearer;
    }
    private static LevelManager _instance;
    private GameObject boxlevel;//记录上次的关卡，用于删除
    private GameObject level;//记录上次的关卡，用于删除
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
        // 输出当前关卡号和金币数
        levelNum = GameDataManager.CurrentGameData.levelNum;
    }
    // 提取文件名中的数字部分
    private int ExtractNumber(string name)
    {
        string number = new string(name.Where(char.IsDigit).ToArray());
        return string.IsNullOrEmpty(number) ? 0 : int.Parse(number);
    }

    /// <summary>
    /// 将关卡实例化出来
    /// </summary>
    public async void InitLevel()
    {
        GameManager.Instance.EnterMainScene();
        ClearLevel();
        //实例化关卡
        InitLevelGameObj();

        GlobalInit();

        UniqueInitObj();
        Invoke(nameof(DelayInitBoxLevel), 0.005f);

        await LoadLevelAsync();
    }
    /// <summary>
    /// 全局生成
    /// </summary>
    private void GlobalInit()
    {
        Invoke(nameof(CreateRope), 0.05f);//延迟生成绳子，先让关卡初始化完成
        Invoke(nameof(CreateIce), 0.05f);//延迟生成冰
        Invoke(nameof(CreateDoor), 0.05f);
        Invoke(nameof(CreateBoom), 0.05f);
        Invoke(nameof(CreateChain), 0.05f);
        Invoke(nameof(CreateKey), 0.05f);
        Invoke(nameof(CreateLock), 0.05f);
    }
    /// <summary>
    /// 特定关卡生成
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
                    MainSceneUI.Instance.SetLevelTip(true, "用匹配的图钉填充工具箱");
                }
                break;
            case 5:
                //解锁洞口道具
                if (GameDataManager.CurrentGameData.isHoleLocked == false)
                {
                    Debug.Log("已经解锁过洞道具");
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
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("获得新的球洞道具!");
                }
                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.Hole, 2);
                break;
            case 10:
                //解锁火箭道具
                if (GameDataManager.CurrentGameData.isRocketLocked == false)
                {
                    Debug.Log("已经解锁过火箭道具");
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
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("获得新的火箭道具!");
                }
                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.Rocket, 2);
                break;
            case 15:
                //解锁洞口道具
                if (GameDataManager.CurrentGameData.isDoubleBoxLocked == false)
                {
                    Debug.Log("已经解锁过双重箱子道具");
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
                    UIManager.Instance.GetUI<GetNewElemnetUI>().SetContextText("获得新的盒子道具!");
                }

                UIManager.Instance.GetUI<GetNewElemnetUI>().SetMoveIcon(ItemType.DoubleBox, 2);
                break;
            case 11:
                //第十一关，解锁星星螺丝
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Use star shaped pins \nwhen needed");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "需要时使用星型螺丝");
                }

                if (GameDataManager.CurrentGameData.isStarStrewLocked == false)
                {
                    Debug.Log("已经解锁过星星螺丝");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("star");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("star");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 26:
                //第二十六关，解锁绳索
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Connected pins move \ntogether");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "链接的螺丝会一起移动");
                }

                if (GameDataManager.CurrentGameData.isRopeLocked == false)
                {
                    Debug.Log("已经解锁过绳索");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("rope");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("rope");
                UIManager.Instance.ShowUI<UnlockFuture>(); 
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 41:
                //第四十一关，解锁冰
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Break the ice by \ncollecting any 3 pins");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "任意收集三个螺丝\n打破冰面");
                }

                if (GameDataManager.CurrentGameData.isIceLocked == false)
                {
                    Debug.Log("已经解锁过冰");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("ice");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("ice");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 56:
                //第五十六关，解锁门
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Shutter opens or closes \nwith each move");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "快门随每次移动打开或关闭");
                }

                if (GameDataManager.CurrentGameData.isDoorLocked == false)
                {
                    Debug.Log("已经解锁过门");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("door");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("door");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 71:
                //第七十一关，解锁炸弹
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Clear the bomb before \nrunning out of its moves");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "在炸弹数字结束之前清除炸弹");
                }

                if (GameDataManager.CurrentGameData.isBoomLocked == false)
                {
                    Debug.Log("已经解锁过炸弹");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("boom");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("boom");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 86:
                //第八十六关，解锁锁链
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Remove connected \nchains to free the\nchained pin");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "移除连接的链条以释放链式螺丝");
                }

                if (GameDataManager.CurrentGameData.ischainLocked == false)
                {
                    Debug.Log("已经解锁锁过锁链");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("chain");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("chain");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 101:
                //第101关，解锁钥匙
                if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "Free the key to unlock \nthe lock");
                }
                else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
                {
                    MainSceneUI.Instance.SetLevelTip(true, "释放钥匙以开锁");
                }

                if (GameDataManager.CurrentGameData.isKeyLocked == false)
                {
                    Debug.Log("已经解锁过钥匙");
                    break;
                }
                GameDataManager.UnlockGameSpecialItem("key");

                icon = ResourceLoader.Instance.GetUnlockImageSprite("key");
                UIManager.Instance.ShowUI<UnlockFuture>();
                UIManager.Instance.GetUI<UnlockFuture>().SetUnlockIcon(icon);
                break;
            case 116:
                //第116关，解锁闹钟

                if (GameDataManager.CurrentGameData.isClockLocked == false)
                {
                    Debug.Log("已经解锁过闹钟");
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

        //有闹钟
        if (CurLevel.HasClock)
        {
            MainSceneUI.Instance._GamePlayUI.ShowClockUI(CurLevel.GetMinutes, CurLevel.GetSeconds);
            Debug.Log("显示闹钟");
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
    /// 销毁物体时，不能够立即获得数据，需要延迟调用
    /// </summary>
    private void DelayInitBoxLevel()
    {
        GameManager.Instance.InitBoxLevel();
        //初始化关卡数字
        MainSceneUI.Instance.SetLevelNum(levelNum);
    }

    //增加关卡数
    public void AddLevelNum()
    {
        GameDataManager.AddLevelNum();
        levelNum = GameDataManager.CurrentGameData.levelNum;
    }
    public void ReStartGmae()
    {
        InitLevel();
    }

    #region 全局生成
    /// <summary>
    /// 生成绳子
    /// </summary>
    private void CreateRope()
    {
        foreach (Transform trans in level.transform)
        {
            Layer layer = trans.GetComponent<Layer>();
            //该层级有链接的螺丝
            if (layer != null && layer.HasConnected == true)
            {
                List<Glass> curglassList = layer.GlassList;
                foreach (Glass glass in curglassList)
                {
                    //该玻璃有有链接的螺丝
                    if (glass.HasConnect)
                    {
                        List<Screw> curScrewList = glass.ScrewList;
                        foreach (Screw screw in curScrewList)
                        {
                            //该螺丝有链接
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
    /// 生成冰
    /// </summary>
    private void CreateIce()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasIceCovered)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasIceCoverd)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasIceCovered)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成冰
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
    /// 生成门
    /// </summary>
    private void CreateDoor()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasDoor)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasDoor)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasDoor)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成门
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
    /// 生成炸弹
    /// </summary>
    private void CreateBoom()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasBoom)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasBoom)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasBoom)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成炸弹
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
    /// 生成锁
    /// </summary>
    private void CreateChain()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasChain)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasChain)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasChain)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成锁
                                if (curScrew.HasChain)
                                {
                                    //创建锁，加入到锁列表中
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
    /// 生成钥匙
    /// </summary>
    private void CreateKey()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasKey)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasKey)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasKey)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成钥匙
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
    /// 生成门锁
    /// </summary>
    private void CreateLock()
    {
        Level curLevel = level.GetComponent<Level>();
        if (curLevel != null && curLevel.HasLock)
        {
            //遍历关卡层级
            List<Layer> layerList = curLevel.LayerList;
            foreach (Layer curLayer in layerList)
            {
                if (curLayer.HasLock)
                {
                    //遍历玻璃
                    List<Glass> glassList = curLayer.GlassList;
                    foreach (Glass curGlass in glassList)
                    {
                        if (curGlass.HasLock)
                        {
                            //遍历小球
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //生成钥匙
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
    /// 异步加载资源(预加载)
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
        //释放之前的资源
        if (num == 1)
        {
            ResourceLoader.Instance.ReleaseLevelResources(NameUtility.SetLastDigitToZero(levelNum));
        }
    }
}
