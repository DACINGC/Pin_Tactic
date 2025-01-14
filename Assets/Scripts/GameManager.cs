using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState
{ 
    Stop, Start
}
public class GameManager : MonoBehaviour
{
    #region 组件
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject("GameManager");
                    instance = singleton.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private Transform boxLevel;

    private RocketSpawner rocketController; //火箭生成
    private EmptyHoleManager emptyHoleManager;//空槽
    #endregion

    #region 箱子
    private List<Box> boxList = new List<Box>();//箱子的列表
    [SerializeField] private Box curBox;//当前的箱子
    public Box GetCurBox()
    {
        return curBox;
    }
    private Box extraBox;//额外的箱子
    private int curBoxIndex;//当前的索引
    private bool isRocketClick;//当前是否处于火箭点击的状态
    public void SetRocketClickFalse()
    {
        isRocketClick = false;
    }
    private bool isDoubleBox;//当前是否有额外的盒子
    public bool IsDoubleBox
    {
        get => isDoubleBox;
    }

    [Range(0f, 1f)]
    [SerializeField] private float screenFraction = 0.125f; // 从顶部向下的位置比例，默认是 1/8
    private Vector3 boxMoveLeftPos;
    private Vector3 boxMoveCenterPos;
    private Vector3 boxMoveRightPos;
    private Vector3 boxCenterLeftPos;
    private Vector3 boxCenterRightPos;
    public Vector3 RightPos
    {
        get
        {
            return boxMoveRightPos;
        }
    }
    #endregion

    private GameState gameState;//当前游戏状态
    public GameState _GameState { get => gameState; }
    public void SetGameState(GameState state)
    {
        gameState = state;
    }


    [SerializeField] private GameObject HomeScene;
    public Screw CurScrew { get; private set; }//记录当前小球是否在移动
    public void SetISRocketClick(bool value)
    {
        isRocketClick = value;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        rocketController = GetComponent<RocketSpawner>();
    }

