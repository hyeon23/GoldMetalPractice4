using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int stage;
    //public GameObject[] enemyObjs; //type1
    //type2 Object Pulling
    public string[] enemyObjs;
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public TMP_Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public Animator stageAnime;
    public Animator clearAnime;
    public Animator fadeAnime;

    public Transform playerPos;

    private void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    public void StageStart()
    {
        //#.Fade In
        fadeAnime.SetTrigger("In");

        //#.Stage UI Load
        stageAnime.SetTrigger("On");
        stageAnime.GetComponent<TMP_Text>().text = "Stage " + stage + "\nStart";
        clearAnime.GetComponent<TMP_Text>().text = "Stage " + stage + "\nClear";

        //#.Enemy Spawn File Read
        ReadSpawnFile();
    }

    public void StageEnd()
    {
        //#.Fade Out
        fadeAnime.SetTrigger("Out");
        //#.Clear Stage UI
        clearAnime.SetTrigger("On");
        //#.Player Reposition
        player.transform.position = playerPos.position;

        //#.Stage Increment
        stage++;
        if (stage > 2)
            Invoke("GameOver", 6);
        else
            Invoke("StageStart", 5);
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay >= nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            //Ver. Random Spawn
            //nextSpawnDelay = Random.Range(0.5f, 3.0f);//RandomRange()는 이제 안쓴다.
            curSpawnDelay = 0;
        }
        //#UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score); //자리수 단위로 끊음


    }

    void ReadSpawnFile()
    {
        //#1. Initialization
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2. Respawn File Reading
        //-using System.IO;
        //폴더 내 파일 읽어오기
        TextAsset textFile = Resources.Load("Stage" + stage.ToString()) as TextAsset;//Load한 파일이 TextAsset File이 아니면 Null처리 맞으면 처리
        //파일 내 문자열 데이터 읽기 클래스
        StringReader stringReader = new StringReader(textFile.text);
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null)
                break;
            //#리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.enemyType = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //텍스트 파일 닫기
        stringReader.Close();

        //#첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    //#Ver1. Random Spawn
    //void SpawnEnemy()
    //{
    //    int randEnemy = Random.Range(0, 3);
    //    int randPoint = Random.Range(0, 9);
    //    //type2 Object Pulling
    //    //GameObject enemy = objectManager.MakeObj(randEnemy);
    //    GameObject enemy = objectManager.MakeObj(enemyObjs[randEnemy]);
    //    enemy.transform.position = spawnPoints[randPoint].position;
    //    //type1
    //    //Instantiate(enemyObjs[randEnemy], spawnPoints[randPoint].position, spawnPoints[randPoint].rotation);
    //    Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
    //    Enemy enemylogic = enemy.GetComponent<Enemy>();
    //    enemylogic.player = player;
    //    enemylogic.objectManager = objectManager;
    //    if(randPoint == 5 || randPoint == 6)
    //    {
    //        rigid.velocity = new Vector2(enemylogic.speed*(-1), -1);
    //        enemy.transform.Rotate(Vector3.back * 75);
    //    }
    //    else if(randPoint == 7 || randPoint == 8)
    //    {
    //        rigid.velocity = new Vector2(enemylogic.speed, -1);
    //        enemy.transform.Rotate(Vector3.forward * 75);
    //    }
    //    else
    //    {
    //        rigid.velocity = new Vector2(0, enemylogic.speed * (-1));
    //    }
    //}

    //#Ver2. Text File Spawn
    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].enemyType)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        //type1
        //Instantiate(enemyObjs[randEnemy], spawnPoints[randPoint].position, spawnPoints[randPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemylogic = enemy.GetComponent<Enemy>();
        enemylogic.player = player;
        enemylogic.gameManager = this;
        enemylogic.objectManager = objectManager;
        if (enemyPoint == 5 || enemyPoint == 6)
        {
            rigid.velocity = new Vector2(enemylogic.speed * (-1), -1);
            enemy.transform.Rotate(Vector3.back * 75);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            rigid.velocity = new Vector2(enemylogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 75);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemylogic.speed * (-1));
        }
        //#.Respawn Index Increse
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //#.다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void UpdateLifeIcon(int curLife)
    {
        //#UI Life Init Disable
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);//투명화
        }
        //#UI Life Active
        for (int index = 0; index < curLife; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);//투명화
        }
    }
    public void UpdateBoomIcon(int curBoom)
    {
        //#UI Life Init Disable
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);//투명화
        }
        //#UI Life Active
        for (int index = 0; index < curBoom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);//투명화
        }
    }
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;

    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();
        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }
}
