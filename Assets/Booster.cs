using UnityEngine;
using UnityEngine.Events;

namespace Application
{
    public class Booster : MonoBehaviour
    {

        public float effectTime = 2f;
        public float maxDecayTime = 1f;
        [Range(0, 1)]
        public float decayValue = 0.8f;
        public float speedBoost = 1.5f;

        bool running;
        float timer, decayTimer;
        float speed;

        public UnityAction onEffectStart;
        public UnityAction onEffectStop;

        public bool isStarted
        {
            get
            {
                return running;
            }
        }

        public float currentSpeed
        {
            get
            {
                return running ? speed : 0;
            }
        }

        public void boost()
        {
            running = true;
            timer = effectTime;
            decayTimer = maxDecayTime;
            speed = speedBoost;

            if (onEffectStart != null)
                onEffectStart();
        }

        void Start()
        {
            running = false;
        }

        void Update()
        {
            if (running)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    decayTimer -= Time.deltaTime;
                    speed *= decayValue;

                    if (decayTimer <= 0 || speed <= 0)
                    {
                        running = false;
                        if (onEffectStop != null)
                            onEffectStop();
                    }
                }
            }
        }
    }
}