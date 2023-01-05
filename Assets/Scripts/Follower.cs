using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;//진짜 딜레이
    public float curShotDelay;//충전시간 = 공격속도
    public float bulletASpeed;
    public ObjectManager objManager;

    public int followDelay;
    public Vector3 followPos;
    public Transform parent;
    public Queue<Vector3> parentPos;//새 자료구조 큐 사용, list와 유사

    private void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();
        Follow();//=Move
        Fire();
        Reload();
    }

    private void Watch()
    {
        //#Input Position
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);
        //#Output Position
        if (parentPos.Count > followDelay)//Giving Delay
            followPos = parentPos.Dequeue();//if followDelay = 12 => 12프레임 앞의 Position 값을 가져오므로, 조금 늦게 따라오는 효과 발생
        else if (parentPos.Count < followDelay)//큐가 채워지기 전까진 부모 위치 적용
            followPos = parent.position;
    }

    //플레이어 이동
    void Follow()
    {
        transform.position = followPos;
    }



    //총알 발사
    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * bulletASpeed, ForceMode2D.Impulse);
        curShotDelay = 0;//장전
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;

    }
}
