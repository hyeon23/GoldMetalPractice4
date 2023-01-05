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
        viewHeight = Camera.main.orthographicSize * 2;//해당 카메라 모드에 대한 사이즈
    }

    void Update()
    {
        //Parallax: 거리에 따른 상대적 속도를 활용해 원근감을 주는 기법
        //Platformer에서 많이 사용
        //가까운 것 = 빠름
        //먼 것 = 느림
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

            //위와 아래의 바통터치
            int startIndexSave = startIndex;
            startIndex = endIndex;
            //#Very Important
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
            //startIndexSave가 -1이면 끝점으로 오고 아니면 뒤로 와달라고 바통터치하는 문장
        }
    }
}
