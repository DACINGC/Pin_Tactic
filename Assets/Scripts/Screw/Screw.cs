using DG.Tweening;
using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class Screw : MonoBehaviour
{
    #region ���
    private Collider2D cd;
    private Animator screwAnim;
    private Animator explosionAnim;
    private SpriteRenderer sr;
    private SpriteRenderer screwsr;
    private SpriteRenderer shadowsr;
    private SpriteRenderer holesr;
    private Rigidbody2D rb;
    private ScrewHole screwHole;
    public ScrewHole _Hole { get => screwHole; }
    #endregion

    #region ��������
    [SerializeField] private bool isClicked = false;//�Ƿ񱻵����
    [SerializeField] private bool isPlaced = false;//�Ƿ�����˺�����
    public bool IsPlaced
    {
        get => isPlaced;
    }
    [SerializeField] private ScrewColor color;//��˿��ɫ
    public void SetColor(ScrewColor col)
    {
        color = col;
    }
    #region ������˿
    [Header("������˿")]
    [SerializeField] private bool isStarHole;//��������˿
    public bool IsStar()
    {
        return isStarHole;
    }
    public void SetIsStarHole(bool value)
    {
        isStarHole = value;
    }
    #endregion

    #region ���ӵ���˿
    [Header("���ӵ���˿")]
    [SerializeField] private Screw connectedScrew;//���ӵ�С��
    public Screw ConnectedScrew
    {
        get => connectedScrew;
    }
    #endregion

    #region ��
    [Header("��")]
    [SerializeField] private bool hasDoor;//����
    public bool HasDoor
    {
        get => hasDoor;
    }
    public void SetDoorFlase()
    {
        hasDoor = false;
        door = null;
    }

    [SerializeField] private bool isClose;//�ŵ�״̬
    public bool IsDoorClose { get => isClose; }
    private Door door;
    public Door ScrewDoor
    {
        get => door;
        set
        {
            door = value;
        }
    }
    #endregion

    #region ��ը
    [Header("��ը")]
    [SerializeField] private bool hasBoom;//�Ƿ���ը��
    [SerializeField] private int boomCount;
    public Boom ScrewBoom { get; set; }

    public int BoomCount { get => boomCount; }
    public bool HasBoom { get => hasBoom; }

    private Vector3 originalScale;
    private Vector3 enlargedScale;
    #endregion

    #region �ƶ��Ͳ㼶
    [SerializeField]private bool ismoving;
    public bool IsMoving
    {
        get => ismoving;
    }//�Ƿ����ƶ�״̬
    public float GetMoveTime
    {
        get
        {
            return moveUpTime + moveTargetTime + moveDownTime;
        }
    } //�õ��ƶ�������ʱ��

    private float moveUpTime;//�����ƶ���ʱ��
    private float moveTargetTime;//����Ŀ����ʱ��
    private float moveDownTime;//�����ƶ���ʱ��

    public int Layer
    {
        get
        {
            return SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
        }
    }    //�õ�Ŀǰ���ڵڼ���

    public string LayerName
    {
        get
        {
            return sr.sortingLayerName;
        }
    }

    public int LayerOrder
    {
        get
        {
            return sr.sortingOrder;
        }
    }
    #endregion

    #endregion

    #region ����
    public ObiRope rope { get; private set; }
    public void SetObiRope(ObiRope _repe)
    {
        rope = _repe;
    }
    private Vector3 oriPos;


    #endregion

    #region ����
    [Header("����")]
    [SerializeField] private bool hasChain;
    public bool HasChain { get => hasChain; }
    [SerializeField] List<Screw> chainScrewList = new List<Screw>();//��������˿
    private List<GameObject> mainChainList = new List<GameObject>();
    private Screw mainChainScrew;//��������˿
    private ObiRope chain;//������˿����

    public bool IsOtherChain { get; set; }
    private ChainFx chainFx;
    public void SetChain(ChainFx chainfx)
    {
        chainFx = chainfx;
        chainFx.SetPlayCount(chainScrewList.Count);
    }
    #endregion

    #region ��
    [Header("�Ƿ񱻱�����")]
    [SerializeField] private bool isIceCovered;
    public void SetIceCoveredFalse()
    {
        isIceCovered = false;
    }
    private Ice ice;

    public Ice ScrewIce
    {
        get => ice;
        set
        {
            ice = value;
        }
    }

    public bool IsIceCovered
    {
        get => isIceCovered;
    }
    #endregion

    [Header("��/Կ��")]
    [SerializeField] private bool hasKey;
    [SerializeField] private bool hasLock;
    [SerializeField] private Screw lockScrew;//Կ����˿��������������˿
    public Lock ScrewLock { get; set; }
    public Key ScrewKey { get; set; }

    public bool HasKey { get => hasKey; }
    public bool HasLock { get => hasLock; }

    //Կ����˿���õĺ���
    public void KeyMoveToLcok()
    {
        ScrewKey.MoveToLock(lockScrew.ScrewLock, () => lockScrew.hasLock = false);
    }

    private Glass glass;
    public ScrewColor ScrewColor
    {
        get
        {
            return color;
        }
    }
    private void Awake()
    {
        //�õ����Ŷ�����������
        screwAnim = transform.Find("Image/Hole").GetComponent<Animator>();
        explosionAnim = GetComponent<Animator>();
        sr = transform.Find("Image").GetComponent<SpriteRenderer>();
        screwsr = transform.Find("Image/Hole").GetComponent<SpriteRenderer>();
        shadowsr = transform.Find("shadow").GetComponent<SpriteRenderer>();
        holesr = transform.parent.GetComponent<SpriteRenderer>();
        screwHole = transform.parent.GetComponent<ScrewHole>();


        screwsr.sortingLayerName = sr.sortingLayerName;
        screwsr.sortingOrder = 0;

        cd = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        glass = transform.parent.parent.GetComponent<Glass>();
        moveUpTime = 0.16f;
        moveTargetTime = 0.24f;
        moveDownTime = 0.126f;
        oriPos = transform.position;

        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.2f;

        if (door != null)
            door.SetClose(isClose);

        if (!HasGlassCovered())
            shadowsr.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0;
    }


    #region ���
    /// <summary>
    /// �ܷ�Ӳ������Ƴ�
    /// </summary>
    /// <returns></returns>
    public bool CanRemoveFromGlass()
    {
        bool canRemove = true;

        //�����ӵ���˿����˿���ܵ������û�з��������
        if (connectedScrew != null && !connectedScrew.CanClick() && !connectedScrew.IsPlaced)
            canRemove = false;

        return canRemove;
    }
    /// <summary>
    /// ����Ƿ���Ե��
    /// </summary>
    /// <returns></returns>
    public bool CanClick()
    {
        bool canClick = HasGlassCovered() == false;
        cd.isTrigger = true;

        if (HasDoor)
            canClick = canClick && door.IsClose == false;

        if (isIceCovered)
            canClick = false;

        if (hasChain)
            canClick = false;

        if (hasLock)
            canClick = false;

        return canClick && isClicked == false;
    }
    /// <summary>
    /// �Ƿ��в�������
    /// </summary>
    public bool HasGlassCovered()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.45f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Glass>() != null)
            {
                if (Layer < hit.GetComponent<Glass>().Layer)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.45f);
    }
    public void RemoveFromGlass()
    {
        //�ж��Ƿ�����������
        if (glass != null && glass.ScrewList.Count > 0)
            glass.RemveScrew(this);


        //�Ѷ�����
        if (holesr != null && glass.gameObject.name == "glass 0")
        {
            screwHole.RocketExplotion();
        }


    }
    #endregion

    #region ����
    //������˿�Ķ���
    private void SetScrewAnim(bool boolean)
    {
        screwAnim.SetBool("isRun", boolean);
    }

    private void EnterBoomAnim()
    {
        explosionAnim.SetBool("EndBoom", true);
    }

    /// <summary>
    /// ��ը����
    /// </summary>
    public void DoBoomAnim()
    {
        if (ScrewBoom.gameObject == null)
            return;

        ScrewBoom.DecreaseCount();
        Vector3 orboomScale = ScrewBoom.transform.localScale;
        // ͬʱ�Ŵ󲢸ı���ɫ
        transform.DOScale(enlargedScale, 0.15f).SetEase(Ease.OutQuad) // �ȿ����
            .OnStart(() =>
            {
                explosionAnim.SetBool("EndBoom", true);
                ScrewBoom.transform.DOScale(orboomScale * 1.2f, 0.15f).SetEase(Ease.OutQuad); // �ȿ����
            })
            .OnComplete(() => // �Ŵ���ɺ�ִ����С����ɫ�ָ�
            {
                transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutQuad); // �ȿ����
                ScrewBoom.transform.DOScale(orboomScale, 0.15f).SetEase(Ease.OutQuad); // �ȿ����

                if (!ScrewBoom.CanEnterBoomAnim())
                    explosionAnim.SetBool("EndBoom", false);
            });

        if (ScrewBoom.CanEnterBoomAnim())
            EnterBoomAnim();
    }
    public void DestoryBoom()
    {
        if (ScrewBoom != null)
        {
            explosionAnim.SetBool("EndBoom", false);
            Destroy(ScrewBoom.gameObject);
        }
    }

    /// <summary>
    /// ����Ӱ�Ĳ㼶����Ϊ���㼶
    /// </summary>
    public void SetShadow()
    {
        shadowsr.sortingLayerName = sr.sortingLayerName;
    }

    #endregion

    #region �ƶ�
    /// <summary>
    /// �����ƶ��Ķ���
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    public void MoveUp(Vector3 _moveTargetPos, System.Action callback = null)
    {
        isClicked = true;
        sr.sortingLayerName = "End";
        screwsr.sortingLayerName = "End";
        shadowsr.sortingLayerName = "End";

        //�Ƿ�Ϊ����������С��
        HingeJoint2D screwHj = GetComponent<HingeJoint2D>();
        if (screwHj != null)
        {
            screwHj.connectedBody = null;
            screwHj.enabled = false;
        }

        // ����Э�̣��ȴ� ismoving ����Ϊ false
        StartCoroutine(WaitForNotMovingThenMove(_moveTargetPos, callback));
    }

    /// <summary>
    /// �ȴ��ϴ��ƶ����֮��
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    private IEnumerator WaitForNotMovingThenMove(Vector3 _moveTargetPos, System.Action callback = null)
    {
        // �ȴ� ismoving ����Ϊ false
        while (ismoving)
        {
            //Debug.Log("�ȴ���һ���ƶ�");
            yield return null; // �ȴ���һ֡
        }

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        ismoving = true;
        SetScrewAnim(true);
        Vector2 targetPos = new Vector2(transform.position.x, transform.position.y + 1.5f);
        // ��ʼ�ƶ�����
        transform.DOMove(targetPos, moveUpTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                //����Ƿ������ӵ���˿
                //û����ֱ���ƶ���Ŀ��λ��
                if (connectedScrew == null)
                {
                    //Debug.Log(color + "û�����ӵ�����");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && connectedScrew.CanClick())
                {
                    //��������ӵ���˿����Ҫ�ǿ��Ե����״̬
                    //Debug.Log(color + "��������������");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && !connectedScrew.CanClick() && connectedScrew.isPlaced)
                {
                    //��������״̬
                    //Debug.Log("������");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && !connectedScrew.CanClick() && !connectedScrew.isPlaced)
                {
                    //����У����ǲ����Ե��(bug����������С����ƶ���[�ѽ��]))
                    //�ƶ���ȥ
                    Debug.Log("���ӵ�С���޷����");
                    MoveBack();
                }
                else
                {
                    Debug.Log("������ԣ���Ҫ����");
                }
            });
    }
    /// <summary>
    /// ��Ŀ����ƶ��Ķ���
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    private void MoveToTarget(Vector3 _moveTargetPos, System.Action callback = null)
    {
        //Debug.Log(color + "��Ŀ����ƶ�");
        Vector3 movetarget = new Vector3(_moveTargetPos.x, _moveTargetPos.y + 1f);

        transform.DOMove(movetarget, moveTargetTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                SetScrewAnim(false);
                MoveDown(_moveTargetPos, callback);
                //callback?.Invoke();
            });
    }
    /// <summary>
    /// �����ƶ��Ķ���
    /// </summary>
    private void MoveDown(Vector3 _moveTargetPos, System.Action callback = null)
    {
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.DOMove(_moveTargetPos, moveDownTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                ismoving = false;
                cd.enabled = false;
                isPlaced = true;
                callback?.Invoke();
                //�����Ϸ�Ƿ�ʧ��
                GameManager.Instance.loseGame();

                //��������ӵ�С��,����û�з��뵽Ŀ������У���Ҫ�����ӵ�С�����õ�Ŀ��λ��
                if (connectedScrew != null)
                {
                    if (connectedScrew.isPlaced == false)
                    {
                        //���������ӵ���˿֮��ȡ������С���Ч��
                        connectedScrew.isClicked = true;
                        rope.GetComponents<ObiParticleAttachment>()[0].enabled = false; // ����
                        rope.GetComponents<ObiParticleAttachment>()[1].enabled = false; // �յ��
                        Invoke(nameof(DelayReturnRope), 3f);
                        connectedScrew.connectedScrew = null;

                        GameManager.Instance.SetScrew(connectedScrew);
                        connectedScrew = null;
                    }
                }
            });
    }
    //�ƶ���ȥ
    private void MoveBack()
    {
        SetScrewAnim(false);
        transform.DOMove(oriPos, moveDownTime).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                isClicked = false;
                ismoving = false;
            });
    }
    #endregion

    #region ����
    private void DelayReturnRope()
    {
        LevelManager.Instance.ObiRopeCreater.ReturnRopeOrChain(rope);
    }
    #endregion

    #region ����
    /// <summary>
    /// ������������������
    /// </summary>
    public List<ObiRope> CreateChain(ObiRopeChainCreator creator)
    {
        List<ObiRope> ropelist = new List<ObiRope>();
        foreach (Screw screw in chainScrewList)
        {
            ObiRope curRope = creator.CreateRopeOrChain(transform, screw.transform, true);
            screw.chain = curRope;
            screw.mainChainScrew = this;
            screw.IsOtherChain = true;

            ropelist.Add(curRope);
        }

        float angleStep = 360 / chainScrewList.Count;

        for (int i = 0; i < chainScrewList.Count; i++)
        {
            GameObject newChain = Instantiate(ResourceLoader.Instance.GetChain(), transform.parent);
            newChain.transform.position = transform.position;
            newChain.transform.rotation = Quaternion.Euler(0, 0, i * angleStep);

            if (chainScrewList.Count == 2 && i == 1)
                newChain.transform.rotation = Quaternion.Euler(0, 0, 90);

            mainChainList.Add(newChain);
        }
        return ropelist;
    }
    /// <summary>
    /// ������˿�����⿪
    /// </summary>
    public void UnlockChain()
    {
        IsOtherChain = false;
        mainChainScrew.chainFx.PlayParticle();//����˿ ��������Ч��

        if (mainChainScrew.mainChainList.Count > 0)
        {
            Destroy(mainChainScrew.mainChainList[0]);
            mainChainScrew.mainChainList.RemoveAt(0);
        }

        chain.GetComponents<ObiParticleAttachment>()[0].enabled = false; // ����
        chain.GetComponents<ObiParticleAttachment>()[1].enabled = false; // �յ��

        //����˿���б�����Ƴ�
        mainChainScrew.chainScrewList.Remove(this);


        if (mainChainScrew.chainScrewList.Count == 0)
            mainChainScrew.hasChain = false;

        Invoke(nameof(DelayReturnChain), 3);
    }
    private void DelayReturnChain()
    {
        LevelManager.Instance.ObiRopeCreater.ReturnRopeOrChain(chain);
    }
    #endregion
}
