using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using ZPackage.Helper;
using Random = UnityEngine.Random;
using UnityEngine.Pool;
using ZPackage.Utility;
using System.Linq;

public class Player : Mb
{
    List<Vector3> localPoints = new List<Vector3>();
    public List<Node> Nodes;
    public Transform nodesParent;
    MovementForgeRun movement;
    ObjectPool<GameObject> Pool;
    CameraController cameraController;
    SoundManager soundManager;
    UIBar bar;
    URPPP effect;
    int killCount;
    public int pointCount = 2;
    List<Vector2> points = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        points = PoissonDiscSampling.GeneratePoints(0.6f, new Vector2(4, 4));
        Vector2 CenterOffset = FindNearestCenterOffset(points);
        print(CenterOffset);
        for (int i = 0; i < points.Count; i++)
        {
            points[i] += new Vector2(-2, -2) + CenterOffset;
            localPoints.Add(new Vector3(points[i].x, 0, points[i].y));
        }
        localPoints.OrderBy(x => Vector3.Distance(x, Vector3.zero));

        print(localPoints[0]);
        movement = GetComponent<MovementForgeRun>();
        // animationController.OnSpearShoot += SpearShoot;
        soundManager = FindObjectOfType<SoundManager>();
        cameraController = FindObjectOfType<CameraController>();
        // line.positionCount = LineResolution;
        effect = FindObjectOfType<URPPP>();
        GameManager.Instance.GameOverEvent += OnGameOver;
        GameManager.Instance.GamePlay += OnGamePlay;
        GameManager.Instance.LevelCompleted += OnGameOver;
        InitPool();
        GameManager.Instance.Coin = 10;
        Nodes[0].GotoLocalPos(localPoints[0]);
    }
    public Vector2 FindNearestCenterOffset(List<Vector2> ToFindPoints)
    {
        Vector2 nearestPoint = ToFindPoints[0];
        Vector2 center = new Vector2(2,2);
        float nearestDistance = Vector2.Distance(center, nearestPoint);

        foreach (Vector2 point in ToFindPoints)
        {
            float distance = Vector2.Distance(center, point);
            if (distance < nearestDistance)
            {
                nearestPoint = point;
                nearestDistance = distance;
            }
        }

        return center -  nearestPoint;
    }
    public void AddNode(Node node)
    {
        if (!Nodes.Contains(node))
        {
            Nodes.Add(node);
            node.transform.SetParent(nodesParent);
            node.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = Color.green;
            node.transform.localRotation = Quaternion.identity;
            Destroy(node.GetComponent<Rigidbody>());
            node.GotoLocalPos(localPoints[Nodes.Count]);
            movement.SetSpeed(1);
        }
    }
    internal void RemoveNode(Node node)
    {
        if (Nodes.Contains(node))
        {
            Nodes.Remove(node);
            node.transform.SetParent(null);
            Rigidbody rb = node.gameObject.AddComponent<Rigidbody>();
            node.gameObject.layer = 3;
            // rb.velocity = Vector3.zero;
        }
    }
    internal void RemoveAllNode()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].transform.SetParent(null);
            Rigidbody rbNode = Nodes[i].gameObject.AddComponent<Rigidbody>();
            rbNode.isKinematic = true;
            Nodes[i].gameObject.layer = 3;
        }
        Nodes.Clear();
    }
    private void OnCollisionEnter(Collision other)
    {
        Node node = other.gameObject.GetComponent<Node>();
        if (node)
        {
            AddNode(node);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Board"))
        {
            movement.SetSpeed(0);
            movement.SetControlAble(false);
            rb.isKinematic = true;
            GridController controller = other.GetComponent<GridController>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                controller.Slots[i].SetShooter(Nodes[i]);
            }
            RemoveAllNode();
        }
    }

    internal void IncreaseKillCount()
    {
        killCount++;
    }

    private void Update()
    {
    }
    [SerializeField] Transform Target = null;

    private void FindNearestEnemy()
    {
        float shortest = 100;
        Transform nearest = null;
        foreach (var item in Physics.OverlapSphere(transform.position, 10, 1 << 3))
        {
            float Distance = Vector3.Distance(transform.position, item.transform.position);
            if (shortest > Distance)
            {
                nearest = item.transform;
                shortest = Distance;
            }
        }
    }

    private void InitPool()
    {
        Pool = new ObjectPool<GameObject>(() =>
        {
            // GameObject spear = Instantiate(Spear, Vector3.zero, Spear.transform.rotation);
            // spear.SetPool(Pool);
            // return spear;
            return new GameObject();
        }, (s) =>
        {
            // s.transform.position = Spear.transform.position;
            // // s.transform.rotation = Spear.transform.rotation;
            // if (Target)
            // {
            //     s.Throw(Target);
            // }
            // else
            // {
            //     s.Throw(transform.forward);
            // }
        }, (s) =>
        {
            //release
            // s.GotoPool();
        });
    }
    public void Die()
    {
        GameManager.Instance.GameOver(this, EventArgs.Empty);
    }

    private void OnGamePlay(object sender, EventArgs e)
    {
        movement.SetSpeed(1);
        movement.SetControlAble(true);
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        // throw new NotImplementedException();
    }
    private void OnDrawGizmos()
    {
        if (localPoints.Count > 0 && localPoints.Count > pointCount)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Gizmos.DrawSphere(localPoints[i], 0.5f);
            }
        }
    }


}
