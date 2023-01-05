using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;
    float viewHeight;

    private void Awake()
    {
        //get the Camera height
        viewHeight = Camera.main.orthographicSize * 2;//�ش� ī�޶� ��忡 ���� ������
    }

    void Update()
    {
        //Parallax: �Ÿ��� ���� ����� �ӵ��� Ȱ���� ���ٰ��� �ִ� ���
        //Platformer���� ���� ���
        //����� �� = ����
        //�� �� = ����
        Move();
        Scrolling();
    }

    void Move()
    {
        //Background Scrolling
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1f))
        {
            //Sprite Reuse
            Vector3 backSpritesPos = sprites[startIndex].localPosition;
            Vector3 frontSpritesPos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritesPos + Vector3.up * viewHeight;

            //���� �Ʒ��� ������ġ
            int startIndexSave = startIndex;
            startIndex = endIndex;
            //#Very Important
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
            //startIndexSave�� -1�̸� �������� ���� �ƴϸ� �ڷ� �ʹ޶�� ������ġ�ϴ� ����
        }
    }
}
