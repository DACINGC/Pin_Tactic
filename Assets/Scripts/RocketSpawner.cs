using UnityEngine;
using DG.Tweening;

public class RocketSpawner : MonoBehaviour
{
    
    [SerializeField] private Transform spawnPoint;  // ������ɵ�
    [SerializeField] private GameObject rocketPrefab;  // ���Ԥ����
    [SerializeField] private float duration = 2f;  // ����ƶ�ʱ��
    [SerializeField] private float controlPointOffsetX = -5f;  // ���Ƶ�����ƫ��
    [SerializeField] private float controlPointOffsetY = 5f;  // ���Ƶ�����ƫ��
    [SerializeField] private int pathSteps = 50;  // ���������ߵ�ķֱ���

    [Space]
    [SerializeField] private GameObject explosionPrefab;

    private float timer;
    /// <summary>
    /// ���ɻ�������ű����������ƶ�
    /// </summary>
    /// <param name="targetPoint">Ŀ���</param>
    public void SpawnRocket(Vector3 targetPoint, Glass glass)
    {
        // ȷ�� Z ����̶�Ϊ -20
        Vector3 initPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, -20);
        targetPoint.z = -20;

        // ���Ƶ��������յ�֮�䣬����ƫ�Ʋ�̧��
        Vector3 controlPoint = new Vector3(
            (initPos.x + targetPoint.x) / 2 + controlPointOffsetX,  // ����ƫ��
            Mathf.Max(initPos.y, targetPoint.y) + controlPointOffsetY,  // ����̧��
            -(initPos.z + targetPoint.z) / 2);

        // ���ɻ��
        GameObject rocket = Instantiate(rocketPrefab, initPos, Quaternion.Euler(0, 0, 0));

        // ���ɱ���������·��
        Vector3[] path = GenerateBezierPath(initPos, controlPoint, targetPoint);

        // DOTween·������
        rocket.transform.DOPath(path, duration, PathType.Linear)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                GameObject explosionObj = Instantiate(explosionPrefab, glass.transform.position, Quaternion.identity);
                Destroy(explosionObj, 1f);
                glass.SetFalse();
                Destroy(rocket.gameObject);
            });  // �������������ٻ��
    }

    /// <summary>
    /// ���ɱ���������·��
    /// </summary>
    /// <param name="startPoint">���</param>
    /// <param name="controlPoint">���Ƶ�</param>
    /// <param name="endPoint">�յ�</param>
    /// <returns>·��������</returns>
    private Vector3[] GenerateBezierPath(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
    {
        Vector3[] path = new Vector3[pathSteps];
        for (int i = 0; i < pathSteps; i++)
        {
            float t = i / (float)(pathSteps - 1);
            path[i] = GetBezierPoint(startPoint, controlPoint, endPoint, t);
        }
        return path;
    }

    /// <summary>
    /// ���㱴�������ߵ�ĳһ��
    /// </summary>
    private Vector3 GetBezierPoint(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, float t)
    {
        float u = 1 - t;
        return u * u * startPoint + 2 * u * t * controlPoint + t * t * endPoint;
    }
    /// <summary>
    /// ���»���ĳ���ʹ��ͷ��·�������߷���
    /// </summary>
    /// <param name="rocket">�������</param>
    /// <param name="path">·��������</param>
}
