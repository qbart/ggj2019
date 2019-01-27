using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject player;

    public float left;
    public float right;
    public float top;
    public float bottom;

    bool first;

    private void Start()
    {
        first = true;
    }

    void Update()
    {
        if (first)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            first = false;
        }

        var playerPos = player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerPos.x, playerPos.y, transform.position.z), Time.deltaTime * 4);
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, left, right);
        pos.y = Mathf.Clamp(pos.y, bottom, top);
        transform.position = pos;
    }
}