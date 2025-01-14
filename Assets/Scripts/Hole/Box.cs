using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 箱子
/// </summary>
public class Box : MonoBehaviour
{
    [SerializeField] private List<Hole> holeList;//洞列表
    [SerializeField] private ScrewColor color;//箱子的颜色
    private float moveDuration;//移动的速度

    private bool ismoving;
    public bool IsMoving
    {
        get => ismoving;
    }
    public int HoleCount //箱子拥有的洞的数量
    {
        get
        {
            return holeList.Count;
        }
    }
    public ScrewColor BoxColor//箱子的颜色
    {
        get
        {
            return color;
        }
    }
    protected bool hasStarHole = false;//是否拥有星星洞口

    [Space]
    [SerializeField] private int starHoleCount;//拥有星星洞口的数量
    [SerializeField] private int curStarCount;//当前星星螺丝的数量

    [SerializeField] private int normalCount;//普通螺丝的数量
    [SerializeField] private int curNormalCount;//当前普通螺丝的数量
    public bool ISStarFull()//判断当前星星数量是否已满
    {
        return starHoleCount == curStarCount;
    }
    public bool ISNormalFull()//判断当前普通数量是否已满
    {
        return normalCount == curNormalCount;
    }

    [SerializeField]private bool isCompleted;//判断是否已经完成
    public bool IsCompleted
    {
        get => isCompleted;
    }

    [Space]
    [SerializeField] private int curScrewCount;//当前洞口有螺丝的数量
    private float delayTime = 0;
    private float bounceDuration;//Q弹效果的时间
    private float bounceStrength;//Q弹的强度
    private void Start()
    {
        moveDuration = 0.3f;
        bounceDuration = 0.15f;
        bounceStrength = 0.4f;
        for (int i = 0; i < transform.childCount; i++)
        {
            holeList.Add(transform.GetChild(i).GetComponent<Hole>());
            transform.GetChild(i).GetComponent<Hole>().InitColor(color);

            //检查当前盒子是否含有星星洞口
            if (holeList[i] is StarHole)
            { 
                hasStarHole = true;
                starHoleCount++;
            }
        }
        normalCount = holeList.Count - starHoleCount;
    }

    /// <summary>
    /// 设置球到相应的位置,并加入到槽的列表中
    /// </summary>
    public void SetHoleList(Screw _screw)
    {
        if (delayTime == 0)
            delayTime = _screw.GetMoveTime + 0.1f;

        //如果是星星螺丝
        if (_screw.IsStar())
        {
            //放入到星星洞口中
            HandleHole(_screw, true);
        }
        else
        {
            //放入非星星洞口中
            HandleHole(_screw);
        }
    }

    /// <summary>
    /// 将小球放入洞口的函数
    /// </summary>
    private void HandleHole(Screw _screw, bool requireStarHole = false)
    {
        for (int i = 0; i < holeList.Count; i++)
        {
            // 根据传入的参数动态判断，是不是星星洞口
            bool isStarHoleCondition = requireStarHole ? (holeList[i] is StarHole) : !(holeList[i] is StarHole);

            if (holeList[i].HasScrew == false && isStarHoleCondition)
            {
                System.Action callBack = null;

                //如果是有链接的螺丝(需要进行检查)，或者该螺丝已经放入空槽中了
                if (_screw.CanRemoveFromGlass() || _screw.IsPlaced)
                {
                    // 播放螺丝的移动动画
                    _screw.transform.SetParent(holeList[i].transform);
                    holeList[i].HasScrew = true;
                    holeList[i].HoleScrew = _screw;

                    curScrewCount++;

                    if (requireStarHole)
                        curStarCount++;
                    else
                        curNormalCount++;
                }

                //螺丝移动到箱子中之后的回调
                callBack += () =>
                {
                    // 如果箱子中的螺丝数量满了
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
    /// 延迟生成盖子的函数d
    /// </summary>
    private void DelayInitNapGlass()
    {
        if (isCompleted)
        {
            Debug.Log("已经完成过");
            return;
        }
        foreach (Hole hole in holeList)
        {
            if (hole.HoleScrew.IsMoving)
            {
                Debug.Log("有小球在移动，无法立即移动");
                return;
            }
        }

        isCompleted = true;
        //在箱子上方生成一个盖子
        GetNapGlass.Instance.InitNapGlassPrefab(this, () =>
        {
            //盖字移动完毕之后，执行Q弹效果
            BounceBox();
        });
    }

    /// <summary>
    /// 槽满之后，执行切换下一个盒子的函数
    /// </summary>
    private void BoxCompleted()
    {
        //isCompleted = true;
        //当前处于两个箱子的状态
        if (GameManager.Instance.IsDoubleBox)
        {
            //尝试换下一个槽
            GameManager.Instance.ChangeBox();
            return;
        }

        //移动到右侧位置中
        MoveRight(GameManager.Instance.RightPos);
        GameManager.Instance.ChangeBox();
    }
    /// <summary>
    /// 箱子Q弹的效果
    /// </summary>
    private void BounceBox()
    {
        // 设置Q弹参数
        float bounceHeight = bounceStrength;  // 向上的Q弹高度
        float bounceDownDuration = bounceDuration / 2; // 每个Q弹部分的时间（上升和下降各一半）

        // 先向上移动，再向下移动，形成Q弹效果
        transform.DOBlendableMoveBy(new Vector3(0, -bounceHeight, 0), bounceDownDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BoxDrop");
                // 向下移动
                transform.DOBlendableMoveBy(new Vector3(0, bounceHeight, 0), bounceDownDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        // Q弹结束后调用 BoxCompleted

                        BoxCompleted();
                        //Invoke(nameof(BoxCompleted), 0.01f);
                    });
            });
    }
    /// <summary>
    /// 移动到左边的函数
    /// </summary>
    public void MoveLeft(Vector3 moveLeftPos)
    {
        MoveToTarget(moveLeftPos);
    }
    /// <summary>
    /// Box移动到右侧点的位置
    /// </summary>
    public void MoveRight(Vector3 moveRightPos)
    {
        MoveToTarget(moveRightPos, () =>
        {
            gameObject.SetActive(false);
        });
    }
    /// <summary>
    /// 移动到中间的函数
    /// </summary>
    public void MoveCenter(Vector3 moveCenterPos)
    {
        MoveToTarget(moveCenterPos, () => 
        {
            //Debug.Log("移动到中间");
            GameManager.Instance.EmptyScrewToBox();
        });
    }
    /// <summary>
    /// Box移动到右侧的协程
    /// </summary>
    /// <returns></returns>
    public void MoveToTarget(Vector3 targetPos, System.Action onCompleteCallback = null)
    {
        ismoving = true;
        transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.OutCubic) // 使用线性缓动，可以根据需要调整
            .OnComplete(() =>
            {
                // 调用传入的回调函数（如果存在）
                ismoving = false;
                onCompleteCallback?.Invoke();
            });
    }
}
