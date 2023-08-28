using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//calibration 진행도를 표시하는 오른쪽에 위치하는 원형 bar
public class ProgressBar : MonoBehaviour
{   
    public TextMeshProUGUI progressIndicator;
    public Image bar;
    public Calibration calibration;
    float currentValue;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        calibration = FindObjectOfType<Calibration>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentValue < 100)
        {
            currentValue += speed * Time.deltaTime;
            progressIndicator.text = ((int)currentValue).ToString() + "%";
        }
        else
        {
            currentValue = 0;
            calibration.moveToNextPhase();
        }

        bar.fillAmount = currentValue / 100;
    }
}
