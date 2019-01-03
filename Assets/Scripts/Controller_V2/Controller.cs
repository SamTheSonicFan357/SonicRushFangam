////////////////////////
//code borrowed from https://github.com/Brackeys/2D-Character-Controller/blob/master/CharacterController2D.cs
////////////////////////

using UnityEngine;
using UnityEngine.Events;

namespace CharacterController_V2
{
    //ADDED BY ME!
    public enum direction : int { left = -1, still, right };

    public class Controller : MonoBehaviour
    {
        [SerializeField]
        protected LayerMask m_WhatIsGround;                           // A mask determining what is ground to the character
        [SerializeField]
        protected LayerMask m_WhatIsCeiling;                          // A mask determining what is ceiling to the character
        [SerializeField]
        protected Transform m_GroundCheck;                            // A position marking where to check if the player is grounded.
        [SerializeField]
        protected Transform m_CeilingCheck;                           // A position marking where to check for ceilings
        [SerializeField]
        protected float m_JumpForce = 400f;                           // Amount of force added when the player jumps.

        protected Rigidbody2D m_Rigidbody2D;
        protected const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        protected const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
        protected bool m_Grounded;            // Whether or not the player is grounded.
        protected bool m_isTouchingCeiling;   // ADDED BY ME! Checks to see if the player is touching the ceiling.
        protected bool m_isJumping = false; //ADDED BY ME! Check to see if the character is jumping.

        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public virtual void WillJump(bool jumpButtonDown) { }
        public virtual bool GetJumpingStatus() { return false; }
        public virtual int GetCurrentDirection() { return (int)direction.still; }
        public virtual float GetCurrentSpeed() { return 0f; }
        public virtual void ControlVelocity(float directionInput) { }
        public virtual void Move(float move, bool crouch) { }
    }

    public class CharacterController_V2 : Controller
    {
        [Header("Events")]
        [Space]

        public UnityEvent OnLandEvent;

        private void Awake()
        {
            if (OnLandEvent == null)
                OnLandEvent = new UnityEvent();
        }

        private void FixedUpdate()
        {
            bool wasGrounded = m_Grounded;
            m_Grounded = false;
            m_isTouchingCeiling = false; //ADDED BY ME!

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    if (!wasGrounded && m_Rigidbody2D.velocity.y < 0) //ADDED "&& m_Rigidbody2D.velocity.y < 0"
                        OnLandEvent.Invoke();
                }
            }

            //ADDED BY ME! 
            //Ceiling Check!
            colliders = Physics2D.OverlapCircleAll(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsCeiling);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_isTouchingCeiling = true;
                }
            }
        }

        //ADDED BY ME!
        private void OnLanding()
        {
            m_isJumping = false;
        }

        //ADDED BY ME!
        public override void WillJump(bool jumpButtonDown)
        {
            if (IsGrounded() && jumpButtonDown)
                m_isJumping = true;
        }

        //ADDED BY ME!
        public override bool GetJumpingStatus()
        {
            return m_isJumping;
        }

        //ADDED BY ME!
        public bool IsGrounded()
        {
            return m_Grounded;
        }
    }
}