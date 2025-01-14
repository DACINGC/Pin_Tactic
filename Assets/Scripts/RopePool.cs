using UnityEngine;
using Obi;
using System.Collections.Generic;

public class RopePool : MonoBehaviour
{
    public ObiRope prefab; // Rope 或 Chain 的预制体
    public int poolSize = 4; // 对象池大小

    public ObiSolver solver; // 关联的 Solver 对象
    private Queue<ObiRope> pool = new Queue<ObiRope>();

    private void Awake()
    {
        solver = GetComponent<ObiSolver>();
    }

    void Start()
    {
        InitializePool();
    }

    // 初始化对象池
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ObiRope newObject = Instantiate(prefab, solver.transform);
            newObject.gameObject.SetActive(false);
            pool.Enqueue(newObject);
        }
    }

    // 从对象池中获取对象
    public ObiRope GetObject()
    {
        if (pool.Count > 0)
        {
            ObiRope obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // 池为空时动态创建新的对象
            ObiRope newObject = Instantiate(prefab, solver.transform);
            return newObject;
        }
    }

    // 将对象归还到对象池
    public void ReturnObject(ObiRope obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
