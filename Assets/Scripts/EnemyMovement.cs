using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidBody;

    void Start()
    {
        myRigidBody = GetComponent <Rigidbody2D>();
    }

    void Update()
    {
        myRigidBody.velocity = new Vector2 (moveSpeed, 0);
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();

    } 
    void FlipEnemyFacing()
    {

        Vector2 currentScale = transform.localScale;
        currentScale.x = -currentScale.x;
        transform.localScale = currentScale;
        
    }
}
