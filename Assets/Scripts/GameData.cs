using System;
using System.Collections.Generic;


[Serializable]
public class TimedObjectData
{
    public string objectID;  // 唯一标识符
    public SerializableTime startTime;  // 活动开始时间
    public SerializableTime endTime;  // 活动结束时间
    public SerializableTime leftTime;  // 剩余时间（使用SerializableTime保存）

    // 活动状态标识
    public ActivityState activityStatus;  // 活动状态
    public TimeEventType timeEventType;

    // 构造函数
    public TimedObjectData(string objectID, SerializableTime startTime, SerializableTime endTime, ActivityState status, SerializableTime leftTime, TimeEventType type)
    {
        this.objectID = objectID;
        this.startTime = startTime;
        this.endTime = endTime;
        this.activityStatus = status;
        this.leftTime = leftTime; //?? new SerializableTime(TimeSpan.Zero);  // 如果leftTime为空，则设置为0秒
        this.timeEventType = type;
    }

    // 获取当前倒计时的字符串表示
    public string GetCountdownString()
    {
        return leftTime.ToTimeSpan().ToString(@"dd\天 HH\时 mm\分 ss\秒");
    }
}



[Serializable]
public class GameData
{
    // 游戏关卡数和资源
    public int levelNum;
    public int starCount;
    public int coinCount;
    public int piggyCount;
    public int heartCount;

    //关卡物品解锁进度
    public int curFillCount;
    public int allfillCount;
    public string fillSpriteName;

    // 道具数量
    public int holeItemCount;
    public int rocketItemCount;
    public int doubleBoxItemCount;

    // 道具解锁状态
    public bool isHoleLocked;
    public bool isRocketLocked;
    public bool isDoubleBoxLocked;

    //主界面解锁的按钮
    public bool isDailyRewardLocked;
    public bool isStreaklocked;
    public bool isSkyRacelocked;
    public bool isLuckySpinlocked;

    //关卡解锁的特定物品
    public bool isStarStrewLocked;
    public bool isRopeLocked;
    public bool isIceLocked;
    public bool isDoorLocked;
    public bool isBoomLocked;
    public bool ischainLocked;
    public bool isKeyLocked;
    public bool isClockLocked;

    //每日奖励
    public int curDailyIndex;

    //热气球
    public int curStreakChestIndex;//当前解锁到的宝箱索引
    public int curStreakIndex;//当前的连胜索引
    public int preStreakIndex;//之前点击的索引
    public int unlockStreakChestCount;//当前解锁的宝箱数量

    //抽奖
    public int curSpinCount;
    public int curSpinProgress;

    //贴纸
    public bool isNewSticker; // 是否是新贴纸
    public bool isStikcerChestOpen;
    public int curStickerIndex; // 当前处于第几个贴纸
    public int curButtonIndex; // 当前处于第几批按钮
    public int curB1Index; // 第一个按钮的索引
    public int curB2Index;
    public int curB3Index;

    public List<string> sticker1UnlockList;
    public List<string> sticker2UnlockList;
    public List<string> sticker3UnlockList;
    public List<string> sticker4UnlockList;
    public List<string> sticker5UnlockList;
    public List<string> sticker6UnlockList;
    public List<string> sticker7UnlockList;

    //收集
    public int unlockStickerCount;
    public int completeStikcerCount;


}
//时间数据
[Serializable]
public class TimeData
{
    //时间
    public List<TimedObjectData> timedObjects = new List<TimedObjectData>();
    public SerializableTime lastLoginTime;  // 上次登录时间
}