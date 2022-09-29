using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
//using StarterAssets;
using Slider = UnityEngine.UI.Slider;

public class ScoreKeeper : MonoBehaviour
{
    GenRandomBackground genRandomBackground;
    public static int scoreTotal, sceneBuildIndex;

    public GameObject junkInfoCanvasPart, wormholeInfoCanvasPart, canvas3DObjects, mainCanvas, speedSelectorCanvas, canvasStatsLevel1;
    public static bool startNextLevel;
    public bool doATestReset;
    static TextMeshProUGUI scoreText, shipSpeedValueText, missionAccomplishedText, timerText, goInfoText, currentText, collectBonusText;
    TextMeshProUGUI speedNumber;
    public TextMeshProUGUI highScore;   //for now just a little display on bottom of level1 playing scene

    [Header("Level1 results items")]
    public TextMeshProUGUI endRoundComment;
    public TextMeshProUGUI totalHits;
    public TextMeshProUGUI satsAlive;
    public TextMeshProUGUI teslaHits;
    public TextMeshProUGUI satsHit;
    public TextMeshProUGUI tallyTotalHits;
    public TextMeshProUGUI tallySatsAlive;
    public TextMeshProUGUI tallyTeslaHits;
    public TextMeshProUGUI tallySatsHit;
    public TextMeshProUGUI bestTotalHits;
    public TextMeshProUGUI bestSatsAlive;
    public TextMeshProUGUI bestTeslaHits;
    public TextMeshProUGUI bestSatsHit;
    public TextMeshProUGUI bestTotalScore;
    public TextMeshProUGUI statsScore;
    public TextMeshProUGUI labelNewBestGame;
    public TextMeshProUGUI labelLastBestGame;
    public TextMeshProUGUI labelForNewBestTotalScore;

    public TextMeshProUGUI newHighScoreIndicator;
    // [Header("END OF Level1 results items")]
    [Header("Audio Stuff")]

    public AudioSource audioInfoButtonOk;
    public AudioSource audioInfoButton;
    public AudioSource audioStartNewGame;
    public static AudioSource audioBackground;
    public AudioClip clipInfoButtonOk;
    public AudioClip clipInfoButton;
    public AudioClip clipStartNewGame;
    public AudioClip clipBackGround;
    public AudioClip clipEngineSound;
    public AudioClip clipTheCan;
    public AudioClip clipApert;
    AudioSource audioSource;
    [Header("END Audio Stuff")]
    private static IEnumerator coroutine;

    //private static GameObject gameInfoPanel;
    private static GameObject restartButton, goButton, junkSceneInfoButton,
        exitButton, setupButton, statsButton;// , setupOKButton;
    [Header("JoyStick")]
    public GameObject joyStickCanvasRight;
    public GameObject joyStickCanvasCenter;
    public GameObject joyStickCanvasLeft;
    public GameObject joyStickRightText;
    public GameObject joyStickCenterText;
    public GameObject joyStickLeftText;
    public GameObject joyStickPositionTextOnMain;


    [Header("Misc Level 1")]
    public int initialRoundTime = 60;
    public int levelOneSlowSpeed = 90;
    public int levelOneMediumSpeed = 110;
    public int levelOneFastSpeed = 130;
    public int bonusIncrement = 400;  //score to add for each bonus eligible object (satellites here for now) 
    public int scoreToAchieveNextLevel = 20000;
    public int satellitesRemainingToAchieveNextLevel = 6;
    int currentBestHitsScore, currentBestTeslaHitsScore, currentBestSatsAliveScore, currentBestSatsHit, currentBestTotalScore;
    int currentLevel2HighScore;
    bool previousLevel1PlayerPrefStatsKeysFound, previousLevel2PlayerPrefStatsKeysFound;
    [HideInInspector]
    public int totalGoodHits, totalTeslaHits, totalSatelliteBadHits, totalSatellitesLeftAlive;
    [HideInInspector]
    public float roundTime;   //timeAccumulated,timeRemaining, //these 2 commented 1/25/22
    public bool roundInProgress;
    [Header("Level 2 Specifics")]
    public GameObject wormholeSceneInfoButton;
    public GameObject gOShipSpeedSlider;
    public TextMeshProUGUI level2HighScore;   //for now just a little display on bottom of level2 playing scene
    public GameObject level2ShieldBackground;

    public delegate void GoPressed();
    public static event GoPressed GoButtonPressed;

    public delegate void TimerExpired();
    public static event TimerExpired SceneXTimerExpired;

    public delegate void RestartPressed();  //Now called "Continue"
    public static event RestartPressed RestartButtonPressed;  //Say "RestartButtonPressed();" to emit the Event or "RestartButtonPressed?.Invoke();" without the if shit

    int levelInProgress; // we will subtract 1 from this to account for 1st 2 sceneBuildIndexes 
    LevelChanger levelChanger;
    Animator animHighScoreIndicator;

    WhichStringToShow StringToShow;
    enum WhichStringToShow   //Meant to be shared across multiple game levels
    {
        RoundLost,
        RoundWon,
        NoSatellitesLeft
    };

    bool toggleGOSlow, toggleGOMedium, toggleGOFast;
    readonly string level1Slow = "This level speed is set Slow";
    readonly string level1Medium = "This level speed is set Medium";
    readonly string level1Fast = "This level speed is set Fast";

    readonly string jRight = "Entire Game Joystick is on the Right";
    readonly string jCenter = "Entire Game Joystick is in the Center";
    readonly string jLeft = "Entire Game Joystick is on the Left";

    //Camera cam;   // keep in case 

    Slider shipSpeedSlider;
    FirstPersonController firstPersonController;  //Find this on the Player GameObject
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;  // keep in case 

        levelInProgress = SceneManager.GetActiveScene().buildIndex - 1; //subtract 1 to account for setup scenes 0 and 1 so we get 1 for level1, etc
                                                                        // if (SceneManager.GetActiveScene().buildIndex == 2)  // 3/27/22 start of segregating levels 1 and 2 


