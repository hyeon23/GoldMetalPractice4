using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //#Object Pulling
    //Instantiate or Destroy�� �ܿ� �޸𸮰� �߻��ϴµ� �� ���� ��ġ�� �׿��� GC(Garbage Collection)�� �߻� ��, ���� ���� �ɸ�
    //�̸� �����ϱ� ���� ���� Object Pulling
    //�̸� ������ pull���� ������Ʈ�� Ȱ��ȭ/��Ȱ��ȭ�� ����
    //���ӵ��� ���� ����ǰų� ó�� ������ ��, �ε��ϴ� ����� �ʿ��� ������ �� ��� �͵��� Instantiate�� Object Pull�� �����ϱ� ����

    //1. Pull ����(Create Empty -> Object Manager)
    //public ���X -> Inspector â�� ���̰� �Ǿ� �Ϲ������� ����

    //2. Arrange all of Prefabs

    public GameObject enemyBPrefab;
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletEnemyCPrefab;
    public GameObject bulletEnemyDPrefab;
    public GameObject bulletFollowerPrefab;
    public GameObject explosionPrefab;

    GameObject[] enemyB;
    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;

    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletEnemyC;
    GameObject[] bulletEnemyD;
    GameObject[] bulletFollower;

    GameObject[] explosion;

    GameObject[] targetPool;

    //3. Initialization
    //-�ѹ��� ������ ������ ����� �迭 ���� �Ҵ�
    private void Awake()
    {
        enemyB = new GameObject[3];
        enemyL = new GameObject[10];
        enemyM = new GameObject[10];
        enemyS = new GameObject[20];

        itemCoin = new GameObject[20];
        itemPower = new GameObject[20];
        itemBoom = new GameObject[20];

        bulletPlayerA = new GameObject[100];
        bulletPlayerB = new GameObject[100];
        bulletEnemyA = new GameObject[200];
        bulletEnemyB = new GameObject[200];
        bulletEnemyC = new GameObject[200];
        bulletEnemyD = new GameObject[200];
        bulletFollower = new GameObject[200];
        explosion = new GameObject[200];
        Generate();
    }

    private void Generate()
    {
        //Instantiate ��� -> frefabs �غ�
        //position ���� �ʿ�X

        //#Enemies
        for (int index = 0; index < enemyB.Length; index++)
        {
            enemyB[index] = Instantiate(enemyBPrefab);
            enemyB[index].SetActive(false);
        }
        for (int index = 0; index < enemyL.Length; index++)
        {   
            enemyL[index] = Instantiate(enemyLPrefab);
            enemyL[index].SetActive(false);
        }
        for (int index = 0; index < enemyM.Length; index++)
        { 
            enemyM[index] = Instantiate(enemyMPrefab);
            enemyM[index].SetActive(false);
        }
        for (int index = 0; index < enemyS.Length; index++)
        { 
            enemyS[index] = Instantiate(enemySPrefab);
            enemyS[index].SetActive(false);
        }
        //Items
        for (int index = 0; index < itemCoin.Length; index++)
        { 
            itemCoin[index] = Instantiate(itemCoinPrefab);
            itemCoin[index].SetActive(false);
        }
        for (int index = 0; index < itemPower.Length; index++)
        { 
            itemPower[index] = Instantiate(itemPowerPrefab);
            itemPower[index].SetActive(false);
        }
        for (int index = 0; index < itemBoom.Length; index++)
        { 
            itemBoom[index] = Instantiate(itemBoomPrefab);
            itemBoom[index].SetActive(false);
        }
        //PlayerBullets
        for (int index = 0; index < bulletPlayerA.Length; index++)
        { 
            bulletPlayerA[index] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[index].SetActive(false);
        }
        for (int index = 0; index < bulletPlayerB.Length; index++)
        {
            bulletPlayerB[index] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[index].SetActive(false);
        }
        //EnemyBullets
        for (int index = 0; index < bulletEnemyA.Length; index++)
        {
            bulletEnemyA[index] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[index].SetActive(false);
        }
        for (int index = 0; index < bulletEnemyB.Length; index++)
        {
            bulletEnemyB[index] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[index].SetActive(false);
        }
        for (int index = 0; index < bulletEnemyC.Length; index++)
        {
            bulletEnemyC[index] = Instantiate(bulletEnemyCPrefab);
            bulletEnemyC[index].SetActive(false);
        }
        for (int index = 0; index < bulletEnemyD.Length; index++)
        {
            bulletEnemyD[index] = Instantiate(bulletEnemyDPrefab);
            bulletEnemyD[index].SetActive(false);
        }
        //#follower
        for (int index = 0; index < bulletFollower.Length; index++)
        {
            bulletFollower[index] = Instantiate(bulletFollowerPrefab);
            bulletFollower[index].SetActive(false);
        }
        //#explosion
        for (int index = 0; index < explosion.Length; index++)
        {
            explosion[index] = Instantiate(explosionPrefab);
            explosion[index].SetActive(false);
        }
    }
    
    public GameObject MakeObj(string type)
    {
        GetPool(type);
        for (int index = 0; index < targetPool.Length; index++)
        {
            if (!targetPool[index].activeSelf)//��Ȱ��ȭ�� ������Ʈ�� ������ Ȱ��ȭ �� ��ȯ
            {
                //���� ��� �װ����Կ� �������� �Ҷ�� ������, �ϳ��� ������ �� Ȱ��ȭ������ ���� ��������, Ȱ��ȭ�� ǥ���� ��ȯ�ϴ� ��
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }
        //���ٸ� null
        return null;
    }

    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;

            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;

            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;

            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletEnemyC":
                targetPool = bulletEnemyC;
                break;
            case "BulletEnemyD":
                targetPool = bulletEnemyD;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
            case "Explosion":
                targetPool = explosion;
                break;
        }
        return targetPool;
    }

    public void DeleteAllObj(string type)
    {
        if (type == "Boss")
        {
            for (int index = 0; index < bulletEnemyC.Length; index++)
                bulletEnemyC[index].SetActive(false);

            for (int index = 0; index < bulletEnemyD.Length; index++)
                bulletEnemyD[index].SetActive(false);
        }
    }
}
