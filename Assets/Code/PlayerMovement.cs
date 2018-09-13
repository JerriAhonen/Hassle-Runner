using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private bool isRunning = false;

    private CharacterController cc;
    private Animator anim;

    private float jumpForce = 6.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1;

    // Speed modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        speed = originalSpeed;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        #region Change Lane
        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);
        
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        #endregion

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            verticalVelocity = -0.1f;
            
            if (MobileInput.Instance.SwipeUp)
            {
                // Jump
                
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                // Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast fall
            if (MobileInput.Instance.SwipeDown)
            {
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

    private void MoveLane(bool goRight)
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

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        cc.height /= 2;
        cc.center = new Vector3(cc.center.x, cc.center.y / 2, cc.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        cc.height *= 2;
        cc.center = new Vector3(cc.center.x, cc.center.y * 2, cc.center.z);
    }

    private void Crash()
    {
        anim.SetTrigger("Die");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}
