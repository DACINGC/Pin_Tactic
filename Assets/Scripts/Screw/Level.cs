using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private List<Layer> layerList = new List<Layer>();

    [SerializeField] private bool hasIceCovered;
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
    [SerializeField] private bool hasKey;
    public bool HasKey { get => hasKey; }
    [SerializeField] private bool hasLock;
    public bool HasLock { get => hasLock; }

    [Header("倒计时")]
    [SerializeField] private bool hasClock;
    public bool HasClock { get => hasClock; }
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;

    [Header("是否是困难")]
    [SerializeField] private bool isHard;
    public bool IsHard { get => isHard; }
    public int GetMinutes { get => minutes; }
    public int GetSeconds { get => seconds; }

    public List<Layer> LayerList
    {
        get => layerList;
    }
    
    private void Start()
    {
        InitLayerList();
    }

    private void InitLayerList()
    {
        foreach (Transform tran in transform)
        {
            Layer curLayer = tran.GetComponent<Layer>();

            if (curLayer != null)
            {
                //初始化layer
                curLayer.InitGlassList();

                layerList.Add(tran.GetComponent<Layer>());

                if (curLayer.HasIceCoverd && hasIceCovered == false)
                   hasIceCovered = true;

                if (curLayer.HasDoor && hasDoor == false)
                    hasDoor = true;

                if (curLayer.HasBoom && hasBoom == false)
                    hasBoom = true;

                if (curLayer.HasChain && hasChain == false)
                    hasChain = true;

                if (curLayer.HasKey && hasKey == false)
                    hasKey = true;

                if (curLayer.HasLock && hasLock == false)
                    hasLock = true;
            }
        }
    }
}
