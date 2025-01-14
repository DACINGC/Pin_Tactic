using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����
/// </summary>
public class Box : MonoBehaviour
{
    [SerializeField] private List<Hole> holeList;//���б�
    [SerializeField] private ScrewColor color;//���ӵ���ɫ
    private float moveDuration;//�ƶ����ٶ�

    private bool ismoving;
    public bool IsMoving
    {
        get => ismoving;
    }
    public int HoleCount //����ӵ�еĶ�������
    {
        get
        {
            return holeList.Count;
        }
    }
    public ScrewColor BoxColor//���ӵ���ɫ
    {
        get
        {
            return color;
        }
    }
    protected bool hasStarHole = false;//�Ƿ�ӵ�����Ƕ���

    [Space]
    [SerializeField] private int starHoleCount;//ӵ�����Ƕ��ڵ�����
    [SerializeField] private int curStarCount;//��ǰ������˿������

    [SerializeField] private int normalCount;//��ͨ��˿������
    [SerializeField] private int curNormalCount;//��ǰ��ͨ��˿������
    public bool ISStarFull()//�жϵ�ǰ���������Ƿ�����
    {
        return starHoleCount == curStarCount;
    }
    public bool ISNormalFull()//�жϵ�ǰ��ͨ�����Ƿ�����
    {
        return normalCount == curNormalCount;
    }

    [SerializeField]private bool isCompleted;//�ж��Ƿ��Ѿ����
    public bool IsCompleted
    {
        get => isCompleted;
    }

    [Space]
    [SerializeField] private int curScrewCount;//��ǰ��������˿������
    private float delayTime = 0;
    private float bounceDuration;//Q��Ч����ʱ��
    private float bounceStrength;//Q����ǿ��
    private void Start()
    {
        moveDuration = 0.3f;
        bounceDuration = 0.15f;
        bounceStrength = 0.4f;
        for (int i = 0; i < transform.childCount; i++)
        {
            holeList.Add(transform.GetChild(i).GetComponent<Hole>());
            transform.GetChild(i).GetComponent<Hole>().InitColor(color);

            //��鵱ǰ�����Ƿ������Ƕ���
            if (holeList[i] is StarHole)
            { 
                hasStarHole = true;
                starHoleCount++;
            }
        }
        normalCount = holeList.Count - starHoleCount;
    }

    /// <summary>
    /// ��������Ӧ��λ��,�����뵽�۵��б���
    /// </summary>
    public void SetHoleList(Screw _screw)
    {
        if (delayTime == 0)
            delayTime = _screw.GetMoveTime + 0.1f;

        //�����������˿
        if (_screw.IsStar())
        {
            //���뵽���Ƕ�����
            HandleHole(_screw, true);
        }
        else
        {
            //��������Ƕ�����
            HandleHole(_screw);
        }
    }

    /// <summary>
    /// ��С����붴�ڵĺ���
    /// </summary>
    private void HandleHole(Screw _screw, bool requireStarHole = false)
    {
        for (int i = 0; i < holeList.Count; i++)
        {
            // ���ݴ���Ĳ�����̬�жϣ��ǲ������Ƕ���
            bool isStarHoleCondition = requireStarHole ? (holeList[i] is StarHole) : !(holeList[i] is StarHole);

            if (holeList[i].HasScrew == false && isStarHoleCondition)
            {
                System.Action callBack = null;

                //����������ӵ���˿(��Ҫ���м��)�����߸���˿�Ѿ�����ղ�����
                if (_screw.CanRemoveFromGlass() || _screw.IsPlaced)
                {
                    // ������˿���ƶ�����
                    _screw.transform.SetParent(holeList[i].transform);
                    holeList[i].HasScrew = true;
                    holeList[i].HoleScrew = _screw;

                    curScrewCount++;

                    if (requireStarHole)
                        curStarCount++;
                    else
                        curNormalCount++;
                }

                //��˿�ƶ���������֮��Ļص�
                callBack += () =>
                {
                    // ��������е���˿��������
                    if (curScrewCount == holeList.Count)
                    {
                        ismoving = true;
                        DelayInitNapGlass();
                        //Invoke(nameof(DelayInitNapGlass), 0.05f);
                    }
                };
                _screw.MoveUp(holeList[i].transform.position, callBack);
                return;
            }
        }
    }

    /// <summary>
    /// �ӳ����ɸ��ӵĺ���d
    /// </summary>
    private void DelayInitNapGlass()
    {
        if (isCompleted)
        {
            Debug.Log("�Ѿ���ɹ�");
            return;
        }
        foreach (Hole hole in holeList)
        {
            if (hole.HoleScrew.IsMoving)
            {
                Debug.Log("��С�����ƶ����޷������ƶ�");
                return;
            }
        }

        isCompleted = true;
        //�������Ϸ�����һ������
        GetNapGlass.Instance.InitNapGlassPrefab(this, () =>
        {
            //�����ƶ����֮��ִ��Q��Ч��
            BounceBox();
        });
    }

    /// <summary>
    /// ����֮��ִ���л���һ�����ӵĺ���
    /// </summary>
    private void BoxCompleted()
    {
        //isCompleted = true;
        //��ǰ�����������ӵ�״̬
        if (GameManager.Instance.IsDoubleBox)
        {
            //���Ի���һ����
            GameManager.Instance.ChangeBox();
            return;
        }

        //�ƶ����Ҳ�λ����
        MoveRight(GameManager.Instance.RightPos);
        GameManager.Instance.ChangeBox();
    }
    /// <summary>
    /// ����Q����Ч��
    /// </summary>
    private void BounceBox()
    {
        // ����Q������
        float bounceHeight = bounceStrength;  // ���ϵ�Q���߶�
        float bounceDownDuration = bounceDuration / 2; // ÿ��Q�����ֵ�ʱ�䣨�������½���һ�룩

        // �������ƶ����������ƶ����γ�Q��Ч��
        transform.DOBlendableMoveBy(new Vector3(0, -bounceHeight, 0), bounceDownDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BoxDrop");
                // �����ƶ�
                transform.DOBlendableMoveBy(new Vector3(0, bounceHeight, 0), bounceDownDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        // Q����������� BoxCompleted

                        BoxCompleted();
                        //Invoke(nameof(BoxCompleted), 0.01f);
                    });
            });
    }
    /// <summary>
    /// �ƶ�����ߵĺ���
    /// </summary>
    public void MoveLeft(Vector3 moveLeftPos)
    {
        MoveToTarget(moveLeftPos);
    }
    /// <summary>
    /// Box�ƶ����Ҳ���λ��
    /// </summary>
    public void MoveRight(Vector3 moveRightPos)
    {
        MoveToTarget(moveRightPos, () =>
        {
            gameObject.SetActive(false);
        });
    }
    /// <summary>
    /// �ƶ����м�ĺ���
    /// </summary>
    public void MoveCenter(Vector3 moveCenterPos)
    {
        MoveToTarget(moveCenterPos, () => 
        {
            //Debug.Log("�ƶ����м�");
            GameManager.Instance.EmptyScrewToBox();
        });
    }
    /// <summary>
    /// Box�ƶ����Ҳ��Э��
    /// </summary>
    /// <returns></returns>
    public void MoveToTarget(Vector3 targetPos, System.Action onCompleteCallback = null)
    {
        ismoving = true;
        transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.OutCubic) // ʹ�����Ի��������Ը�����Ҫ����
            .OnComplete(() =>
            {
                // ���ô���Ļص�������������ڣ�
                ismoving = false;
                onCompleteCallback?.Invoke();
            });
    }
}
