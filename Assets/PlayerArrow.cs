using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float showArrowEvery = 5f;
    public float arrowVisibleTime = 1f;

    public GameObject other;

    Renderer arrowRenderer;
    Transform arrowTransform;

    bool first;

    void Start()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        arrowRenderer = renderers[renderers.Length - 1];
        arrowRenderer.enabled = false;

        var transforms = GetComponentsInChildren<Transform>();
        arrowTransform = transforms[transforms.Length - 1];

        first = true;
    }

    void Update()
    {
        if (first)
        {
            first = false;
            StartCoroutine(showArrow());
        }

    }

    IEnumerator showArrow()
    {
        while (true)
        {
            yield return new WaitForSeconds(showArrowEvery);

            var dir = other.transform.position - transform.position;
           
            var rot = Quaternion.LookRotation(dir, -Vector3.forward);
            rot.x = 0;
            rot.y = 0;
            arrowTransform.rotation = rot;


            arrowRenderer.enabled = true;

            yield return new WaitForSeconds(arrowVisibleTime);
            arrowRenderer.enabled = false;
        }
    }
}
