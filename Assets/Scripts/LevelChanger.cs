using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    private void Start()
    {
        Debug.Log("Hello from LevelChanger.cs");
    }
    // Update is called once per frame
    //void Update()    //This is TEMPORARY! 
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        FadeToLevel(1);
    //    }
    //}

    public void FadeToNextLevel()
    {
        Debug.Log("FadeToNextLevel running.");
        FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void FadeToLevel (int levelIndex)
    {
        Debug.Log("FadeToLevel running.  " + levelIndex);
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }
    public void OnFadeComplete()   //The animator fires this off 
    {
       // Debug.Log("onFadeComplete running.");
        SceneManager.LoadScene(levelToLoad);     //(levelToLoad + 1); this will obviously throw an error if levelToLoad is at max
    }


}
