using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHoleManager : MonoBehaviour
{
    [SerializeField] private List<EmptyHole> holeList = new List<EmptyHole>(); // 主列表
    [SerializeField] private List<EmptyHole> extraHoles = new List<EmptyHole>(); // 额外列表
    [SerializeField] private GameObject emptyHolePrefab; // 额外空槽的预制体
    [SerializeField] private ParticleSystem apearFx;
    [SerializeField] private int curScrewCount = 0; // 当前螺丝计数

    private void Start()
    {
        // 初始化主列表
        for (int i = 0; i < transform.childCount; i++)
        {
            holeList.Add(transform.GetChild(i).GetComponent<EmptyHole>());
        }
    }

    /// <summary>
    /// 加入到空的槽中
    /// </summary>
    public void AddToEmptyHole(Screw _screw)
    {
        // 遍历主列表和额外列表
        foreach (EmptyHole hole in GetAllHoles())
        {
            if (hole.IsEmpty())
            {

                if (_screw.CanRemoveFromGlass())
                { 
                    hole.Screw =_screw;
                    _screw.transform.SetParent(hole.transform);
                    curScrewCount++;
                }

                _screw.MoveUp(hole.transform.position);
                return;
            }
        }

        Debug.Log("无法加入到空槽中");
    }

    /// <summary>
    /// 从待选区选出小球放入目标箱子
    /// </summary>
    /// <param name="targetBox"></param>
    public void AddToBox(Box targetBox)
    {
        ////如果箱子没有星星洞口
        //if (targetBox.ISStarFull())
        //{
        //    SetScrewToBox(targetBox, true);
        //}
        //else
        //{
        //    //有星星洞口
        //    SetScrewToBox(targetBox, false);
        //}
        SetScrewToBox(targetBox);

    }
    /// <summary>
    /// 遍历容器将螺丝加入到箱子中（是否跳过星星螺丝）
    /// </summary>
    public void SetScrewToBox(Box targetBox, bool skipStarBalls = false)
    {
        //Debug.Log("调用空槽函数");
        foreach (EmptyHole hole in GetAllHoles())
        {
            if (!hole.IsEmpty())
            {
                // 根据传入的参数决定是否跳过星星小球
                if (skipStarBalls && hole.Screw.IsStar())
                    continue;

                //目标的普通小球满了，则不需要继续添加普通螺丝了
                if (targetBox.ISNormalFull() && !hole.Screw.IsStar())
                    continue;

                //目标的星星螺丝满了，则不需要继续添加星星螺丝了
                if (targetBox.ISStarFull() && hole.Screw.IsStar())
                    continue;

                if (targetBox.BoxColor == hole.Screw.ScrewColor)
                {
                    targetBox.SetHoleList(hole.Screw);
                    //Debug.Log(hole.Screw.ScrewColor + "放入盒子中");
                    // 将槽内容置为空
                    hole.Screw = null;
                    curScrewCount--;
                }
            }
        }
    }

    /// <summary>
    /// 检查游戏是否失败的函数
    /// </summary>
    public bool CheckGameFailed()
    {
        if (curScrewCount >= GetAllHoles().Count)
        {
            //Debug.Log("游戏失败: 空槽已经满了");
            //Debug.Log("curCount: " + curScrewCount + " listCount: " + GetAllHoles().Count);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 清空所有空槽
    /// </summary>
    public void ClearEmptyHole()
    {
        curScrewCount = 0;
        transform.position = Vector3.zero;

        foreach (EmptyHole hole in GetAllHoles())
        {
            if (!hole.IsEmpty())
            {
                Destroy(hole.Screw.gameObject);
                hole.Screw = null;
            }
        }

        foreach (EmptyHole hole in extraHoles)
        {
            Destroy(hole.gameObject);
        }
        extraHoles.Clear();
    }

    /// <summary>
    /// 激活额外的空槽Buff
    /// </summary>
    public void ActivateExtraHole()
    {
        GameObject newHole = Instantiate(emptyHolePrefab, transform);
        ParticleSystem fx = Instantiate(apearFx, transform);
        EmptyHole extraHole = newHole.GetComponent<EmptyHole>();

        // 动态设置位置（可根据需求调整）
        newHole.transform.localPosition = new Vector3(extraHoles.Count + 3, 6.2f, 0);
        fx.transform.position = newHole.transform.position;
        fx.Play();
        Destroy(fx.gameObject, 1.2f);

        extraHoles.Add(extraHole);
        //Debug.Log("激活额外的空槽Buff！");

        // 使用Dotween让父物体移动以保持居中
        transform.DOLocalMoveX(-(extraHoles.Count * 0.75f), 0.35f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// 移除额外的空槽Buff
    /// </summary>
    public void DeactivateExtraHole()
    {
        foreach (EmptyHole hole in extraHoles)
        {
            if (!hole.IsEmpty())
            {
                Destroy(hole.Screw.gameObject); // 清除附加的螺丝
            }
            Destroy(hole.gameObject); // 清除附加的空槽
        }

        extraHoles.Clear();
        Debug.Log("移除额外的空槽Buff！");
    }

    /// <summary>
    /// 获取所有空槽（主列表 + 额外列表）
    /// </summary>
    private List<EmptyHole> GetAllHoles()
    {
        List<EmptyHole> allHoles = new List<EmptyHole>(holeList);
        allHoles.AddRange(extraHoles);
        return allHoles;
    }

    /// <summary>
    /// 只能加2个洞口
    /// </summary>
    /// <returns></returns>
    public bool CanAddExtraHole()
    {
        if (extraHoles.Count < 2)
            return true;

        return false;
    }
}
