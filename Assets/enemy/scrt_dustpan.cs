using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class scrt_dustpan : MonoBehaviour
{
    //�ɷ�ġ
    float attack = 20f; //���ݷ�
    float health = 100f; //ü��
    float speed = 2f; //�̵��ӷ�
    float detectionRange = 20f; //Ž������
    float attackRange = 4f; //���ݹ���
    int attackDelay = 60; //���ݵ�����
    int attackTime = 20; //�������ӽð�

    int state = 0; //0: normal, 1: alert, 2: stunned, 3: dead
    int delay = 0;
    int direction = 0;
    float distance = 0f;

    Transform player;
    SpriteRenderer spriteRenderer;
    GameObject attackObj;
    public GameObject enemyAttack; //dustpanAttack ��ũ��Ʈ�� �� ������ �����ؾ���

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("player").transform;
        gameObject.tag = "enemy";
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) { state = 3; }
        delay--;

        switch(state)
        {
            case 0: 
                Move();
                if (delay <= 0) //���� �ֱ⸶�� �����̴� ���� ����
                {
                    delay = UnityEngine.Random.Range(120, 601);
                    direction = UnityEngine.Random.Range(-1, 2);
                }
                break;
            case 1: Move(); Attack(); break;
            case 2:
                if (delay <= 0)
                {
                    state = 0;
                }
                    break;
            case 3: break;
        }

        
    }

    void Move()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);
        if (direction != 0) { spriteRenderer.flipX = (direction == 1); }
        
        distance=Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange && Math.Abs(player.transform.position.y-this.transform.position.y)<detectionRange/2) {
            if (state != 1) { delay = 0; }
            state = 1; 
        }
        else { state = 0; }
    }

    void Attack()
    {
        if (player.transform.position.x - this.transform.position.x < -1*attackRange/4) { direction = -1; }
        else if (player.transform.position.x - this.transform.position.x > attackRange/4) { direction = 1; }
        else { direction = 0; }

        if (distance < attackRange && delay <= 0)
        {
            attackObj = Instantiate(enemyAttack, transform.position, Quaternion.identity);
            attackObj.GetComponent<scrt_enemyAttack>().attack = attack;
            attackObj.GetComponent<scrt_enemyAttack>().enemyCode = 0;
            delay = attackDelay;
        }
        if (delay <= attackDelay - attackTime)
        {
            Destroy(attackObj);
        }

    }

    public void Damage(float damage) //�÷��̾� ������Ʈ���� �� �Լ��� ���� �������� ���� �� ����. enemy tag�� ã�Ƽ� ���ݰ� �浹������ ���� ��� ȣ���ϸ� ��
    {
        health -= damage;
    }

    public void GetStunned(int time) //time ��ŭ ���Ͽ� �ɸ��� ��. time�����Ӹ�ŭ ���Ͽ� �ɸ�
    {
        state = 2;
        delay = time;
    }
}