    private void Start()
    {
        emptyHoleManager = transform.Find("EmptyHoleManager").GetComponent<EmptyHoleManager>();

        if (gameState == GameState.Stop)
            emptyHoleManager.gameObject.SetActive(false);
        //EnterHomeScene();
        //EnterMainScene();
        //检查当前关卡数是否小于5

        if (LevelManager.Instance.GetLevleNum() <= 5)
        {
            //显示游戏场景
            EnterMainScene();
            LevelManager.Instance.InitLevel();
        }
        else
        {
            //显示主页面场景
            EnterHomeScene();
        }

        //计算箱子移动的点位
        CalculatePositions();
    }
    // 检测点击的位置并发送射线
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 当鼠标左键被按下
        {
            if (gameState == GameState.Stop)
            {
                //Debug.Log("游戏状态为结束状态，无法点击");
                return;
            }

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 将屏幕坐标转换为世界坐标
            Vector3 rayOrigin = new Vector3(mousePosition.x, mousePosition.y, 0);

            #region 火箭效果点击
            //点击发射火箭
            if (isRocketClick)
            {
                Collider2D[] glass = Physics2D.OverlapCircleAll(rayOrigin, 0.001f);
                //点击到玻璃才发射火箭
                foreach (Collider2D obj in glass)
                {
                    if (obj.GetComponent<Glass>() != null && obj.GetComponent<Glass>().IsExplosion == false)
                    {
                        AudioManager.Instance.PlaySFX("ScrewClick");

                        rocketController.SpawnRocket(new Vector3(rayOrigin.x, rayOrigin.y, -20), obj.GetComponent<Glass>());
                        isRocketClick = false;
                        UIManager.Instance.HideUI<RocketUI>();

                        MainSceneUI.Instance._GamePlayUI.UpdateRocketText();
                        return;
                    }
                }
                
            }
            #endregion

            //点击放入小球
            //将射中的第一个小球放入槽中
            Collider2D[] hits = Physics2D.OverlapCircleAll(rayOrigin, 0.001f);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<Screw>() != null)
                {
                    Screw curScrew = hit.GetComponent<Screw>();
                    if (curScrew.CanClick())
                    {
                        CurScrew = curScrew;

                        #region 从玻璃中移除螺丝
                        //从玻璃中移除螺丝
                        if (curScrew.CanRemoveFromGlass())
                        { 
                            curScrew.RemoveFromGlass();

                            if (curScrew.ConnectedScrew != null)
                                curScrew.ConnectedScrew.RemoveFromGlass();

                            #region 全局效果检查
                            CheckIceBreak();//是否有冰可以进行破坏

                            if (curScrew.HasDoor)//如果有门，需要将当前的门设置为关闭
                            {
                                curScrew.ScrewDoor.CloseDoor();
                                curScrew.SetDoorFlase();
                            }

                            CheckDoor();//是否有门

                            if (curScrew.HasBoom)//如果有炸弹需要将炸弹消除
                            {
                                curScrew.DestoryBoom();
                            }
                            CheckBoom();//是否有炸弹

                            if (curScrew.IsOtherChain) //是否有锁链
                            {
                                curScrew.UnlockChain();
                            }

                            if (curScrew.HasKey)
                            {
                                curScrew.KeyMoveToLcok();
                            }
                            #endregion
                        }
                        else
                            Debug.Log(curScrew.ScrewColor +  "无法从玻璃中移除！");
                        #endregion

                        AudioManager.Instance.PlaySFX("ScrewClick");
                        SetScrew(curScrew);//设置球到相应的槽中
                        return;
                    }
                    else
                    {
                        //Debug.Log("小球无法点击 : " + curScrew.ScrewColor);
                    }
                }
            }

        }
    }
    #region 螺丝
    /// <summary>
    /// 检查球与当前槽颜色是相等
    /// </summary>
    private bool CheckBoxCrewColor(Box checkBox ,ScrewColor _color)
    {
        return checkBox.BoxColor == _color;
    }
    /// <summary>
    /// 设置球到当前槽的相应位置
    /// </summary>
    public void SetScrew(Screw screw)
    {
        //不是两个箱子的状态
        if (isDoubleBox == false)
        {
            SetboxWihtScrew(curBox, screw);
        }
        else
        {
            //两个箱子的状态
            if (CheckBoxCrewColor(curBox, screw.ScrewColor))
            {
                //与第一个箱子的颜色相同
                SetboxWihtScrew(curBox, screw);
            }
            else if (CheckBoxCrewColor(extraBox, screw.ScrewColor))
            {
                //与第二个箱子的颜色相同
                SetboxWihtScrew(extraBox, screw);
            }
            else
            {
                //都不相同，加入到空槽中
                emptyHoleManager.AddToEmptyHole(screw);
            }

        }

    }
    private void SetboxWihtScrew(Box box, Screw screw)
    {
        //检查颜色是否一致
        if (CheckBoxCrewColor(box, screw.ScrewColor) && box.IsMoving == false)
        {
            //是星星螺丝
            if (screw.IsStar())
            {
                SetStarScrew(box, screw);
            }
            else
            {
                SetNormalScrew(box, screw);
            }
        }
        else
        {
            //颜色不一致
            emptyHoleManager.AddToEmptyHole(screw);
        }

    }
    /// <summary>
    /// 安置普通螺丝
    /// </summary>
    private void SetNormalScrew(Box box, Screw screw)
    {
        //普通螺丝
        //普通洞口已满
        if (box.ISNormalFull())
        {
            emptyHoleManager.AddToEmptyHole(screw);
        }
        else
        {
            box.SetHoleList(screw);
        }
    }
    /// <summary>
    /// 安置星星螺丝
    /// </summary>
    private void SetStarScrew(Box box, Screw screw)
    {
        //箱子星星洞口是否已满
        if (box.ISStarFull())
        {
            emptyHoleManager.AddToEmptyHole(screw);
        }
        else
        {
            box.SetHoleList(screw);
        }
    }
    #endregion

    #region 箱子
    /// <summary>
    /// 更换当前的槽
    /// </summary>
    public void ChangeBox()
    {
        //检查是否满足有结束的条件
        if (CheckGameCompleted())
        {
            //游戏完成执行的逻辑
            WinGame();
            return;
        }
        //没有额外的箱子
        if (isDoubleBox == false)
        {
            ChangeSingleBox();
        }
        else
        {
            //有额外的箱子
            ChangeDoubleBox();
        }

    }
    private void ChangeDoubleBox()
    {
        //检查两个箱子是否都已经满足条件
        if (curBox.IsCompleted && extraBox.IsCompleted)
        {
            Box preBox = curBox;//记录上一个箱子（两个盒子的状态需要移动）
            curBoxIndex++;
            curBox = boxList[curBoxIndex];

            //后面只有一个箱子
            if (curBoxIndex == boxList.Count - 1)
            {
                curBox.MoveCenter(boxMoveCenterPos);
            }
            else
            {
                Debug.Log("移动两个物体");
                //将之前的两个箱子移动到右边
                MoveObjectsToPositions(preBox.gameObject, extraBox.gameObject, boxMoveRightPos, boxMoveRightPos, () =>
                {
                    //移动完成后
                    //将后面的两个箱子移动到指定位置
                    MoveObjectsToPositions(boxList[curBoxIndex + 1].gameObject, curBox.gameObject, boxMoveLeftPos, boxMoveCenterPos, () => {
                        //移动到中心位置后尝试将螺丝加入到箱子中
                        emptyHoleManager.AddToBox(curBox);
                    });
                }, 0.3f, 0.15f);
            }
            preBox = null;
            extraBox = null;
            isDoubleBox = false;
        }
    }
    private void ChangeSingleBox()
    {
        curBoxIndex++;
        curBox = boxList[curBoxIndex];
        curBox.MoveCenter(boxMoveCenterPos);
        //移动下一个箱子到左边的位置
        if (curBoxIndex + 1 != boxList.Count)
        {
            boxList[curBoxIndex + 1].MoveLeft(boxMoveLeftPos);
        }
    }

    /// <summary>
    /// 外部调用的空槽加入箱子的函数
    /// </summary>
    public void EmptyScrewToBox()
    {
        emptyHoleManager.AddToBox(curBox);
    }

    /// <summary>
    /// 根据屏幕比例计算点位
    /// </summary>
    private void CalculatePositions()
    {
        // 获取主摄像机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // 获取屏幕宽高
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 根据屏幕比例计算纵向的 Y 坐标
        float yPos = screenHeight * (1 - screenFraction);

        // 屏幕空间点位
        Vector3 screenPosLeft = new Vector3(0, yPos, 0);                     // 最左侧点
        Vector3 screenPosCenter = new Vector3(screenWidth / 2, yPos, 0);    // 中间点
        Vector3 screenPosRight = new Vector3(screenWidth, yPos, 0);         // 最右侧点
        Vector3 screenPosCenterLeft = new Vector3(screenWidth * 5/ 16, yPos, 0); // 左侧 1/4
        Vector3 screenPosCenterRight = new Vector3(screenWidth * 11 / 16, yPos, 0); // 右侧 3/4

        // 转换到世界空间
        boxMoveLeftPos = mainCamera.ScreenToWorldPoint(screenPosLeft);
        boxMoveCenterPos = mainCamera.ScreenToWorldPoint(screenPosCenter);
        boxMoveRightPos = mainCamera.ScreenToWorldPoint(screenPosRight) + Vector3.right * 6;
        boxCenterLeftPos = mainCamera.ScreenToWorldPoint(screenPosCenterLeft);
        boxCenterRightPos = mainCamera.ScreenToWorldPoint(screenPosCenterRight);

        // 确保 Z 值为 0 (适配 2D 平面)
        boxMoveLeftPos.z = 0;
        boxMoveCenterPos.z = 0;
        boxMoveRightPos.z = 0;
        boxCenterLeftPos.z = 0;
        boxCenterRightPos.z = 0;

    }

    /// <summary>
    /// 在场景视图中绘制点用于调试
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(boxMoveLeftPos, 0.3f); // 最左侧点
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(boxMoveCenterPos, 0.3f); // 中间点
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(boxMoveRightPos, 0.3f); // 最右侧点
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(boxCenterLeftPos, 0.3f); // 左侧 1/4 点
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(boxCenterRightPos, 0.3f); // 右侧 1/4 点
    }
    #endregion

    #region 游戏相关
    public void AddSpaceToReviewGame()
    {
        UIManager.Instance.HideUI<OutOfSpaceUI>();
        AddExtraHole();
        gameState = GameState.Start;

    }
    public void EnterHomeScene()
    {
        LevelManager.Instance.ClearLevel();
        MainSceneUI.Instance.SetMainScene(false);
        HomeSceneUI.Instance.SetHomeScene(true);
        emptyHoleManager.gameObject.SetActive(false);

        //生命值不足，显示生命值不足的UI
        if (GameDataManager.CurrentGameData.heartCount == 0)
        { 
            UIManager.Instance.ShowUI<OutOfLiveUI>();
        }

        int levelNum = LevelManager.Instance.GetLevleNum();
        switch (levelNum)
        {
            case 6:
                if (GameDataManager.CurrentGameData.isDailyRewardLocked == false)
                    break;

                //解锁每日
                GameDataManager.UnlockHomeButton("daily");
                UIManager.Instance.ShowUI<DailyBonusUI>();
                TimeManager.Instance.StartDailyTime();//开始每日计时

                break;

            case 10:
                //第10关解锁Streak
                if (GameDataManager.CurrentGameData.isStreaklocked == false)
                    break;

                
                GameDataManager.UnlockHomeButton("streak");
                UIManager.Instance.ShowUI<StartEvnetStreakUI>();
                HomeSceneUI.Instance.homeUI.FreshHomeButton();
                TimeManager.Instance.StartStreakTime();//开始条纹计时
                break;

            case 20:
                //解锁抽奖
                if (GameDataManager.CurrentGameData.isLuckySpinlocked == false)
                    break;

                GameDataManager.UnlockHomeButton("luckyspin");
                HomeSceneUI.Instance.homeUI.FreshHomeButton();
                TimeManager.Instance.StartLuckySpinTime();
                break;
            case 30:
                //解锁飞机
                break;

                //if (GameDataManager.CurrentGameData.isSkyRacelocked == false)
                //    break;

                //GameDataManager.UnlockHomeButton("sky");
                //HomeSceneUI.Instance.homeUI.FreshHomeButton();
                //break;
        }

        //更新homeUI
        //HomeSceneUI.Instance.homeUI.UpdateHomeUI();

        gameState = GameState.Stop;
    }
    public void EnterMainScene()
    {
        HomeSceneUI.Instance.SetHomeScene(false);
        MainSceneUI.Instance.SetMainScene(true);
        emptyHoleManager.gameObject.SetActive(true);
        emptyHoleManager.ClearEmptyHole();
        gameState = GameState.Start;

        //HomeSceneUI.Instance.homeUI.winStreakUI.InitStreakPos();
        //LevelManager.Instance.InitLevel();
    }

    /// <summary>
    /// 检查游戏是否结束的函数
    /// </summary>
    private bool CheckGameCompleted()
    {
        return curBoxIndex == boxList.Count - 1;
    }

    /// <summary>
    /// 初始化关卡(记录关卡数据)
    /// </summary>
    public void InitBoxLevel()
    {
        //清空操作
        EnterMainScene();

        boxList.Clear();
        boxLevel = null;
        extraBox = null;
        isDoubleBox = false;
        curBoxIndex = 0;

        //初始化
        boxLevel = transform.Find("BoxLevel").transform;
        boxLevel.transform.position = new Vector3(0, 0, 0);
        for (int i = 0; i < boxLevel.childCount; i++)
        {
            if (boxLevel.GetChild(i).GetComponent<Box>() != null)
                boxList.Add(boxLevel.GetChild(i).GetComponent<Box>());
        }
        curBox = boxList[0];

        //设置box到相应的位置
        for (int i = 0; i < boxLevel.childCount; i++)
        {
            if (i == 0)
            {
                boxList[i].transform.position = new Vector3(0, 11.5f, 0);
            }
            else if (i == 1)
            {
                boxList[i].transform.position = new Vector3(-8, 11.5f, 0);
            }
            else
            {
                boxList[i].transform.position = new Vector3(-16, 11.5f, 0);
            }
        }

        gameState = GameState.Start;
    }

    /// <summary>
    /// 退出游戏场景
    /// </summary>
    public void ExitGame()
    {
        gameState = GameState.Stop;
        LevelManager.Instance.ClearLevel();
        UIManager.Instance.HideUI<ExitGameUI>(0, ()=>
        {
            EnterHomeScene();
        });
    }
    /// <summary>
    /// 游戏胜利的逻辑
    /// </summary>
    private void WinGame()
    {
        //Debug.Log("游戏胜利");
        gameState = GameState.Stop;
        UIManager.Instance.ShowUI<WinUI>();
    }
    /// <summary>
    /// 游戏失败(空槽满了)
    /// </summary>
    public void loseGame()
    {
        if (emptyHoleManager.CheckGameFailed() && gameState == GameState.Start)
        {
            gameState = GameState.Stop;

            //检查是否还可以加入额外的洞口
            if (emptyHoleManager.CanAddExtraHole())
            {
                UIManager.Instance.ShowUI<OutOfSpaceUI>();
            }
            else
            {
                //如果没有额外的洞口可以添加了，则无法返回上一级
                //如果当前连胜，并且解锁了连胜
                if (GameDataManager.CurrentGameData.isStreaklocked == false && GameDataManager.CurrentGameData.curStreakIndex != 0)
                {
                    UIManager.Instance.GetUI<LoseStreakUI>().SetBackObj(false);
                    UIManager.Instance.ShowUI<LoseStreakUI>();
                }
                else
                {
                    UIManager.Instance.GetUI<LoseUI>().SetBackObj(false);
                    UIManager.Instance.ShowUI<LoseUI>();
                }
                AudioManager.Instance.PlaySFX("Lose");
            }
        }

    }
    #endregion

    #region 道具

    /// <summary>
    /// 道具一的调用函数（启用额外的洞口）
    /// </summary>
    public void AddExtraHole()
    {
        emptyHoleManager.ActivateExtraHole();
    }

    /// <summary>
    /// 道具三的移动函数
    /// </summary>
    public void MoveObjectsToPositions(GameObject moveLeftObj, GameObject moveRightObj, Vector3 leftPos, Vector3 rightPos, System.Action callBack = null, float time1 = 0.3f, float time2 = 0.2f)
    {
        // 同时启动两个 DOTween 动画
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(moveLeftObj.transform.DOMove(leftPos, time1).SetEase(Ease.InOutSine));
        moveSequence.Join(moveRightObj.transform.DOMove(rightPos, time2).SetEase(Ease.InOutSine));
        //动画完成后的回调
        moveSequence.OnComplete(() =>
        {
            callBack?.Invoke();
        });
    }
    /// <summary>
    /// 启用额外的箱子
    /// </summary>
    public void ActiveExtraBox()
    {
        if (curBoxIndex + 1 < boxList.Count)
        {
            //将之前的箱子设置为额外的盒子
            extraBox = curBox;
            //将下一个盒子作为当前盒子
            curBox = boxList[++curBoxIndex];

            isDoubleBox = true;
            MoveObjectsToPositions(curBox.gameObject, extraBox.gameObject, boxCenterLeftPos, boxCenterRightPos, () => {
                //尝试将螺丝加入到下一个箱子中
                emptyHoleManager.AddToBox(curBox);
            });

        }
        else
        {
            Debug.Log("以及没有额外的箱子了");
        }
    }

    public bool CanAddExtraBox()
    {
        if (isDoubleBox)
        {
            Debug.Log("已经有额外的箱子了");
            return false;
        }
        else if (curBox.IsMoving)
        {
            Debug.Log("当前箱子正在移动");
            return false;
        }

        return true;
    }
    #endregion

    #region 全局效果
    private void CheckIceBreak()
    {
        Level curLevel = LevelManager.Instance.CurLevel;
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
                            //遍历螺丝
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //查看螺丝是否有冰，并且可以进行点击
                                if (curScrew.IsIceCovered && curScrew.HasGlassCovered() == false)
                                {
                                    //检查冰是否已经破坏完全
                                    if (curScrew.ScrewIce.IceBreak())
                                    {
                                        curScrew.SetIceCoveredFalse();
                                    }
                                }
                            }
                        }

                    }
                }

            }
        }
    }

    private void CheckDoor()
    {
        Level curLevel = LevelManager.Instance.CurLevel;
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
                                //查看是否有门 && 并且没有被玻璃覆盖
                                if (curScrew.HasDoor && curScrew.HasGlassCovered() == false)
                                {
                                    
                                    curScrew.ScrewDoor.OperateDoor();
                                }
                            }
                        }

                    }
                }

            }
        }
    }

    private void CheckBoom()
    {
        Level curLevel = LevelManager.Instance.CurLevel;
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
                                //有炸弹，并且没有被覆盖
                                if (curScrew.HasBoom && curScrew.HasGlassCovered() == false)
                                {
                                    curScrew.DoBoomAnim();
                                }
                            }
                        }

                    }
                }

            }
        }
    }

    #endregion
}
