using UnityEngine;
namespace Application
{
    public class Booster : MonoBehaviour
    {
        public bool boostStarted = false;
        public float boost = 1;
        public float initialSpeed = 1;
        float speed;
        public Booster() { }

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
