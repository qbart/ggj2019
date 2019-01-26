using UnityEngine;
namespace Application
{
    public class Booster : MonoBehaviour
    {
        public bool isStarted = false;
        public float boostValue = 1.5f;
        float initialSpeed;
        public float decayIncreaser = 0.5f;
        public Booster() { }

        public void boost(Player player)
        {
            isStarted = true;
            initialSpeed = player.movementSpeed;
            player.movementSpeed += boostValue;
        }
        public void decreaseToBoostEnd(Player player)
        {
            if (player.movementSpeed <= initialSpeed)
            {
                player.movementSpeed = initialSpeed;
                isStarted = false;
            }
            else
            {
                player.movementSpeed -= (Time.deltaTime * decayIncreaser);
            }
        }
    }
}
