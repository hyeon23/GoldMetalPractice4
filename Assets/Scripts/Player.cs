using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life = 3;
    public int score;
    public float speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;
    public float maxShotDelay;//진짜 딜레이
    public float curShotDelay;//충전시간 = 공격속도
    public float bulletASpeed;
    public bool isHit;
    public bool isBoomTime;
    public bool isCollisionTop;
    public bool isCollisionLeft;
    public bool isCollisionRight;
    public bool isCollisionButtom;
    public Animator anime;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;
    public GameManager gameManager;
    public ObjectManager objManager;

    public GameObject[] followers;
    public bool isRespawnTime;
    SpriteRenderer spriteRenderer;

    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    private void Awake()
    {
        anime = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    private void Boom()
    {
        //#Ver1. Desktop Play
        //if (!Input.GetButton("Fire2"))
        //    return;

        //#Ver2. Mobile Play
        if (!isButtonB)
            return;

        if (isBoomTime)
            return;
        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);
        //Effect Plus
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);

        //Damaged to All of Enemy
        //Find도 성능 이슈 존재 -> overload
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] enemiesL = objManager.GetPool("EnemyL");
        GameObject[] enemiesM = objManager.GetPool("EnemyM");
        GameObject[] enemiesS = objManager.GetPool("EnemyS");
        //for (int index = 0; index < enemies.Length; index++)
        //{
        //    Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
        //    enemyLogic.OnHit(1000);
        //}
        //Ver. Object Falling
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        ////Remove Enemies Bullets
        //GameObject[] enemiesBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        //for (int index = 0; index < enemiesBullets.Length; index++)
        //{
        //    Destroy(enemiesBullets[index]);
        //}

        //Ver.Object Falling
        //Remove Enemies Bullets
        GameObject[] enemiesBulletsA = objManager.GetPool("BulletEnemyA");
        GameObject[] enemiesBulletsB = objManager.GetPool("BulletEnemyB");
        for (int index = 0; index < enemiesBulletsA.Length; index++)
        {
            if (enemiesBulletsA[index].activeSelf)
            {
                enemiesBulletsA[index].SetActive(false);
            }
        }
        for (int index = 0; index < enemiesBulletsB.Length; index++)
        {
            if (enemiesBulletsB[index].activeSelf)
            {
                enemiesBulletsB[index].SetActive(false);
            }
        }

    }

    public void JoyPanel(int type)
    {
        for(int index = 0; index < 9; index++)
        {
            joyControl[index] = (index == type);
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }

    public void JoyUp()
    {
        isControl = false;
    }

    //플레이어 이동
    void Move()
    {
        //#Ver2. Mobile JoyStick Control
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (joyControl[0]) { h = -1; v = 1; }//#좌측 상단
        if (joyControl[1]) { h = 0; v = 1; }//#상단
        if (joyControl[2]) { h = 1; v = 1; }//#우측 상단
        if (joyControl[3]) { h = -1; v = 0; }//#좌측
        if (joyControl[4]) { h = 0; v = 0; }//#중심
        if (joyControl[5]) { h = 1; v = 0; }//#우측
        if (joyControl[6]) { h = -1; v = -1; }//#좌측 하단
        if (joyControl[7]) { h = 0; v = -1; }//#하단
        if (joyControl[8]) { h = 1; v = -1; }//#우측 하단

        if ((isCollisionLeft && h == -1) || (isCollisionRight && h == 1) || !isControl)
            h = 0;
        if ((isCollisionButtom && v == -1) || (isCollisionTop && v == 1) || !isControl)
            v = 0;
        Vector3 curPos = transform.position;//소문자 transform은 MonoBehaviour에 달려있는 변수이다.
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        //물리적 이동이 아닌 transform 이동은 Time.deltaTime 곱해줘야 함

        transform.position = curPos + nextPos;

        //if(h == 1)
        //{
        //    anime.SetInteger("Input", 1);
        //}
        //else if(h == 0)
        //{
        //    anime.SetInteger("Input", 0);
        //}
        //else if(h == -1)
        //{
        //    anime.SetInteger("Input", -1);
        //}

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anime.SetInteger("Input", (int)h);
        }


        //#Ver1. Desktop Control
        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        //if ((isCollisionLeft && h == -1) || (isCollisionRight && h == 1))
        //    h = 0;
        //if ((isCollisionButtom && v == -1) || (isCollisionTop && v == 1))
        //    v = 0;
        //Vector3 curPos = transform.position;//소문자 transform은 MonoBehaviour에 달려있는 변수이다.
        //Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        ////물리적 이동이 아닌 transform 이동은 Time.deltaTime 곱해줘야 함

        //transform.position = curPos + nextPos;

        ////if(h == 1)
        ////{
        ////    anime.SetInteger("Input", 1);
        ////}
        ////else if(h == 0)
        ////{
        ////    anime.SetInteger("Input", 0);
        ////}
        ////else if(h == -1)
        ////{
        ////    anime.SetInteger("Input", -1);
        ////}

        //if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        //{
        //    anime.SetInteger("Input", (int)h);
        //}
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }

    //총알 발사
    void Fire()
    {
        //#Ver1. Desktop Play
        //if (!Input.GetButton("Fire1"))
        //    return;

        //#Ver2. JoyStick Play
        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = objManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                    //Instantiate(bulletObjA, transform.position, transform.rotation);//첫번재 인자 = Prefabs, 두번째 인자 = 생성위치, 세번째 인자 = 방향
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletL = objManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                    //Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);
                GameObject bulletR = objManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                //Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                rigidR.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                break;
            default://power 3 ...
                GameObject bulletLL = objManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.25f;
                    //Instantiate(bulletObjA, transform.position + Vector3.left * 0.25f, transform.rotation);
                GameObject bulletCC = objManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position + Vector3.up * 0.1f;
                //Instantiate(bulletObjB, transform.position + Vector3.up * 0.1f, transform.rotation);
                GameObject bulletRR = objManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.25f;
                //Instantiate(bulletObjA, transform.position + Vector3.right * 0.25f, transform.rotation);
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                rigidRR.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
                break;
        }

        //Power One
        

        curShotDelay = 0;//장전
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isCollisionTop = true;
                    break;
                case "Left":
                    isCollisionLeft = true;
                    break;
                case "Right":
                    isCollisionRight = true;
                    break;
                case "Buttom":
                    isCollisionButtom = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet"){
            if (isRespawnTime)
                return;
            if (isHit)
                return;
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");
            if (life == 0){
                gameManager.GameOver();
            }
            else { 
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Boom"://HighLight
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    ////Effect Plus
                    //boomEffect.SetActive(true);
                    //Invoke("OffBoomEffect", 3f);
                    ////Damaged to All of Enemy
                    //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    //for (int index = 0; index < enemies.Length; index++)
                    //{
                    //    Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
                    //    enemyLogic.OnHit(1000);
                    //}
                    ////Remove Enemies Bullets
                    //GameObject[] enemiesBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
                    //for (int index = 0; index < enemiesBullets.Length; index++)
                    //{
                    //    Destroy(enemiesBullets[index]);
                    //}
                    break;
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if(power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }break;
            }
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    private void AddFollower()
    {
        if(power == 4)
            followers[0].SetActive(true);
        else if(power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isCollisionTop = false;
                    break;
                case "Left":
                    isCollisionLeft = false;
                    break;
                case "Right":
                    isCollisionRight = false;
                    break;
                case "Buttom":
                    isCollisionButtom = false;
                    break;
            }
        }
    }
}

