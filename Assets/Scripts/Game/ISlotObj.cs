using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlotObj
{
    public Slot Slot { get; set; }
    public bool IsUpgradeAble { get;}
    public int ModelIndex { get; set; }
    public Transform transform { get;}
    public GameObject gameObject { get;}
    public void Upgrade();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
