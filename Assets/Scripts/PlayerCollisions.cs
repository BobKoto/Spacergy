using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{ //You'll find this script under PlayerParent/Player in the editor
    public AudioSource apert2;
    public AudioSource theCan;
    public AudioSource voltage;
    public AudioSource warn;
    public AudioClip theCanClip;
    public AudioClip apert2Clip;
    public AudioClip voltageClip;
    public AudioClip shieldHitClip;
    public AudioClip shieldsOffClip;
    public AudioClip warnClip;
    public AudioClip doh;
    public AudioClip explos;
    public ParticleSystem psExplosion;
    public ParticleSystem psSparks;
    AudioSource audiosource; // the right way ?  //not necessarily if u want to inermix sounds
    Color psColorRed = Color.red;
    Color psColorWhite = Color.white;
    Color psColorYellow = Color.yellow;
    Color psColorGreen = Color.green;
   // MinMaxGradient minColor;
    //ParticleSystem.ColorOverLifetimeModule psColorModule;
    ParticleSystem ps;
    ScoreKeeper scoreKeeper;
    public GameObject playerShield, shieldStatusIndicator, playerCrosshair, playerSphere;
    Image imageShieldStatusIndicator;
    Material playerCrosshairMat, playerSphereMat;
    public Material playerSphereBlue, playerSphereGreen, playerSphereYellow, playerSphereRed;
    Color playerCrosshairColor, playerSphereColor;
    SpriteRenderer playerCrosshairSpriteRenderer;
    MeshRenderer playerSphereMeshRenderer;

    public TextMeshProUGUI shieldsText, shieldStatusCount;
    MeshRenderer[] playerMeshes;
    int levelInProgress;

    bool wormholePlayerShieldOn = true;  //start play with at lease 1 shield - for now 
    public int wormholePlayerShieldCount = 5;//5/17/22 made public for testing purposes -- and maybe for future difficulty setting
    bool roundInProgress = true;
    int currentFrame, lastFrame, currentiId, lastiId;
    public delegate void CollisionOccurred(GameObject objectHit);
    public static event CollisionOccurred PlayerCollisionReport;

    public delegate void SendOutMsgType(int msgType, int count);
    public static event SendOutMsgType PlayerMsgTypeReport;
    

    // Start is called before the first frame update
    void Start()
    {

        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        ps = GetComponent<ParticleSystem>();  // the one for non-fatal collisions only 
        audiosource = GetComponent<AudioSource>();
        levelInProgress = SceneManager.GetActiveScene().buildIndex - 1; //subtract 1 to account for setup scenes 0 and 1 so we get 1 for level1, etc
        if (levelInProgress == 2)
        {
           // playerSphereMat = playerSphereBlue;
           // playerSphereMat = playerSphere.GetComponent<MeshRenderer>().material;
           // Debug.Log(playerSphereColor);
           // playerSphereMat.color = Color.green;
            playerSphereMeshRenderer = playerSphere.GetComponent<MeshRenderer>();
            //playerSphereMeshRenderer.material = playerSphereBlue;  // just use editor's setting (blue)
            //playerCrosshairSpriteRenderer = playerCrosshair.GetComponent<SpriteRenderer>();
            //playerCrosshairMat = playerCrosshairSpriteRenderer.material;
            //playerCrosshairColor = playerCrosshairMat.color;
            //playerCrosshairMat.color = Color.green;

            imageShieldStatusIndicator = shieldStatusIndicator.GetComponent<Image>();
            shieldsText.text = wormholePlayerShieldCount.ToString();
            shieldStatusCount.text = wormholePlayerShieldCount.ToString();
            playerMeshes = GetComponentsInChildren<MeshRenderer>();
        }

    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnCollisionEnter(Collision other)
    {
       // Debug.Log("Player hit  " + other.gameObject.name);
        var mainParticleSystem = ps.main;

        //if (roundInProgress)  // added 6/10/22 so we don't do ANYTHING if not in progress
        //{
        //    if (other.gameObject.layer == 6 || other.gameObject.CompareTag("Face"))  // 3/15/22 this is all shit and needs to be cleaned up
        //    {
        //        if (Application.isMobilePlatform) Handheld.Vibrate();
        //        apert2.clip = apert2Clip;  //play the sound of a Hit
        //        apert2.Play();
        //        if (other.gameObject.CompareTag("Face"))
        //        {
        //            mainParticleSystem.startColor = psColorGreen;
        //        }
        //        else mainParticleSystem.startColor = psColorYellow;

        //        ps.Play();

        //        if (other.gameObject.CompareTag("Satellite"))
        //        {
        //            var originalPriority = audiosource.priority;
        //            audiosource.priority = 1;
        //            audiosource.clip = doh; // = hiccup;
        //            audiosource.Play();
        //            audiosource.priority = originalPriority;
        //            mainParticleSystem.startColor = psColorRed;
        //            ps.Play();
        //        }
        //        else if (roundInProgress)
        //        {
        //            //  Debug.Log("Player hit, add 100 score for " + collision.gameObject.name + " at zpos " + zpos);
        //            ps.Play();
        //            // Debug.Log("Playing on the else branch");
        //        }
        //    }
        //    if (other.gameObject.CompareTag("RogueTesla"))  // added 3/15/22
        //    {
        //        scoreKeeper.totalTeslaHits++;
        //    }
            if (other.gameObject.CompareTag("WormholePipe"))
            {
                if (roundInProgress)
                {
                    //  main.startColor = psColorRed;
                    //  ps.Play();
                    psSparks.transform.position = transform.position;
                    psSparks.Play();
                   // Debug.Log("Playing SPARKS HERE!"); // we just couldn't see 'em
                    if (Application.isMobilePlatform) Handheld.Vibrate();
                    voltage.clip = voltageClip;  //play the sound of a Hit
                    voltage.Play();

                    return;
                }
            }
            PlayerCollisionReport?.Invoke(other.gameObject); //for now cause event for all Player collisions - may want to narrow this down?
           // Debug.Log("Reporting Collisions  HERE!" + other.gameObject); // we just couldn't see 'em

        }
    //}
    private void OnTriggerEnter(Collider other)
    {
        var mainParticleSystem = ps.main;
        currentiId = other.GetInstanceID();
        currentFrame = Time.frameCount;
        var main = ps.main;
        var frameDelta = currentFrame - lastFrame;
       // Debug.Log(" frame = " + currentFrame + "  iId = " + currentiId + " lastid = " + lastiId + " lastFr = " + lastFrame);
        if (currentiId == lastiId && frameDelta < 5)
        {
        //    Debug.Log("dup");
            return;
        }

        lastFrame = currentFrame;
        lastiId = currentiId;
        if (levelInProgress == 1)
        {
            if (roundInProgress)  // added 6/10/22 so we don't do ANYTHING if not in progress
            {
                if (other.gameObject.layer == 6 || other.gameObject.CompareTag("Face"))  // 3/15/22 this is all shit and needs to be cleaned up
                {
                    if (Application.isMobilePlatform) Handheld.Vibrate();
                    apert2.clip = apert2Clip;  //play the sound of a Hit
                    apert2.Play();
                    if (other.gameObject.CompareTag("Face"))
                    {
                        mainParticleSystem.startColor = psColorGreen;
                    }
                    else mainParticleSystem.startColor = psColorYellow;

                    ps.Play();

                    if (other.gameObject.CompareTag("Satellite"))
                    {
                        var originalPriority = audiosource.priority;
                        audiosource.priority = 1;
                        audiosource.clip = doh; // = hiccup;
                        audiosource.Play();
                        audiosource.priority = originalPriority;
                        mainParticleSystem.startColor = psColorRed;
                        ps.Play();
                    }
                    else if (roundInProgress)
                    {
                        //  Debug.Log("Player hit, add 100 score for " + collision.gameObject.name + " at zpos " + zpos);
                        ps.Play();
                        // Debug.Log("Playing on the else branch");
                    }
                }
                if (other.gameObject.CompareTag("RogueTesla"))  // added 3/15/22
                {
                    scoreKeeper.totalTeslaHits++;
                }
                PlayerCollisionReport?.Invoke(other.gameObject);
            }

            //PlayerCollisionReport?.Invoke(other.gameObject);  // 7/3/22 moved to above "if" block to avoid spurious event 
        }
    if (levelInProgress == 2 && roundInProgress)  //  7/3/22  added "&& roundInProgress" to fix timing issue 
        switch (other.tag)
        {
            case "WormholeMenace":
                {
                    if (!wormholePlayerShieldOn)
                    {
                        audiosource.clip = explos;
                        audiosource.Play();
                        psExplosion.transform.position = transform.position;
                        psExplosion.Play();
                        ExplodeAndDestroyPlayer(); //uncommented 6/20/22 
                        PlayerMsgTypeReport?.Invoke(1, 0); //Now tell scoreKeeper player is destroyed
                        roundInProgress = false;
                        break; // 6/25/22 TEST
                    }
                    if (wormholePlayerShieldOn)
                    {
                        audiosource.clip = apert2Clip;  //we hit a Menace
                        audiosource.Play();
                        main.startColor = psColorRed;
                        ps.Play();
                        wormholePlayerShieldCount--;
                        shieldsText.text = wormholePlayerShieldCount.ToString();
                        shieldStatusCount.text = wormholePlayerShieldCount.ToString();
                        if (wormholePlayerShieldCount < 3)
                        {
                            SetImageShieldStatusIndicatorColor();
                            SetPlayerSphereColor();
                        }

                        if (wormholePlayerShieldCount <= 0)
                        {
                            // warn.loop = false;  // 6/5/22 stop timing issue when shields status flipflop rapidly
                            audiosource.clip = shieldsOffClip;
                            audiosource.Play();
                            StartCoroutine(PlayWarnAudioAfterCurrentClip());
                            playerShield.SetActive(false);
                            wormholePlayerShieldOn = false;
                        }
                        PlayerCollisionReport?.Invoke(other.gameObject); // 6/2/22 
                    }
                    break;
                }  //end case "WormholeMenace"
            case "ShieldSphere":
                //if (other.gameObject.CompareTag("ShieldSphere"))
                {
                    audiosource.volume = 1f;
                    audiosource.clip = shieldHitClip;  //suction
                    audiosource.Play();
                    wormholePlayerShieldOn = true;
                    wormholePlayerShieldCount++;  //(at very end of timer) is it possible this get set to 1 but the next 2 calls don't happen?
                    SetImageShieldStatusIndicatorColor();
                    SetPlayerSphereColor();
                    shieldsText.text = wormholePlayerShieldCount.ToString();
                    shieldStatusCount.text = wormholePlayerShieldCount.ToString();
                    playerShield.SetActive(true);
                    // Debug.Log("ShieldSphere collision ...");
                    PlayerCollisionReport?.Invoke(other.gameObject);
                    break;
                }
            default:
                Debug.Log("some other case happened in OnTriggerEnter..." + other);
                break;
        }
        if (other.gameObject.CompareTag("WormholePipe"))  //Defunct? I think. Is configured as collider / not trigger
        {
            Debug.Log("worm trigger entered");  //So we should NEVER see this!
        }
        // We need SOUNDS Here!

    }
    void SetImageShieldStatusIndicatorColor()
    {
        imageShieldStatusIndicator.color = wormholePlayerShieldCount switch
        {
            0 => Color.red,
            1 => Color.yellow,
            2 => Color.green,
            _ => Color.blue
        };
    }
    //void SetPlayerCrosshairColor()
    //{
    //    playerCrosshairMat.color = wormholePlayerShieldCount switch
    //    {
    //        0 => Color.red,
    //        1 => Color.yellow,
    //        2 => Color.yellow,
    //        _ => Color.green,
    //    };
    //}
    void SetPlayerSphereColor()
    {
        playerSphereMeshRenderer.material = wormholePlayerShieldCount switch
        {
            0 => playerSphereRed,
            1 => playerSphereYellow,
            2 => playerSphereGreen,
            _ => playerSphereBlue
        };
    }
    void ExplodeAndDestroyPlayer()
    {
        //warn.loop = false;
        foreach (MeshRenderer playerMesh in playerMeshes)
            playerMesh.enabled = false;

    }

    IEnumerator PlayWarnAudioAfterCurrentClip()
    {
        while (audiosource.isPlaying)
        {
            yield return null;
        }
        while (roundInProgress && !wormholePlayerShieldOn)
        {
           // warn.loop = true;
            warn.volume = .2f;
            warn.clip = warnClip;
            warn.Play();
            warn.volume = 1f;
            yield return new WaitForSeconds(2);
        }

    }
    IEnumerator PlayExplosionAudioAfterCurrentClip()
    {
        while (audiosource.isPlaying)
        {
            yield return null;
        }
        audiosource.clip = explos;
        audiosource.Play();
        psExplosion.transform.position = transform.position;
        psExplosion.Play();

    }
    void OnTimerExpired()   //Added 5/17/22
    {
       // if (levelInProgress == 2) warn.loop = false;
        roundInProgress = false;
        PlayerMsgTypeReport?.Invoke(2, wormholePlayerShieldCount);  //how is it possible for Count to be 1 while game shows 0?
    }
    private void OnEnable()
    {
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
    }
    private void OnDisable()
    {
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        StopAllCoroutines();
    }
}
