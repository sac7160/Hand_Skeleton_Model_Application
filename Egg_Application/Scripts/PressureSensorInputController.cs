using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
각 손가락 endpoint의 fsr 데이터 받아와서 데이터별 endpoint 색상 변화
*/
public class PressureSensorInputController : MonoBehaviour
{
    //현재 pressure data 저장할 string list
    string[] current_pressure_data = new string[6];

    //current pressure data (row) index 
    int data_index = 1;

    public double pressure_avg;
    public double pressure_thumb;

    //fsr csv 파일 읽어 저장할 2차원 리스트 
    List<List<string>> pressure_data;

    /*
    name : SelectColor
    parameter : fsr sensor data
    return : sensor 값별 endpoint 색상
    사용하는 fsr 센서마다 range가 달라 값 설정 필요
    */
    Color SelectColor(double data)
    {
        if(data < 4) return Color.white;
        else if(data<8) return Color.yellow;
        else if(data<12) return Color.green;
        else if(data<16) return Color.red;
        else return Color.black;
    }


     /*
    name : ChangeEndpointColor
    parameter : 
    return : 
    endpoint 색상 변경
    */
    void ChangeEndpointColor()
    {
        GameObject thumb,index,middle,ring,pinky;
        thumb = GameObject.FindWithTag("Thumb_endpoint");
        index = GameObject.FindWithTag("Index_finger_endpoint");
        middle = GameObject.FindWithTag("middle_finger_endpoint");
        ring = GameObject.FindWithTag("ring_finger_endpoint");
        pinky = GameObject.FindWithTag("pinky_finger_endpoint");

        Renderer renderer_thumb = thumb.GetComponent<Renderer>();
        Renderer renderer_index = index.GetComponent<Renderer>();
        Renderer renderer_middle = middle.GetComponent<Renderer>();
        Renderer renderer_ring = ring.GetComponent<Renderer>();
        Renderer renderer_pinky = pinky.GetComponent<Renderer>();
        
        renderer_thumb.material.color = SelectColor(Convert.ToDouble(current_pressure_data[5]));
        renderer_index.material.color = SelectColor(Convert.ToDouble(current_pressure_data[4]));
        renderer_middle.material.color = SelectColor(Convert.ToDouble(current_pressure_data[3]));
        renderer_ring.material.color = SelectColor(Convert.ToDouble(current_pressure_data[2]));
        renderer_pinky.material.color = SelectColor(Convert.ToDouble(current_pressure_data[1]));

        pressure_thumb = Convert.ToDouble(current_pressure_data[5]);    //달걀 잡았는지 판정을 위한 엄지fsr 데이터
    }

    void Start()
    {
        //fsr csv 파일 읽기
        pressure_data = CSVReader.Read("P000, CNA, Ball, trialSession-1, 60s, merged.csv", "fsr");   
        print("fsr 시작 시간 : " + DateTime.Now);
    }

    /*
    fsr을 통해 달걀이 깨짐을 판정하기 위한 값 리턴
    최소 2개 이상의 손가락이 잡고있을때
    fsr의 평균값을 리턴함
    */
    double define_crack()
    {
        double cnt = 0;
        double tmp = 0;
        for(int i=1; i<=5; i++)
        {
            if(Convert.ToDouble(current_pressure_data[i]) > 50) 
            {
                cnt++;   //손가락 fsr이 물체를 잡고있을때 
                tmp += Convert.ToDouble(current_pressure_data[i]);
            }
        }

        if(cnt < 2) return 0;   //최소 손가락 2개 이상일 때
        else return tmp/cnt;
    }

    void Update()
    {
        //current pressure data 저장
        for(int j=1; j<=5; j++) 
        {
            current_pressure_data[j] = pressure_data[data_index][j];
        } 

        ChangeEndpointColor();

        pressure_avg = define_crack();

        //data_index++;
        //실제 모션, 애니메이션 싱크 관련 사용하는 데이터 1/3
        //data_index += 3;
        //hmd 착용시 1/24
        data_index += 24;

        //fsr 데이터 끝났을때 동작 설정 필요
        if(data_index >= pressure_data.Count) {
            print("fsr 종료 시간 : " + DateTime.Now);
            data_index =1;
        }
    }


}
