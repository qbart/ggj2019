using Application;
using UnityEngine;


public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;
    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;
    RaycastHit2D hitter;

    public float movementSpeed;
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    public Vector3 prevMoveBy = new Vector3(0, 0, 0);
    Booster booster = new Booster();

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        direction = Direction.NONE;
    }

    void Update()
    {
        Direction previousDirection = direction;
        float horizontalDir = Input.GetAxis("Horizontal_" + playerNumber);
        float verticalDir = Input.GetAxis("Vertical_" + playerNumber);
        Vector3 moveBy = new Vector3(horizontalDir, verticalDir, 0);
        string boostSuffix = booster.isStarted ? "_boost" : "";
        if (prevMoveBy != moveBy)
        {
            if (moveBy.x == 1f)
            {
                m_Animator.SetTrigger("move_right" + boostSuffix);
            } else if (moveBy.x == -1f)
            {
                m_Animator.SetTrigger("move_left" + boostSuffix);
            }
            else if (moveBy.y == -1f)
            {
                m_Animator.SetTrigger("move_down" + boostSuffix);
            }
            else if (moveBy.y == 1f)
            {
                m_Animator.SetTrigger("move_up" + boostSuffix);
            }
            else
            {
                m_Animator.SetTrigger("idle" + boostSuffix);
            }

           
        }

        prevMoveBy = moveBy;
        hitter = Physics2D.Raycast(transform.position + bottomRayOffset, moveBy, 0.2f);
        if (hitter.collider != null)
        {
            if (hitter.collider.tag == "booster")
            {
                if (!booster.isStarted)
                {
                    booster.boost(this);
                }
            }
            else
            {
                moveBy = new Vector3(0, 0, 0);
                Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hitter.distance, Color.yellow, 5);
            }
           
        }
        transform.Translate(moveBy * movementSpeed * Time.deltaTime);
        if (booster.isStarted)
        {
            booster.decreaseToBoostEnd(this);
        }
        GetComponent<SpriteRenderer>().sortingOrder = (Mathf.RoundToInt(52 - transform.position.y) * 100) + 10 + playerNumber;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
