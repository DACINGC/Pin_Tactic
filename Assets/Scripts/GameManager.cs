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
    #region ���
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

    private RocketSpawner rocketController; //�������
    private EmptyHoleManager emptyHoleManager;//�ղ�
    #endregion

    #region ����
    private List<Box> boxList = new List<Box>();//���ӵ��б�
    [SerializeField] private Box curBox;//��ǰ������
    public Box GetCurBox()
    {
        return curBox;
    }
    private Box extraBox;//���������
    private int curBoxIndex;//��ǰ������
    private bool isRocketClick;//��ǰ�Ƿ��ڻ�������״̬
    public void SetRocketClickFalse()
    {
        isRocketClick = false;
    }
    private bool isDoubleBox;//��ǰ�Ƿ��ж���ĺ���
    public bool IsDoubleBox
    {
        get => isDoubleBox;
    }

    [Range(0f, 1f)]
    [SerializeField] private float screenFraction = 0.125f; // �Ӷ������µ�λ�ñ�����Ĭ���� 1/8
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

    private GameState gameState;//��ǰ��Ϸ״̬
    public GameState _GameState { get => gameState; }
    public void SetGameState(GameState state)
    {
        gameState = state;
    }


    [SerializeField] private GameObject HomeScene;
    public Screw CurScrew { get; private set; }//��¼��ǰС���Ƿ����ƶ�
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
        //��鵱ǰ�ؿ����Ƿ�С��5

        if (LevelManager.Instance.GetLevleNum() <= 5)
        {
            //��ʾ��Ϸ����
            EnterMainScene();
            LevelManager.Instance.InitLevel();
        }
        else
        {
            //��ʾ��ҳ�泡��
            EnterHomeScene();
        }

        //���������ƶ��ĵ�λ
        CalculatePositions();
    }
    // �������λ�ò���������
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ��������������
        {
            if (gameState == GameState.Stop)
            {
                //Debug.Log("��Ϸ״̬Ϊ����״̬���޷����");
                return;
            }

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // ����Ļ����ת��Ϊ��������
            Vector3 rayOrigin = new Vector3(mousePosition.x, mousePosition.y, 0);

            #region ���Ч�����
            //���������
            if (isRocketClick)
            {
                Collider2D[] glass = Physics2D.OverlapCircleAll(rayOrigin, 0.001f);
                //����������ŷ�����
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

            //�������С��
            //�����еĵ�һ��С��������
            Collider2D[] hits = Physics2D.OverlapCircleAll(rayOrigin, 0.001f);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<Screw>() != null)
                {
                    Screw curScrew = hit.GetComponent<Screw>();
                    if (curScrew.CanClick())
                    {
                        CurScrew = curScrew;

                        #region �Ӳ������Ƴ���˿
                        //�Ӳ������Ƴ���˿
                        if (curScrew.CanRemoveFromGlass())
                        { 
                            curScrew.RemoveFromGlass();

                            if (curScrew.ConnectedScrew != null)
                                curScrew.ConnectedScrew.RemoveFromGlass();

                            #region ȫ��Ч�����
                            CheckIceBreak();//�Ƿ��б����Խ����ƻ�

                            if (curScrew.HasDoor)//������ţ���Ҫ����ǰ��������Ϊ�ر�
                            {
                                curScrew.ScrewDoor.CloseDoor();
                                curScrew.SetDoorFlase();
                            }

                            CheckDoor();//�Ƿ�����

                            if (curScrew.HasBoom)//�����ը����Ҫ��ը������
                            {
                                curScrew.DestoryBoom();
                            }
                            CheckBoom();//�Ƿ���ը��

                            if (curScrew.IsOtherChain) //�Ƿ�������
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
                            Debug.Log(curScrew.ScrewColor +  "�޷��Ӳ������Ƴ���");
                        #endregion

                        AudioManager.Instance.PlaySFX("ScrewClick");
                        SetScrew(curScrew);//��������Ӧ�Ĳ���
                        return;
                    }
                    else
                    {
                        //Debug.Log("С���޷���� : " + curScrew.ScrewColor);
                    }
                }
            }

        }
    }
    #region ��˿
    /// <summary>
    /// ������뵱ǰ����ɫ�����
    /// </summary>
    private bool CheckBoxCrewColor(Box checkBox ,ScrewColor _color)
    {
        return checkBox.BoxColor == _color;
    }
    /// <summary>
    /// �����򵽵�ǰ�۵���Ӧλ��
    /// </summary>
    public void SetScrew(Screw screw)
    {
        //�����������ӵ�״̬
        if (isDoubleBox == false)
        {
            SetboxWihtScrew(curBox, screw);
        }
        else
        {
            //�������ӵ�״̬
            if (CheckBoxCrewColor(curBox, screw.ScrewColor))
            {
                //���һ�����ӵ���ɫ��ͬ
                SetboxWihtScrew(curBox, screw);
            }
            else if (CheckBoxCrewColor(extraBox, screw.ScrewColor))
            {
                //��ڶ������ӵ���ɫ��ͬ
                SetboxWihtScrew(extraBox, screw);
            }
            else
            {
                //������ͬ�����뵽�ղ���
                emptyHoleManager.AddToEmptyHole(screw);
            }

        }

    }
    private void SetboxWihtScrew(Box box, Screw screw)
    {
        //�����ɫ�Ƿ�һ��
        if (CheckBoxCrewColor(box, screw.ScrewColor) && box.IsMoving == false)
        {
            //��������˿
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
            //��ɫ��һ��
            emptyHoleManager.AddToEmptyHole(screw);
        }

    }
    /// <summary>
    /// ������ͨ��˿
    /// </summary>
    private void SetNormalScrew(Box box, Screw screw)
    {
        //��ͨ��˿
        //��ͨ��������
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
    /// ����������˿
    /// </summary>
    private void SetStarScrew(Box box, Screw screw)
    {
        //�������Ƕ����Ƿ�����
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

    #region ����
    /// <summary>
    /// ������ǰ�Ĳ�
    /// </summary>
    public void ChangeBox()
    {
        //����Ƿ������н���������
        if (CheckGameCompleted())
        {
            //��Ϸ���ִ�е��߼�
            WinGame();
            return;
        }
        //û�ж��������
        if (isDoubleBox == false)
        {
            ChangeSingleBox();
        }
        else
        {
            //�ж��������
            ChangeDoubleBox();
        }

    }
    private void ChangeDoubleBox()
    {
        //������������Ƿ��Ѿ���������
        if (curBox.IsCompleted && extraBox.IsCompleted)
        {
            Box preBox = curBox;//��¼��һ�����ӣ��������ӵ�״̬��Ҫ�ƶ���
            curBoxIndex++;
            curBox = boxList[curBoxIndex];

            //����ֻ��һ������
            if (curBoxIndex == boxList.Count - 1)
            {
                curBox.MoveCenter(boxMoveCenterPos);
            }
            else
            {
                Debug.Log("�ƶ���������");
                //��֮ǰ�����������ƶ����ұ�
                MoveObjectsToPositions(preBox.gameObject, extraBox.gameObject, boxMoveRightPos, boxMoveRightPos, () =>
                {
                    //�ƶ���ɺ�
                    //����������������ƶ���ָ��λ��
                    MoveObjectsToPositions(boxList[curBoxIndex + 1].gameObject, curBox.gameObject, boxMoveLeftPos, boxMoveCenterPos, () => {
                        //�ƶ�������λ�ú��Խ���˿���뵽������
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
        //�ƶ���һ�����ӵ���ߵ�λ��
        if (curBoxIndex + 1 != boxList.Count)
        {
            boxList[curBoxIndex + 1].MoveLeft(boxMoveLeftPos);
        }
    }

    /// <summary>
    /// �ⲿ���õĿղۼ������ӵĺ���
    /// </summary>
    public void EmptyScrewToBox()
    {
        emptyHoleManager.AddToBox(curBox);
    }

    /// <summary>
    /// ������Ļ���������λ
    /// </summary>
    private void CalculatePositions()
    {
        // ��ȡ�������
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // ��ȡ��Ļ���
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // ������Ļ������������� Y ����
        float yPos = screenHeight * (1 - screenFraction);

        // ��Ļ�ռ��λ
        Vector3 screenPosLeft = new Vector3(0, yPos, 0);                     // ������
        Vector3 screenPosCenter = new Vector3(screenWidth / 2, yPos, 0);    // �м��
        Vector3 screenPosRight = new Vector3(screenWidth, yPos, 0);         // ���Ҳ��
        Vector3 screenPosCenterLeft = new Vector3(screenWidth * 5/ 16, yPos, 0); // ��� 1/4
        Vector3 screenPosCenterRight = new Vector3(screenWidth * 11 / 16, yPos, 0); // �Ҳ� 3/4

        // ת��������ռ�
        boxMoveLeftPos = mainCamera.ScreenToWorldPoint(screenPosLeft);
        boxMoveCenterPos = mainCamera.ScreenToWorldPoint(screenPosCenter);
        boxMoveRightPos = mainCamera.ScreenToWorldPoint(screenPosRight) + Vector3.right * 6;
        boxCenterLeftPos = mainCamera.ScreenToWorldPoint(screenPosCenterLeft);
        boxCenterRightPos = mainCamera.ScreenToWorldPoint(screenPosCenterRight);

        // ȷ�� Z ֵΪ 0 (���� 2D ƽ��)
        boxMoveLeftPos.z = 0;
        boxMoveCenterPos.z = 0;
        boxMoveRightPos.z = 0;
        boxCenterLeftPos.z = 0;
        boxCenterRightPos.z = 0;

    }

    /// <summary>
    /// �ڳ�����ͼ�л��Ƶ����ڵ���
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(boxMoveLeftPos, 0.3f); // ������
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(boxMoveCenterPos, 0.3f); // �м��
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(boxMoveRightPos, 0.3f); // ���Ҳ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(boxCenterLeftPos, 0.3f); // ��� 1/4 ��
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(boxCenterRightPos, 0.3f); // �Ҳ� 1/4 ��
    }
    #endregion

    #region ��Ϸ���
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

        //����ֵ���㣬��ʾ����ֵ�����UI
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

                //����ÿ��
                GameDataManager.UnlockHomeButton("daily");
                UIManager.Instance.ShowUI<DailyBonusUI>();
                TimeManager.Instance.StartDailyTime();//��ʼÿ�ռ�ʱ

                break;

            case 10:
                //��10�ؽ���Streak
                if (GameDataManager.CurrentGameData.isStreaklocked == false)
                    break;

                
                GameDataManager.UnlockHomeButton("streak");
                UIManager.Instance.ShowUI<StartEvnetStreakUI>();
                HomeSceneUI.Instance.homeUI.FreshHomeButton();
                TimeManager.Instance.StartStreakTime();//��ʼ���Ƽ�ʱ
                break;

            case 20:
                //�����齱
                if (GameDataManager.CurrentGameData.isLuckySpinlocked == false)
                    break;

                GameDataManager.UnlockHomeButton("luckyspin");
                HomeSceneUI.Instance.homeUI.FreshHomeButton();
                TimeManager.Instance.StartLuckySpinTime();
                break;
            case 30:
                //�����ɻ�
                break;

                //if (GameDataManager.CurrentGameData.isSkyRacelocked == false)
                //    break;

                //GameDataManager.UnlockHomeButton("sky");
                //HomeSceneUI.Instance.homeUI.FreshHomeButton();
                //break;
        }

        //����homeUI
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
    /// �����Ϸ�Ƿ�����ĺ���
    /// </summary>
    private bool CheckGameCompleted()
    {
        return curBoxIndex == boxList.Count - 1;
    }

    /// <summary>
    /// ��ʼ���ؿ�(��¼�ؿ�����)
    /// </summary>
    public void InitBoxLevel()
    {
        //��ղ���
        EnterMainScene();

        boxList.Clear();
        boxLevel = null;
        extraBox = null;
        isDoubleBox = false;
        curBoxIndex = 0;

        //��ʼ��
        boxLevel = transform.Find("BoxLevel").transform;
        boxLevel.transform.position = new Vector3(0, 0, 0);
        for (int i = 0; i < boxLevel.childCount; i++)
        {
            if (boxLevel.GetChild(i).GetComponent<Box>() != null)
                boxList.Add(boxLevel.GetChild(i).GetComponent<Box>());
        }
        curBox = boxList[0];

        //����box����Ӧ��λ��
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
    /// �˳���Ϸ����
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
    /// ��Ϸʤ�����߼�
    /// </summary>
    private void WinGame()
    {
        //Debug.Log("��Ϸʤ��");
        gameState = GameState.Stop;
        UIManager.Instance.ShowUI<WinUI>();
    }
    /// <summary>
    /// ��Ϸʧ��(�ղ�����)
    /// </summary>
    public void loseGame()
    {
        if (emptyHoleManager.CheckGameFailed() && gameState == GameState.Start)
        {
            gameState = GameState.Stop;

            //����Ƿ񻹿��Լ������Ķ���
            if (emptyHoleManager.CanAddExtraHole())
            {
                UIManager.Instance.ShowUI<OutOfSpaceUI>();
            }
            else
            {
                //���û�ж���Ķ��ڿ�������ˣ����޷�������һ��
                //�����ǰ��ʤ�����ҽ�������ʤ
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

    #region ����

    /// <summary>
    /// ����һ�ĵ��ú��������ö���Ķ��ڣ�
    /// </summary>
    public void AddExtraHole()
    {
        emptyHoleManager.ActivateExtraHole();
    }

    /// <summary>
    /// ���������ƶ�����
    /// </summary>
    public void MoveObjectsToPositions(GameObject moveLeftObj, GameObject moveRightObj, Vector3 leftPos, Vector3 rightPos, System.Action callBack = null, float time1 = 0.3f, float time2 = 0.2f)
    {
        // ͬʱ�������� DOTween ����
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(moveLeftObj.transform.DOMove(leftPos, time1).SetEase(Ease.InOutSine));
        moveSequence.Join(moveRightObj.transform.DOMove(rightPos, time2).SetEase(Ease.InOutSine));
        //������ɺ�Ļص�
        moveSequence.OnComplete(() =>
        {
            callBack?.Invoke();
        });
    }
    /// <summary>
    /// ���ö��������
    /// </summary>
    public void ActiveExtraBox()
    {
        if (curBoxIndex + 1 < boxList.Count)
        {
            //��֮ǰ����������Ϊ����ĺ���
            extraBox = curBox;
            //����һ��������Ϊ��ǰ����
            curBox = boxList[++curBoxIndex];

            isDoubleBox = true;
            MoveObjectsToPositions(curBox.gameObject, extraBox.gameObject, boxCenterLeftPos, boxCenterRightPos, () => {
                //���Խ���˿���뵽��һ��������
                emptyHoleManager.AddToBox(curBox);
            });

        }
        else
        {
            Debug.Log("�Լ�û�ж����������");
        }
    }

    public bool CanAddExtraBox()
    {
        if (isDoubleBox)
        {
            Debug.Log("�Ѿ��ж����������");
            return false;
        }
        else if (curBox.IsMoving)
        {
            Debug.Log("��ǰ���������ƶ�");
            return false;
        }

        return true;
    }
    #endregion

    #region ȫ��Ч��
    private void CheckIceBreak()
    {
        Level curLevel = LevelManager.Instance.CurLevel;
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
                            //������˿
                            List<Screw> screwList = curGlass.ScrewList;
                            foreach (Screw curScrew in screwList)
                            {
                                //�鿴��˿�Ƿ��б������ҿ��Խ��е��
                                if (curScrew.IsIceCovered && curScrew.HasGlassCovered() == false)
                                {
                                    //�����Ƿ��Ѿ��ƻ���ȫ
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
                                //�鿴�Ƿ����� && ����û�б���������
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
                                //��ը��������û�б�����
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
