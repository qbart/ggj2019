using UnityEngine;

public class Player2 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        else if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
        }
        else if (Input.GetKey("w"))
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
        else if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
    }
}
