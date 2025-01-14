using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHole : MonoBehaviour
{
    [SerializeField] private Screw screw;

    public Screw Screw
    {
        get
        {
            return screw;
        }
        set
        {
            screw = value;
        }
    }
    /// <summary>
    /// ��鵱ǰ���Ƿ������
    /// </summary>
    public bool IsEmpty()
    {
        return screw == null;// && transform.childCount == 0;
    }

}
