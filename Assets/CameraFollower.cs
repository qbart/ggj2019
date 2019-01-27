using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject player;
    public float minBoundary = 0;
    public float maxBoundary = 53;

    void Update()
    {
        var playerDistance = Vector2.Distance(player.transform.position, transform.position);
        var playerPos = player.transform.position;

        var playerInMap = playerPos.x >= minBoundary && playerPos.x <= maxBoundary && playerPos.y >= minBoundary && playerPos.y <= maxBoundary;


        if (playerDistance > 2.5 && playerInMap)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerPos.x, playerPos.y, -10), Time.deltaTime * 4);
        }
    }
}
