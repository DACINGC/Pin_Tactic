using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemMoveManager : MonoBehaviour
{
    // ������̬ʵ��
    public static ItemMoveManager Instance { get; private set; }
    [SerializeField] private Sprite hole;
    [SerializeField] private Sprite rocket;
    [SerializeField] private Sprite doubleBox;

    private float dropDistance = 1f;    // ��ʼ���µľ���
    private float moveDuration = 1f;    // �ܵ��˶�ʱ��
    private float delayTime = 0.06f;

    private float textTime = 1;
    private float scaleCount = 1.5f;
    private List<Vector3> coinLeftPosList = new List<Vector3>();
    private List<Vector3> coinRightPosList = new List<Vector3>();

    private Transform coinLeftTrans;
    private Transform coinRightTrans;
    private Transform unlockItemTrans;
    private Image itemIcon;
    private Vector3 orginItemPos;

    private Transform starTrans;
    private Vector3 orginStarPos;

    private Transform gameLeft;
    private Transform gameRight;
    private Transform homeLeft;
    private Transform homeRight;
    private Transform starMoveTarget;

    private void Awake()
    {
        // ����ģʽʵ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ����Ѿ�����ʵ�������ٵ�ǰ����
            return;
        }
        Instance = this;

        // ��ʼ���߼�
        coinLeftTrans = transform.Find("CoinLeft").transform;
        coinRightTrans = transform.Find("CoinRight").transform;

        unlockItemTrans = transform.Find("Hole").transform;
        orginItemPos = unlockItemTrans.position;
        itemIcon = unlockItemTrans.GetComponent<Image>();

        starTrans = transform.Find("Par Star").transform;
        orginStarPos = starTrans.position;

    }
    private void Start()
    {
        unlockItemTrans.gameObject.SetActive(false);
        starTrans.gameObject.SetActive(false);

        gameLeft = UIManager.Instance.GetUI<WinUI>().CoinTrans;
        gameRight = UIManager.Instance.GetUI<WinUI>().PiggyTrans;

        starMoveTarget = HomeSceneUI.Instance.homeUI.StarTrans;

        InitCoin(coinLeftTrans, coinLeftPosList);
        InitCoin(coinRightTrans, coinRightPosList);

    }

    private void InitCoin(Transform parent, List<Vector3> InitList)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            child.gameObject.SetActive(false);
            InitList.Add(child.position);
        }
    }
    #region Ӳ���ƶ�
    public void MoveCoin(int startVal1, int endVal1,int startVal2, int endVal2, System.Action onComplete = null)
    {
        AudioManager.Instance.PlaySFX("Coin");

        int totalCoins = coinLeftTrans.childCount + coinRightTrans.childCount;
        int completedCoins = 0;

        // ����ֲ��ص�
        Action onChildComplete = () =>
        {
            completedCoins++;
            if (completedCoins >= totalCoins)
            {
                onComplete?.Invoke();
            }
        };
        Transform leftTarget = gameLeft;
        Transform rightTaget = gameRight;

        //��Ϸ�ڵ��next��ťʱ���ؿ����Ѿ�������
        if (LevelManager.Instance.GetLevleNum() >= 6)
        {
            leftTarget = homeLeft;
            rightTaget = homeRight;

            if (homeLeft == null)
            {
                homeLeft = HomeSceneUI.Instance.homeUI.CoinTrans;
                homeRight = HomeSceneUI.Instance.homeUI.PiggyTrans;
                leftTarget = homeLeft;
                rightTaget = homeRight;
            }
        }

        // ��ʼ�ƶ�������
        MoveChildrenToTarget(coinLeftTrans, leftTarget, coinLeftPosList, onChildComplete);
        MoveChildrenToTarget(coinRightTrans, rightTaget, coinRightPosList, onChildComplete);

        Text text1 = null;
        if (LevelManager.Instance.GetLevleNum() < 6)
        {
            text1 = UIManager.Instance.GetUI<WinUI>().CoinText;
            Text text2 = UIManager.Instance.GetUI<WinUI>().PiggyText;
            TextEffect(text2, startVal2, endVal2);
        }
        else
        {
            text1 = HomeSceneUI.Instance.homeUI.GetCoinText(); 
        }
        TextEffect(text1, startVal1, endVal1);
    }
    private void MoveChildrenToTarget(Transform parent, Transform target,List<Vector3> InitPos, Action onChildComplete)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            child.gameObject.SetActive(true);
            child.position = InitPos[i];
            // ����ÿ����������ӳ�
            float delay = i * delayTime;

            // �����ƶ��߼�
            MoveToTarget(child, delay, target.position, onChildComplete);
        }
    }

    private void MoveToTarget(Transform child, float delay, Vector3 targetPos, Action onChildComplete)
    {
        Vector3 startPos = child.position;                      // ��ʼλ��
        Vector3 dropPos = startPos + Vector3.down * dropDistance; // ����ƫ�Ƶ�λ��    

        Sequence sequence = DOTween.Sequence();

        sequence.Append(child.DOMove(dropPos, moveDuration * 0.5f).SetEase(Ease.OutQuad));

        sequence.Append(child.DOMove(targetPos, moveDuration * 0.5f).SetEase(Ease.InOutQuad));

        sequence.PrependInterval(delay);

        // ������ɺ�ص�
        sequence.OnComplete(() =>
        {
            child.gameObject.SetActive(false);
            onChildComplete?.Invoke(); // ÿ�����������ʱ���þֲ��ص�
        });
    }
    #endregion

    /// <summary>
    /// �仯�ı�
    /// </summary>
    /// <param name="targetText"></param>
    /// <param name="startValue">��ʼ����</param>
    /// <param name="endValue">��������</param>
    private void TextEffect(Text targetText, int startValue, int endValue)
    {
        // ��ֵ�仯
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            targetText.text = startValue.ToString(); // ����UI�ı�
        }, endValue, textTime).SetEase(Ease.Linear).SetDelay(0.15f);

        // ���Ŷ���
        targetText.transform.DOScale(scaleCount, textTime * 0.75f) // �Ŵ�
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetText.transform.DOScale(1f, textTime * 0.25f) // ����ԭ����С
                    .SetEase(Ease.InQuad);
            }).SetDelay(0.15f);
    }

    /// <summary>
    /// �������ƶ�
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="callback"></param>
    public void MoveItem(ItemType type, System.Action callback = null)
    {   

        Vector3 movePos = MainSceneUI.Instance._GamePlayUI.HoleTrans.position;
        itemIcon.sprite = hole;

        if (type == ItemType.Rocket)
        {
            movePos = MainSceneUI.Instance._GamePlayUI.RocketTrans.position;
            itemIcon.sprite = rocket;
        }
        else if (type == ItemType.DoubleBox)
        {
            movePos = MainSceneUI.Instance._GamePlayUI.DoubleBoxTrans.position;
            itemIcon.sprite = doubleBox;
        }

        unlockItemTrans.gameObject.SetActive(true);
        unlockItemTrans.position = orginItemPos;

        Sequence sequence = DOTween.Sequence();
        // ����ƶ�����
        sequence.Append(unlockItemTrans.DOMove(movePos, 0.8f).SetEase(Ease.OutQuad));

        // ������Ŷ���
        sequence.Join(unlockItemTrans.DOScale(new Vector3(0.42f, 0.42f, 0.42f), 0.8f).SetEase(Ease.OutQuad));

        // �ڶ���ȫ����ɺ���ûص�
        sequence.OnComplete(() =>
        {
            unlockItemTrans.gameObject.SetActive(false);
            callback?.Invoke();
        });
    }

    /// <summary>
    /// �����ƶ�
    /// </summary>
    public void MoveAndRotateStar()
    {
        starTrans.gameObject.SetActive(true);
        starTrans.position = orginStarPos;

        // Ŀ������ֵ
        Vector3 targetScale = new Vector3(1f, 1f, 1f);

        // ������תȦ����360��ΪһȦ��
        float totalRotationTime = moveDuration / 1; // ����� 1 ����תһȦ��ʱ�䣬���Ը��������޸�
        float totalRotation = 360f * Mathf.Ceil(totalRotationTime);

        // ������������
        Sequence sequence = DOTween.Sequence();

        // ����ƶ�����
        sequence.Join(starTrans.DOMove(starMoveTarget.position, moveDuration)
            .SetEase(Ease.InCubic)); 

        // �����ת����
        sequence.Join(starTrans.DORotate(new Vector3(0, 0, totalRotation), moveDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InCubic));

        // ������Ŷ���
        sequence.Join(starTrans.DOScale(targetScale, moveDuration)
            .SetEase(Ease.InCubic)); 

        // ������ɻص�
        sequence.OnComplete(() =>
        {
            starTrans.gameObject.SetActive(false);
            //Debug.Log("�ƶ�����ת�����Ŷ�����ɣ�");
        });
    }


    public void TextAddStarCount()
    {
        GameDataManager.CurrentGameData.starCount += 200;
        HomeSceneUI.Instance.homeUI.UpdateResourceText();
    }
}

