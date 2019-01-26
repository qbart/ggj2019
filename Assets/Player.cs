using UnityEngine;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;
    public float speed;
    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;
    RaycastHit2D hitter;
    RaycastHit2D hitter2;
   
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    public Vector3 prevMoveBy = new Vector3(0, 0, 0);

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

                GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y / 80f) * -1;
            }
            else
            {
                m_Animator.SetTrigger("idle");
            }

            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y);
        }

        prevMoveBy = moveBy;

        hitter = Physics2D.Raycast(transform.position + bottomRayOffset, moveBy, 0.2f);

        if (hitter.collider != null)
        {
            if (hitter.collider.tag == "booster")
            {
                speed = 2;
                Task.Delay(2000).ContinueWith((t) =>
                {
                    speed = 1;
                });
            }
            else
            {
                moveBy = new Vector3(0, 0, 0);
                //Display the point in world space where the ray hit the collider's surface.
                Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hitter.distance, Color.yellow, 5);
                Debug.Log(hitter.collider.tag);
                Debug.Log("HIT");
            }
           
        }
        transform.Translate(moveBy * speed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
