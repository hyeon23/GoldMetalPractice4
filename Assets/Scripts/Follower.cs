using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;//��¥ ������
    public float curShotDelay;//�����ð� = ���ݼӵ�
    public float bulletASpeed;
    public ObjectManager objManager;

    public int followDelay;
    public Vector3 followPos;
    public Transform parent;
    public Queue<Vector3> parentPos;//�� �ڷᱸ�� ť ���, list�� ����

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
            followPos = parentPos.Dequeue();//if followDelay = 12 => 12������ ���� Position ���� �������Ƿ�, ���� �ʰ� ������� ȿ�� �߻�
        else if (parentPos.Count < followDelay)//ť�� ä������ ������ �θ� ��ġ ����
            followPos = parent.position;
    }

    //�÷��̾� �̵�
    void Follow()
    {
        transform.position = followPos;
    }



    //�Ѿ� �߻�
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
        curShotDelay = 0;//����
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;

    }
}
