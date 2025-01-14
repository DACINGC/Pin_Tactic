using UnityEngine;
using System.Collections.Generic;
public class Glass : MonoBehaviour
{
    #region ���
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D cd;
    public SpriteRenderer SR { get => sr; }
    #endregion
    private Vector3 worldSize;

    private bool isExposion = false;
    public bool IsExplosion
    {
        get => isExposion;
    }

    [SerializeField] private bool hasConnect;//�Ƿ������ӵ���˿
    public bool HasConnect
    {
        get => hasConnect;
    }
    [SerializeField] private bool hasIceCovered;//�Ƿ��б������ǵ���˿
    public bool HasIceCovered
    {
        get => hasIceCovered;
    }
    [SerializeField] private bool hasDoor;
    public bool HasDoor { get => hasDoor; }
    [SerializeField] private bool hasBoom;
    public bool HasBoom { get => hasBoom; }

    [SerializeField] private bool hasChain;
    public bool HasChain { get => hasChain; }
    [SerializeField] private List<Screw> screwList = new List<Screw>(); //�����ӵ���˿����

    [SerializeField] private List<ScrewHole> screwHoleList = new List<ScrewHole>();
    [SerializeField] private bool hasKey;
    public bool HasKey { get => hasKey; }
    [SerializeField] private bool hasLock;
    public bool HasLock { get => hasLock; }
    public List<Screw> ScrewList
    {
        get => screwList;
    }
    public int Layer
    {
        get
        {
            return SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
        }
    }
    public Screw FirstScrew
    {
        get 
        {
            if (screwList.Count > 0)
                return screwList[0];
            else
                return null;
        } 
    }
    public string LayerName
    {
        get
        {
            return sr.sortingLayerName;
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cd = GetComponent<Collider2D>();

        screwList.Clear();

    }
    private void Start()
    {
        if (sr != null && sr.enabled != false)
        {
            worldSize = new Vector3(sr.sprite.rect.width / sr.sprite.pixelsPerUnit * transform.lossyScale.x,
            sr.sprite.rect.height / sr.sprite.pixelsPerUnit * transform.lossyScale.y
            );
        }
    }
    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="root"></param>
    public void InitScrewList()
    {
        foreach (Transform child in transform)
        {
            ScrewHole screwHole = child.GetComponent<ScrewHole>();
            if (screwHole != null)
            {
                screwHoleList.Add(screwHole);
                //�ȳ�ʼ��������
                screwHole.InitScrewHole();

                Screw screw = screwHole.HoleScrew;
                if (screw != null)
                {
                    
                    screwList.Add(screw);

                    if (hasConnect == false && screw.ConnectedScrew != null)
                        hasConnect = true;

                    if (hasIceCovered == false && screw.IsIceCovered)
                        hasIceCovered = true;

                    if (hasDoor == false && screw.HasDoor)
                        hasDoor = true;

                    if (screw.HasBoom && hasBoom == false)
                        hasBoom = true;

                    if (screw.HasChain && hasChain == false)
                        hasChain = true;

                    if (screw.HasKey && hasKey == false)
                        hasKey = true;

                    if (screw.HasLock && hasLock == false)
                        hasLock = true;
                }
            }


        }

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        CheckScrweListCount();
    }
    /// <summary>
    /// �Ӳ������Ƴ���˿
    /// </summary>
    /// <param name="nail"></param>
    public void RemveScrew(Screw nail)
    {
        Screw removeScrew = nail;
        screwList.Remove(nail);

        if (gameObject.name == "glass 0")
            return;

        if (screwList.Count == 1)
        {
            //������ֻʣ��һ��С��
            Rigidbody2D screwrb = screwList[0].gameObject.GetComponent<Rigidbody2D>();
            if (screwrb == null)
            {
                screwrb = screwList[0].gameObject.AddComponent<Rigidbody2D>();
                screwrb.gravityScale = 0;
                screwrb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            rb.gravityScale = 1;
            rb.constraints = RigidbodyConstraints2D.None;

            HingeJoint2D screwHj = screwList[0].gameObject.AddComponent<HingeJoint2D>();
            screwHj.connectedBody = rb;

            //fj.enabled = true;
            //fj.connectedBody = screwrb;
        }
        else if (screwList.Count == 0)
        {
            //fj.enabled = false;
            //������û��С����
            Rigidbody2D screwrb = removeScrew.gameObject.GetComponent<Rigidbody2D>();
            if (screwrb == null)
            { 
                screwrb = removeScrew.gameObject.AddComponent<Rigidbody2D>();
                screwrb.gravityScale = 0;
                screwrb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            rb.constraints = RigidbodyConstraints2D.None;
            rb.gravityScale = 1;
            //fj.connectedBody = null;
            CheckBehindScrew();
        }
    }
    /// <summary>
    /// ����Ƿ������ص���˿
    /// </summary>
    public void CheckBehindScrew()
    {
        // ʹ���������ת�Ƕȣ��� Z ����ת��
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, worldSize, transform.eulerAngles.z);
        foreach (Collider2D hit in hits)
        {
            Screw screw = hit.GetComponent<Screw>();
            if (screw != null)
            {
                // ����ò����Ĳ㼶֮���б����ص�С�򣬽�С�����Ӱ��������
                if (Layer - screw.Layer == 1)
                {
                    screw.SetShadow();
                }
            }
        }
    }

    /// <summary>
    /// �����ը֮�����
    /// </summary>
    public void SetFalse()
    {
        sr.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //fj.enabled = false;

        cd.isTrigger = true;
        cd.enabled = false;
        sr.sortingLayerName = "Glass3";
        CheckBehindScrew();
        isExposion = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            SpriteRenderer sr = transform.GetChild(i).GetComponent<SpriteRenderer>();

            if (sr != null)
                sr.enabled = false;
        }
        foreach (ScrewHole screwHole in screwHoleList)
        {
            screwHole.RocketExplotion();
        }
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (sr == null)
            return;

        Gizmos.color = Color.red;

        // ��ȡ����ռ�ĺ���λ�á���С����ת�Ƕ�
        Vector3 boxPosition = transform.position;         // ���ӵ�λ��
        Vector2 boxSize = worldSize;                      // ���ӵĴ�С������ռ䣩
        Quaternion boxRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z); // �� Z �����ת

        // ���浱ǰ Gizmos ����
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // �� Gizmos �ľ�������Ϊ��ǰ�����λ�ú���ת
        Gizmos.matrix = Matrix4x4.TRS(boxPosition, boxRotation, Vector3.one);

        // ����һ���߿����
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // �ָ�ԭ���� Gizmos ����
        Gizmos.matrix = oldMatrix;
    }

    /// <summary>
    ///�����˿�б�����
    /// </summary>
    private void CheckScrweListCount()
    {
        if (screwList.Count == 1 && gameObject.name != "glass 0")
        {
            HingeJoint2D screwHj = screwList[0].gameObject.AddComponent<HingeJoint2D>();
            screwHj.connectedBody = rb;
            screwHoleList[0].GetComponent<Collider2D>().isTrigger = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }
        else if (screwList.Count == 0)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.gravityScale = 1;
        }
    }
}
