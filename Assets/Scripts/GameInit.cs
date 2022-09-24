using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {

#if UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);  //Not sure the #if works here.
#endif

        Handheld.StartActivityIndicator();
        yield return new WaitForSeconds(0);
        Debug.Log("GameInit loading scene 1");
        SceneManager.LoadScene(1);
    }

}
