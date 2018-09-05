using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private const float LANE_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.05f;

    private CharacterController cc;
    private Animator anim;

    private float jumpForce = 14.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private float speed = 7.0f;
    private int desiredLane = 1;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(false);
        }

        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        Vector3 moveVector = Vector3.zero;

        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded) // if grounded
        {
            verticalVelocity = -0.1f;
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Jump
                Debug.Log("Jump activated");

                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast fall
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("Fast fall activated");
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the player
        cc.Move(moveVector * Time.deltaTime);

        // Rotate the player
        Vector3 dir = cc.velocity;

        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void MoveLane (bool goRight)
    {
        desiredLane += (goRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(
                cc.bounds.center.x,
                (cc.bounds.center.y - cc.bounds.extents.y) + 0.2f,
                cc.bounds.center.z),
            Vector3.down);

        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }
}
