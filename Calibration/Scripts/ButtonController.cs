using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//왼쪽에 위치한 start 버튼 컨트롤러
public class ButtonController : MonoBehaviour
{
    public bool pressed = false;

    public void buttonPressed()
    {
        pressed = true;
        print("pressed!!!!!!!!!!!!!");
    }
}
