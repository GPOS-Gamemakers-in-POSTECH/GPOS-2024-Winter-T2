using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class scrt_player : MonoBehaviour
{
    float health = 100f;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "player";
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage) //�������� �Դ� �Լ�
    {
        health -= damage;
        //UnityEngine.Debug.Log(health);
        animator.SetTrigger("trigger_getAttacked");
    }

    public void GetStunned(float time) //���ϴ��ϴ� �Լ�
    {
        animator.SetTrigger("trigger_getStunned");
    }
}
