using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class player : MonoBehaviour
{
    float health = 100f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "player";
    }

    // Update is called once per frame
    void Update()
    {


        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "enemyWeapon" || col.tag=="enemy")
            {
                Damage(col.gameObject.attack);
            }
            UnityEngine.Debug.Log(health);
        }

        void Damage(float damage) //�������� �ִ� �Լ�, �������� ���� �� �׷��� ��ȭ���� �� �� ����
        {
            health -= damage;
        }
    }
}
