using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndController : MonoBehaviour
{
    public GameObject timelinessObj;
    private TextMeshProUGUI timeliness;
    public GameObject efficiencyObj;
    private TextMeshProUGUI efficiency;
    public GameObject letter;
    // Start is called before the first frame update
    void Start() {
        timeliness = timelinessObj.GetComponent<TextMeshProUGUI>();
        efficiency= efficiencyObj.GetComponent<TextMeshProUGUI>();
        
        string levelName = PlayerPrefs.GetString("level");
        Debug.Log(levelName);
        switch (levelName) {
            case "Level1":
                letter.transform.Find("content1").gameObject.SetActive(true);
                break;
            case "Level2":
                letter.transform.Find("content2").gameObject.SetActive(true);
                break;
            case "Level3":
                letter.transform.Find("content3").gameObject.SetActive(true);
                break;
        }
        efficiency.text = PlayerPrefs.GetInt("flaps") + " flaps";
    }

    // Update is called once per frame
    void Update() {
        //float t = Time.timeSinceLevelLoad; // time since scene loaded
        float t = PlayerPrefs.GetFloat("timePassed");
        float milliseconds = (Mathf.Floor(t * 100) % 100); // calculate the milliseconds for the timer
        int seconds = (int) (t % 60); // return the remainder of the seconds divide by 60 as an int
        t /= 60; // divide current time y 60 to get minutes
        int minutes = (int) (t % 60); //return the remainder of the minutes divide by 60 as an int
        t /= 60; // divide by 60 to get hours
        int hours = (int) (t % 24); // return the remainder of the hours divided by 60 as an int
        timeliness.text = string.Format("{0}:{1}:{2}.{3}", 
            hours.ToString("00"), 
            minutes.ToString("00"), 
            seconds.ToString("00"), 
            milliseconds.ToString("00"));
        
    }

    public void FinishLevel() {
        if(letter.activeSelf == false) {
            PlayerPrefs.SetInt("flaps", 0);
            SceneManager.LoadScene("MainMenu");
        }
	}
}
