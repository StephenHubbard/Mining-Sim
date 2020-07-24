using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    [SerializeField] float speed = 6f;
    [SerializeField] float jumpSpeed = 6f;
    [SerializeField] float miningSpeed = 1f;

    public Animator myAnimator;
    MineBlock mineBlock;

    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        mineBlock = FindObjectOfType<MineBlock>();
    }

    void Update()
    {
        Movement();
        Swing();
    }

    private void Movement()
    {
        velocity.y += gravity * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(0f, 0f, horizontal).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
        }

        if (!controller.isGrounded)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            // for controller.isGrounded
            controller.Move(Vector3.forward * 0.001f);
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Jump();
    }

    private void Jump()
    {

        if (controller.isGrounded && Input.GetKeyDown("space"))
        {
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void Swing()
    {
        if (Input.GetMouseButton(0))
        {
            myAnimator.SetBool("isSwinging", true);
            myAnimator.speed = miningSpeed;
        }
        else
        {
            myAnimator.SetBool("isSwinging", false);
        }
    }

    // Animator function
    public void Hit()
    {
        mineBlock.checkIfMineBlockExists();
    }
}
