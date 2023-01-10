using UnityEngine;
using System;
using TMPro;
using ZPackage;
using Pathfinding;
using System.Collections.Generic;

public class MovementEggRun : Mb
{
    public AnimationController animationController;
    public bool ControlAble;
    public Transform LookTarget;
    [SerializeField] float MaxSpeed = 10;
    [SerializeField] Transform targetPos;
    [SerializeField] Vector3? NoLookTaget;
    [SerializeField] float speed = 0;
    [SerializeField] Seeker seeker;
    Path path;
    List<Vector3> goPos = new List<Vector3>();

    private void Start()
    {
        // seeker = GetComponent<Seeker>();

    }
    void Update()
    {
        if (IsPlaying)
        {
            if (ControlAble)
            {
                ForwardMove();
            }
            else if (targetPos)
            {
                // ForwardMove(targetPos.position);
                ForwardMovePathFind(targetPos.position);
            }
            else if (goPos.Count > 0)
            {
                MoveToPath();
            }
            // else if (NoLookTaget != null)
            // {
            //     MoveNoLook();
            // }
        }
    }

    private void MoveToPath()
    {
        Vector3 dir = (goPos[0] - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
        float distance = Vector3.Distance(goPos[0], transform.position);
        if (distance < CancelDistance)
        {
            goPos.RemoveAt(0);
            if (goPos.Count == 0)
            {
                Cancel();
            }
        }
    }
    public void GotoPath(List<Vector3> path, float speedPercent = 1, Action afterAction = null)
    {
        DestroyCreated();
        goPos = path;
        SetSpeed(speedPercent);
        targetPos = null;
        CancelDistance = 0.1f;
        afterGoAction = afterAction;
    }

    public void SetSpeed(float percent)
    {
        speed = MaxSpeed * percent;
        animationController.SetSpeed(speed / MaxSpeed);
        // animationController.SetXY(0, 1);
        // player.animationController.SetSpeed(speed / MaxSpeed);

        // if (speeTExt)
        // {
        //     speeTExt.text = speed.ToString();
        // }
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetSpeedPerc()
    {
        return speed / MaxSpeed;
    }
    public void GoToPosition(Vector3 _target, bool noLook, float speedPercent = 1, Action afterAction = null)
    {
        DestroyCreated();
        SetSpeed(speedPercent);
        targetPos = null;
        NoLookTaget = _target;
        CancelDistance = 0.1f;
        afterGoAction = afterAction;
    }
    public void GoToPosition(Transform _target, float _cancelDistance = 0.1f, float speedPercent = 1, Action afterAction = null)
    {
        DestroyCreated();
        SetSpeed(speedPercent);
        NoLookTaget = null;
        targetPos = _target;
        CancelDistance = _cancelDistance;
        afterGoAction = afterAction;
        path = seeker.StartPath(transform.position, targetPos.position);
        print(path.vectorPath.Count);
        // Debug.Break();
    }
    public Action afterGoAction;
    public void GoToPosition(Vector3 _target, float _cancelDistance = 0.1f, float speedPer = 1, Action afterAction = null)
    {
        DestroyCreated();
        SetSpeed(speedPer);
        CancelDistance = _cancelDistance;
        GameObject Goto = new GameObject("Goto");
        Goto.transform.position = _target;
        targetPos = Goto.transform;
        afterGoAction = afterAction;
        path = seeker.StartPath(transform.position, targetPos.position);
    }
    public void Cancel(bool cancelAfterInvoke = false)
    {
        DestroyCreated();
        targetPos = null;
        NoLookTaget = null;
        SetSpeed(0);
        if (cancelAfterInvoke)
        {
            afterGoAction = null;
        }
        afterGoAction?.Invoke();
        // animationController.SetSpeed(0);
        animationController.SetXY(0, 0);
        path = null;
        currentWaypoint = 0;
    }

    private void DestroyCreated()
    {
        if (targetPos && targetPos.name == "Goto")
        {
            Destroy(targetPos.gameObject);
        }
    }

    public void SetControlAble(bool value)
    {
        ControlAble = value;
    }

    public void ClampPosition()
    {
        if (transform.position.x <= -10.1f || transform.position.x >= 10.1f)
        {
            var pos = transform.position;
            pos.x = Mathf.Clamp(rb.position.x, -10.1f, 10.1f);
            transform.position = pos;
        }
    }
    bool IsGrounded()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo, 1.05f);
        return hitInfo.collider != null;
    }
    bool IsOnAir()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo, 2f);
        // print(hitInfo.collider == null);
        return hitInfo.collider == null;
    }

    Collider FrontRayCast(float length)
    {
        RaycastHit hitInfo;
        float rayCastHeight = 0.2f;
        for (int i = 0; i < 3; i++)
        {
            Physics.Raycast(transform.position + Vector3.up * rayCastHeight, transform.forward, out hitInfo, length);
            rayCastHeight += 0.7f;
            if (hitInfo.collider)
            {
                return hitInfo.collider;
            }
        }
        return null;
    }
    float dis, radius = 100;
    public void ForwardMove()
    {
        if (IsDown)
            mp = MP;
        if (IsClick)
        {
            dis = Vector3.Distance(MP, mp);
            if (dis > radius)
                mp = Vector3.MoveTowards(MP, mp, radius);
            Vector3 dirLookTarget = (MP - mp).normalized;
            Vector3 WorldDirection = new Vector3(dirLookTarget.x, dirLookTarget.z, dirLookTarget.y);
            if (LookTarget)
            {
                Vector3 targetPos = LookTarget.position;
                targetPos.y = transform.position.y;
                Vector3 targetDirection = (transform.position - targetPos.normalized).normalized;
                float Y = Vector3.Dot(transform.forward, WorldDirection);
                float X = Vector3.Dot(transform.right, WorldDirection);
                animationController.SetXY(X, Y);
                // transform.LookAt(targetPos);
                Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
                rb.MovePosition(rb.position + WorldDirection * speed * Time.deltaTime);
            }
            else
            {
                if (WorldDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(WorldDirection);
                }
                animationController.SetXY(0, 1);
                rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
            }

            animationController.SetSpeed(1);
        }
        if (IsUp)
        {
            rb.velocity = Vector3.zero;
            animationController.SetSpeed(0);
        }
    }

    float CancelDistance = 0.1f;
    public void ForwardMove(Vector3 _targetPos)
    {
        animationController.SetSpeed(1);
        _targetPos.y = 0;
        // Vector3 lookTaget = _targetPos;
        // lookTaget.y = 0;
        // transform.LookAt(lookTaget);
        Quaternion targetRotation = Quaternion.LookRotation(_targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f * Time.deltaTime);

        // Vector3 forwardMove = Vector3.forward * Speed * Time.fixedDeltaTime;
        // transform.position += transform.forward * speed * Time.fixedDeltaTime;
        // Vector3 forwardMove = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        // rb.position += forwardMove;
        float distance = Vector3.Distance(transform.position, _targetPos);
        if (distance < CancelDistance)
        {
            Cancel();
        }

        // ClampPosition();
    }
    int currentWaypoint = 0;
    float distanceToWaypoint;
    float renewPathTime = 2;
    public void ForwardMovePathFind(Vector3 _targetPos)
    {

        if (path != null && path.vectorPath.Count > 0)
        {

            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            // if (path.vectorPath.Count <= currentWaypoint + 1)
            // {
            //     print(currentWaypoint);
            //     print(path.vectorPath.Count);
            //     Debug.Break();
            // }
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < 0.2f)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    Cancel();
                }
            }

            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
            animationController.SetSpeed(1);
            // float distance = Vector3.Distance(transform.position, _targetPos);
            // if (distance < CancelDistance)
            // {
            //     Cancel();
            // }
            renewPathTime -= Time.deltaTime;
            if (renewPathTime < 0)
            {
                renewPathTime = 2;
                path = seeker.StartPath(transform.position, targetPos.position);
                currentWaypoint = 0;
            }
        }
        // ClampPosition();
    }
    private void MoveNoLook()
    {
        // transform.position += transform.forward * speed * Time.fixedDeltaTime;
        rb.position = Vector3.MoveTowards(rb.position, NoLookTaget.Value, speed * Time.fixedDeltaTime);
        float distance = Vector3.Distance(transform.position, NoLookTaget.Value);
        if (distance < CancelDistance)
        {
            Cancel();
        }
    }
}
