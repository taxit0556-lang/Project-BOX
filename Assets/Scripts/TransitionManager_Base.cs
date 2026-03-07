using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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

    public GameObject Cam;


    [Header("Settings")]
    public Sprite Test;
    public  bool TransitionPlaying;
    public float TransitionDuration;
    public float waitTime;
    public string SceneName;
    public string State;
    

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

        Child.transform.position = new Vector2(Cam.transform.position.x, Cam.transform.position.y);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void PlayTransition()
    {
        State = "start";
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
        State = "2Part";
        if(transitionAction.Equals(TransitionAction.loadScene))
            LoadScene();

        if(endTransition.Equals(EndTypeTransition.fadeAway))
            FadeAway();

        if(endTransition.Equals(EndTypeTransition.srink))
            Srink();
    } 

    
    void StopTransition()
    {
        State = "done";
        TransitionPlaying = false;
    }

    //WaitsBeforeTransition Into the EndTransition
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime + TransitionDuration);
        EndTransition();
    }

    IEnumerator MiddleState()
    {
        yield return new WaitForSeconds(waitTime);
        State = "Middle";
    }

    void FadeIn()
    {
        Child.transform.position = new Vector2(Cam.transform.position.x + 0, 0);

        color.a = 0;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        spriteRenderer.color = color;
        localScale = Child.transform.localScale;

        localScale = new Vector3(100,70,50);

        Child.transform.localScale = localScale;

        LeanTween.alpha(Child, 1, TransitionDuration);
        StartCoroutine(WaitTime());
        StartCoroutine(MiddleState());
    }
    void FadeAway()
    {
        LeanTween.alpha(Child, 0, TransitionDuration).setOnComplete(StopTransition);
    }

    void Grow()
    {
        Child.transform.position = new Vector2(Cam.transform.position.x + 0, 0);

        color.a = 1;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        localScale = new Vector3(0,0,0);
        Child.transform.localScale = localScale;

        Vector3 WantedSize = new Vector3(200,200,200);

        LeanTween.scale(Child, WantedSize, TransitionDuration);
        StartCoroutine(WaitTime());
        StartCoroutine(MiddleState());
    }

    void Srink()
    {
        Child.transform.position = new Vector2(Cam.transform.position.x + 0, 0);
        Vector3 WantedSize = new Vector3(0,0,0);

        LeanTween.scale(Child, WantedSize, TransitionDuration).setOnComplete(StopTransition);
    }


    void Slide()
    {
        Child.transform.position = new Vector2(Cam.transform.position.x + -8.74f,3);

        color.a = 1;
        spriteRenderer.color = color;

        TransitionPlaying = true;
        localScale = new Vector3(1,20,1);
        Child.transform.localScale = localScale;

        Vector3 WantedSize = new Vector3(37,50,1);

        LeanTween.scale(Child, WantedSize, TransitionDuration).setOnComplete(SlideDone);
        StartCoroutine(WaitTime());
        StartCoroutine(MiddleState());
    }
    void SlideDone()
    {
        Child.transform.position = new Vector2(0, 0);
    }
}