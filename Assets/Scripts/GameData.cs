using System;
using System.Collections.Generic;


[Serializable]
public class TimedObjectData
{
    public string objectID;  // Ψһ��ʶ��
    public SerializableTime startTime;  // ���ʼʱ��
    public SerializableTime endTime;  // �����ʱ��
    public SerializableTime leftTime;  // ʣ��ʱ�䣨ʹ��SerializableTime���棩

    // �״̬��ʶ
    public ActivityState activityStatus;  // �״̬
    public TimeEventType timeEventType;

    // ���캯��
    public TimedObjectData(string objectID, SerializableTime startTime, SerializableTime endTime, ActivityState status, SerializableTime leftTime, TimeEventType type)
    {
        this.objectID = objectID;
        this.startTime = startTime;
        this.endTime = endTime;
        this.activityStatus = status;
        this.leftTime = leftTime; //?? new SerializableTime(TimeSpan.Zero);  // ���leftTimeΪ�գ�������Ϊ0��
        this.timeEventType = type;
    }

    // ��ȡ��ǰ����ʱ���ַ�����ʾ
    public string GetCountdownString()
    {
        return leftTime.ToTimeSpan().ToString(@"dd\�� HH\ʱ mm\�� ss\��");
    }
}



[Serializable]
public class GameData
{
    // ��Ϸ�ؿ�������Դ
    public int levelNum;
    public int starCount;
    public int coinCount;
    public int piggyCount;
    public int heartCount;

    //�ؿ���Ʒ��������
    public int curFillCount;
    public int allfillCount;
    public string fillSpriteName;

    // ��������
    public int holeItemCount;
    public int rocketItemCount;
    public int doubleBoxItemCount;

    // ���߽���״̬
    public bool isHoleLocked;
    public bool isRocketLocked;
    public bool isDoubleBoxLocked;

    //����������İ�ť
    public bool isDailyRewardLocked;
    public bool isStreaklocked;
    public bool isSkyRacelocked;
    public bool isLuckySpinlocked;

    //�ؿ��������ض���Ʒ
    public bool isStarStrewLocked;
    public bool isRopeLocked;
    public bool isIceLocked;
    public bool isDoorLocked;
    public bool isBoomLocked;
    public bool ischainLocked;
    public bool isKeyLocked;
    public bool isClockLocked;

    //ÿ�ս���
    public int curDailyIndex;

    //������
    public int curStreakChestIndex;//��ǰ�������ı�������
    public int curStreakIndex;//��ǰ����ʤ����
    public int preStreakIndex;//֮ǰ���������
    public int unlockStreakChestCount;//��ǰ�����ı�������

    //�齱
    public int curSpinCount;
    public int curSpinProgress;

    //��ֽ
    public bool isNewSticker; // �Ƿ�������ֽ
    public bool isStikcerChestOpen;
    public int curStickerIndex; // ��ǰ���ڵڼ�����ֽ
    public int curButtonIndex; // ��ǰ���ڵڼ�����ť
    public int curB1Index; // ��һ����ť������
    public int curB2Index;
    public int curB3Index;

    public List<string> sticker1UnlockList;
    public List<string> sticker2UnlockList;
    public List<string> sticker3UnlockList;
    public List<string> sticker4UnlockList;
    public List<string> sticker5UnlockList;
    public List<string> sticker6UnlockList;
    public List<string> sticker7UnlockList;

    //�ռ�
    public int unlockStickerCount;
    public int completeStikcerCount;


}
//ʱ������
[Serializable]
public class TimeData
{
    //ʱ��
    public List<TimedObjectData> timedObjects = new List<TimedObjectData>();
    public SerializableTime lastLoginTime;  // �ϴε�¼ʱ��
}