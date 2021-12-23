using UnityEngine;

public class PayerMovement : MonoBehaviour
{
    //SerializeField is to show the parameter in UNITY so that you can customize it.
    [SerializeField]private float speed;
    [SerializeField]private float jumpPower;
    [SerializeField]private float wallPushPowerSideways;
    [SerializeField]private float wallPushPowerUpwards;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator animate;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        //Grab References for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        animate = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Flip Player when moving left/right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);



        //Set animator Parameters
        animate.SetBool("run", horizontalInput != 0);
        animate.SetBool("grounded", isGrounded());

        //Wall Jumping logic
        if (wallJumpCooldown > 0.02f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 5;

            if (Input.GetKey(KeyCode.Space))
                Jump();

        }
        else
            wallJumpCooldown += Time.deltaTime;
         
        //The below is to print out to console that the method is working or not
        //print(onWall());
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animate.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 20, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) , transform.localScale.y , transform.localScale.z);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallPushPowerSideways, wallPushPowerUpwards);

            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        //RayCast is actually shooting out a virtual ray and checking if it hits something.
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
