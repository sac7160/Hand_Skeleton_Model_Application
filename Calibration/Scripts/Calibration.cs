using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    Calibration_Phase cal_phase;
    public TextMeshProUGUI instructionTextUI,phaseTextUI,progressIndicator;
    private ButtonController buttonController;
    public GameObject progressBar;
    private GameObject palm,fist;

    private int phase_num = 1;  //각 phase 마다 1, moveToNextPhase할 때 마다 1씩 증가

    public enum Calibration_Phase
    {
        Ready_Start,    //시작
        Ready_Count,    //calibration 시작버튼 눌렀을때
        Fist,
        Palm
    }

    //calibration 다음 단계로 이동
    public void moveToNextPhase()
    {
        if(cal_phase == Calibration_Phase.Fist || cal_phase == Calibration_Phase.Palm)
        {
            cal_phase = Calibration_Phase.Ready_Start;
        }
        else if(cal_phase == Calibration_Phase.Ready_Count)
        {
            if(phase_num == 2) cal_phase = Calibration_Phase.Fist;
            else if(phase_num == 5) cal_phase = Calibration_Phase.Palm;
        }
        else cal_phase++;
        
        phase_num++;
    }

    //중앙 지시 Text 그리기
    void setInstructionText()
    {
        if(cal_phase == Calibration_Phase.Ready_Start)  
        {
            instructionTextUI.text = "If you are ready, press the 'start' button";
        }
        else if(cal_phase == Calibration_Phase.Ready_Count)
        {
            if(phase_num == 2) instructionTextUI.text = "Curl your fingers fully and keep your thumb to the side";
            else if(phase_num == 5) instructionTextUI.text = "Extend Your fingers naturally";
        }
        else if(cal_phase == Calibration_Phase.Fist)
        {
            instructionTextUI.text = "Curl your fingers fully and keep your thumb to the side";
        }
        else if(cal_phase == Calibration_Phase.Palm)
        {
            instructionTextUI.text = "Extend Your fingers naturally";   
        }
    }
    
    //왼쪽 Text 그리기
    void setPhaseText()
    {
        if(cal_phase == Calibration_Phase.Ready_Start)
        {
            phaseTextUI.text = "Ready";   
        }
        else if(cal_phase == Calibration_Phase.Fist)
        {
            phaseTextUI.text = "Fist";
        }
        else if(cal_phase == Calibration_Phase.Palm)
        {
            phaseTextUI.text = "Palm";
        }
           
    }
    
    //각 HandModel 초기화
    void initiateObject()
    {
        palm = GameObject.FindGameObjectWithTag("Palm");
        fist = GameObject.FindGameObjectWithTag("Fist");
    }

    //각 단계에 따라 오브젝트 Active 설정
    void enableObject()
    {
        if(cal_phase == Calibration_Phase.Ready_Start)
        {
            palm.SetActive(true);
            fist.SetActive(false);
            progressBar.SetActive(false);
        }
        else if(cal_phase == Calibration_Phase.Ready_Count)
        {
            if(phase_num == 2) 
            {
                palm.SetActive(false);
                fist.SetActive(true);
            }
            else if(phase_num == 5)
            {
                palm.SetActive(true);
                fist.SetActive(false);
            }
        }
        else if(cal_phase == Calibration_Phase.Fist)
        {
            palm.SetActive(false);
            fist.SetActive(true);
            progressBar.SetActive(true);
        }
        else if(cal_phase == Calibration_Phase.Palm)
        {
            palm.SetActive(true);
            fist.SetActive(false);
            progressBar.SetActive(true);
        }
    }


    //3초 카운트다운
    private float time = 3f;
    private bool startTimer = false;
    void countDown()
    {
        if(time>0) time -= Time.deltaTime;
        phaseTextUI.text = Mathf.Ceil(time).ToString();
    }




    void Start()
    {
        initiateObject();
        buttonController = FindObjectOfType<ButtonController>();
        cal_phase = Calibration_Phase.Ready_Start;
    }



    void Update()
    {
        setInstructionText();
        setPhaseText();
        enableObject();
        if(buttonController.pressed == true)
        {
            moveToNextPhase();
            startTimer = true;
            buttonController.pressed = false;
        }
        
        if(startTimer == true) countDown();
        
        if(time <= 0)    //타이머 완료시
        {
            time = 3f;
            startTimer = false;
            moveToNextPhase();
        }
    }
}
