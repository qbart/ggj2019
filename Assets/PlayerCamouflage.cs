using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCamouflage : MonoBehaviour
{
    public UnityAction onCamouflageStart;
    public UnityAction onCamouflageStop;

    public float camouflageTime = 2f;
    public float cooldownTime = 2f;

    public MapEntity.Section[] sections;

    Renderer spriteRenderer;
    Renderer pooferRenderer;
    Animator pooferAnimator;


    public bool isOn
    {
        get
        {
            return running;
        }
    }

    bool running;
    bool allowNextCamouflage;
    List<GameObject> poofedObjs;

    void Start()
    {
        poofedObjs = new List<GameObject>();
        running = false;
        allowNextCamouflage = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        var animators = GetComponentsInChildren<Animator>();
        pooferAnimator = animators[animators.Length - 1];
        pooferRenderer = renderers[renderers.Length - 1];
        pooferRenderer.enabled = false;
        pooferAnimator.enabled = false;

        onCamouflageStart += camouflageStarted;
        onCamouflageStop += camouflageStopped;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (allowNextCamouflage)
                StartCoroutine(camouflage());
        }
    }

    IEnumerator camouflage()
    {
        Debug.Log("cam begin");

        allowNextCamouflage = false;
        running = true;

        if (onCamouflageStart != null)
            onCamouflageStart();

        yield return new WaitForSeconds(0.2f);

        pooferRenderer.enabled = false;
        pooferAnimator.enabled = false;
        poof();

        yield return new WaitForSeconds(camouflageTime - 0.2f);

        if (onCamouflageStop != null)
            onCamouflageStop();

        running = false;
        Debug.Log("cam end");
        StartCoroutine(cooldown());
    }

    IEnumerator cooldown()
    {
        Debug.Log("cooldown begin");
        yield return new WaitForSeconds(cooldownTime);
        allowNextCamouflage = true;
        Debug.Log("cooldown end");
    }


    void poof()
    {
        var prefab = sections[0].obstacles[1];
        var offsetX = 0f;
        if (prefab.GetComponent<BoxCollider2D>())
        {
            var col = prefab.GetComponent<BoxCollider2D>();
            offsetX = col.size.x / 2;
        }
        var instance = Instantiate(prefab, transform.position + new Vector3(offsetX, 0, 0), Quaternion.identity, transform);
        instance.GetComponentInChildren<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;

        poofedObjs.Add(instance);
    }

    void camouflageStarted()
    {
        pooferAnimator.enabled = true;
        pooferRenderer.enabled = true;
        spriteRenderer.enabled = false;

        pooferAnimator.Play(0);
    }

    void camouflageStopped()
    {
        foreach (var o in poofedObjs)
            Destroy(o);

        poofedObjs.Clear();

        pooferAnimator.enabled = false;
        pooferRenderer.enabled = false;
        spriteRenderer.enabled = true;
    }

}
