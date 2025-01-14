using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [SerializeField] private List<Glass> glassList = new List<Glass>();
    public List<Glass> GlassList
    {
        get => glassList;
    }
    [SerializeField] private bool hasConnected;
    public bool HasConnected
    {
        get
        {
            return hasConnected;
        }
    }
    [SerializeField] private bool hasIceCovered;
    public bool HasIceCoverd
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
    private void Awake()
    {
       
    }
    private void Start()
    {

    }
    public void InitGlassList()
    {
        foreach (Transform trans in transform)
        {
            Glass glass = trans.GetComponent<Glass>();
            if (glass != null)
            {
                //≥ı ºªØglass
                glass.InitScrewList();

                glassList.Add(glass);
                if (hasConnected == false && glass.HasConnect)
                {
                    hasConnected = true;
                }

                if (hasIceCovered == false && glass.HasIceCovered)
                {
                    hasIceCovered = true;
                }

                if (hasDoor == false && glass.HasDoor)
                    hasDoor = true;

                if (glass.HasBoom && hasBoom == false)
                    hasBoom = true;

                if (glass.HasChain && hasChain == false)
                    hasChain = true;

                if (glass.HasKey && hasKey == false)
                    hasKey = true;

                if (glass.HasLock && hasLock == false)
                    hasLock = true;
            }
        }
    }
}
