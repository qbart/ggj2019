using System;
namespace Application
{
    public class Booster
    {
        public bool boostStarted = false;
        public float boost = 1;
        public float speed;
        float initialSpeed;

        public Booster(float initialSpeed)
        {
            this.initialSpeed = initialSpeed;
        }
        public void initBoost(float fromSpeed)
        {
            boostStarted = true;
            this.speed = fromSpeed;
            speed += boost;
        }
        public float decreaseToBoostEnd()
        {
            if (speed <= initialSpeed)
            {
                boostStarted = false;
                speed = initialSpeed;
                return speed;
            }
            else
            {
                speed -= (Time.deltaTime * 0.3f);
                return speed;
            }
        }
    }
}
