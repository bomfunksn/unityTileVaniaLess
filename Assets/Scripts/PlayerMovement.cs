using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (3f, 3f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] float reloadAfterDeath = 2f;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    BoxCollider2D myBodyCollider;
    CapsuleCollider2D myFeetCollider;
    float gravityScaleAtStart;
    bool isAlive = true;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();    
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        myFeetCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if(!isAlive) {return; }
        
        Run();
        FlipSprite ();
        ClimbLadder ();
        Die();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive) {return; }
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if(!isAlive) {return; }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2 (0f, jumpSpeed);
        }
    }
    void OnFire(InputValue value)
    {
        if(!isAlive) {return; }
        if(value.isPressed)
        {
        Instantiate(bullet, gun.position, transform.rotation);    
        }

    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
            Vector2 climbVelocity = new Vector2 (moveInput.x * runSpeed, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            myAnimator.SetBool("isClimbing", true);
            myRigidbody.gravityScale = 0f;       

            bool playerHasVerticalSpeed = Mathf.Abs (myRigidbody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
            

    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs (myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
        
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs (myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
    void Die()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            myRigidbody.gravityScale = gravityScaleAtStart;

            FindObjectOfType<GameSession>().ProcessPlayerDeath();

            StartCoroutine(ReloadLevel());
        }
    }
    IEnumerator ReloadLevel()
    {
        yield return new WaitForSecondsRealtime(reloadAfterDeath);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

    }
    
}
