using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailBox : MonoBehaviour
{
    [SerializeField] private int layer;
    private Glass box;
    private Transform nails;

    public Glass Box
    {
        get 
        {
            return box;
        }
    }
    public Transform NailTrans
    {
        get
        {
            return nails;
        }
    }


    private void Awake()
    {
        box = transform.Find("Box").GetComponent<Glass>();
        nails = transform.Find("Nails").transform;
    }
    private void Start()
    {
        
   
    }
}
