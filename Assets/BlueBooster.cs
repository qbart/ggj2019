using UnityEngine;

public class BlueBooster : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<Player>();
            player.boost();
            Destroy(gameObject);
        }
    }
}
