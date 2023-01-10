using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using ZPackage.Helper;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class Player : Character
{

    ObjectPool<GameObject> Pool;
    CameraController cameraController;
    SoundManager soundManager;
    UIBar bar;
    URPPP effect;
    int killCount;
    int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    internal void IncreaseKillCount()
    {
        killCount++;
    }

    private void Update()
    {
        if (IsAlive)
        {
            FindNearestEnemy();
        }
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

        Target = nearest;
        // movement.LookTarget = Target;
        if (Target)
        {
            animationController.Throw();
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

    private void OnTriggerEnter(Collider other)
    {
        // Ga collect = other.GetComponent<Collect>();
        // if (collect)
        // {
        //     inventory.AddInventory(collect.gameObject);
        // }
    }
    public override void Die()
    {
        base.Die();
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
}
