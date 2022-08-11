using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector3 CheckPoint;
    Vector2 MovementInput;
    float ScaleX = 1.0f;
    bool CollisionWall, CollisionFloor, JumpInput;
    public Rigidbody2D rb2D;
    
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
        if(CollisionWall == false)
        {
            Vector3 translateVelocity = new Vector3(MovementInput.x * Time.deltaTime, 0.0f, 0.0f);
            transform.Translate(translateVelocity * 2.0f);
        }

        Vector3 FinalScale = new Vector3(ScaleX * 0.75f, 0.75f, 0);
        transform.localScale = Vector3.Lerp(transform.localScale, FinalScale, 5.0f * Time.deltaTime);

        if(CollisionWall == true || CollisionFloor == true)
        {
            if(JumpInput == true)
            {
                rb2D.AddForce(transform.up * 270.0f);
                JumpInput = false;
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
    }

    void SaveCheckPoint()
    {
        CheckPoint = transform.position; ;
    }

    void OnCollisionEnter2D(Collision2D Col)
    {
        if(Col.transform.gameObject.tag == "Wall")
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
        if (Col.transform.gameObject.tag == "Wall")
        {
            CollisionWall = false;
        }
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

        Col.transform.gameObject.SetActive(false);
    }
}
