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

    Renderer spriteRenderer;

    public bool isOn
    {
        get
        {
            return running;
        }
    }

    bool running;
    bool allowNextCamouflage;

    void Start()
    {
        running = false;
        allowNextCamouflage = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

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

        yield return new WaitForSeconds(camouflageTime);

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

    void camouflageStarted()
    {
        spriteRenderer.enabled = false;
    }

    void camouflageStopped()
    {
        spriteRenderer.enabled = true;
    }

}
