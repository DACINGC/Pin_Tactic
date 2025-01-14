using UnityEngine;
using Obi;

public class ObiRopeChainCreator : MonoBehaviour
{
    public RopePool ropePool;  // 管理绳索的对象池
    public RopePool chainPool; // 管理锁链的对象池

    private ObiRope currentRopeOrChain;

    // 动态创建绳索或锁链并链接两个点
    public ObiRope CreateRopeOrChain(Transform startObject, Transform endObject, bool isChain)
    {
        currentRopeOrChain = isChain ? chainPool.GetObject() : ropePool.GetObject();

        if (currentRopeOrChain == null)
        {
            Debug.LogError("Failed to get object from pool!");
            return null;
        }

        // 配置绳索或锁链
        ConfigureRopeOrChain(currentRopeOrChain, startObject, endObject, isChain);

        return currentRopeOrChain;
    }

    // 通用配置方法
    private void ConfigureRopeOrChain(ObiRope rope, Transform startObject, Transform endObject, bool isChain)
    {
        // 卸载旧蓝图
        if (rope.ropeBlueprint != null)
        {
            rope.RemoveFromSolver();
            rope.ropeBlueprint = null;
        }

        // 创建新蓝图
        ObiRopeBlueprint blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.path.Clear();
        // 获取物体的世界坐标作为绳子端点，并加上偏移

        Vector3 offset = new Vector3(4.3f, -0.6f, 0);

        if (isChain) offset = new Vector3(-1f, -3.1f, -2.3f);

        Vector3 startPoint = startObject.position + offset;
        Vector3 endPoint = endObject.position + offset;

        // 添加起点控制点
        blueprint.path.AddControlPoint(
            startPoint, Vector3.zero, Vector3.zero, Vector3.zero,
            1, 1, 1, 1, Color.white, "Start"
        );

        // 添加终点控制点
        blueprint.path.AddControlPoint(
            endPoint, Vector3.zero, Vector3.zero, Vector3.zero,
            1, 1, 1, 1, Color.white, "End"
        );

        // 刷新路径并生成蓝图
        blueprint.path.FlushEvents();
        blueprint.GenerateImmediate();

        // 应用蓝图到当前对象
        rope.ropeBlueprint = blueprint;

        // 如果是锁链，设置链节模型
        //if (isChain)
        //{
        //    ObiRopeExtrudedRenderer renderer = rope.GetComponent<ObiRopeExtrudedRenderer>();
        //    if (renderer != null)
        //    {
        //        renderer.section = Resources.Load<ObiRopeSection>("PathToYourChainSection"); // 替换为实际链节资源路径
        //    }
        //}

        // 将对象添加到 Solver
        ropePool.solver.AddActor(rope);

        rope.GetComponents<ObiParticleAttachment>()[0].enabled = true; // 起点
        rope.GetComponents<ObiParticleAttachment>()[1].enabled = true; // 终点
        // 动态绑定两端
        AddAttachment(rope, startObject, true);
        AddAttachment(rope, endObject, false);
    }

    // 添加绑定点
    private void AddAttachment(ObiRope rope, Transform target, bool isStart)
    {
        var attachment = isStart
            ? rope.GetComponents<ObiParticleAttachment>()[0] // 起点
            : rope.GetComponents<ObiParticleAttachment>()[1]; // 终点

        attachment.target = target;

        var particleGroup = isStart
            ? rope.ropeBlueprint.groups[0] // 起点粒子组
            : rope.ropeBlueprint.groups[rope.ropeBlueprint.groups.Count - 1]; // 终点粒子组

        attachment.particleGroup = particleGroup;
    }

    // 归还对象到池
    public void ReturnRopeOrChain(ObiRope rope)
    {
        if (rope != null)
        {
            ropePool.solver.RemoveActor(rope);

            if(rope.GetComponent<ObiRopeChainRenderer>() != null)
                chainPool.ReturnObject(rope);
            else
                ropePool.ReturnObject(rope);                
        }
    }
}
