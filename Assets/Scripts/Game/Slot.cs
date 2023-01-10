using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Shooter shooter;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetShooter(Shooter incoShooter)
    {
        shooter = incoShooter;
        if (incoShooter)
        {
            shooter.SetSlot(this);
            shooter.transform.position = transform.position;
        }
    }

}
