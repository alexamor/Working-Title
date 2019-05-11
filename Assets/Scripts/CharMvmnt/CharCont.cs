using UnityEngine;
using UnityEngine.Events;

public class CharCont : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 100f;                          // Amount of force added when the player jumps.
    [SerializeField] public float m_JumpVelocity = 1f;
    [SerializeField] public float m_WallJumpForce = 200f;
    [SerializeField] public float m_WallJumpVelocity = 40f;
    [SerializeField] public float m_WallJumpLimit = 0.2f;
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    //[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Transform circleCenter;                            // Center of Circle Collider for down, left and right checks
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    private float distToGround;
    private float offset = 5f;
    //private float wallJumpTimer = 0;
    private int jumpDiretion = 0;
    public float deceleration = 30f;

    public bool left, right, up, down;
    public bool goingUp = false, goingDown = false, goingLeft = false, goingRight = false; 
    //private Vector2 prevPos;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        distToGround = GetComponent<CircleCollider2D>().bounds.extents.y;

        //prevPos = new Vector2(transform.position.x, transform.position.y);

    }

    private void FixedUpdate()
    {
        //bool wasGrounded = m_Grounded;
        m_Grounded = false;

        //UpdateDirections();
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }


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

        //Avoid movement on wall jump to climb
        if (jumpDiretion != 0 && (jumpDiretion == (int) -move))
        {
            move = 0;
            Debug.Log("Disable Movement " + Time.fixedDeltaTime);
        }
            

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Jump(float jumpTimer)
    {
        if(jumpTimer == 0)
        {
            if(!down)
                jumpDiretion = HorizDirectToInt();

            m_Rigidbody2D.AddForce(new Vector2(jumpDiretion * m_WallJumpForce, m_JumpForce));
            down = false;

            //Fix the glued to wall bug
            if (jumpDiretion != 0)
            {
                transform.Translate(Vector3.right * jumpDiretion * 0.1f + Vector3.up * 0.1f);
                m_Rigidbody2D.AddForce(new Vector2(m_JumpForce, 0f));
            }

        }
        else
        {
            m_Rigidbody2D.velocity = m_Rigidbody2D.velocity + Vector2.up * m_JumpVelocity * Time.deltaTime + Vector2.right * jumpDiretion * m_WallJumpVelocity * Time.fixedDeltaTime;
        }
 
    }

    public void StopJump()
    {
        m_Rigidbody2D.velocity = m_Rigidbody2D.velocity + Vector2.down * deceleration * Time.fixedDeltaTime + Vector2.right * jumpDiretion * m_WallJumpVelocity * Time.fixedDeltaTime;
    }

    private int HorizDirectToInt()
    {
        int direction;

        if (left)
            direction = 1;
        else if (right)
            direction = -1;
        else
            direction = 0;

        return direction;
    }

    private int VertDirectToInt()
    {
        if (down)
            return 1;
        else
            return 0;
    }

    public bool CanJump()
    {
        return (down || left || right);
    }

    public void UpdateDirections()
    {
        //Going down
        if (m_Rigidbody2D.velocity.y > offset/*prevPos.y > transform.position.y*/)
        {
            goingDown = true;
            goingUp = false;

            jumpDiretion = 0;

            //Debug.Log("Going down");
        }
        //Going up
        else if(m_Rigidbody2D.velocity.y < -offset/*prevPos.y < transform.position.y*/)
        {
            goingUp = true;
            goingDown = false;


            ////Always move opposite to the wall while walljumping
            //if(jumpDiretion != 0)
            //    m_Rigidbody2D.velocity = m_Rigidbody2D.velocity + Vector2.right * jumpDiretion * m_WallJumpVelocity * Time.fixedDeltaTime;

            //Debug.Log("Going up");

        }
        else
        {
            goingUp = false;
            goingDown = false;
        }

        //prevPos = new Vector2(transform.position.x, transform.position.y);
    }

    public bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * 1000, Color.red);

        return Physics.Raycast(transform.position, Vector3.left, distToGround + 0.1f, m_WhatIsGround);
    }

    //For now collisions will be checked every frame due to collision exit not having collision points, not very efficient
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obs")
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
           if(collision.gameObject.tag == "Obs")
            {
                //Vertical touch
                if(Mathf.Abs(collision.GetContact(i).point.y - circleCenter.transform.position.y) > Mathf.Abs(collision.GetContact(i).point.x - circleCenter.transform.position.x))
                {
                    if (collision.GetContact(i).point.y < circleCenter.transform.position.y)
                        down = true;
                    else
                        up = true;
                }
                //Horizontal touch
                else
                {
                    if (collision.GetContact(i).point.x < circleCenter.transform.position.x)
                        left = true;
                    else
                        right = true;
                }

                //Debug.Log("point " + collision.GetContact(i).point + "  this " + circleCenter.transform.position + "  up down left right " + up + down + left + right);
            }

        }
        
    }
}