using UnityEngine;

public class Player : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKey("right"))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        else if (Input.GetKey("left"))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
        }
        else if(Input.GetKey("up"))
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
        else if (Input.GetKey("down"))
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
    }
}
