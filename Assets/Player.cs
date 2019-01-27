using Application;
using UnityEngine;
[RequireComponent(typeof(Booster))]


public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;

    RaycastHit2D hitter;

    public float movementSpeed;
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    Vector3 prevMoveBy = Vector3.zero;
    Vector3 moveBy;

    Booster booster;
    PlayerCamouflage camouflage;

    private void Start()
    {
        booster = GetComponent<Booster>();
        booster.onEffectStart += boostStarted;
        booster.onEffectStop += boostStopped;

        camouflage = GetComponent<PlayerCamouflage>();

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

        if (camouflage != null && camouflage.isOn)
            moveBy = Vector3.zero;

        if (prevMoveBy != moveBy)
        {
            UpdateAnimations();
        }

        prevMoveBy = moveBy;
        hitter = Physics2D.Raycast(transform.position + bottomRayOffset, moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime * 1.5f, 0.3f);
        Debug.DrawRay(transform.position + bottomRayOffset, moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime * 1.5f, Color.yellow, 0.5f);
        if (hitter.collider != null)
        {
            if (hitter.collider.CompareTag("booster"))
            {
                if (!booster.isStarted)
                {
                    booster.boost();
                }
            }
            else
            {
                if (!hitter.collider.isTrigger)
                {
                    moveBy = Vector3.zero;
                }
            }
        }

        transform.Translate(moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime);
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(5200 - (transform.position.y + bottomRayOffset.y) * 100) - playerNumber;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + bottomRayOffset, new Vector3(0.1f, 0.1f, 0));
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