        if (levelInProgress == 2) mainCanvas.SetActive(false);   //kludge for L2 why it is needed and why it works IDK yet 
        SetUpJoyStickPosition();
        SetJoyStickCanvas(true);
        if (levelInProgress == 2) mainCanvas.SetActive(true);    //kludge for L2 why it is needed and why it works IDK yet 

        if (levelInProgress == 1)
        {
            genRandomBackground = GameObject.Find("GenRandomBackground").GetComponent<GenRandomBackground>();
            if (!mainCanvas.activeSelf) mainCanvas.SetActive(true);

            collectBonusText = GameObject.Find("CollectBonus").GetComponent<TextMeshProUGUI>();
            collectBonusText.enabled = false;

            animHighScoreIndicator = GameObject.Find("NewHighScore").GetComponent<Animator>();
            animHighScoreIndicator.enabled = false;
            newHighScoreIndicator.enabled = false;
            if (PlayerPrefs.HasKey("Level1Speed"))  //if we have a Key then we have to set proper speed text beside main Canvas Setup Button 
            {
                //Debug.Log("ScoreKeeper: speed key found in start " + x);
                //genRandomBackground.objectSpeed = x;  // 3/6 move into switch/case below 
                currentText = GameObject.Find("CurrentText").GetComponent<TextMeshProUGUI>();
                var x = PlayerPrefs.GetInt("Level1Speed");
                switch (x)
                {
                    case 1:       //this has to reference the public int variable    i.e.     public int levelOneSlowSpeed = 90;  110, 130 etc
                        currentText.text = level1Slow; // "This level speed is set Slow";
                        genRandomBackground.objectSpeed = levelOneSlowSpeed;
                        StaticBackground.objectSpeed = levelOneSlowSpeed;
                        break;
                    case 2:       //this has to reference the public int variable 
                        currentText.text = level1Medium; // "This level speed is set Medium";
                        genRandomBackground.objectSpeed = levelOneMediumSpeed;
                        StaticBackground.objectSpeed = levelOneMediumSpeed;
                        break;
                    case 3:       //this has to reference the public int variable 
                        currentText.text = level1Fast; // "This level speed is set Fast";
                        genRandomBackground.objectSpeed = levelOneFastSpeed;
                        StaticBackground.objectSpeed = levelOneFastSpeed;
                        break;
                    default:
                        break;
                }
            }
            else    //this is first time user runs game so take the default 
            {
                Debug.Log("speed key NOT found in start  Setting key to levelOneSlowSpeed...");
                PlayerPrefs.SetInt("Level1Speed", 1); //here levelOneSlowSpeed is taken from public int   // change to "1" on 3/6
            }

        }

        // setupOKButton = GameObject.Find("SetupOKButton");  // part of speed/setup canvas
        setupButton = GameObject.Find("SetupButton");
        // if (speedSetupCanvasPart) speedSetupCanvasPart.SetActive(false);
        if (canvasStatsLevel1) canvasStatsLevel1.SetActive(false);
        junkSceneInfoButton = GameObject.Find("JunkSceneInfoButton");
        if (junkInfoCanvasPart) junkInfoCanvasPart.SetActive(false);
        restartButton = GameObject.Find("RestartButton");
        if (restartButton) restartButton.SetActive(false);
        goButton = GameObject.Find("GoButton");
        //  if (goButton) goButton.SetActive(false);
        exitButton = GameObject.Find("ExitButton");
        // if (exitButton) exitButton.SetActive(false);
        statsButton = GameObject.Find("StatsButton");
        if (statsButton) statsButton.SetActive(false);

        // level2 stuff starts here...
        if (levelInProgress == 2)
        {
            if (wormholeInfoCanvasPart) wormholeInfoCanvasPart.SetActive(false);
        }

        levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();
        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        goInfoText = GameObject.Find("GoInfoText").GetComponent<TextMeshProUGUI>();

        missionAccomplishedText = GameObject.Find("MissionAccomplishedText").GetComponent<TextMeshProUGUI>();
        missionAccomplishedText.enabled = false;

        scoreTotal = 0; // 12/12
        scoreText.text = scoreTotal.ToString("#,###");
        roundTime = initialRoundTime;

        coroutine = WaitOnAudio(3);  //These 2 lines wait 3 seconds then start the background audio
        audioSource = GetComponent<AudioSource>();

