using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using ZPackage;

public class GridController : Mb
{
    [SerializeField] List<Slot> Slots;
    Camera cam;

    RaycastHit hit;
    Vector3 mouseWorldPos;
    Ray ray;
    [SerializeField] Shooter draggingObject;
    [SerializeField] Shooter shooterPF;
    Vector3 dragObjOriginPos, lastDragPos;
    LayerMask roadMask;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        roadMask = LayerMask.GetMask("Road");
    }
    float countDown = 2;
    public bool dragging = false;
    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown < 0 && !dragging)
        {
            countDown = 2;
            InstantiatePF();
        }

        if (IsDown)
        {
            dragging = true;
        }
        else if (IsClick && dragging)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            // Vector3 mousePos = Input.mousePosition;
            // mousePos.z = 6;
            // mouseWorldPos = cam.ScreenToWorldPoint(mousePos);
            if (draggingObject == null && Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Shooter>())
            {
                draggingObject = hit.transform.GetComponent<Shooter>();
                dragObjOriginPos = draggingObject.transform.position;
                draggingObject.GetSlot()?.SetShooter(null);
            }
            else if (draggingObject && Physics.Raycast(ray, out hit, 100, roadMask))
            {
                draggingObject.transform.position = hit.point + Vector3.up;
                lastDragPos = hit.point;
            }
        }
        else if (IsUp)
        {
            if (draggingObject)
            {
                Slot nearestSlot = null;
                float distance = 100;
                foreach (var item in Slots)
                {
                    float dis = Vector3.Distance(lastDragPos, item.transform.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        nearestSlot = item;
                    }
                }
                if (distance < 1 && nearestSlot.shooter == null)
                {
                    nearestSlot.SetShooter(draggingObject);
                }
                else if (distance < 1 && nearestSlot.shooter != null && draggingObject.UpgradeAble() && nearestSlot.shooter.GetModelIndex() == draggingObject.GetModelIndex())
                {
                    nearestSlot.shooter.UpGrade();
                    Destroy(draggingObject.gameObject);
                }
                else
                {
                    draggingObject.GetSlot()?.SetShooter(draggingObject);
                }
                draggingObject = null;
            }

            dragging = false;
        }
    }

    private void InstantiatePF()
    {
        Slot firstFreeSlot = Slots.Where(x => x.shooter == null).FirstOrDefault();
        if (firstFreeSlot)
        {
            Shooter shooter = Instantiate(shooterPF, firstFreeSlot.transform.position, Quaternion.identity);
            firstFreeSlot.SetShooter(shooter);
        }
    }
}
