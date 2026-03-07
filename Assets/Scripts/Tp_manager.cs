using UnityEngine;

public class Tp_manager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject Panel;
    [SerializeField] Transform Player;

    [Header("TP_Points")]
    [SerializeField] Transform TpPoint1;
    [SerializeField] Transform TpPoint2;
    [SerializeField] Transform TpPoint3;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            Panel.SetActive(!Panel.activeSelf);
    }


    public void GoTo1()
    {
        Debug.Log("1");
        Player.position = new Vector2(TpPoint1.position.x, TpPoint1.position.y);
    }
    public void GoTo2()
    {
        Debug.Log("2");
        Player.position = new Vector2(TpPoint2.position.x, TpPoint2.position.y);
    }
    public void GoTo3()
    {
        Debug.Log("3");
        Player.position = new Vector2(TpPoint3.position.x, TpPoint3.position.y);
    }
}
