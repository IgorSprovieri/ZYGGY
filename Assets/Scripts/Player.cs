using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector3 CheckPoint;
    Vector2 MovementInput;
    float ScaleX = 1.0f;
    public bool CollisionWall, CollisionFloor, JumpInput, ClimbMode, DoubleJump;
    public Rigidbody2D rb2D;
    public LayerMask Ground;
    public bool b = false;

    void OnMove(InputValue value)
    {
        MovementInput = value.Get<Vector2>();

        if (MovementInput.x < -0.1f)
        {
            ScaleX = -1;
        }

        if (MovementInput.x > 0.1f)
        {
            ScaleX = 1;
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            JumpInput = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveCheckPoint();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float translateVelocityX = MovementInput.x * Time.deltaTime;
        float translateVelocityY = 0.0f;

        bool isTouchingUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.3f), transform.right, 0.4f, Ground);
        bool isTouchingDown = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), transform.right, 0.4f, Ground); 

        if (isTouchingUp == true && isTouchingDown == true)
        {
            translateVelocityX = 0.0f;
            translateVelocityY = MovementInput.y * Time.deltaTime;
            rb2D.simulated = false;
            rb2D.velocity = new Vector2(0, 0);
            //rb2D.isKinematic = true;
            b = false;
        }

        if (isTouchingUp == false && isTouchingDown == true)
        {
            translateVelocityX = 0.0f;
            translateVelocityY = 0.0f;
            rb2D.simulated = true;
            //rb2D.isKinematic = false;

            if (b == false)
            {
                rb2D.AddForce(transform.up * 270.0f);
                b = true;
            }
        }

        if (isTouchingUp == true && isTouchingDown == false)
        {
            translateVelocityX = MovementInput.x * Time.deltaTime;
            translateVelocityY = 0.0f;
            rb2D.simulated = true;
            //rb2D.isKinematic = false;
        }

        transform.Translate(translateVelocityX * 2.0f, translateVelocityY * 2.0f, 0.0f);

        Vector3 FinalScale = new Vector3(ScaleX * 0.75f, 0.75f, 0);
        transform.localScale = Vector3.Lerp(transform.localScale, FinalScale, 5.0f * Time.deltaTime);

        if(CollisionFloor == true)
        {
            DoubleJump = false;
            if (JumpInput == true)
            {
                rb2D.AddForce(transform.up * 270.0f);
                JumpInput = false;
            }
        }
        else
        {
            if (JumpInput == true)
            {
                if (DoubleJump == false)
                {
                    rb2D.AddForce(transform.up * 150.0f);
                    DoubleJump = true;
                }
            }
        }

        if(transform.position.y < -7)
        {
            RestartOnCheckPoint();
        }
    }

    void RestartOnCheckPoint()
    {
        transform.position = CheckPoint;
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(0.75f, 0.75f, 0);
        CollisionWall = false;
        CollisionFloor = false;
        JumpInput = false;
    }

    void SaveCheckPoint()
    {
        CheckPoint = transform.position; ;
    }

    void OnCollisionEnter2D(Collision2D Col)
    {
        if (Col.transform.gameObject.tag == "Wall")
        {
            CollisionWall = true;
        }
        if (Col.transform.gameObject.tag == "Floor")
        {
            CollisionFloor = true;
        }
        if (Col.transform.gameObject.tag == "Thorn")
        {
            RestartOnCheckPoint();
        }
    }
    void OnCollisionExit2D(Collision2D Col)
    {
        if (Col.transform.gameObject.tag == "Floor")
        {
            CollisionFloor = false;
        }
    }

    void OnTriggerEnter2D(Collider2D Col)
    {
        if (Col.transform.gameObject.tag == "Respawn")
        {
            SaveCheckPoint();
        }

        if (Col.transform.gameObject.tag == "Finish")
        {
            //nextlevel
        }

        Col.transform.gameObject.SetActive(false);
    }
}
