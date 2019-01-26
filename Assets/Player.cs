using UnityEngine;

public class Player : MonoBehaviour
{
    Animator m_Animator;

    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        direction = Direction.NONE;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + bottomRayOffset, Vector2.right, 0.2f);
        if (hit.collider != null)
        {
            //Display the point in world space where the ray hit the collider's surface.
            Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow, 5);
            Debug.Log(hit.point);
            Debug.Log("HIT");
        }
        Direction previousDirection = direction;

        if(Input.GetKey("right"))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
            direction = Direction.RIGHT;
        }
        else if (Input.GetKey("left"))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
            direction = Direction.LEFT;
        }
        else if(Input.GetKey("up"))
        {
            transform.Translate(Vector3.up * Time.deltaTime);
            direction = Direction.UP;
        }
        else if (Input.GetKey("down"))
        {
            transform.Translate(Vector3.down * Time.deltaTime);
            direction = Direction.DOWN;
        }
        else
        {
            direction = Direction.NONE;
        }

        if (previousDirection != direction)
        {
            switch(direction)
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
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }
}
