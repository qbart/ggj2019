using Application;
using UnityEngine;


public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;
    public float speed;
    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;
    RaycastHit2D hitter;

    public static float movementSpeed = 1;
    public float boostValue = 1;
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    public Vector3 prevMoveBy = new Vector3(0, 0, 0);
    Booster booster = new Booster(movementSpeed);

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        direction = Direction.NONE;
    }

    private void decreaseIfBoost()
    {  
        if (booster.boostStarted)
        {
            movementSpeed = booster.decreaseToBoostEnd();
        }
    }

    void Update()
    {
        Direction previousDirection = direction;
        float horizontalDir = Input.GetAxis("Horizontal_" + playerNumber);
        float verticalDir = Input.GetAxis("Vertical_" + playerNumber);
        Vector3 moveBy = new Vector3(horizontalDir, verticalDir, 0);
        if (prevMoveBy != moveBy)
        {
            if (moveBy.x == 1f)
            {
                m_Animator.SetTrigger("move_right");
            } else if (moveBy.x == -1f)
            {
                m_Animator.SetTrigger("move_left");
            }
            else if (moveBy.y == -1f)
            {
                m_Animator.SetTrigger("move_down");
            }
            else if (moveBy.y == 1f)
            {
                m_Animator.SetTrigger("move_up");
            }
            else
            {
                m_Animator.SetTrigger("idle");
            }

           
        }

        prevMoveBy = moveBy;
        hitter = Physics2D.Raycast(transform.position + bottomRayOffset, moveBy, 0.2f);
        if (hitter.collider != null)
        {
            if (hitter.collider.tag == "booster")
            {
                if (!booster.boostStarted)
                {
                    booster.initBoost(movementSpeed);
                }
            }
            else
            {
                moveBy = new Vector3(0, 0, 0);
                Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hitter.distance, Color.yellow, 5);
            }
           
        }
        transform.Translate(moveBy * movementSpeed * Time.deltaTime);
        GetComponent<SpriteRenderer>().sortingOrder = (Mathf.RoundToInt(52 - transform.position.y) * 100) + 10;

        decreaseIfBoost();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
