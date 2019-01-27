using Application;
using UnityEngine;
[RequireComponent(typeof(Booster))]


public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;

    RaycastHit2D hitter;

    public float movementSpeed;
    public Vector3 offset = new Vector3(0, -0.51f, 0);
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    public Vector3 prevMoveBy = Vector3.zero;
    Booster booster;
    Vector3 moveBy;

    private void Start()
    {
        booster = GetComponent<Booster>();
        booster.onEffectStart += boostStarted;
        booster.onEffectStop += boostStopped;
        m_Animator = gameObject.GetComponent<Animator>();
        moveBy = Vector3.zero;
    }

    void UpdateAnimations()
    {
        string boostSuffix = booster.isStarted ? "_boost" : "";
        if (moveBy.x == 1f)
        {
            m_Animator.SetTrigger("move_right" + boostSuffix);
        }
        else if (moveBy.x == -1f)
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

    void Update()
    {
        float horizontalDir = Input.GetAxis("Horizontal_" + playerNumber);
        float verticalDir = Input.GetAxis("Vertical_" + playerNumber);
        moveBy = new Vector3(horizontalDir, verticalDir, 0);
       
        if (prevMoveBy != moveBy)
        {
            UpdateAnimations();
        }

        prevMoveBy = moveBy;
        hitter = Physics2D.Raycast(transform.position + bottomRayOffset, moveBy, 0.2f);
        if (hitter.collider != null)
        {
            if (hitter.collider.tag == "booster")
            {
                if (!booster.isStarted)
                {
                    booster.boost();
                }
            }
            else
            {
                moveBy = Vector3.zero;
                Debug.DrawRay(transform.position + bottomRayOffset, transform.TransformDirection(Vector3.right) * hitter.distance, Color.yellow, 5);
            }
           
        }
        transform.Translate(moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime);
        GetComponent<SpriteRenderer>().sortingOrder = (Mathf.RoundToInt(52 - transform.position.y) * 100) + 10 + playerNumber;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset, new Vector3(0.2f, 0.4f, 0.2f));
    }

    void boostStarted()
    {
        UpdateAnimations();
    }

    void boostStopped()
    {
        UpdateAnimations();
    }
}
