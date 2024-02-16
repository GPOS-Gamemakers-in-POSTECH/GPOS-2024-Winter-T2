using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrt_golem : MonoBehaviour, IEnemyCommon
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
    float detectionRangeY = 15f; //����Ž������
    float attackRange = 10f; //���ݹ���
    float attackAngle = 60f; //���ݽ� �ٸ��� �� �ִ� ����
    float attackLegSpeed = 0.3f; //���ݽ� �ٸ��� �� �ӵ�
    int attackDelay = 60; //������ �̵� ������
    int attackTime = 20; //�������ӽð�
    public int floorLoc = 0; //��� �ִ� �ٴ��� ��ġ, 0: �ٴ�, 1: ���ʺ� 2: õ�� 3: �����ʺ�

    int state = 0; //0: normal, 1: alert, 2: stunned, 3: dead
    int delay = 0;
    int walkDelay = 0;
    int direction = 0;
    int walkingState = 3;
    int walkingLegNum = 0;
    float distance = 0f;
    bool alertOn = false;
    bool attackOn = false;
    bool attackLeg = false; //false: left  true: right
    bool flipXbool = false;
    bool wallCollide = false;
    Vector3 moveVector = Vector3.right;

    Transform player;
    SpriteRenderer spriteRenderer;
    GameObject attackObj;
    GameObject leftLeg;
    GameObject rightLeg;
    Animator animator;
    Quaternion rotateAngle;
    Vector3 leftLegPos;
    Vector3 rightLegPos;
    Vector3 moveVec1;
    Vector3 moveVec2;

    public GameObject golemLeg; //golemLeg ��ũ��Ʈ�� �� ������ �����ؾ���
    public GameObject enemyAttack; //dustpanAttack ��ũ��Ʈ�� �� ������ �����ؾ���

    Vector3 rotateVector(float x, float y)
    {
        return new Vector3(x*(floorLoc%2-1)*(floorLoc-1)+y*(floorLoc%2)*(floorLoc-2)*(-1),x*(floorLoc%2)*(floorLoc-2)+y*(floorLoc%2-1)*(floorLoc-1), 0f);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("player").transform;
        gameObject.tag = "enemy";
        animator = GetComponent<Animator>();
        rotateAngle = Quaternion.Euler(0f, 0f, (4 - floorLoc) * 90f);
        leftLegPos = rotateVector(-1 * golemLegX, golemLegY);
        rightLegPos = rotateVector(golemLegX, golemLegY);

        if (floorLoc % 2 == 0) { moveVector = Vector3.right; }
        else { moveVector = Vector3.up; }
        if (floorLoc != 0) { spriteRenderer.transform.localRotation = rotateAngle; }

        leftLeg = Instantiate(golemLeg, transform.position + leftLegPos, rotateAngle);
        rightLeg = Instantiate(golemLeg, transform.position + rightLegPos, rotateAngle);
        rightLeg.GetComponent<SpriteRenderer>().flipX = true;
    }

    void Update()
    {
        if (health <= 0) { state = 3; }
        delay--;

        if (wallCollide) { direction = 0; }

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

        wallCollide = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "wall") //���� �浹
        {
            this.transform.position += rotateVector(-1 * direction * speed / 2, 0);
            wallCollide = true;
        }
    }

    void Move()
    {
        if (floorLoc % 2 == 0) //x�������� ������ ��� (�ٴ�, õ��)
        {
            alertOn = (Math.Abs(player.transform.position.x - this.transform.position.x) < detectionRangeX) && (Math.Abs(player.transform.position.y - this.transform.position.y) < detectionRangeY);
            if (direction != 0) { flipXbool = (direction == (floorLoc * -1) + 1); }
        }
        else //y�������� ������ ��� (��)
        {
            alertOn = (Math.Abs(player.transform.position.y - this.transform.position.y) < detectionRangeX) && (Math.Abs(player.transform.position.x - this.transform.position.x) < detectionRangeY);
            if (direction != 0) { flipXbool = (direction == (floorLoc * -1) + 2); }
        }

        if (floorLoc >= 2)
        {
            if(flipXbool) { flipXbool = false; }
            else { flipXbool = true; }  
        }

        spriteRenderer.flipX = flipXbool;

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
                if (floorLoc % 2 == 0)
                {
                    if (player.transform.position.x - this.transform.position.x < -1 * walkLength/2f) { direction = 1 * (floorLoc-1); }
                    else if (player.transform.position.x - this.transform.position.x > walkLength/2f) { direction = -1 * (floorLoc-1); }
                    else { direction = 0; }
                }
                else
                {
                    if (player.transform.position.y - this.transform.position.y < -1 * walkLength/2f) { direction = -1 * (floorLoc-2); }
                    else if (player.transform.position.y - this.transform.position.y > walkLength/2f) { direction = 1 * (floorLoc-2); }
                    else { direction = 0; }
                }
            }

        }

        if (!attackOn)
        {
            moveVec1 = rotateVector(-1 * speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed));
            moveVec2 = rotateVector(speed, -1 * speed / walkLength * Math.Sign(walkingLegNum * 2 - walkLength / speed));

            switch (walkingState) //0: ���ʴٸ� ���ű� 1: ��� ���� 2: ���ʴٸ� ���ű� 3, 4: �޽�
            {
                case 0:
                    if (direction == -1) { leftLeg.transform.position += moveVec1; }
                    else if (direction == 1) { rightLeg.transform.position += moveVec2; }
                    this.transform.position += rotateVector(direction * speed / 2, 0);
                    if (walkingLegNum>=walkLength/speed) { walkDelay = (int)(walkLength / speed); walkingLegNum = 0; walkingState = 1; }
                    walkingLegNum++;
                    break;
                case 1:
                    walkDelay--;
                    if (walkDelay == 0) { walkingState = 2; }
                    break;
                case 2:
                    if (direction == -1) { rightLeg.transform.position += moveVec1; }
                    else if (direction == 1) { leftLeg.transform.position += moveVec2; }
                    this.transform.position += rotateVector(direction * speed / 2, 0);
                    if (walkingLegNum > walkLength / speed) { walkingLegNum = 0; walkingState = 3; }
                    walkingLegNum++;
                    break;
                case 3:
                    leftLeg.transform.position = transform.position + leftLegPos;
                    rightLeg.transform.position = transform.position + rightLegPos;
                    delay = 60;
                    walkingState = 4;
                    if (!alertOn) //idle������ ����� direction ����
                    {
                        direction = UnityEngine.Random.Range(-1, 4);
                        if (direction != 1 && direction != -1) { direction = 0; }
                    }
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
            switch(floorLoc)
            {
                case 0:
                    if (player.transform.position.x - this.transform.position.x < 0) { attackLeg = false; }
                    else { attackLeg = true; }
                    break;
                case 1:
                    if (player.transform.position.y - this.transform.position.y < 0) { attackLeg = true; }
                    else { attackLeg = false; }
                    break;
                case 2:
                    if (player.transform.position.x - this.transform.position.x < 0) { attackLeg = true; }
                    else { attackLeg = false; }
                    break;
                case 3:
                    if (player.transform.position.y - this.transform.position.y < 0) { attackLeg = false; }
                    else { attackLeg = true; }
                    break;
            }

            if (!attackLeg) //���ʿ� ���� �� ����
            {
                leftLeg.transform.Rotate(0f, 0f, -1 * attackLegSpeed);
            }
            else //�����ʿ� ���� �� ����
            {
                rightLeg.transform.Rotate(0f, 0f, attackLegSpeed);
            }

            if(Math.Abs(attackLegSpeed * delay) >= attackAngle) //�ٸ��� ���İ��� ���������� ���ݿ�����Ʈ ����
            {
                if (!attackLeg) { leftLeg.transform.Rotate(0f, 0f, -1 * attackLegSpeed * delay); }
                else { rightLeg.transform.Rotate(0f, 0f, attackLegSpeed * delay); }
                if (floorLoc % 2 == 0)
                {
                    attackObj = Instantiate(enemyAttack, transform.position + rotateVector((floorLoc-1)*Math.Sign(-1 * player.transform.position.x + this.transform.position.x) * attackX, attackY), rotateAngle);
                }
                else
                {
                    attackObj = Instantiate(enemyAttack, transform.position + rotateVector((floorLoc-2)*Math.Sign(player.transform.position.y - this.transform.position.y) * attackX, attackY), rotateAngle);
                }
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
        leftLeg.transform.position = transform.position + leftLegPos;
        rightLeg.transform.position = transform.position + rightLegPos;
    }
}