using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{

    public float timeLeft; 
    public Text countdown; 

    void Start()
    {
        StartCoroutine("LoseTime");
        Time.timeScale = 1;
    }

    void Update()
    {
        float minutes = Mathf.Floor(timeLeft / 60f);
        float seconds = Mathf.RoundToInt(timeLeft % 60f);
        string minutesText = "00";
        string secondsText = "00";

       
            minutesText = minutes.ToString("00");
     

            secondsText = seconds.ToString("00");

        countdown.text = minutesText + ":" + secondsText;

        if(timeLeft <= 0)
        {
            SceneManager.LoadScene("HouseWin", LoadSceneMode.Additive);
        }
    }

    //Simple Coroutine
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }

    }
}