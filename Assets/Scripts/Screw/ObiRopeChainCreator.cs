using UnityEngine;
using Obi;

public class ObiRopeChainCreator : MonoBehaviour
{
    public RopePool ropePool;  // ���������Ķ����
    public RopePool chainPool; // ���������Ķ����

    private ObiRope currentRopeOrChain;

    // ��̬��������������������������
    public ObiRope CreateRopeOrChain(Transform startObject, Transform endObject, bool isChain)
    {
        currentRopeOrChain = isChain ? chainPool.GetObject() : ropePool.GetObject();

        if (currentRopeOrChain == null)
        {
            Debug.LogError("Failed to get object from pool!");
            return null;
        }

        // ��������������
        ConfigureRopeOrChain(currentRopeOrChain, startObject, endObject, isChain);

        return currentRopeOrChain;
    }

    // ͨ�����÷���
    private void ConfigureRopeOrChain(ObiRope rope, Transform startObject, Transform endObject, bool isChain)
    {
        // ж�ؾ���ͼ
        if (rope.ropeBlueprint != null)
        {
            rope.RemoveFromSolver();
            rope.ropeBlueprint = null;
        }

        // ��������ͼ
        ObiRopeBlueprint blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.path.Clear();
        // ��ȡ���������������Ϊ���Ӷ˵㣬������ƫ��

        Vector3 offset = new Vector3(4.3f, -0.6f, 0);

        if (isChain) offset = new Vector3(-1f, -3.1f, -2.3f);

        Vector3 startPoint = startObject.position + offset;
        Vector3 endPoint = endObject.position + offset;

        // ��������Ƶ�
        blueprint.path.AddControlPoint(
            startPoint, Vector3.zero, Vector3.zero, Vector3.zero,
            1, 1, 1, 1, Color.white, "Start"
        );

        // ����յ���Ƶ�
        blueprint.path.AddControlPoint(
            endPoint, Vector3.zero, Vector3.zero, Vector3.zero,
            1, 1, 1, 1, Color.white, "End"
        );

        // ˢ��·����������ͼ
        blueprint.path.FlushEvents();
        blueprint.GenerateImmediate();

        // Ӧ����ͼ����ǰ����
        rope.ropeBlueprint = blueprint;

        // �������������������ģ��
        //if (isChain)
        //{
        //    ObiRopeExtrudedRenderer renderer = rope.GetComponent<ObiRopeExtrudedRenderer>();
        //    if (renderer != null)
        //    {
        //        renderer.section = Resources.Load<ObiRopeSection>("PathToYourChainSection"); // �滻Ϊʵ��������Դ·��
        //    }
        //}

        // ��������ӵ� Solver
        ropePool.solver.AddActor(rope);

        rope.GetComponents<ObiParticleAttachment>()[0].enabled = true; // ���
        rope.GetComponents<ObiParticleAttachment>()[1].enabled = true; // �յ�
        // ��̬������
        AddAttachment(rope, startObject, true);
        AddAttachment(rope, endObject, false);
    }

    // ��Ӱ󶨵�
    private void AddAttachment(ObiRope rope, Transform target, bool isStart)
    {
        var attachment = isStart
            ? rope.GetComponents<ObiParticleAttachment>()[0] // ���
            : rope.GetComponents<ObiParticleAttachment>()[1]; // �յ�

        attachment.target = target;

        var particleGroup = isStart
            ? rope.ropeBlueprint.groups[0] // ���������
            : rope.ropeBlueprint.groups[rope.ropeBlueprint.groups.Count - 1]; // �յ�������

        attachment.particleGroup = particleGroup;
    }

    // �黹���󵽳�
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
