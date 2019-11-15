using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float smooth;                // 平滑度
    public Transform cameraFPTran;  //人物活动时摄像机跟随位置
    public Transform cameraStartTran;  //摄像机初始位置

    public GameObject jetSlider;

    private GameObject player;        // 目标
    private Vector3 targetPosition;        // 摄像机移动目标
    private Vector3 cameraFPOffset;
    private bool isStart = true;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraFPOffset = cameraFPTran.position - player.transform.position;
        //摄像机放在初始位置，置标志位为f
        jetSlider.SetActive(false);
    }
    void LateUpdate()
    {
        if (isStart)
        {
            CameraFollow(cameraFPOffset);
        }
    }
    public void SetJetSlider(float value)
    {
        jetSlider.GetComponent<Image>().fillAmount = value;
    }
    void CameraFollow(Vector3 follow)
    {
        targetPosition = player.transform.position + follow;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
        transform.LookAt(player.transform);
    }
}
