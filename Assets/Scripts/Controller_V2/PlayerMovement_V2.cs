using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController_V2;

public class PlayerMovement_V2 : MonoBehaviour
{
    public Controller character;
    public Animator anim;

    [SerializeField]
    private float runSpeedAnimFactor = 0.05f;

    private float horizMove = 0f;
    private float directionInput;

    // Update is called once per frame
    void Update()
    {
        directionInput = Input.GetAxisRaw("Horizontal");

        horizMove = character.GetCurrentDirection() * character.GetCurrentSpeed();
        anim.SetFloat("Speed", Mathf.Abs(GetRunSpeedAnim()));
        
        character.WillJump(Input.GetButtonDown("Jump"));
        anim.SetBool("isJumping", character.GetJumpingStatus());
    }

    void FixedUpdate()
    {
        //Control the velcity (speed and direction) of the character depending on the user input direction.
        //Finally, move the character.
        character.ControlVelocity(directionInput);
        character.Move(horizMove * Time.fixedDeltaTime, false);
    }

    float GetRunSpeedAnim()
    {
        return horizMove * runSpeedAnimFactor;
    }
}
