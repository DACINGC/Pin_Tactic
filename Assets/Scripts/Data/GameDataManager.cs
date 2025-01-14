using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��Ϸ���ݹ����������������Ϸ���ݵļ��ء�����Ͳ���
/// </summary>
public static class GameDataManager
{
    public static GameData CurrentGameData { get; private set; }

    /// <summary>
    /// ��ʼ����Ϸ���ݹ�����
    /// </summary>
    public static void Initialize()
    {
        TimeDataManager.Initialize();//��ʼ��ʱ������
        CurrentGameData = JsonDataManager.LoadGameData();
        if (CurrentGameData == null)
        {
            Reset(); // ���δ�ҵ������ļ�����ʼ��ΪĬ������
        }
    }

    /// <summary>
    /// ���浱ǰ��Ϸ����
    /// </summary>
    public static void Save()
    {
        JsonDataManager.SaveGameData(CurrentGameData);
    }

    /// <summary>
    /// ������Ϸ����ΪĬ��ֵ������
    /// </summary>
    public static void Reset()
    {
        CurrentGameData = new GameData
        {
            levelNum = 1,
            starCount = 0,
            coinCount = 0,
            piggyCount = 0,
            heartCount = 5,

            curFillCount = 0,
            allfillCount = 10,
            fillSpriteName = "",

            holeItemCount = 0,
            rocketItemCount = 0,
            doubleBoxItemCount = 0,

            isHoleLocked = true,
            isRocketLocked = true,
            isDoubleBoxLocked = true,

            isDailyRewardLocked = true,
            isStreaklocked = true,
            isSkyRacelocked = true,
            isLuckySpinlocked = true,

            isStarStrewLocked = true,
            isRopeLocked = true,
            isIceLocked = true,
            isDoorLocked = true,
            isBoomLocked = true,
            ischainLocked = true,
            isKeyLocked = true,
            isClockLocked = true,

            curDailyIndex = 0,

            curStreakIndex = 0,
            curStreakChestIndex = 0,
            preStreakIndex = 0, // ֮ǰ�����״̬
            unlockStreakChestCount = 1,

            curSpinCount = 0,
            curSpinProgress = 0,

            isNewSticker = false,
            isStikcerChestOpen = false,
            curStickerIndex = 0,
            curButtonIndex = 0,
            curB1Index = 0,
            curB2Index = 0,
            curB3Index = 0,

            sticker1UnlockList = new List<string>(),
            sticker2UnlockList = new List<string>(),
            sticker3UnlockList = new List<string>(),
            sticker4UnlockList = new List<string>(),
            sticker5UnlockList = new List<string>(),
            sticker6UnlockList = new List<string>(),
            sticker7UnlockList = new List<string>(),

            unlockStickerCount = 1,
            completeStikcerCount = 0,
    };
        TimeManager.Instance.AddDefaultTime();

        // ��ʼ�������ݺ󣬽�TimeDataListת��Ϊ�ֵ�
        Save();
    }

    #region ʱ�����


    /// <summary>
    /// �������ƽ���״̬
    /// </summary>
    /// <param name="val"></param>
    public static void SetStreakLockedVal(bool val)
    {
        CurrentGameData.isStreaklocked = val;

        if (val == true)
        {
            CurrentGameData.curStreakIndex = 0;
            CurrentGameData.curStreakChestIndex = 0;
            CurrentGameData.preStreakIndex = 0; // ֮ǰ�����״̬
            CurrentGameData.unlockStreakChestCount = 1;
        }
        Save();
    }
    /// <summary>
    /// ���ó齱����״̬
    /// </summary>
    /// <param name="val"></param>
    public static void SetLuckySpinLockedVal(bool val)
    {
        CurrentGameData.isLuckySpinlocked = val;
        Save();
    }
    #endregion

    #region HomeUI���
    /// <summary>
    /// ���ӽ������
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="piggy"></param>
    public static void AddCoinCount(int coin = 0, int piggy = 0)
    {
        CurrentGameData.coinCount += coin;
        CurrentGameData.piggyCount += piggy;

        if (CurrentGameData.piggyCount >= 750)
        {
            CurrentGameData.piggyCount = 750;
        }

        Save();
    }
    public static bool DecreaseCoinCount(int count)
    {
        //����������㣬��������
        if (CurrentGameData.coinCount - count < 0)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("THERE AREN'T ENOUGH GOLD!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("û���㹻�Ľ���ˣ�");
            }
            return false;
        }

