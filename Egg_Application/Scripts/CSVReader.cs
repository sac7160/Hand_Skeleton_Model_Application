using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
CSV 파일 읽기
',' 기준으로 split
5개의 데이터씩 
*/
public class CSVReader : MonoBehaviour
{
    /*
    name : Read
    parameter : file name, 데이터 종류(ex)pose,fsr
    return : 2차원 String List 
    */
    public static List<List<string>> Read(string file, string data)
    {   
        //file path
        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + file);    
        
        //2차원 리스트 생성 
        //csv 파일 데이터 2차원 리스트에 저장
        List<List<string>> list = new List<List<string>>(); 

        bool endOfFile = false;
        
        //1행씩 읽음
        while (!endOfFile)
        {
            string data_String = sr.ReadLine();
            if (data_String == null)
            {
                endOfFile = true;
                break;
            }

            var data_values = data_String.Split(',');   
            var tmp = new List<string>();   //하나의 행 데이터 저장
            
            //merged 파일에서 각 데이터에 해당하는 열 
            if(data == "fsr") for(int i=0+9; i<6+9; i++) tmp.Add(data_values[i]); //fsr 데이터 개수(열)
            else if(data == "pose") for(int i=0+14; i<20+14; i++) tmp.Add(data_values[i]);    //pose 데이터 개수(열)
           
            list.Add(tmp);  //2차원 리스트에 추가
        }
        return list;
    }
}