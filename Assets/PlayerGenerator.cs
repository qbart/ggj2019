using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    public Player firstPlayer;

    private void generatePositions()
    {
        Vector2 player1RandomPosition = new Vector2(Random.Range(7, 46), Random.Range(7, 46));
        Vector2 player2RandomPosition = new Vector2(Random.Range(7, 46), Random.Range(7, 46));
        //transform.position = randomPosition;
        var playerBetweenDistance = Vector2.Distance(player1RandomPosition, player2RandomPosition);
        var distanceToLow = playerBetweenDistance < 30;

        if (distanceToLow)
        {
            generatePositions();
            return;
        }

        transform.position = player2RandomPosition;
        firstPlayer.transform.position = player1RandomPosition;
    }

    void Start()
    {
        generatePositions();
        Debug.Log("Generator start");



        Debug.Log(firstPlayer.transform.position);

    }
}
