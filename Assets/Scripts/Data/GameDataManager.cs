using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 游戏数据管理器，负责管理游戏数据的加载、保存和操作
/// </summary>
public static class GameDataManager
{
    public static GameData CurrentGameData { get; private set; }

    /// <summary>
    /// 初始化游戏数据管理器
    /// </summary>
    public static void Initialize()
    {
        TimeDataManager.Initialize();//初始化时间数据
        CurrentGameData = JsonDataManager.LoadGameData();
        if (CurrentGameData == null)
        {
            Reset(); // 如果未找到数据文件，初始化为默认数据
        }
    }

    /// <summary>
    /// 保存当前游戏数据
    /// </summary>
    public static void Save()
    {
        JsonDataManager.SaveGameData(CurrentGameData);
    }

    /// <summary>
    /// 重置游戏数据为默认值并保存
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
            preStreakIndex = 0, // 之前点击的状态
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

        // 初始化完数据后，将TimeDataList转换为字典
        Save();
    }

    #region 时间管理


    /// <summary>
    /// 设置条纹解锁状态
    /// </summary>
    /// <param name="val"></param>
    public static void SetStreakLockedVal(bool val)
    {
        CurrentGameData.isStreaklocked = val;

        if (val == true)
        {
            CurrentGameData.curStreakIndex = 0;
            CurrentGameData.curStreakChestIndex = 0;
            CurrentGameData.preStreakIndex = 0; // 之前点击的状态
            CurrentGameData.unlockStreakChestCount = 1;
        }
        Save();
    }
    /// <summary>
    /// 设置抽奖解锁状态
    /// </summary>
    /// <param name="val"></param>
    public static void SetLuckySpinLockedVal(bool val)
    {
        CurrentGameData.isLuckySpinlocked = val;
        Save();
    }
    #endregion

    #region HomeUI相关
    /// <summary>
    /// 增加金币数量
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
        //金币数量不足，弹出警告
        if (CurrentGameData.coinCount - count < 0)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("THERE AREN'T ENOUGH GOLD!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("没有足够的金币了！");
            }
            return false;
        }

        CurrentGameData.coinCount -= count;
        Save();
        return true;
    }
    public static void AddHeartCount(int count)
    {
        //如果处于无限生命，不消耗生命
        if (count < 0 && HomeSceneUI.Instance.homeUI.IsWireLessHeart)
            return;

        CurrentGameData.heartCount += count;

        if (CurrentGameData.heartCount >= 5)
        { 
            //生命已经满了，停止倒计时
            CurrentGameData.heartCount = 5;
            TimedObject timeObj = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
            timeObj.remainingTime = timeObj.startTime.ToTimeSpan();
            timeObj.currentState = ActivityState.Stop;
        }
        else
        {
            TimedObject timeObj = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
            timeObj.currentState = ActivityState.Ongoing;

            //第一次进入heart为4的时候，需要重置生命的倒计时时间
            if (CurrentGameData.heartCount == 4 && count < 0)
            {
                timeObj.remainingTime = timeObj.startTime.ToTimeSpan();
            }

        }

        //生命值为0，返回主界面
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
    /// 解锁按钮
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

    #region 游戏场景相关
    /// <summary>
    /// 增加解锁特定道具的填充数
    /// </summary>
    public static void AddCurFillCount()
    {
        CurrentGameData.curFillCount++;
        Save();
    }

    /// <summary>
    /// 重新开始计时
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
    /// 解锁指定道具
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
                Debug.LogWarning($"[GameDataManager] 未知的道具名称");
                return;
        }

        Save();
    }

    /// <summary>
    /// 增加指定道具的数量
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
                Debug.LogWarning($"[GameDataManager] 未知的道具名称");
                return;
        }

        Save();
    }

    /// <summary>
    /// 增加关卡数
    /// </summary>
    public static void AddLevelNum()
    {
        CurrentGameData.levelNum++;
        CurrentGameData.starCount++;
        Save();
    }

    /// <summary>
    /// 解锁关卡特定物品
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
            Debug.Log("名字输入不正确, 未能解锁特定物品");
        }
        Save();
    }
    #endregion


    #region 每日
    /// <summary>
    /// 增加每日奖励的索引
    /// </summary>
    public static void AddDailyRewardIndex()
    {
        CurrentGameData.curDailyIndex++;
        Save();
    }

    /// <summary>
    /// 重置每日奖励
    /// </summary>
    public static void ResertRewardIndex()
    {
        CurrentGameData.curDailyIndex = 0;
        Save();
    }
    #endregion

    /// <summary>
    /// 根据精灵名字增加物品
    /// </summary>
    /// <param name="name"></param>
    public static void AddItemCountBySpriteName(string name, int count)
    {
        // 将名字转为小写
        string lowerCaseName = name.ToLower();

        // 检查名字中的关键字并执行操作
        if (lowerCaseName.Contains("coin"))
        {
            CurrentGameData.coinCount += count;
            Debug.Log("增加金币");
        }
        else if (lowerCaseName.Contains("heart"))
        {
            //增加无限生命的时间
            TimeManager.Instance.AddWirelessHeartTime(0, count, 0);
            Debug.Log("增加无限生命的时间");
        }
        else if (lowerCaseName.Contains("hole"))
        {
            CurrentGameData.holeItemCount += count;
            Debug.Log("增加洞口道具");
        }
        else if (lowerCaseName.Contains("rocket"))
        {
            CurrentGameData.rocketItemCount += count;
            Debug.Log("增加火箭道具");
        }
        else if (lowerCaseName.Contains("box"))
        {
            CurrentGameData.doubleBoxItemCount += count;
            Debug.Log("增加盒子道具");
        }
        else
        {
            Debug.Log("名字不正确：" + name);
        }
        Save();
    }

    #region 条纹
    /// <summary>
    /// 增加streak
    /// </summary>
    public static void AddStreak()
    {
        CurrentGameData.curStreakIndex++;
        Save();
    }
    /// <summary>
    /// 增加当前Streak宝箱的索引
    /// </summary>
    public static void AddStreakChestIndex()
    {
        CurrentGameData.curStreakChestIndex++;
        CurrentGameData.unlockStreakChestCount++;
        Save();
    }

    /// <summary>
    /// 更新点击的索引
    /// </summary>
    /// <param name="count"></param>
    public static void ChangePreStreakIndex()
    {
        CurrentGameData.preStreakIndex = CurrentGameData.curStreakIndex;
        Save();
    }

    /// <summary>
    /// 重新计数Streak
    /// </summary>
    public static void ResetStreak()
    {
        CurrentGameData.preStreakIndex = 0;
        CurrentGameData.curStreakIndex = 0;
        Save();
    }

    /// <summary>
    /// 重新开始连胜活动
    /// </summary>
    public static void ReStartStreakEvent()
    {
        CurrentGameData.curStreakIndex = 0;
        CurrentGameData.curStreakChestIndex = 0;
        CurrentGameData.preStreakIndex = 0; // 之前点击的状态
        CurrentGameData.unlockStreakChestCount = 1;
        Save();
    }
    /// <summary>
    /// 时间管理调用的，重新开始活动
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

    #region 抽奖
    public static void AddSpinProgress()
    {
        CurrentGameData.curSpinProgress++;

        //如果进度大于10则增加抽奖票
        if (CurrentGameData.curSpinProgress >= 10)
        {
            CurrentGameData.curSpinCount++;
            CurrentGameData.curSpinProgress = 0;
        }
        Save();
    }

    /// <summary>
    /// 减少抽奖票数
    /// </summary>
    public static void DecreaseSpinCount()
    {
        CurrentGameData.curSpinCount--;
        Save();
    }

    /// <summary>
    /// 重新开始转盘活动
    /// </summary>
    public static void RestartLuckySpinEvent()
    {
        CurrentGameData.curSpinProgress = 0;
        Save();
    }
    #endregion

    #region 贴纸
    /// <summary>
    /// 增加贴纸索引
    /// </summary>
    public static void AddStickerIndex()
    {
        CurrentGameData.curStickerIndex++;
        Save();
    }

    /// <summary>
    /// 解锁对应的贴纸
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
    /// 得到当前解锁的贴纸列表
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

        Debug.Log("没有得到列表");
        return null;
    }

    /// <summary>
    /// 更新当前按钮的索引
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

    //重置按钮索引
    public static void ResetStickerButton()
    {
        CurrentGameData.curButtonIndex = 0;
        CurrentGameData.curB1Index = 0;
        CurrentGameData.curB2Index = 0;
        CurrentGameData.curB3Index = 0;

        Save();
    }
    /// <summary>
    /// 设置是否为新贴纸按钮
    /// </summary>
    /// <param name="val"></param>
    public static void SetNewStickerButton(bool val)
    {
        CurrentGameData.isNewSticker = val;
        Save();
    }

    /// <summary>
    /// 当前贴纸按钮是否打开
    /// </summary>
    /// <param name="val"></param>
    public static void SetStikcerChestOpen(bool val)
    {
        CurrentGameData.isStikcerChestOpen = val;
        Save();
    }

    /// <summary>
    /// 增加新的收集贴纸
    /// </summary>
    public static void AddUnlockStikcerList()
    {
        CurrentGameData.unlockStickerCount++;
        Save();
    }

    /// <summary>
    /// 增加收集完毕的贴纸
    /// </summary>
    public static void AddCompleteStickerList()
    {
        CurrentGameData.completeStikcerCount++;
        Save();
    }
    #endregion
}


