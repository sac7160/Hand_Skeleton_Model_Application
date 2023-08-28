using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour
{
    TextMeshProUGUI textUI;
    private PressureSensorInputController pressure;
    private CustomHandAnimationExample pose;
    
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        pressure = FindObjectOfType<PressureSensorInputController>();
        pose = FindObjectOfType<CustomHandAnimationExample>();
    }

    void Update()
    {
        /*
        pressure data, pose data를 가져와서 계란을 잡았는지 판단함
        엄지의 pressure값이 50보다 크고, 두개 이상의손가락 각도가 90도 이하일때 잡은 것으로 판단
        */
        if(pressure.pressure_thumb >50 && pose.pose_grabbed == true)
        {
            textUI.text = "Grabbed";
        }
        else
        {
            textUI.text = "not grabbed";
        }
    }
}
