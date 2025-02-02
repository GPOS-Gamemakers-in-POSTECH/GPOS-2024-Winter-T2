using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static int playerHealth;
    public static int playerMoveSpeed;
    public static int playerAttributeDefense;
    public static int playerStunEffectDefense;
    public static int playerChaosEffectDefense;
    public static Vector2 gravityVector;
    public static Vector2 playerCoordinateVector;
    public static Vector2 gravitySourceVector;
    public static float gravitentialForce;
    public static int gravitySourceDirection = 1;
    void Awake()
    {
        playerHealth = 100;
        playerMoveSpeed = 5;
        playerAttributeDefense = 0;
        playerStunEffectDefense = 1;
        playerChaosEffectDefense = 10;
        gravityVector = Vector2.down;
        gravitentialForce = 100000.0f;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        playerCoordinateVector = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

        return;
    }
}
