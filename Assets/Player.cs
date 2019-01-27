using Application;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Booster))]


public class Player : MonoBehaviour
{
    Animator m_Animator;
    public int playerNumber;

    RaycastHit2D hitter;

    public LayerMask obstacleMask;
    public LayerMask otherPlayerMask;
    public float movementSpeed;
    public Vector3 bottomRayOffset = new Vector3(0f, -0.72f, 0);
    Vector3 prevMoveBy = Vector3.zero;
    Vector3 moveBy;


    Booster booster;
    PlayerCamouflage camouflage;

    public void boost()
    {
        if (!booster.isStarted)
        {
            booster.boost();
        }
    }

    private void Start()
    {
     
        booster = GetComponent<Booster>();
        booster.onEffectStart += boostStarted;
        booster.onEffectStop += boostStopped;

        camouflage = GetComponentInChildren<PlayerCamouflage>();

        m_Animator = gameObject.GetComponent<Animator>();
        moveBy = Vector3.zero;

        Vector2 randomPosition = new Vector2(Random.Range(7, 46), Random.Range(7, 46));
        transform.position = randomPosition;
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
        hitter = Physics2D.Raycast(
            transform.position + bottomRayOffset,
            moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime * 1.5f,
            0.3f,
            obstacleMask

        );
        Debug.DrawRay(transform.position + bottomRayOffset, moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime * 1.5f, Color.yellow, 0.5f);

        if (hitter.collider != null)
        {
            if (!hitter.collider.isTrigger)
            {
                moveBy = Vector3.zero;
            }
        }


        if (playerNumber == 1)
        {
            Collider2D otherPlayerHit = Physics2D.OverlapCircle(
            transform.position + bottomRayOffset,
            0.6f,
            otherPlayerMask
        );

            if (otherPlayerHit != null)
            {
                SceneManager.LoadScene("WitchWin");
            }
        }

        transform.Translate(moveBy * (movementSpeed + booster.currentSpeed) * Time.deltaTime);
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(5200 - (transform.position.y + bottomRayOffset.y) * 100) - playerNumber;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + bottomRayOffset, new Vector3(0.1f, 0.1f, 0));
        //Gizmos.DrawSphere(transform.position + bottomRayOffset, 0.6f);
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
