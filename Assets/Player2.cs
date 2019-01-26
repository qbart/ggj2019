using UnityEngine;

public class Player2 : MonoBehaviour
{
    Animator m_Animator;

    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    public Vector3 moveBy = new Vector3(0, 0, 0);

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        direction = Direction.NONE;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + bottomRayOffset, Vector2.right, 0.2f);

        Direction previousDirection = direction;

        if (Input.GetKey("d"))
        {
            moveBy = Vector3.right;
            direction = Direction.RIGHT;
        }
        else if (Input.GetKey("a"))
        {
            moveBy = Vector3.left;
            direction = Direction.LEFT;
        }
        else if (Input.GetKey("w"))
        {
            moveBy = Vector3.up;
            direction = Direction.UP;
        }
        else if (Input.GetKey("s"))
        {
            moveBy = Vector3.down;
            direction = Direction.DOWN;
        }
        else
        {
            direction = Direction.NONE;
        }

        if (hit.collider != null)
        {
            //Display the point in world space where the ray hit the collider's surface.
            Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow, 5);
            Debug.Log(hit.point);
            Debug.Log("HIT");
            moveBy = new Vector3(0, 0, 0);
        }

        if (previousDirection != direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    m_Animator.SetTrigger("move_down");
                    break;
                case Direction.UP:
                    m_Animator.SetTrigger("move_up");
                    break;
                case Direction.LEFT:
                    m_Animator.SetTrigger("move_left");
                    break;
                case Direction.RIGHT:
                    m_Animator.SetTrigger("move_right");
                    break;
                case Direction.NONE:
                    m_Animator.SetTrigger("idle");
                    break;
            }
        }
        transform.Translate(moveBy * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
