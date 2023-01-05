using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anime;

    private void Awake()
    {
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Invoke("Disable", 2f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    public void StartExplosion(string target)
    {
        anime.SetTrigger("OnExplosion");

        switch (target) {
            case "S":
                transform.localScale = Vector3.one * 0.5f;
                break;
            case "M":
                transform.localScale = Vector3.one * 1f;
                break;
            case "P":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 3f;
                break;
            case "B":
                transform.localScale = Vector3.one * 4f;
                break;
        }
    }
}
