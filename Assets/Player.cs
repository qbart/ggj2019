using UnityEngine;

public class Player : MonoBehaviour
{
    Animator m_Animator;

    enum Direction { NONE, LEFT, UP, RIGHT, DOWN };

    Direction direction;

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        direction = Direction.NONE;
    }

    void Update()
    {
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
}
