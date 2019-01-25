using UnityEngine;

public class Player : MonoBehaviour
{
    Animator m_Animator;
    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetKey("right"))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
            m_Animator.SetTrigger("move_right");
        }
        else if (Input.GetKey("left"))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
            m_Animator.SetTrigger("move_left");
        }
        else if(Input.GetKey("up"))
        {
            transform.Translate(Vector3.up * Time.deltaTime);
            m_Animator.SetTrigger("move_up");
        }
        else if (Input.GetKey("down"))
        {
            transform.Translate(Vector3.down * Time.deltaTime);
            m_Animator.SetTrigger("move_down");
        }
        else
        {
            m_Animator.SetTrigger("idle");
        }
    }
}
