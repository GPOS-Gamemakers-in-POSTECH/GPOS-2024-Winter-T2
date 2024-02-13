using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class scrt_drone : MonoBehaviour
{
    //�ɷ�ġ
    float attack = 20f; //���ݷ�
    float health = 100f; //ü��
    float speed = 2f; //�̵��ӷ�
    float detectionRangeX = 20f; //����Ž������
    float detectionRangeY = 10f; //����Ž������
    float attackRange = 10f; //���ݹ���
    float bulletSpeed = 5f; //�Ѿ��� �ӵ�
    int attackDelay = 60; //���ݵ�����
    int attackTime = 20; //�������ӽð�
    public int floorLoc = 0; //��� �ִ� �ٴ��� ��ġ, 0: �ٴ�, 1: ���ʺ� 2: õ�� 3: �����ʺ�

    int state = 0; //0: normal, 1: alert, 2: stunned, 3: dead
    int delay = 0;
    int direction = 0;
    float distance = 0f;
    bool alertOn = false;
    Vector3 moveVector = Vector3.right;

    Transform player;
    SpriteRenderer spriteRenderer;
    GameObject attackObj;
    Animator animator;
    public GameObject enemyAttack; //dustpanAttack ��ũ��Ʈ�� �� ������ �����ؾ���

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("player").transform;
        gameObject.tag = "enemy";
        animator = GetComponent<Animator>();

        if (floorLoc % 2 == 0) { moveVector = Vector3.right; }
        else { moveVector = Vector3.up; }

        if (floorLoc != 0) { spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, (4 - floorLoc) * 90f); }

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
            case 3: animator.SetBool("bool_death", true); break;
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "wall") //���� �浹
        {
            if (state == 0) { direction *= -1; } //idle ������ ��� �ݴ�� ���ư�
            else if (state == 1) { direction = 0; } //�߰� ���� ��� ���� �ɷ�����
        }
    }

    void Move()
    {

        if (floorLoc % 2 == 0) //x�������� ������ ��� (�ٴ�, õ��)
        {
            alertOn = (Math.Abs(player.transform.position.x - this.transform.position.x) < detectionRangeX) && (Math.Abs(player.transform.position.y - this.transform.position.y) < detectionRangeY);
            //�ٴ��̸� 0, õ���̸� 2�̹Ƿ� floorLoc-1�� ��ȣ�� ������ �ٴ�, ����� õ��. �� ��ȣ�� �÷��̾�� �ڽ��� y��ǥ ������ ��ȣ�� ���ƾ���
            if(floorLoc-1 != Math.Sign(player.transform.position.y - this.transform.position.y)) { alertOn = false; }
            if (direction != 0) { spriteRenderer.flipX = (direction == (floorLoc * -1) + 1); }
        }
        else //y�������� ������ ��� (��)
        {
            alertOn = (Math.Abs(player.transform.position.y - this.transform.position.y) < detectionRangeX) && (Math.Abs(player.transform.position.x - this.transform.position.x) < detectionRangeY);
            //���������� ������ 1, �������� 3, -2�� ��ȣ�� x��ǥ ������ ��ȣ�� ���ƾ���
            if (floorLoc - 2 != Math.Sign(player.transform.position.x - this.transform.position.x)) { alertOn = false; }
            if (direction != 0) { spriteRenderer.flipX = (direction == floorLoc - 2); }
        }

        transform.Translate(moveVector * direction * speed * Time.deltaTime, Space.World);

        if (direction != 0) { animator.SetBool("bool_move", true); }
        else { animator.SetBool("bool_move", false); }
        
        distance=Vector3.Distance(transform.position, player.transform.position);
        if (alertOn) {
            if (state != 1) { delay = 0; }
            state = 1;
            animator.SetBool("bool_alert", true);
        }
        else { 
            state = 0;
            animator.SetBool("bool_alert", false);
        }
    }

    void Attack()
    {

        if (floorLoc % 2 == 0)
        {
            if (player.transform.position.x - this.transform.position.x < -1 * attackRange / 4) { direction = -1; }
            else if (player.transform.position.x - this.transform.position.x > attackRange / 4) { direction = 1; }
            else { direction = 0; }
        }
        else
        {
            if (player.transform.position.y - this.transform.position.y < -1 * attackRange / 4) { direction = -1; }
            else if (player.transform.position.y - this.transform.position.y > attackRange / 4) { direction = 1; }
            else { direction = 0; }
        }

        if (distance < attackRange && delay <= 0)
        {
            attackObj = Instantiate(enemyAttack, transform.position, Quaternion.Euler(0f, 0f, (4 - floorLoc) * 90f));
            attackObj.GetComponent<scrt_enemyAttack>().attack = attack;
            attackObj.GetComponent<scrt_enemyAttack>().enemyCode = 1;
            attackObj.GetComponent<scrt_enemyAttack>().bulletSpeed = bulletSpeed;
            attackObj.GetComponent<scrt_enemyAttack>().lifeDuration = attackTime;
            delay = attackDelay;
            animator.SetTrigger("trigger_attack");
        }

    }

    public void Damage(float damage) //�÷��̾� ������Ʈ���� �� �Լ��� ���� �������� ���� �� ����. enemy tag�� ã�Ƽ� ���ݰ� �浹������ ���� ��� ȣ���ϸ� ��
    {
        health -= damage;
        animator.SetTrigger("trigger_getAttacked");
    }

    public void GetStunned(int time) //time ��ŭ ���Ͽ� �ɸ��� ��. time�����Ӹ�ŭ ���Ͽ� �ɸ�
    {
        state = 2;
        delay = time;
        animator.SetTrigger("trigger_getStunned");
    }
}
