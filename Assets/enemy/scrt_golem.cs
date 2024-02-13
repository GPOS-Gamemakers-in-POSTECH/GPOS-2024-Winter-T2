using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrt_golem : MonoBehaviour
{
    float attack = 20f; //���ݷ�
    float health = 100f; //ü��
    float speed = 0.1f; //�� ���� �ٸ��� �̵� �ӵ�
    float walkLength = 3f; //�� ���� �ٸ��� �ִ��� �̵��� �Ÿ�
    float golemLegX = 4.5f; //�ٸ��� �޸� ������ ��ǥ, �� �߰� �ھ���� �߽��� �������� �����ٸ��� ��ǥ
    float golemLegY = -2f;
    float attackX = 4.5f; //���� ������Ʈ�� ���� ��ǥ, �������� golemLeg�� ����
    float attackY = -3f;
    float detectionRangeX = 40f; //����Ž������
    float detectionRangeY = 5f; //����Ž������
    float attackRange = 10f; //���ݹ���
    float attackAngle = 60f; //���ݽ� �ٸ��� �� �ִ� ����
    float attackLegSpeed = 0.3f; //���ݽ� �ٸ��� �� �ӵ�
    int attackDelay = 60; //������ �̵� ������
    int attackTime = 20; //�������ӽð�

    int state = 0; //0: normal, 1: alert, 2: stunned, 3: dead
    int delay = 0;
    int walkDelay = 0;
    int direction = 0;
    int walkingState = 3;
    int walkingLegNum = 0;
    float distance = 0f;
    bool alertOn = false;
    bool attackOn = false;
    Vector3 moveVector = Vector3.right;

    Transform player;
    SpriteRenderer spriteRenderer;
    GameObject attackObj;
    GameObject leftLeg;
    GameObject rightLeg;
    Animator animator;

    public GameObject golemLeg; //golemLeg ��ũ��Ʈ�� �� ������ �����ؾ���
    public GameObject enemyAttack; //dustpanAttack ��ũ��Ʈ�� �� ������ �����ؾ���

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("player").transform;
        gameObject.tag = "enemy";
        animator = GetComponent<Animator>();

        leftLeg = Instantiate(golemLeg, transform.position + new Vector3(-1 * golemLegX, golemLegY, 0), Quaternion.identity);
        rightLeg = Instantiate(golemLeg, transform.position + new Vector3(golemLegX, golemLegY, 0), Quaternion.identity);
        rightLeg.GetComponent<SpriteRenderer>().flipX = true;
    }

    void Update()
    {
        if (health <= 0) { state = 3; }
        delay--;

        switch (state)
        {
            case 0: Move(); break;
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

    void Move()
    {
        alertOn = (Math.Abs(player.transform.position.x - transform.position.x) < detectionRangeX) && (Math.Abs(player.transform.position.y - transform.position.y) < detectionRangeY); //alert����

        if (alertOn) //alert���·��� ����
        {
            if (state != 1) { delay = 0; }
            state = 1;
            animator.SetBool("bool_alert", true);
        }
        else
        {
            state = 0;
            animator.SetBool("bool_alert", false);
        }

        distance = Vector3.Distance(transform.position, player.transform.position);

        if (direction != 0) { animator.SetBool("bool_move", true); } //������ �ִϸ��̼�
        else { animator.SetBool("bool_move", false); }

        if (walkingState >= 3) //�����̴� ������ ����
        {
            if (alertOn) //alert������ ����� ����
            {
                if (player.transform.position.x - this.transform.position.x < -1 * attackRange / 4) { direction = -1; }
                else if (player.transform.position.x - this.transform.position.x > attackRange / 4) { direction = 1; }
                else { direction = 0; }
            }
            else //idle������ ����� ����
            {
                direction = UnityEngine.Random.Range(-1, 4);
                if(direction!=1 && direction != -1) { direction = 0; }
            }
        }

        if (!attackOn)
        {
            switch (walkingState) //0: ���ʴٸ� ���ű� 1: ��� ���� 2: ���ʴٸ� ���ű� 3, 4: �޽�
            {
                case 0:
                    if (direction == -1) { leftLeg.transform.position += new Vector3(-1 * speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed), 0); }
                    else if (direction == 1) { rightLeg.transform.position += new Vector3(speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed), 0); }
                    this.transform.position += new Vector3(direction * speed / 2, 0, 0);
                    if (walkingLegNum>=walkLength/speed) { walkDelay = (int)(walkLength / speed); walkingLegNum = 0; walkingState = 1; }
                    walkingLegNum++;
                    break;
                case 1:
                    walkDelay--;
                    if (walkDelay == 0) { walkingState = 2; }
                    break;
                case 2:
                    if (direction == -1) { rightLeg.transform.position += new Vector3(-1 * speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed), 0); }
                    else if (direction == 1) { leftLeg.transform.position += new Vector3(speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed), 0); }
                    this.transform.position += new Vector3(direction * speed / 2, 0, 0);
                    if (walkingLegNum > walkLength / speed) { walkingLegNum = 0; walkingState = 3; }
                    walkingLegNum++;
                    break;
                case 3:
                    leftLeg.transform.position = transform.position + new Vector3(-1 * attackX, golemLegY, 0);
                    rightLeg.transform.position = transform.position + new Vector3(golemLegX, golemLegY, 0);
                    delay = 60;
                    walkingState = 4;
                    break;
                case 4:
                    if (delay <= 0 && distance >= attackRange) { walkingState = 0; }
                    break;
            }
        }

    }

    void Attack()
    {
        
        if (!attackOn && delay <=0 && distance < attackRange && walkingState >= 3) { attackOn = true; delay = 0; }

        if (attackOn)
        {
            if (player.transform.position.x - this.transform.position.x < 0) //���ʿ� ���� �� ����
            {
                leftLeg.transform.Rotate(0f, 0f, -1 * attackLegSpeed);
            }
            else //�����ʿ� ���� �� ����
            {
                rightLeg.transform.Rotate(0f, 0f, attackLegSpeed);
            }

            if(Math.Abs(attackLegSpeed * delay) >= attackAngle) //�ٸ��� ���İ��� ���������� ���ݿ�����Ʈ ����
            {
                if (player.transform.position.x - this.transform.position.x < 0) { leftLeg.transform.Rotate(0f, 0f, -1 * attackLegSpeed * delay); }
                else { rightLeg.transform.Rotate(0f, 0f, attackLegSpeed * delay); }
                attackObj = Instantiate(enemyAttack, transform.position 
                    + new Vector3((player.transform.position.x - this.transform.position.x)/Math.Abs(player.transform.position.x - this.transform.position.x) * attackX, attackY, 0), Quaternion.Euler(0f, 0f, 0f));
                attackObj.GetComponent<scrt_enemyAttack>().attack = attack;
                attackObj.GetComponent<scrt_enemyAttack>().enemyCode = 0;
                attackObj.GetComponent<scrt_enemyAttack>().lifeDuration = attackTime;
                delay = attackDelay;
                animator.SetTrigger("trigger_attack");
                attackOn = false;
            }
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
        walkingState = 3;
        leftLeg.transform.position = transform.position + new Vector3(-1 * golemLegX, golemLegY, 0);
        rightLeg.transform.position = transform.position + new Vector3(golemLegX, golemLegY, 0);
    }
}
