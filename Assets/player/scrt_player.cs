using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class scrt_player : MonoBehaviour
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
        
    }

    public void Damage(float damage) //�������� �Դ� �Լ�
    {
        health -= damage;
        UnityEngine.Debug.Log(health);
    }
}
