using UnityEngine;
using UnityEngine.Events;

namespace CharacterController_V2
{
    public class MovementController : Controller
    {
        [SerializeField]
        private float m_maxSpeed; //ADDED BY ME! Maximum speed the character can reach.
        [SerializeField]
        private float m_skidDecel; //ADDED BY ME! Maximum speed the character can reach.
        [Range(0, .3f)]
        [SerializeField]
        private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
        [SerializeField]
        private bool m_AirControl = false;                          // Whether or not a player can steer while jumping;
        [SerializeField]
        private Collider2D m_CrouchDisableCollider;             // A collider that will be disabled when crouching
        [Range(0, 1)]
        [SerializeField]
        private float m_CrouchSpeed = .36f;         // Amount of maxSpeed applied to crouching movement. 1 = 100%

        private Vector3 m_Velocity = Vector3.zero;
        private int m_curDirection = (int)direction.still; //ADDED BY ME! Current Direction of character.
        private float m_runSpeed = 0f; //ADDED BY ME! Run speed of the character.
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        [Header("Events")]
        [Space]

        public BoolEvent OnCrouchEvent;
        private bool m_wasCrouching = false;

        private void Awake()
        {
            if (OnCrouchEvent == null)
                OnCrouchEvent = new BoolEvent();
        }

        public override void Move(float move, bool crouch) //REMOVED "bool jump"
        {
            Debug.Log("Move");
            // If crouching, check to see if the character can stand up
            if (!crouch)
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {

                // If crouching
                if (crouch)
                {
                    if (!m_wasCrouching)
                    {
                        m_wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }

                    // Reduce the speed by the crouchSpeed multiplier
                    move *= m_CrouchSpeed;

                    // Disable one of the colliders when crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = false;
                }
                else
                {
                    // Enable the collider when not crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = true;

                    if (m_wasCrouching)
                    {
                        m_wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip(transform);
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip(transform);
                }
            }

            if (m_isJumping) //REPLACED "jump" with "m_isJumping"
            {
                if (m_Grounded && m_isTouchingCeiling)
                {
                    m_isJumping = false;
                }
                else if (m_Grounded)
                {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                }
                else if (m_isTouchingCeiling)
                {
                    m_isTouchingCeiling = false;
                }
            }
        }

        private void Flip(Transform transform)
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        //ADDED BY ME!
        private void ReduceSpeed(float reductionFactor = 1)
        {
            if (m_runSpeed > 0)
                m_runSpeed -= reductionFactor;

            if (m_runSpeed < 0)
            {
                m_runSpeed = 0;
            }
        }

        //ADDED BY ME!
        public override void ControlVelocity(float directionInput)
        {
            if (m_curDirection == directionInput * -1)
            {
                ReduceSpeed(m_skidDecel);
            }
            else if (directionInput == (int)direction.still)
            {
                ReduceSpeed();
            }

            SetDirection(directionInput);

            if (m_curDirection == directionInput)
            {
                if (m_runSpeed < m_maxSpeed)
                    m_runSpeed++;
            }
        }

        //ADDED BY ME!
        public void SetDirection(float directionInput)
        {
            if (directionInput == (int)direction.still)
            {
                if (m_runSpeed == 0)
                    m_curDirection = (int)direction.still;
            }
            else
            {
                if (m_curDirection == (int)direction.still)
                {
                    m_curDirection = (int)directionInput;
                }
                else
                {
                    if (m_curDirection != (int)directionInput * -1)
                        m_curDirection = (int)directionInput;
                }
            }
        }

        //ADDED BY ME!
        public override int GetCurrentDirection()
        {
            if (m_runSpeed == 0)
                m_curDirection = (int)direction.still;

            return m_curDirection;
        }

        //ADDED BY ME!
        public override float GetCurrentSpeed()
        {
            return m_runSpeed;
        }
    }
}