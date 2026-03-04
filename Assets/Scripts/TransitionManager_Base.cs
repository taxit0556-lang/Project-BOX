using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
//TRANSITIONS LOLOLOLOL
public enum StartTypeTransition
{
    fadeIn,
    grow,
    slide
}
public enum EndTypeTransition
{
    fadeAway,
    srink
}

public enum TransitionAction
{
    loadScene,
    Nothing
}


public class TransitionManager_Base : MonoBehaviour
{
    [Header("Transition")]
    public StartTypeTransition startTrantition;
    public EndTypeTransition endTransition;
    
    public TransitionAction transitionAction;
    


    [Header("Refs")]
    //Transform Child;
    GameObject Child;
    SpriteRenderer spriteRenderer;
    Color color;


    [Header("Settings")]
    public Sprite Test;
    public  bool TransitionPlaying;
    public float TransitionDuration;
    public float waitTime;
    public string SceneName;
    

    Vector3 localScale;



    void Awake()
    {
        Child = this.gameObject.transform.GetChild(0).gameObject;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Child);
    }

    void Start()
    {
        //Child = transform.GetChild(0);

        spriteRenderer = Child.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = null;

        LeanTween.init(1000);
    }
    void Update()
    {
        color = spriteRenderer.color;
    }

    void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void PlayTransition()
    {
        spriteRenderer.sprite = Test;
        if(!TransitionPlaying)
            color.a = 1;

        if(startTrantition.Equals(StartTypeTransition.fadeIn))
            FadeIn();

        if(startTrantition.Equals(StartTypeTransition.grow))
            Grow();

        if(startTrantition.Equals(StartTypeTransition.slide))
            Slide();

    }
    //Second Part of the transition
    public void EndTransition()
    {
        if(transitionAction.Equals(TransitionAction.loadScene))
            LoadScene();

        if(endTransition.Equals(EndTypeTransition.fadeAway))
            FadeAway();

        if(endTransition.Equals(EndTypeTransition.srink))
            Srink();
    } 

    
    void StopTransition()
    {
        TransitionPlaying = false;
    }

    //WaitsBeforeTransition Into the EndTransition
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime + TransitionDuration);
        EndTransition();
    }

    void FadeIn()
    {
        Child.transform.position = new Vector2(0, 0);

        color.a = 0;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        spriteRenderer.color = color;
        localScale = Child.transform.localScale;

        localScale = new Vector3(50,50,50);

        Child.transform.localScale = localScale;

        LeanTween.alpha(Child, 1, TransitionDuration);
        StartCoroutine(WaitTime());
    }
    void FadeAway()
    {
        LeanTween.alpha(Child, 0, TransitionDuration).setOnComplete(StopTransition);
    }

    void Grow()
    {
        Child.transform.position = new Vector2(0, 0);

        color.a = 1;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        localScale = new Vector3(0,0,0);
        Child.transform.localScale = localScale;

        Vector3 WantedSize = new Vector3(50,50,50);

        LeanTween.scale(Child, WantedSize, TransitionDuration);
        StartCoroutine(WaitTime());
    }

    void Srink()
    {
        Child.transform.position = new Vector2(0, 0);
        Vector3 WantedSize = new Vector3(0,0,0);

        LeanTween.scale(Child, WantedSize, TransitionDuration).setOnComplete(StopTransition);
    }


    void Slide()
    {
        Child.transform.position = new Vector2(-8.74f,3);

        color.a = 1;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        localScale = new Vector3(1,20,1);
        Child.transform.localScale = localScale;

        Vector3 WantedSize = new Vector3(37,20,1);

        LeanTween.scale(Child, WantedSize, TransitionDuration).setOnComplete(SlideDone);
        StartCoroutine(WaitTime());
    }
    void SlideDone()
    {
        Child.transform.position = new Vector2(0, 0);
    }
}