        switch (levelInProgress)
        {
            case 1:  //Space Junk scene/Level 1
                     //DisableNewStatsLabels(); // 5/27/22
                labelForNewBestTotalScore.enabled = false;
                CheckForLevel1ScoreStatsPlayerPrefs();
                break;
            case 2:
                // if needed to do something w/Wormhole/Level 2
                // Debug.Log("Skeeper switch is in LEVEL2");
                CheckForLevel2ScoreStatsPlayerPrefs();
                SetLevel2ShipSpeed();
                break;
            default:
                break;
        }

    }
    // Update is called once per frame
    //void Update()
    //{

    //}
    void SetUpJoyStickPosition()
    {   //Only based on PlayerPref found on/in Start() -- Button prompted changes handled in "OnButton..." methods
        //However Scene1 (SpaceJunkSweeper) 
        //Debug.Log("from Prefs our joystick position is " + joyStickPosition);

        joyStickCanvasRight.SetActive(false);
        joyStickCanvasCenter.SetActive(false);
        joyStickCanvasLeft.SetActive(false);
        var joyStickPosition = PlayerPrefs.GetInt("JoyStickPosition");

        switch (joyStickPosition)
        {    // here (in each case) we need to decide to manipulate canvases or reposition joystick on the same canvas 
             // and we need to handle both level 1 and 2 main canvases - next time we won't put ANYthing in the player control UI area :<

            case 1:  //Right
                Debug.Log("stick position is RIGHT");
                joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jRight;
                // here we need to see comments above  // below is draft of changing the text of jstick position
                if (joyStickCenterText) joyStickCenterText.SetActive(false);
                if (joyStickLeftText) joyStickLeftText.SetActive(false);
                if (joyStickRightText) joyStickRightText.SetActive(true);
                switch (levelInProgress)  //leave Case 1 for now 
                {
                    case 1:
                        break;
                    case 2:
                        //Move the shield indicator to the LEFT (when joystick goes to CENTER or RIGHT)
                        Transform rectNewRect = level2ShieldBackground.GetComponent<RectTransform>();
                        Vector3 newTransform = new Vector3(69.62f, rectNewRect.position.y, 0f);  //Move indicator to RIGHT  //was 1164
                        level2ShieldBackground.transform.position = newTransform;  //Here is the move to RIGHT
                        break;
                }
                break;
            case 2: //Center
                Debug.Log("stick position is CENTER");
                joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jCenter;
                if (joyStickCenterText) joyStickCenterText.SetActive(true);
                if (joyStickLeftText) joyStickLeftText.SetActive(false);
                if (joyStickRightText) joyStickRightText.SetActive(false);
                switch (levelInProgress)  //leave Case 1 for now 
                {
                    case 1:
                        break;
                    case 2:
                        //Move the shield indicator to the LEFT (when joystick goes to CENTER or RIGHT)
                        Transform rectNewRect = level2ShieldBackground.GetComponent<RectTransform>();
                        Vector3 newTransform = new Vector3(69.62f, rectNewRect.position.y, 0f);  //Move indicator to RIGHT  //was 1164
                        level2ShieldBackground.transform.position = newTransform;  //Here is the move to RIGHT
                        break;
                }
                break;
            case 3: //Left
                Debug.Log("stick position is LEFT");
                joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jLeft;
                if (joyStickCenterText) joyStickCenterText.SetActive(false);
                if (joyStickLeftText) joyStickLeftText.SetActive(true);
                if (joyStickRightText) joyStickRightText.SetActive(false);
                switch (levelInProgress)  //leave Case 1 for now 
                {
                    case 1:

                        break;
                    case 2:
                        //Move the shield indicator to the RIGHT (only when the joystick goes to LEFT)
                        Transform rectNewRect = level2ShieldBackground.GetComponent<RectTransform>();
                        //Debug.Log("rectNewRect = " + rectNewRect.position.x + "  " + rectNewRect.position.y);
                        //Transform rectNewTrans = level2ShieldBackground.GetComponent<Transform>();
                        //Debug.Log("rectNewTrans = " + rectNewTrans.position.x + "  " + rectNewTrans.position.y);
                        Vector3 newTransform = new Vector3(731.5f, rectNewRect.position.y, 0f);  //Move indicator to RIGHT  //was 1164

                        //Debug.Log("v3 newTransform on set x to 731.5  = " + newTransform);
                        //Debug.Log("level2ShieldBackground.x and .y  = " + rectNewRect.position.x + "  " + rectNewRect.position.y);

                        level2ShieldBackground.transform.position = newTransform;  //Here is the move to RIGHT
                        break;
                }
                break;
            default:
                Debug.Log("stick position is RIGHT -- defaulted uh oh");
                break;

        }
        //SetJoyStickCanvas(true);  //placed here to eliminate redundancy in the switch/case logic 
    }
    public void CheckForLevel1ScoreStatsPlayerPrefs() //
    {
        if (!PlayerPrefs.HasKey("BestHitsScore")) //1st time played or keys deleted... int hitsScore, int teslaHitsScore, int satsAliveScore
        {
            PlayerPrefs.SetInt("BestHitsScore", 0);
            PlayerPrefs.SetInt("BestTeslaHitsScore", 0);
            PlayerPrefs.SetInt("BestSatsAliveScore", 0);
            PlayerPrefs.SetInt("BestSatsHit", 0);
            PlayerPrefs.SetInt("BestTotalScore", 0);
        }
        else
        {
            previousLevel1PlayerPrefStatsKeysFound = true;
            currentBestHitsScore = PlayerPrefs.GetInt("BestHitsScore", 0);
            currentBestTeslaHitsScore = PlayerPrefs.GetInt("BestTeslaHitsScore", 0);
            currentBestSatsAliveScore = PlayerPrefs.GetInt("BestSatsAliveScore", 0);
            currentBestSatsHit = PlayerPrefs.GetInt("BestSatsHit", 0);
            currentBestTotalScore = PlayerPrefs.GetInt("BestTotalScore", 0);
        }
        highScore.text = currentBestTotalScore.ToString();
    }
    public void CheckForLevel2ScoreStatsPlayerPrefs() //
    {
        if (!PlayerPrefs.HasKey("Level2HighScore") || doATestReset) //1st time played or keys deleted...
        {
            PlayerPrefs.SetInt("Level2HighScore", 0);
            // level2HighScore.enabled = false;
            level2HighScore.text = 0.ToString();
        }
        else
        {
            previousLevel2PlayerPrefStatsKeysFound = true;
            currentLevel2HighScore = PlayerPrefs.GetInt("Level2HighScore", 0);
            level2HighScore.enabled = true;
        }
        if (level2HighScore) level2HighScore.text = currentLevel2HighScore.ToString("#,###");
    }
    public void UpdateScore(int points)
    {
        scoreTotal += points;
        scoreText.text = scoreTotal.ToString("#,###");
    }
    public void UpdateScore(bool setScoreToZero)
    {
        //scoreText.text = scoreTotal.ToString();
        if (setScoreToZero) scoreTotal = 0;
    }
    public void EnableRestartButtonAndStopAudio()
    {
        // Debug.Log("Turning Restart Button ON...");
        if (restartButton) restartButton.SetActive(true); //Restored on 9/28/22   
        if (exitButton) exitButton.SetActive(true);  // 2/6/22
        if (levelInProgress == 1)
        {
            missionAccomplishedText.enabled = true;
            Debug.Log("From EnableRestart...() Good Hits " + totalGoodHits + " TeslaHits " + totalTeslaHits
            + " Satellit(bad) hits " + totalSatelliteBadHits + " Score = " + scoreTotal);
        }
        if (levelInProgress == 2) SetLevel2Stats(scoreTotal);
        audioSource.Stop();
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
    public void OnExitStatsButtonPressed()
    {
        //audioSource.clip = clipApert;
        audioSource.Play();
        canvasStatsLevel1.SetActive(false);
        if (animHighScoreIndicator) animHighScoreIndicator.enabled = false;
        mainCanvas.SetActive(true);
    }
    public void OnRestartButtonPressed()  //THE CONTINUE button
    {
        audioSource.clip = clipApert;
        audioSource.Play();
        if (exitButton) exitButton.SetActive(false);
        restartButton.SetActive(false);
        if (RestartButtonPressed != null)
        {
            scoreTotal = 0;
            scoreText.text = scoreTotal.ToString("#,###");
            StartCoroutine(WaitBeforeSceneChange(.5f));
        }
    }
    public void OnJunkSceneInfoButtonPressed()
    {
        audioSource.clip = clipApert;
        audioSource.Play();
        Debug.Log("Junk INfo Button Pressed!");
        // code to show INFO for Junk Scene 
        //joyStickCanvas.SetActive(false);
        SetJoyStickCanvas(false);
        mainCanvas.SetActive(false);
        junkInfoCanvasPart.SetActive(true);
    }
    public void OnJunkInfoGotItButtonPressed()
    {
        audioSource.Play();
        junkInfoCanvasPart.SetActive(false);
        //joyStickCanvas.SetActive(true);
        SetJoyStickCanvas(true);
        mainCanvas.SetActive(true);
    }
    public void OnWormholeSceneInfoButtonPressed()
    {
        Debug.Log("Wormhole INfo Button Pressed!");
        audioSource.clip = clipApert;
        audioSource.Play();
        // code to show INFO for Junk Scene 
        //joyStickCanvas.SetActive(false);
        SetJoyStickCanvas(false);
        mainCanvas.SetActive(false);
        wormholeInfoCanvasPart.SetActive(true);
    }
    public void OnWormholeInfoGotItButtonPressed()
    {
        audioSource.Play();
        wormholeInfoCanvasPart.SetActive(false);
        //joyStickCanvas.SetActive(true);
        SetJoyStickCanvas(true);
        mainCanvas.SetActive(true);
    }
    public void SetLevel2ShipSpeed()
    {
        firstPersonController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        if (PlayerPrefs.HasKey("Level2ShipSpeed"))
        {
            firstPersonController.MoveSpeed = PlayerPrefs.GetFloat("Level2ShipSpeed");    // shipSpeedValueText.text = shipSpeedSlider.value.ToString();
        }
        else
        {
            PlayerPrefs.SetFloat("Level2ShipSpeed", firstPersonController.MoveSpeed);
        }
    }
    public void OnWormholeSetupButtonPressed()     //setupButton is in mainCanvas  
    {
        Debug.Log("Setup Wormhole Button Pressed!");
        audioSource.clip = clipApert;
        audioSource.Play();
        mainCanvas.SetActive(false);
        if (!speedSelectorCanvas.activeSelf)
        {
            speedSelectorCanvas.SetActive(true);
            // Debug.Log("activate speed selector canvas");
        }
        gOShipSpeedSlider = GameObject.Find("ShipSpeedSlider");
        if (gOShipSpeedSlider)
        {
            shipSpeedSlider = gOShipSpeedSlider.GetComponent<Slider>();
            // Debug.Log("Got the slider ...");
        }
        if (gOShipSpeedSlider)
        {
            if (PlayerPrefs.HasKey("Level2ShipSpeed"))
            {
                shipSpeedSlider.value = PlayerPrefs.GetFloat("Level2ShipSpeed");
                //  Debug.Log("slider set to Key");
            }
            //Debug.Log("Ship Speed slider found in ONSetup");
            shipSpeedValueText = GameObject.Find("ShipSpeedText").GetComponent<TextMeshProUGUI>();
            firstPersonController = GameObject.Find("Player").GetComponent<FirstPersonController>();
            shipSpeedValueText.text = shipSpeedSlider.value.ToString("##.#");
        }
        else Debug.Log("Ship Speed slider NOT found in ONSetup");
    }
    public void OnShipSpeedChanged(float value)  //for level 2 only   added 7/13/22
    {
        //if (value != 0)  //method/on event is triggered just by touching setup button ??? so we check for 0
        //{
        //    Debug.Log("ShipSpeedSlider moved :  " + value);
        firstPersonController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        string v = shipSpeedSlider.value.ToString("##.#");
        shipSpeedValueText = GameObject.Find("ShipSpeedText").GetComponent<TextMeshProUGUI>();
        shipSpeedValueText.text = v;
        firstPersonController.MoveSpeed = shipSpeedSlider.value;

        PlayerPrefs.SetFloat("Level2ShipSpeed", shipSpeedSlider.value);
        //}
    }

    public void OnWormholeSetupOKButtonPressed()   //setupOKButton is in speedSetupCanvasPart
    {
        audioSource.Play();
        speedSelectorCanvas.SetActive(false);   //NOTE: Reuse of Level1 JunkScene
        mainCanvas.SetActive(true);
        SetJoyStickCanvas(true);
    }
    public void OnSetupButtonPressed()     //setupButton is in mainCanvas  -- here we also show joystick positioning buttons
    {
        Debug.Log("Setup Button Pressed!");
        audioSource.clip = clipApert;
        audioSource.Play();
        mainCanvas.SetActive(false);
        if (!speedSelectorCanvas.activeSelf) speedSelectorCanvas.SetActive(true);
        //next we need to sync toggles to PlayerPref if there is a key, otherwise let it default  
        if (PlayerPrefs.HasKey("Level1Speed"))
        {
            int x = PlayerPrefs.GetInt("Level1Speed");
            toggleGOSlow = GameObject.Find("Slow").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            toggleGOMedium = GameObject.Find("Medium").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            toggleGOFast = GameObject.Find("Fast").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            Debug.Log(" intX is " + x + "   Slow is " + toggleGOSlow + "    Medium is " + toggleGOMedium + "   Fast is " + toggleGOFast);
            switch (x)
            {
                //case 90:       //this has to reference the public int variable    i.e.     public int levelOneSlowSpeed = 90;  // 3/6
                case 1:       //this has to reference the public int variable    i.e.     public int levelOneSlowSpeed = 90;
                    //Debug.Log("OnSetupButton Pressed! now IN CASE 1 set speed/toggle to " + x);
                    UnityEngine.UI.Toggle slowToggle = GameObject.Find("Slow").GetComponent<UnityEngine.UI.Toggle>();
                    slowToggle.isOn = true;
                    break;
                //case 110:       //this has to reference the public int variable // 3/6
                case 2:       //this has to reference the public int variable 
                    //Debug.Log("OnSetupButton Pressed! now IN CASE 2 set speed/toggle to " + x);
                    UnityEngine.UI.Toggle mediumToggle = GameObject.Find("Medium").GetComponent<UnityEngine.UI.Toggle>();
                    mediumToggle.isOn = true;
                    break;
                //case 130:       //this has to reference the public int variable // 3/6
                case 3:       //this has to reference the public int variable 
                    //Debug.Log("OnSetupButton Pressed! now IN CASE 3 set speed/toggle to " + x);
                    UnityEngine.UI.Toggle fastToggle = GameObject.Find("Fast").GetComponent<UnityEngine.UI.Toggle>();
                    fastToggle.isOn = true;
                    break;
                default:
                    //Debug.Log("OnSetupButtonPressed switch(x) DEFAULTED to " + x);
                    break;

            }

        }
        //  if no key we did nothing and took the Original Setting of Slow   //NOTE CurrentText is set in OnOKButton...
    }
    public void OnSetupOKButtonPressed()   //setupOKButton is in speedSetupCanvasPart
    {
        audioSource.Play();
        speedSelectorCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        SetJoyStickCanvas(true);
        currentText = GameObject.Find("CurrentText").GetComponent<TextMeshProUGUI>();
        speedNumber = GameObject.Find("SpeedNumber").GetComponent<TextMeshProUGUI>();
        var x = PlayerPrefs.GetInt("Level1Speed");
        switch (x)
        {
            case 1:
                currentText.text = level1Slow; // "Currently Slow";
                speedNumber.text = levelOneSlowSpeed.ToString("###");
                StaticBackground.objectSpeed = levelOneSlowSpeed;
                break;
            case 2:
                currentText.text = level1Medium; // "Currently Medium";
                speedNumber.text = levelOneMediumSpeed.ToString("###");
                StaticBackground.objectSpeed = levelOneMediumSpeed;
                break;
            case 3:
                currentText.text = level1Fast; // "Currently Fast";
                speedNumber.text = levelOneFastSpeed.ToString("###");
                StaticBackground.objectSpeed = levelOneFastSpeed;
                break;
            default:
                currentText.text = "Currently Slow";
                speedNumber.text = levelOneSlowSpeed.ToString("###");
                StaticBackground.objectSpeed = levelOneSlowSpeed;
                break;
        }

    }
    public void OnSlowSpeedToggleValueChanged(bool tog)
    {
        Debug.Log(" SlowSpeed Value Changed " + tog);
        if (tog)
        {
            // PlayerPrefs.SetInt("Level1Speed", levelOneSlowSpeed);  // 3/6
            PlayerPrefs.SetInt("Level1Speed", 1);
            genRandomBackground.objectSpeed = levelOneSlowSpeed;
        }
    }
    public void OnMediumSpeedToggleValueChanged(bool tog)
    {
        Debug.Log(" MediumSpeed Value Changed " + tog);
        if (tog)
        {
            // PlayerPrefs.SetInt("Level1Speed", levelOneMediumSpeed);  // 3/6
            PlayerPrefs.SetInt("Level1Speed", 2);
            genRandomBackground.objectSpeed = levelOneMediumSpeed;
        }
    }
    public void OnFastSpeedToggleValueChanged(bool tog)
    {
        Debug.Log(" FastSpeed Value Changed " + tog);
        if (tog)
        {
            //  PlayerPrefs.SetInt("Level1Speed", levelOneFastSpeed);  // 3/6
            PlayerPrefs.SetInt("Level1Speed", 3);
            genRandomBackground.objectSpeed = levelOneFastSpeed;
        }
    }

    public void OnStatsButtonPressed()
    {
        audioSource.clip = clipApert;
        audioSource.Play();
        switch (levelInProgress)
        {
            case 1:
                DisplayLevel1Stats();
                break;
            case 2:
                break; // for now 
        }

    }
    public void OnResetStatsButtonPressed()
    {
        Debug.Log("currentBestTS = " + currentBestTotalScore);
        PlayerPrefs.DeleteKey("BestHitsScore");  //5/23/22 Change Setint to DeleteKey to mimic actual play (not Editor Clear All Prefs)
        PlayerPrefs.DeleteKey("BestTeslaHitsScore");
        PlayerPrefs.DeleteKey("BestSatsAliveScore");
        PlayerPrefs.DeleteKey("BestTotalScore");
        previousLevel1PlayerPrefStatsKeysFound = false;  // 5/24/22 
        //PlayerPrefs.SetInt("BestHitsScore", 0);
        //PlayerPrefs.SetInt("BestTeslaHitsScore", 0);
        //PlayerPrefs.SetInt("BestSatsAliveScore", 0);
        //PlayerPrefs.SetInt("BestTotalScore", 0);
        currentBestTotalScore = 0;
        highScore.text = currentBestTotalScore.ToString();
        audioSource.clip = clipTheCan;
        audioSource.Play();
    }
    public void OnGoButtonPressed()
    {
        if (junkSceneInfoButton) junkSceneInfoButton.SetActive(false);
        if (wormholeSceneInfoButton) wormholeSceneInfoButton.SetActive(false);
        if (exitButton) exitButton.SetActive(false);
        //if (goInfoText) goInfoText.enabled = false;
        if (setupButton) setupButton.SetActive(false);
        goButton.SetActive(false);
        if (GoButtonPressed != null)
        {
            roundInProgress = true;
            StartCoroutine(TimerAndDisplay());
            //  scoreTotal = 0;
            //  scoreText.text = scoreTotal.ToString("#,###");
            audioSource.clip = clipEngineSound;
            if (levelInProgress == 1) audioSource.loop = true;  // A fucked up kludge because I didn't prepare audio properly
            audioSource.Play();
            GoButtonPressed();
        }
    }

    public void OnButtonJoyStickRightPressed()
    {
        if (joyStickCenterText) joyStickCenterText.SetActive(false);
        if (joyStickLeftText) joyStickLeftText.SetActive(false);
        if (joyStickRightText) joyStickRightText.SetActive(true);
      //  SetJoyStickCanvas(false);
        PlayerPrefs.SetInt("JoyStickPosition", 1);
      //  SetJoyStickCanvas(true);
        joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jRight;
        //var jx = joyStick.GetComponent<RectTransform>().position.x;
        //Debug.Log("stick RIGHT position is x value of the rect  " + jx + "  js = " + joyStick);
        SetUpJoyStickPosition();

    }
    public void OnButtonJoyStickCenterPressed()
    {
        if (joyStickCenterText) joyStickCenterText.SetActive(true);
        if (joyStickLeftText) joyStickLeftText.SetActive(false);
        if (joyStickRightText) joyStickRightText.SetActive(false);
      //  SetJoyStickCanvas(false);
        PlayerPrefs.SetInt("JoyStickPosition", 2);
      //  SetJoyStickCanvas(true);
        joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jCenter;
        //var jx = joyStick.GetComponent<RectTransform>().position.x;

        //jx = 500f;
        //Debug.Log("stick CENTER position is x value of the rect  " + jx + "  js = " + joyStick);
        SetUpJoyStickPosition();
    }
    public void OnButtonJoyStickLeftPressed()
    {
        if (joyStickCenterText) joyStickCenterText.SetActive(false);
        if (joyStickLeftText) joyStickLeftText.SetActive(true);
        if (joyStickRightText) joyStickRightText.SetActive(false);
    //    SetJoyStickCanvas(false);
        PlayerPrefs.SetInt("JoyStickPosition", 3);
    //    SetJoyStickCanvas(true);
        joyStickPositionTextOnMain.GetComponent<TextMeshProUGUI>().text = jLeft;
        //var jx = joyStick.GetComponent<RectTransform>().position.x;
        //Debug.Log("stick LEFT position is x value of the rect  " + jx + "  js = " + joyStick);
        SetUpJoyStickPosition();
    }

    private void SetJoyStickCanvas(bool canvasSwitcher)
    {   // 
        var theCanvas = PlayerPrefs.GetInt("JoyStickPosition");

        switch (theCanvas)
        {
            case 1:
                joyStickCanvasRight.SetActive(canvasSwitcher);
                break;
            case 2:
                joyStickCanvasCenter.SetActive(canvasSwitcher);
                break;
            case 3:
                joyStickCanvasLeft.SetActive(canvasSwitcher);
                break;
            default:
                break;
        }
    }
    IEnumerator WaitOnAudio(float waitTime)
    {
        // Debug.Log("waiting" + waitTime);
        yield return new WaitForSeconds(waitTime);
        audioBackground.clip = clipBackGround;
        audioBackground.loop = true;
        audioBackground.Play();

    }
    IEnumerator WaitBeforeSceneChange(float waitTime)
    {
        // Debug.Log("waiting" + waitTime);
        yield return new WaitForSeconds(waitTime);
        scoreTotal = 0;
        if (startNextLevel)
        {
            levelChanger.FadeToLevel(0);   // 3/14/22 changed 2 to 0
            startNextLevel = false;
        }
        else
        {
            levelChanger.FadeToLevel(0);  // 3/14/22 changed 1 to 0
        }
        // SceneManager.LoadScene(sceneBuildIndex);   //to be continued ! this is fucking hardcoded to 2 
    }
    IEnumerator TimerAndDisplay()
    {
        while (roundInProgress)
        {
            roundTime -= .1f;
            timerText.text = roundTime.ToString("##.##");
            yield return new WaitForSeconds(.1f);  //Should reengineer to rid "new" and time a different way to reduce GC? Future...

            if (SceneXTimerExpired != null && roundTime <= .1f)
            {
                roundInProgress = false; //added 12/12
                //EnableRestartButtonAndStopAudio();
                //UpdateScore(0);  //added 12/12 Cause UpdateScore to execute and close the round --3/17/22 MAYBE NOT a good idea !
                // UpdateScore(bonusIncrement * GenRandomBackground.satellitesInPlay);
                SceneXTimerExpired();  // fire off the Event 
                audioSource.loop = false;
                timerText.text = roundTime.ToString("#0.##"); //2/24/22 to keep a "0" displayed
                roundTime = initialRoundTime;
            }
        }
        //var _buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (levelInProgress)
        {
            case 1:
                Debug.Log("Goodbye from TimerAndDisplay  -- Starting  ApplyRemainingObjectsBonuses(GenRandomBackground.satellitesInPlay) ");
                coroutine = ApplyRemainingObjectsBonuses(GenRandomBackground.satellitesInPlay);
                StartCoroutine(coroutine);
                break;
            case 2:
                EnableRestartButtonAndStopAudio();
                break;
            default:
                Debug.Log(" Invalid levelInProgress in TimerAndDisplay()");
                break;
        }
        yield return null;
    }
    private void OnPlayerCollisionReport(GameObject objectHit)
    {
        // to move UpdateScore() calls out of other scripts and centralize here - clean our mess
        switch (objectHit.tag)
        {
            case "RogueTesla":
                //  Debug.Log("Tesla reported to SKeeper");
                UpdateScore(900);
                StartCoroutine(FlashBonus());
                break;
            case "Satellite":
                //  Debug.Log("Satellite reported to SKeeper");
                UpdateScore(-400);
                totalSatelliteBadHits++;
                break;
            case "BackgroundObject":
                //   Debug.Log("BackgroundObject reported to SKeeper");
                UpdateScore(100);
                totalGoodHits++;
                break;
            case "Face":   //counted with "BackgroundObject"s   // 3/31/22 no it aint 
                           //  Debug.Log("Face reported to SKeeper");
                totalGoodHits++;
                UpdateScore(100);  // 3/21/22 
                break;
            case "WormholePipe":
                //  Debug.Log("WormholeSegmentNonRigidbody reported to SKeeper");
                if (roundInProgress) UpdateScore(-400);  //5/14/22 added if (roundInProgress)   (as a start to segregate levels)
                break;
            case "WormholeMenace":   // player is shielded else we wouldn't get here - unshieded player gets destroyed
                //  Debug.Log("WormholeMenace reported to SKeeper");
                UpdateScore(100);
                break;
            case "ShieldSphere":
                // here we should award and tally shields.
                // Debug.Log("ShieldSphere collision reported to SKeeper");
                // UpdateScore(1);
                break;
            default:
                //   Debug.Log("SOME OTHER objectHit tag reported to Skeeper: " + objectHit.tag);
                break;
        }
    }
    private void OnPlayerMsgTypeReport(int messageType, int count)
    {
        switch (messageType)
        {
            case 1:
                //Debug.Log("Skeeper received a messageType call for type 1 -- Player Destroyed");
                roundInProgress = false; //5/17/22 cause  IEnumerator TimerAndDisplay() to end
                UpdateScore(true); // 6/16/22 set score to 0 on play destruction
                break;
            case 2:  //level 2 player made it home - now tally shields left penalty OR bonus for zero shields left
                //Debug.Log("Skeeper received a messageType call for type 2 with a shieldCount of " + count);
                if (levelInProgress == 2)
                {
                    if (count > 0)  //apply penalty (deduct 100) for each leftover
                    {
                        var x = count * 100;
                        Debug.Log("SKeeper says Deducting " + x + " points for unused shields. Total score was " + scoreTotal);
                        UpdateScore(-x);
                    }
                    else
                    {
                        Debug.Log("SKeeper says Zero shields left!!! AWARD 1000 POINTS");
                        UpdateScore(1000); //to grant a bonus -- How 'bout doubling it? or just 1000 points
                    }
                }

                break;
            default:
                Debug.Log("Skeeper received an unknown messageType call");
                break;


        }
    }
    IEnumerator ApplyRemainingObjectsBonuses(int objectsRemaining)   //ONLY Satellites for now //ONLY Level 1(scene 2)
    {
        var midScreen = new Vector3(-15.277f, 90f, 0f);  // we need to replace this with a Stats Panel
        audioSource.loop = false;
        yield return new WaitForSeconds(1f);
        collectBonusText.rectTransform.localPosition = midScreen;// new Vector3(-15.277f, 90f, 0f);
        totalSatellitesLeftAlive = objectsRemaining;
        if (objectsRemaining > 0)
        {
            UpdateScore(bonusIncrement * objectsRemaining);  // 3/18/22 moved to scorekeeper 
            audioSource.clip = clipTheCan;
            audioSource.Play();
            Debug.Log(" Display Paying Hitcoin");
            collectBonusText.text = "Paying Hitcoin Bonus: " + objectsRemaining.ToString() + " Satellites earns " + bonusIncrement * objectsRemaining;
            collectBonusText.enabled = true;

            yield return new WaitForSeconds(3f);
        }
        else
        {
            Debug.Log(" Display NO SATS");
            StringToShow = WhichStringToShow.NoSatellitesLeft;
            collectBonusText.text = "NO SATELLITES REMAINING -- NO BONUS -- YOU'RE GONNA GET FIRED.";
            collectBonusText.enabled = true;
            yield return new WaitForSeconds(3f);
            //UpdateScore(0);  // 3/20/22 force round end

        }
        if (scoreTotal >= scoreToAchieveNextLevel || objectsRemaining >= satellitesRemainingToAchieveNextLevel)
        {
            collectBonusText.color = Color.green;
            collectBonusText.fontSize = 30;
            collectBonusText.text = "Round Won!";
            collectBonusText.enabled = true;
            StringToShow = WhichStringToShow.RoundWon;
            sceneBuildIndex = 2;
            PlayerPrefs.SetInt("LevelToPlay", 3); // 3/15/22 A win so unlock L2
            startNextLevel = true;
        }
        else
        {
            collectBonusText.color = Color.red;
            collectBonusText.fontSize = 30;
            //audioSource.clip = clipTheCan;
            //audioSource.Play();
            //Debug.Log(" Display Nice try");
            collectBonusText.text = "Round Lost.";
            //"Nice try!  You scored " + scoreTotal.ToString() + " and left " + objectsRemaining.ToString()
            //+ " satellites alive." +
            //"\r\n You need 20,000 HitCoins or 6 satellites for next level";
            collectBonusText.enabled = true;
            // 3/26/22 All above replaced by  DisplayLevel1Stats()
            StringToShow = WhichStringToShow.RoundLost;
            startNextLevel = false;
        }
        //Somewhere around here we need to fill the StatsPanel with the results... Or call another method
        //yield return new WaitForSeconds(5f);
        SetLevel1Stats(totalGoodHits * 100, totalTeslaHits * 900, totalSatellitesLeftAlive * 400, scoreTotal);
        // DisplayLevel1Stats();  4/5/22 change to activate via Stats Button 
        if (statsButton != null) statsButton.SetActive(true);
        EnableRestartButtonAndStopAudio();
    }
    IEnumerator FlashBonus()   //ONLY RogueTesla for now 
    {
        collectBonusText.text = "                 Rogue Tesla Bonus 900 Hitcoin!";
        collectBonusText.enabled = true;
        // scoreKeeper.UpdateScore(900);   // 3/18/22 moved to ScoreKeeper 
        //audioSource.Play();  //should delete this after modify of PlayerCollisions
        yield return new WaitForSeconds(.9f);
        collectBonusText.enabled = false;
        // Debug.Log("End Flash bonus for RogueTesla");
    }
    private void DisplayLevel1Stats()
    {
        string roundWonString = "Well done! \r\n You destroyed enough junk / Left enough satellites for next level!";
        string roundLostString = "Nice try!  You scored less than 20,000 and left less than 6 satellites alive. " +
                   " \r\n You need 20,000 HitCoins or 6 satellites for next level";
        string noSatellitesLeftString = "NO SATELLITES REMAINING -- NO BONUS -- YOU'RE GONNA GET FIRED.";
        mainCanvas.SetActive(false);
        canvasStatsLevel1.SetActive(true);
        endRoundComment.enabled = true;
        //First apply the result comment at the top

        switch (StringToShow)
        {
            case WhichStringToShow.RoundLost:
                endRoundComment.color = Color.red;
                endRoundComment.text = roundLostString;
                break;

            case WhichStringToShow.RoundWon:
                endRoundComment.color = Color.green;
                endRoundComment.text = roundWonString;
                break;

            case WhichStringToShow.NoSatellitesLeft:  //this never happens because of logic sequence in ApplyRemainingObjectsBonuses method
                endRoundComment.text = noSatellitesLeftString;  //but we'll keep it around for now
                break;

            default: break;
        }
        //    public int totalGoodHits, totalTeslaHits, totalSatelliteBadHits, totalSatellitesLeftAlive;  //just for ref.
        //Now populate the stats grid   // 5/22/22 OR move to SetLevel1Stats(...) 
    }
    //private void DisplayLevel2Stats()  for the future
    //{

    //}

    private void SetLevel1Stats(int hitsScore, int teslaHitsScore, int satsAliveScore, int totalScore)
    {
        //Now populate the stats grid   // 5/22/22 OR move to SetLevel1Stats(...) 
        totalHits.text = totalGoodHits.ToString();
        satsAlive.text = totalSatellitesLeftAlive.ToString();
        teslaHits.text = totalTeslaHits.ToString();
        satsHit.color = Color.red;
        satsHit.text = totalSatelliteBadHits.ToString();
        statsScore.text = scoreTotal.ToString();
        tallyTotalHits.text = (totalGoodHits * 100).ToString();
        tallySatsAlive.text = (totalSatellitesLeftAlive * 400).ToString();
        tallyTeslaHits.text = (totalTeslaHits * 900).ToString();
        tallySatsHit.text = (-totalSatelliteBadHits * 400).ToString();

        if (previousLevel1PlayerPrefStatsKeysFound)
        {
            bestTotalHits.text = PlayerPrefs.GetInt("BestHitsScore", 0).ToString();
            bestTeslaHits.text = PlayerPrefs.GetInt("BestTeslaHitsScore", 0).ToString();
            bestSatsAlive.text = PlayerPrefs.GetInt("BestSatsAliveScore", 0).ToString();
            bestSatsHit.text = PlayerPrefs.GetInt("BestSatsHit", 0).ToString();
            bestTotalScore.text = PlayerPrefs.GetInt("BestTotalScore", 0).ToString();
            labelLastBestGame.enabled = true;
            labelNewBestGame.enabled = false;
        }  //end Now Populate (of moved to SetLevel1Stats(...)
        else  //no key/1st game so set bests to the current 
        {
            PlayerPrefs.SetInt("BestTotalScore", totalScore);
            PlayerPrefs.SetInt("BestHitsScore", hitsScore);
            PlayerPrefs.SetInt("BestTeslaHitsScore", teslaHitsScore);
            PlayerPrefs.SetInt("BestSatsAliveScore", satsAliveScore);
            PlayerPrefs.SetInt("BestSatsHit", -totalSatelliteBadHits * 400);
            labelLastBestGame.enabled = false;
            labelNewBestGame.enabled = false;
        }

        if (totalScore > currentBestTotalScore && previousLevel1PlayerPrefStatsKeysFound)   //changed to show Best Game -- NOT best of each -- which was confusing 
        {
            labelNewBestGame.enabled = true;
            labelForNewBestTotalScore.enabled = true;
            animHighScoreIndicator.enabled = true;
            newHighScoreIndicator.enabled = true;
            PlayerPrefs.SetInt("BestTotalScore", totalScore);
            PlayerPrefs.SetInt("BestHitsScore", hitsScore);
            PlayerPrefs.SetInt("BestTeslaHitsScore", teslaHitsScore);
            PlayerPrefs.SetInt("BestSatsAliveScore", satsAliveScore);
            PlayerPrefs.SetInt("BestSatsHit", -totalSatelliteBadHits * 400);
        }
    }
    private void SetLevel2Stats(int score)
    {
        if (previousLevel2PlayerPrefStatsKeysFound)  // we have a key so compare
        {
            if (score > currentLevel2HighScore)
            {
                PlayerPrefs.SetInt("Level2HighScore", score);
            }
        }
        else   // we don't have a key (very 1st game or user reset the stats)
        {
            PlayerPrefs.SetInt("Level2HighScore", score);
        }
    }
    private void OnEnable()
    {
        PlayerCollisions.PlayerCollisionReport += OnPlayerCollisionReport;
        PlayerCollisions.PlayerMsgTypeReport += OnPlayerMsgTypeReport;
    }
    private void OnDisable()
    {
        PlayerCollisions.PlayerCollisionReport -= OnPlayerCollisionReport;
        PlayerCollisions.PlayerMsgTypeReport -= OnPlayerMsgTypeReport;
        //   StopAllCoroutines();
    }

}
