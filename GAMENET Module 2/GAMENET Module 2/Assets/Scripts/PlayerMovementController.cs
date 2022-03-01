using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick Joystick;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;
    public FixedTouchField FixedTouchField;

    private Animator animator;
    private float runningThreshold = 0.9f;
    private float runningSpeed = 10f;
    private float walkingSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rigidbodyFirstPersonController.JoystickInputAxis.x = Joystick.Horizontal;
        rigidbodyFirstPersonController.JoystickInputAxis.y = Joystick.Vertical;
        rigidbodyFirstPersonController.mouseLook.LookInputAxis = FixedTouchField.TouchDist;

        animator.SetFloat("horizontal", Joystick.Horizontal);
        animator.SetFloat("vertical", Joystick.Vertical);

        if(Mathf.Abs(Joystick.Horizontal) > runningThreshold || Mathf.Abs(Joystick.Vertical) > runningThreshold)
        {
            animator.SetBool("isRunning", true);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = runningSpeed;
        }
        else
        {
            animator.SetBool("isRunning", false);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = walkingSpeed;
        }
    }
}
