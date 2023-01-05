using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int enemyScore;
    public int hp;
    public bool isDead = false;
    public float speed;
    public string enemyName;
    public Sprite[] sprites;
    
    public GameObject enemyBulletObjA;
    public GameObject enemyBulletObjB;
    public GameObject ItemCoin;
    public GameObject ItemBoom;
    public GameObject ItemPower;
    public GameObject player;
    public ObjectManager objectManager;
    public GameManager gameManager;

    public float maxShotDelay;//진짜 딜레이
    public float curShotDelay;//충전시간 = 공격속도

    SpriteRenderer spriteRenderer;
    Animator anime;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyName == "B")
            anime = GetComponent<Animator>();
    }

    private void OnEnable()//Component가 활성화될 때, 호출
    {
        //Prefab 생성시 hp가 자동으로 초기화되지만, Object Falling으로 생성시, hp가 자동 초기화되지 않음
        //=> 초기화 필요
        switch (enemyName)
        {
            case "B":
                hp = 100;
                Invoke("Stop", 2);
                break;
            case "L":
                hp = 50;
                break;
            case "M":
                hp = 15;
                break;
            case "S":
                hp = 3;
                break;
        }
        isDead = false;
    }

    private void Stop()
    {
        if (!gameObject.activeSelf)
            return;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    private void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }

    }

    private void FireAround()
    {
        if (hp <= 0)
            return;

        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        //#. Fire Arc Continue Fire
        for (int index = 0; index < roundNum; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyC");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));//#원의 둘레, 더 많이 왕복을 원하면 2를 늘리면 됨
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.15f);
        else
            Invoke("Think", 4f);
    }

    private void FireArc()
    {
        if (hp <= 0)
            return;
        //#. Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyD");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Sin(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1f);//#원의 둘레, 더 많이 왕복을 원하면 2를 늘리면 됨
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 4f);
    }

    private void FireShot()
    {
        if (hp <= 0)
            return;
        //#. Fire Random Shotgun Bullet to Player
        for (int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyC");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 randVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += randVec;
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
            
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 0.15f);
        else
            Invoke("Think", 4f);
    }

    private void FireFoward()
    {
        if (hp <= 0)
            return;
        //#Pattern 1
        GameObject bulletLL = objectManager.MakeObj("BulletEnemyD");
        bulletLL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLR = objectManager.MakeObj("BulletEnemyD");
        bulletLR.transform.position = transform.position + Vector3.left * 0.45f;
        GameObject bulletRL = objectManager.MakeObj("BulletEnemyD");
        bulletRL.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletEnemyD");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;

        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLR = bulletLR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRL = bulletRL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();

        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //#Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward", 0.15f);
        else
            Invoke("Think", 4f);
    }

    void Update()
    {
        if (enemyName == "B")
            return;
        Fire();
        Reload();
    }

    //총알 발사
    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;
        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
                //Instantiate(enemyBulletObjA, transform.position, transform.rotation);//첫번재 인자 = Prefabs, 두번째 인자 = 생성위치, 세번째 인자 = 방향
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            //플레이어의 위치를 유도해 발사
            Vector3 dirVec = player.transform.position - transform.position;

            rigid.AddForce(dirVec.normalized * 6, ForceMode2D.Impulse);
        }
        else if(enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
                //Instantiate(enemyBulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);//첫번재 인자 = Prefabs, 두번째 인자 = 생성위치, 세번째 인자 = 방향
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
                //Instantiate(enemyBulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);//첫번재 인자 = Prefabs, 두번째 인자 = 생성위치, 세번째 인자 = 방향

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            //플레이어의 위치를 유도해 발사
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }
        

        curShotDelay = 0;//장전
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;

    }

    public void OnHit(int dmg)
    {
        if (hp <= 0)
            return; 

        hp -= dmg;
        if (enemyName == "B")
        {
            anime.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }
        
        if (hp <= 0)
        {
            isDead = true;
            Player PlayerLogic = player.GetComponent<Player>();
            objectManager.DeleteAllObj("Boss");
            PlayerLogic.score += enemyScore;
            //#.Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            if(ran < 5)//Not Item
            {
                Debug.Log("Not Item");
            }
            else if(ran < 8)//Coin
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
                //Instantiate(ItemCoin, transform.position, ItemCoin.transform.rotation);
            }
            else if(ran < 9)//Power
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
                //Instantiate(ItemPower, transform.position, ItemCoin.transform.rotation);
            }
            else//Boom
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
                //Instantiate(ItemBoom, transform.position, ItemCoin.transform.rotation);
            }
            //Type1
            //Destroy(gameObject);
            //Type2 Object Pulling
            gameObject.SetActive(false);
            CancelInvoke();
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);

            //#.Boss Kill
            if(enemyName == "B")
            {
                gameManager.StageEnd();
            }
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            //Type1
            //Destroy(gameObject);
            //Type2 Object Pulling
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (isDead)
                return;
            OnHit(bullet.dmg);
            //Type1
            //Destroy(collision.gameObject);
            //Type2 Object Pulling
            collision.gameObject.SetActive(false);
        }

    }
}
