using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    [Header("Joy Stick")]
    public TextMeshProUGUI joyStickPositionText;
    //public TextMeshProUGUI 
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clipSpaceJunkMission;
    public AudioClip clipWormholeMission;
    public AudioClip clipExpansion;
    public AudioClip clipExit;
    [Header("Misc")]
    public bool canvasTest;
    public bool playerPrefActive;


    [Header("Canvases")]
    public GameObject canvasLevel1;
    public GameObject canvasLevel2;
    public GameObject canvasGameStart ;
    [Header("Level 2 Button")]
    public Button level2Button; //just for changing "interactable" ---  level1Button so far is not modified in script
    private static bool level1IntroAlreadyDisplayed, level2IntroAlreadyDisplayed;
    //public static int joyStickPosition;
    readonly string jRight = "Bottom Right";
    readonly string jCenter = "Bottom Center";
    readonly string jLeft = "Bottom Left";
    public LevelChanger levelChanger;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log("Hello from GameMenuScript.cs");
        //canvasLevel1.enabled = false;
        //canvasLevel2.enabled = false;
        //canvasGameStart.enabled = true;
        canvasLevel1.SetActive(false);
        canvasLevel2.SetActive(false);
        canvasGameStart.SetActive(true);

        if (PlayerPrefs.HasKey("LevelToPlay"))
        {
            var x = PlayerPrefs.GetInt("LevelToPlay");
            //Debug.Log("FOUND A PLAYER PREF for LevelToPlay    x = " + x);
            if (playerPrefActive) LoadPlayerPrefLevel(x);
            if (x > 1) level2Button.interactable = true;
        }
        // else Debug.Log ("NO PLAYER PREF FOUND");
        if (!PlayerPrefs.HasKey("JoyStickPosition"))  //we never set a position
        {
            PlayerPrefs.SetInt("JoyStickPosition", 1); //right side -default
        }

        SetTextForJoyStickPosition();
        //joyStickPosition = PlayerPrefs.GetInt("JoyStickPosition");

      if (canvasTest) Debug.Log("WARNING: Canvas Test is ON so you're not going anywhere. But you can test buttons/sounds!");
      //  levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();  // 3/14/22  put in public/Inspector  seems ok
    }
    void SetTextForJoyStickPosition()
    {
        var joyStickPosition = PlayerPrefs.GetInt("JoyStickPosition");
        switch (joyStickPosition)
        {
            case 1:
                joyStickPositionText.text = jRight;
                break;
            case 2:
                joyStickPositionText.text = jCenter;
                break;
            case 3:
                joyStickPositionText.text = jLeft;
                break;
            default:
                break;

        }

    }

    void LoadPlayerPrefLevel(int sceneIndex)
    {
        levelChanger.FadeToLevel(2);
        //SceneManager.LoadScene(sceneIndex);
    }
    // The levelButtons just swap canvas from Select Level to respective canvas
    public void OnLevel1ButtonPressed()
    {
        audioSource.clip = clipSpaceJunkMission;
        audioSource.Play();
        if (level1IntroAlreadyDisplayed)
        {
            Debug.Log("level1IntroAlreadyDisplayed  " + level1IntroAlreadyDisplayed);
            levelChanger.FadeToLevel(2);
            return;
        }

        if (!canvasTest)
        {
            // canvasGameStart.enabled = false;
            canvasGameStart.SetActive(false);
            if (!canvasLevel1.activeSelf) canvasLevel1.SetActive(true);
           // canvasLevel1.enabled = true;
            level1IntroAlreadyDisplayed = true;
        }
    }
    public void OnLevel2ButtonPressed()
    {
        audioSource.clip = clipWormholeMission;
        audioSource.Play();
        if (level2IntroAlreadyDisplayed)
        {
            levelChanger.FadeToLevel(3);
            return;
        }

        if (!canvasTest)
        {
            //canvasGameStart.enabled = false;
            //canvasLevel2.enabled = true;
            canvasGameStart.SetActive(false);
            canvasLevel2.SetActive(true);
            level2IntroAlreadyDisplayed = true;
        }
    }
    public void OnPlayMission1ButtonPressed()
    {
        audioSource.clip = clipSpaceJunkMission;
        audioSource.Play();
        if (!canvasTest)
        {
            levelChanger.FadeToLevel(2);
        }
    }
    public void OnPlayMission2ButtonPressed()
    {
        audioSource.clip = clipWormholeMission;
        audioSource.Play();
        if (!canvasTest)
        {
            levelChanger.FadeToLevel(3);
        }
    }
    public void OnExitButtonPressed()
    {
        // Debug.Log("EXIT Pressed");
        audioSource.clip = clipExit;
        audioSource.Play();
        if (!canvasTest)
        {
            Application.Quit();
           // Destroy(this);
        }
    }
    public void ExpansionButtonPressed()
    {
        audioSource.clip = clipExpansion;
        audioSource.Play();
        if (!canvasTest)
        {
            SceneManager.LoadScene(1);
        }
    }
}