        CurrentGameData.coinCount -= count;
        Save();
        return true;
    }
    public static void AddHeartCount(int count)
    {
        //���������������������������
        if (count < 0 && HomeSceneUI.Instance.homeUI.IsWireLessHeart)
            return;

        CurrentGameData.heartCount += count;

        if (CurrentGameData.heartCount >= 5)
        { 
            //�����Ѿ����ˣ�ֹͣ����ʱ
            CurrentGameData.heartCount = 5;
            TimedObject timeObj = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
            timeObj.remainingTime = timeObj.startTime.ToTimeSpan();
            timeObj.currentState = ActivityState.Stop;
        }
        else
        {
            TimedObject timeObj = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
            timeObj.currentState = ActivityState.Ongoing;

            //��һ�ν���heartΪ4��ʱ����Ҫ���������ĵ���ʱʱ��
            if (CurrentGameData.heartCount == 4 && count < 0)
            {
                timeObj.remainingTime = timeObj.startTime.ToTimeSpan();
            }

        }

        //����ֵΪ0������������
        if (CurrentGameData.heartCount <= 0)
        {
            GameManager.Instance.EnterHomeScene();
            CurrentGameData.heartCount = 0;
        }

        Save();
    }
    public static void DecreaseStarCount(int count)
    {
        CurrentGameData.starCount -= count;
        Save();
    }

    /// <summary>
    /// ������ť
    /// </summary>
    /// <param name="name">1.streak 2.sky 3.luckyspin, 4.daily</param>
    public static void UnlockHomeButton(string name)
    {
        if (name == "streak")
            CurrentGameData.isStreaklocked = false;
        else if (name == "sky")
            CurrentGameData.isSkyRacelocked = false;
        else if (name == "luckyspin")
            CurrentGameData.isLuckySpinlocked = false;
        else if (name == "daily")
            CurrentGameData.isDailyRewardLocked = false;

        Save();
    }

    public static void AddPiggyCount()
    {
        CurrentGameData.coinCount += 750;
        CurrentGameData.piggyCount = 0;
        Save();
    }
    #endregion

    #region ��Ϸ�������
    /// <summary>
    /// ���ӽ����ض����ߵ������
    /// </summary>
    public static void AddCurFillCount()
    {
        CurrentGameData.curFillCount++;
        Save();
    }

    /// <summary>
    /// ���¿�ʼ��ʱ
    /// </summary>
    /// <param name="count"></param>
    /// <param name="spriteName"></param>
    public static void ReStartFill(int count, string spriteName)
    {
        CurrentGameData.curFillCount = 0;
        CurrentGameData.allfillCount = count;
        CurrentGameData.fillSpriteName = spriteName;
        Save();
    }
    /// <summary>
    /// ����ָ������
    /// </summary>
    public static void UnlockItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Hole:
                CurrentGameData.isHoleLocked = false;
                break;
            case ItemType.Rocket:
                CurrentGameData.isRocketLocked = false;
                break;
            case ItemType.DoubleBox:
                CurrentGameData.isDoubleBoxLocked = false;
                break;
            default:
                Debug.LogWarning($"[GameDataManager] δ֪�ĵ�������");
                return;
        }

        Save();
    }

    /// <summary>
    /// ����ָ�����ߵ�����
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="count"></param>
    public static void AddItemCount(ItemType itemType, int count)
    {
        switch (itemType)
        {
            case ItemType.Hole:
                CurrentGameData.holeItemCount += count;
                break;
            case ItemType.Rocket:
                CurrentGameData.rocketItemCount += count;
                break;
            case ItemType.DoubleBox:
                CurrentGameData.doubleBoxItemCount += count;
                break;
            default:
                Debug.LogWarning($"[GameDataManager] δ֪�ĵ�������");
                return;
        }

        Save();
    }

    /// <summary>
    /// ���ӹؿ���
    /// </summary>
    public static void AddLevelNum()
    {
        CurrentGameData.levelNum++;
        CurrentGameData.starCount++;
        Save();
    }

    /// <summary>
    /// �����ؿ��ض���Ʒ
    /// </summary>
    /// <param name="name">1.star  </param>
    public static void UnlockGameSpecialItem(string name)
    {
        if (name == "star")
        {
            CurrentGameData.isStarStrewLocked = false;
        }
        else if (name == "rope")
        {
            CurrentGameData.isRopeLocked = false;
        }
        else if (name == "ice")
        {
            CurrentGameData.isIceLocked = false;
        }
        else if (name == "door")
        {
            CurrentGameData.isDoorLocked = false;
        }
        else if (name == "boom")
        {
            CurrentGameData.isBoomLocked = false;
        }
        else if (name == "key")
        {
            CurrentGameData.isKeyLocked = false;
        }
        else if (name == "chain")
        {
            CurrentGameData.ischainLocked = false;
        }
        else if (name == "clock")
        {
            CurrentGameData.isClockLocked = false;
        }
        else
        {
            Debug.Log("�������벻��ȷ, δ�ܽ����ض���Ʒ");
        }
        Save();
    }
    #endregion


    #region ÿ��
    /// <summary>
    /// ����ÿ�ս���������
    /// </summary>
    public static void AddDailyRewardIndex()
    {
        CurrentGameData.curDailyIndex++;
        Save();
    }

    /// <summary>
    /// ����ÿ�ս���
    /// </summary>
    public static void ResertRewardIndex()
    {
        CurrentGameData.curDailyIndex = 0;
        Save();
    }
    #endregion

    /// <summary>
    /// ���ݾ�������������Ʒ
    /// </summary>
    /// <param name="name"></param>
    public static void AddItemCountBySpriteName(string name, int count)
    {
        // ������תΪСд
        string lowerCaseName = name.ToLower();

        // ��������еĹؼ��ֲ�ִ�в���
        if (lowerCaseName.Contains("coin"))
        {
            CurrentGameData.coinCount += count;
            Debug.Log("���ӽ��");
        }
        else if (lowerCaseName.Contains("heart"))
        {
            //��������������ʱ��
            TimeManager.Instance.AddWirelessHeartTime(0, count, 0);
            Debug.Log("��������������ʱ��");
        }
        else if (lowerCaseName.Contains("hole"))
        {
            CurrentGameData.holeItemCount += count;
            Debug.Log("���Ӷ��ڵ���");
        }
        else if (lowerCaseName.Contains("rocket"))
        {
            CurrentGameData.rocketItemCount += count;
            Debug.Log("���ӻ������");
        }
        else if (lowerCaseName.Contains("box"))
        {
            CurrentGameData.doubleBoxItemCount += count;
            Debug.Log("���Ӻ��ӵ���");
        }
        else
        {
            Debug.Log("���ֲ���ȷ��" + name);
        }
        Save();
    }

    #region ����
    /// <summary>
    /// ����streak
    /// </summary>
    public static void AddStreak()
    {
        CurrentGameData.curStreakIndex++;
        Save();
    }
    /// <summary>
    /// ���ӵ�ǰStreak���������
    /// </summary>
    public static void AddStreakChestIndex()
    {
        CurrentGameData.curStreakChestIndex++;
        CurrentGameData.unlockStreakChestCount++;
        Save();
    }

    /// <summary>
    /// ���µ��������
    /// </summary>
    /// <param name="count"></param>
    public static void ChangePreStreakIndex()
    {
        CurrentGameData.preStreakIndex = CurrentGameData.curStreakIndex;
        Save();
    }

    /// <summary>
    /// ���¼���Streak
    /// </summary>
    public static void ResetStreak()
    {
        CurrentGameData.preStreakIndex = 0;
        CurrentGameData.curStreakIndex = 0;
        Save();
    }

    /// <summary>
    /// ���¿�ʼ��ʤ�
    /// </summary>
    public static void ReStartStreakEvent()
    {
        CurrentGameData.curStreakIndex = 0;
        CurrentGameData.curStreakChestIndex = 0;
        CurrentGameData.preStreakIndex = 0; // ֮ǰ�����״̬
        CurrentGameData.unlockStreakChestCount = 1;
        Save();
    }
    /// <summary>
    /// ʱ�������õģ����¿�ʼ�
    /// </summary>
    public static void TimeRestartSteak()
    {
        CurrentGameData.curStreakIndex = 0;
        CurrentGameData.curStreakChestIndex = 0;
        CurrentGameData.preStreakIndex = 0; 
        CurrentGameData.unlockStreakChestCount = 1;
        Save();
    }
    #endregion

    #region �齱
    public static void AddSpinProgress()
    {
        CurrentGameData.curSpinProgress++;

        //������ȴ���10�����ӳ齱Ʊ
        if (CurrentGameData.curSpinProgress >= 10)
        {
            CurrentGameData.curSpinCount++;
            CurrentGameData.curSpinProgress = 0;
        }
        Save();
    }

    /// <summary>
    /// ���ٳ齱Ʊ��
    /// </summary>
    public static void DecreaseSpinCount()
    {
        CurrentGameData.curSpinCount--;
        Save();
    }

    /// <summary>
    /// ���¿�ʼת�̻
    /// </summary>
    public static void RestartLuckySpinEvent()
    {
        CurrentGameData.curSpinProgress = 0;
        Save();
    }
    #endregion

    #region ��ֽ
    /// <summary>
    /// ������ֽ����
    /// </summary>
    public static void AddStickerIndex()
    {
        CurrentGameData.curStickerIndex++;
        Save();
    }

    /// <summary>
    /// ������Ӧ����ֽ
    /// </summary>
    /// <param name="name"></param>
    public static void UnlockSticker(string name)
    {
        switch (CurrentGameData.curStickerIndex)
        {
            case 0:
                CurrentGameData.sticker1UnlockList.Add(name);
                break;
            case 1:
                CurrentGameData.sticker2UnlockList.Add(name);
                break;
            case 2:
                CurrentGameData.sticker3UnlockList.Add(name);
                break;
            case 3:
                CurrentGameData.sticker4UnlockList.Add(name);
                break;
            case 4:
                CurrentGameData.sticker5UnlockList.Add(name);
                break;
            case 5:
                CurrentGameData.sticker6UnlockList.Add(name);
                break;
            case 6:
                CurrentGameData.sticker7UnlockList.Add(name);
                break;
        }

    }

    /// <summary>
    /// �õ���ǰ��������ֽ�б�
    /// </summary>
    /// <returns></returns>
    public static List<string> GetUnlockStickerList()
    {
        switch (CurrentGameData.curStickerIndex)
        {
            case 0:
                return CurrentGameData.sticker1UnlockList;
            case 1:
                return CurrentGameData.sticker2UnlockList;
            case 2:
                return CurrentGameData.sticker3UnlockList;
            case 3:
                return CurrentGameData.sticker4UnlockList;
            case 4:
                return CurrentGameData.sticker5UnlockList;
            case 5:
                return CurrentGameData.sticker6UnlockList;
            case 6:
                return CurrentGameData.sticker7UnlockList;
        }

        Debug.Log("û�еõ��б�");
        return null;
    }

    /// <summary>
    /// ���µ�ǰ��ť������
    /// </summary>
    /// <param name="val"></param>
    public static void ChangeStickerButtonIndex()
    {
        CurrentGameData.curButtonIndex = CurrentGameData.curB1Index;
        Save();
    }
    public static void AddB1Index()
    {
        CurrentGameData.curB1Index++;
        Save();
    }
    public static void AddB2Index()
    {
        CurrentGameData.curB2Index++;
        Save();
    }
    public static void AddB3Index()
    {
        CurrentGameData.curB3Index++;
        Save();
    }

    //���ð�ť����
    public static void ResetStickerButton()
    {
        CurrentGameData.curButtonIndex = 0;
        CurrentGameData.curB1Index = 0;
        CurrentGameData.curB2Index = 0;
        CurrentGameData.curB3Index = 0;

        Save();
    }
    /// <summary>
    /// �����Ƿ�Ϊ����ֽ��ť
    /// </summary>
    /// <param name="val"></param>
    public static void SetNewStickerButton(bool val)
    {
        CurrentGameData.isNewSticker = val;
        Save();
    }

    /// <summary>
    /// ��ǰ��ֽ��ť�Ƿ��
    /// </summary>
    /// <param name="val"></param>
    public static void SetStikcerChestOpen(bool val)
    {
        CurrentGameData.isStikcerChestOpen = val;
        Save();
    }

    /// <summary>
    /// �����µ��ռ���ֽ
    /// </summary>
    public static void AddUnlockStikcerList()
    {
        CurrentGameData.unlockStickerCount++;
        Save();
    }

    /// <summary>
    /// �����ռ���ϵ���ֽ
    /// </summary>
    public static void AddCompleteStickerList()
    {
        CurrentGameData.completeStikcerCount++;
        Save();
    }
    #endregion
}


