using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrt_mimic : MonoBehaviour
{
    float attack = 20f; //���ݷ�
    float speed = 2f; //�̵��ӷ�
    float detectionRangeX = 20f; //����Ž������
    float detectionRangeY = 5f; //����Ž������
    float attackRange = 4f; //���ݹ���
    int attackDelay = 60; //���ݵ�����
    int attackTime = 20; //�������ӽð�

    int health = 2;

    Transform player;
    SpriteRenderer spriteRenderer;
    GameObject attackObj;
    Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("player").transform;
        gameObject.tag = "enemy";
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void Move()
    {

    }

    void Attack()
    {
        if(health == 2)
        {

        }
        else if(health == 1)
        {

        }
    }

    void Damage(float damage) //�������� �ִ� �Լ�, �׷��� damage���ڴ� ���ϼ��� ���� ���̸� �Ѵ� ������ Exposed, �δ� ������ Dead�� �ǰ� damage�� ���� ����
    {
        health--;

    }

}
