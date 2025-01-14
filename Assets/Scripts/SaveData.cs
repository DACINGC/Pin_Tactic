public class SaveData
{
    // ��̬ʵ��
    private static SaveData _instance;

    // ����������
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
    }

    // ˽�й��캯������ֹ�ⲿʵ����
    private SaveData() 
    {
        HoleLocked = true;
        RocketLocked = true;
        DoubleBoxLocked = true;
    }

    // ����
    public int PiggyCount { get; private set; }
    public int CoinCount { get; private set; }
    public int HoleItem { get; private set; }
    public int StarCount { get; private set; }

    public int RocketItem { get; private set; }
    public int DoubleBoxItem { get; private set; }

    public bool HoleLocked { get; private set; }
    public bool RocketLocked { get; private set; }
    public bool DoubleBoxLocked { get; private set; }
    // ����
    public void AddPiggyCoinCount()
    {
        CoinCount += 10;
        PiggyCount += 30;
    }

    public void SetHolItemCount(int val)
    {
        HoleItem += val;
    }

    public void UnlockedItem(ItemType type)
    {
        if (type == ItemType.Hole)
            HoleLocked = false;
        else if (type == ItemType.Rocket)
            RocketLocked = false;
        else if (type == ItemType.DoubleBox)
            DoubleBoxLocked = false;
    }

    public void SetStarCount(int val)
    {
        StarCount += val;
    }

    public void SetRocketCount(int val)
    {
        RocketItem += val;
    }

}
