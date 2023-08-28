using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Oculus;


public class CustomHandAnimationExample : MonoBehaviour
{
    // OVRSkeleton 컴포넌트를 가지고 있는 GameObject (OVRCustomHandPrefab_R 프리팹)
    public OVRCustomSkeleton skeleton;
    
    // 움직이는 손 애니메이션을 위한 손 관절 Transform 배열
    public Transform[] handBones;

    string[] current_pose_data = new string[21];
    string[] previous_pose_data = new string[21];

    //current pressure data (row) index 
    int data_index = 1;
    bool start = true;

    //test_0712_회전 속도 조절
    bool rotation_complete = false;

    List<List<string>> pose_data;

    //Thumb joint 관련 변수 선언
    float rotation_thumb_cmc_spread, rotation_thumb_cmc_stretch, rotation_thumb_mcp_stretch, rotation_thumb_ip_stretch;
    Quaternion currentRotation_thumb_cmc, currentRotation_thumb_mcp, currentRotation_thumb_ip;
    Quaternion targetRotation_thumb_cmc_spread, targetRotation_thumb_cmc_stretch, targetRotation_thumb_mcp,targetRotation_thumb_ip;

    //Index finger joint 관련 변수 선언
    float rotation_index_mcp_spread, rotation_index_mcp_stretch, rotation_index_pip, rotation_index_dip ;  
    public Quaternion currentRotation_index_mcp, currentRotation_index_pip, currentRotation_index_dip;
    Quaternion targetRotation_index_mcp_spread, targetRotation_index_mcp_stretch, targetRotation_index_pip, targetRotation_index_dip;

    //Middle finger joint 관련 변수 선언
    float rotation_middle_mcp_spread, rotation_middle_mcp_stretch, rotation_middle_pip, rotation_middle_dip;        
    Quaternion currentRotation_middle_mcp, currentRotation_middle_pip, currentRotation_middle_dip;
    Quaternion targetRotation_middle_mcp_spread, targetRotation_middle_mcp_stretch, targetRotation_middle_pip, targetRotation_middle_dip;

    //Ring finger joint 관련 변수 선언
    float rotation_ring_mcp_spread, rotation_ring_mcp_stretch, rotation_ring_pip, rotation_ring_dip;        
    Quaternion currentRotation_ring_mcp, currentRotation_ring_pip, currentRotation_ring_dip;
    Quaternion targetRotation_ring_mcp_spread, targetRotation_ring_mcp_stretch, targetRotation_ring_pip, targetRotation_ring_dip;

    //pinky finger joint 관련 변수 선언
    float rotation_pinky_mcp_spread, rotation_pinky_mcp_stretch, rotation_pinky_pip, rotation_pinky_dip;         
    Quaternion currentRotation_pinky_mcp, currentRotation_pinky_pip, currentRotation_pinky_dip;
    Quaternion targetRotation_pinky_mcp_spread, targetRotation_pinky_mcp_stretch, targetRotation_pinky_pip, targetRotation_pinky_dip;

    public Transform myTransfrom;

    void Start()
    {
        //pose csv 파일 읽기
        pose_data = CSVReader.Read("P000, CNA, Free Motion (on shoulder height), trialSession-0, 120s, merged.csv", "pose");
        
        print("pose 시작시간 : "+ DateTime.Now);

        print(myTransfrom+"의 parrent : " + myTransfrom.parent.name);
        print("global position 출력");
        print("wrist" + skeleton.CustomBones[0].position);
        print("thumb1 : " + skeleton.CustomBones[3].position);
        print("thumb2 : " + skeleton.CustomBones[4].position);
        print("thumb3 : " + skeleton.CustomBones[5].position);
        print("thumb4 : " + skeleton.CustomBones[19].position);

        print("index1 : " + skeleton.CustomBones[6].position);
        print("index2 : " + skeleton.CustomBones[7].position);
        print("index3 : " + skeleton.CustomBones[8].position);
        print("index4 : " + skeleton.CustomBones[20].position); 
        //customBones[8]과 비슷한 위치 -> Hand_IndexTip = r_index_finger_pad_marker로 수정

        print("middle1 : " + skeleton.CustomBones[9].position);
        print("middle2 : " + skeleton.CustomBones[10].position);
        print("middle3 : " + skeleton.CustomBones[11].position);
        print("middle4 : " + skeleton.CustomBones[21].position); 

        print("ring1 : " + skeleton.CustomBones[12].position);
        print("ring2 : " + skeleton.CustomBones[13].position);
        print("ring3 : " + skeleton.CustomBones[14].position);
        print("ring4 : " + skeleton.CustomBones[22].position); 

        print("pinky1 : " + skeleton.CustomBones[16].position);
        print("pinky2 : " + skeleton.CustomBones[17].position);
        print("pinky3 : " + skeleton.CustomBones[18].position);
        print("pinky4 : " + skeleton.CustomBones[23].position); 
        //pose data 출력
        /*
        for(int i=0; i<pose_data.Count; i++)
        {
            for(int j=0; j< 21; j++) 
            {
                current_pose_data[j] = pose_data[i][j];
            } 

            
            string tmp ="";
            for(int x=0; x<21; x++) 
            {
                tmp += current_pose_data[x];
                tmp += ',';
            } 
            print("current_pose_data : " + tmp);
            
            tmp = "";
        } 
        */
    }
   
    void Update()
    {   
        print("TIme : " + Time.deltaTime);
        //current pose data 저장 
        for(int j=0; j< 20; j++) 
        {
            if(data_index==1) 
            {
                current_pose_data[j] = pose_data[data_index][j];
                previous_pose_data[j] = current_pose_data[j];
            }
            else 
            {
                previous_pose_data[j] = current_pose_data[j];
                current_pose_data[j] = pose_data[data_index][j];
            }
        } 
        if(start) InitiateHand();

        // 손 애니메이션 제어
        if(!rotation_complete)AnimateHand();

/*
0712 
coroutine 사용하든 안하든 동일 
첫번째 관절밖에 업데이트 안됨 -> AnimateHand() 첫번째 관절에만 적용됨
*/

        if(rotation_complete)
        {
            //data_index++;
            //실제 모션, 애니메이션 싱크 관련 사용하는 데이터 1/3 (hmd 미착용시)
            //0821. hmd상에서 매우 느리게 움직임. hmd의 처리속도, 프레임 시간 문제인듯함. 
            //데이터 1/24만 사용 (hmd 착용시)
            //hmd 착용 안하고 플레이시 deltaTime : 0.0008
            //hmd 착용 하고 플레이시 deltaTime : 0.01
            data_index += 24;
            rotation_complete =false;
        }
        
        if(data_index >= pose_data.Count) 
        {   
            print(pose_data.Count);
            print("pose 종료시간 : "+ DateTime.Now);
            Quit();
        };
    }

    //joint 초기화
    /*
    0714
    mcp, cmc의 경우  marker가 
    aa_axis : 좌우 움직임
    fe_axis : 앞뒤 구부림
    => 움직임 관련 수정 필요함 (spread stretch 곱해준거 필요 없을듯. customBones에서 각 관절 ID,name확인)
    */
    void InitiateHand()
    {   
        //thumb 초기화
        rotation_thumb_cmc_spread = -float.Parse(current_pose_data[0]);   //[thumb]-cmc-spread
        rotation_thumb_cmc_stretch = -(60-float.Parse(current_pose_data[1]));  //[thumb]-cmc-stretch
        rotation_thumb_mcp_stretch = -float.Parse(current_pose_data[2]);  //[thumb]-mcp-stretch
        rotation_thumb_ip_stretch = -float.Parse(current_pose_data[3]);   //[thunb]-ip-stretch

        currentRotation_thumb_cmc = skeleton.CustomBones[3].localRotation;
        currentRotation_thumb_mcp = skeleton.CustomBones[4].localRotation;
        currentRotation_thumb_ip = skeleton.CustomBones[5].localRotation;

        targetRotation_thumb_cmc_spread = Quaternion.Euler(0f, rotation_thumb_cmc_spread, 0f) * currentRotation_thumb_cmc;
        targetRotation_thumb_cmc_stretch = Quaternion.Euler(0f, 0f, rotation_thumb_cmc_stretch) * currentRotation_thumb_cmc;
        targetRotation_thumb_mcp = Quaternion.Euler(0f, 0f, rotation_thumb_mcp_stretch) * currentRotation_thumb_mcp;
        targetRotation_thumb_ip = Quaternion.Euler(0f, 0f, rotation_thumb_ip_stretch) * currentRotation_thumb_ip;

        skeleton.CustomBones[3].localRotation = targetRotation_thumb_cmc_spread;
        skeleton.CustomBones[3].localRotation = targetRotation_thumb_cmc_stretch;
        skeleton.CustomBones[4].localRotation = targetRotation_thumb_mcp;
        skeleton.CustomBones[5].localRotation = targetRotation_thumb_ip;



        //index finger 초기화
        rotation_index_mcp_spread = -float.Parse(current_pose_data[4]);     //[index]-mcp-spread
        rotation_index_mcp_stretch = -float.Parse(current_pose_data[5]);     //[index]-mcp-stretch
        rotation_index_pip = -float.Parse(current_pose_data[6]);     //[index]-pip-stretch
        rotation_index_dip = -float.Parse(current_pose_data[7]);     //[index]-dip-stretch

        // 손 관절의 현재 회전 값을 가져옴
        currentRotation_index_mcp = skeleton.CustomBones[6].localRotation;
        currentRotation_index_pip = skeleton.CustomBones[7].localRotation;
        currentRotation_index_dip = skeleton.CustomBones[8].localRotation;

        // 손 관절의 회전 값을 조작하여 애니메이션 적용
        targetRotation_index_mcp_spread = Quaternion.Euler(0f, rotation_index_mcp_spread, 0f) * currentRotation_index_mcp;
        targetRotation_index_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_index_mcp_stretch) * currentRotation_index_mcp;
        targetRotation_index_pip = Quaternion.Euler(0f, 0f, rotation_index_pip) * currentRotation_index_pip;
        targetRotation_index_dip = Quaternion.Euler(0f, 0f, rotation_index_dip) * currentRotation_index_dip;

        // 손 관절의 회전 값을 업데이트
        skeleton.CustomBones[6].localRotation = targetRotation_index_mcp_spread;
        skeleton.CustomBones[6].localRotation = targetRotation_index_mcp_stretch;
        skeleton.CustomBones[7].localRotation = targetRotation_index_pip;
        skeleton.CustomBones[8].localRotation = targetRotation_index_dip;




        //middle finger 초기화
        rotation_middle_mcp_spread = -float.Parse(current_pose_data[8]);      //[middle]-mcp-spread
        rotation_middle_mcp_stretch = -float.Parse(current_pose_data[9]);    //[middle]-mcp-stretch
        rotation_middle_pip = -float.Parse(current_pose_data[10]);            //[middle]-pip-stretch
        rotation_middle_dip = -float.Parse(current_pose_data[11]);            //[middle]-dip-stretch
        
        currentRotation_middle_mcp = skeleton.CustomBones[9].localRotation;
        currentRotation_middle_pip = skeleton.CustomBones[10].localRotation;
        currentRotation_middle_dip = skeleton.CustomBones[11].localRotation;

        targetRotation_middle_mcp_spread = Quaternion.Euler(0f, rotation_middle_mcp_spread, 0f) * currentRotation_middle_mcp;
        targetRotation_middle_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_middle_mcp_stretch) * currentRotation_middle_mcp;
        targetRotation_middle_pip = Quaternion.Euler(0f, 0f, rotation_middle_pip) * currentRotation_middle_pip;
        targetRotation_middle_dip = Quaternion.Euler(0f, 0f, rotation_middle_dip) * currentRotation_middle_dip;

        skeleton.CustomBones[9].localRotation = targetRotation_middle_mcp_spread;
        skeleton.CustomBones[9].localRotation = targetRotation_middle_mcp_stretch;
        skeleton.CustomBones[10].localRotation = targetRotation_middle_pip;
        skeleton.CustomBones[11].localRotation = targetRotation_middle_dip;



        //ring finger 초기화
        rotation_ring_mcp_spread = -float.Parse(current_pose_data[12]);      //[ring]-mcp-spread
        rotation_ring_mcp_stretch = -float.Parse(current_pose_data[13]);    //[ring]-mcp-stretch
        rotation_ring_pip = -float.Parse(current_pose_data[14]);            //[ring]-pip-stretch
        rotation_ring_dip = -float.Parse(current_pose_data[15]);            //[ring]-dip-stretch
        
        currentRotation_ring_mcp = skeleton.CustomBones[12].localRotation;
        currentRotation_ring_pip = skeleton.CustomBones[13].localRotation;
        currentRotation_ring_dip = skeleton.CustomBones[14].localRotation;

        targetRotation_ring_mcp_spread = Quaternion.Euler(0f, rotation_ring_mcp_spread, 0f) * currentRotation_ring_mcp;
        targetRotation_ring_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_ring_mcp_stretch) * currentRotation_ring_mcp;
        targetRotation_ring_pip = Quaternion.Euler(0f, 0f, rotation_ring_pip) * currentRotation_ring_pip;
        targetRotation_ring_dip = Quaternion.Euler(0f, 0f, rotation_ring_dip) * currentRotation_ring_dip;

        skeleton.CustomBones[12].localRotation = targetRotation_ring_mcp_spread;
        skeleton.CustomBones[12].localRotation = targetRotation_ring_mcp_stretch;
        skeleton.CustomBones[13].localRotation = targetRotation_ring_pip;
        skeleton.CustomBones[14].localRotation = targetRotation_ring_dip;



        //pinky finger 초기화
        rotation_pinky_mcp_spread = -float.Parse(current_pose_data[16]);      //[pinky]-mcp-spread
        rotation_pinky_mcp_stretch = -float.Parse(current_pose_data[17]);    //[pinky]-mcp-stretch
        rotation_pinky_pip = -float.Parse(current_pose_data[18]);            //[pinky]-pip-stretch
        rotation_pinky_dip = -float.Parse(current_pose_data[19]);            //[pinky]-dip-stretch
        
        currentRotation_pinky_mcp = skeleton.CustomBones[16].localRotation;
        currentRotation_pinky_pip = skeleton.CustomBones[17].localRotation;
        currentRotation_pinky_dip = skeleton.CustomBones[18].localRotation;

        targetRotation_pinky_mcp_spread = Quaternion.Euler(0f, rotation_pinky_mcp_spread, 0f) * currentRotation_pinky_mcp;
        targetRotation_pinky_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_pinky_mcp_stretch) * currentRotation_pinky_mcp;
        targetRotation_pinky_pip = Quaternion.Euler(0f, 0f, rotation_pinky_pip) * currentRotation_pinky_pip;
        targetRotation_pinky_dip = Quaternion.Euler(0f, 0f, rotation_pinky_dip) * currentRotation_pinky_dip;

        skeleton.CustomBones[16].localRotation = targetRotation_pinky_mcp_spread;
        skeleton.CustomBones[16].localRotation = targetRotation_pinky_mcp_stretch;
        skeleton.CustomBones[17].localRotation = targetRotation_pinky_pip;
        skeleton.CustomBones[18].localRotation = targetRotation_pinky_dip;

        start =false;
    }



    //시간별 각 관절 각도 차이값 -> 회전값
     /*
        0713. 
        첫번째 관절의 경우 두개의 Quaternion을 합침 (spread, stretch)
        합치기 위해서는 곱셈 연산을 사용하나 너무 빠르게 회전하게됨
        이를 해결하기 위해 Quaternion의 보간을 사용함 
        Quaternion.Slerp()함수 사용

        0717.
        엄지 제외 손가락의 경우
        spread,stretch 합치지 않고 순서대로 update -> 잘됨
        엄지는 동일하게 할 경우 너무 빨리 회전
    */
    void AnimateHand()
    {   
        //회전속도 조절 보간 test
        //float t = 0.0000005f;

        /*thumb*/
        rotation_thumb_cmc_spread = -(float.Parse(current_pose_data[0]) - float.Parse(previous_pose_data[0]));
        rotation_thumb_cmc_stretch =  (float.Parse(current_pose_data[1]) - float.Parse(previous_pose_data[1])); //thumb_cmc_stretch만 manus와 handmodel joint 기준 다름
        rotation_thumb_mcp_stretch = -(float.Parse(current_pose_data[2]) - float.Parse(previous_pose_data[2]));
        rotation_thumb_ip_stretch = -(float.Parse(current_pose_data[3]) - float.Parse(previous_pose_data[3]));

        currentRotation_thumb_cmc = skeleton.CustomBones[3].localRotation;
        currentRotation_thumb_mcp = skeleton.CustomBones[4].localRotation;
        currentRotation_thumb_ip = skeleton.CustomBones[5].localRotation;

        targetRotation_thumb_cmc_spread = Quaternion.Euler(0f, rotation_thumb_cmc_spread, 0f) * currentRotation_thumb_cmc;
        targetRotation_thumb_cmc_stretch = Quaternion.Euler(0f, 0f, rotation_thumb_cmc_stretch) * currentRotation_thumb_cmc;
        targetRotation_thumb_mcp = Quaternion.Euler(0f, 0f, rotation_thumb_mcp_stretch) * currentRotation_thumb_mcp;
        targetRotation_thumb_ip = Quaternion.Euler(0f, 0f, rotation_thumb_ip_stretch) * currentRotation_thumb_ip;

        //targetRotation_thumb_cmc_spread = Quaternion.Slerp(currentRotation_thumb_cmc, targetRotation_thumb_cmc_spread * targetRotation_thumb_cmc_stretch, t);

        skeleton.CustomBones[3].localRotation = targetRotation_thumb_cmc_spread; //slerp 제거하면서 수정
        skeleton.CustomBones[3].localRotation = targetRotation_thumb_cmc_stretch;
        skeleton.CustomBones[4].localRotation = targetRotation_thumb_mcp;
        skeleton.CustomBones[5].localRotation = targetRotation_thumb_ip;


        /*index finger*/

        // 손 애니메이션에 사용할 회전 값 계산
        //float rotation = Mathf.Sin(Time.time * rotationSpeed) * 0.1f;
        rotation_index_mcp_spread = -(float.Parse(current_pose_data[4]) - float.Parse(previous_pose_data[4]));
        rotation_index_mcp_stretch =  -(float.Parse(current_pose_data[5]) - float.Parse(previous_pose_data[5]));
        rotation_index_pip = -(float.Parse(current_pose_data[6]) - float.Parse(previous_pose_data[6]));
        rotation_index_dip = -(float.Parse(current_pose_data[7]) - float.Parse(previous_pose_data[7]));

        // 손 관절의 현재 회전 값을 가져옴
        
        currentRotation_index_mcp = skeleton.CustomBones[6].localRotation;
        currentRotation_index_pip = skeleton.CustomBones[7].localRotation;
        currentRotation_index_dip = skeleton.CustomBones[8].localRotation;

        // 손 관절의 회전 값을 조작하여 애니메이션 적용
        targetRotation_index_mcp_spread = Quaternion.Euler(0f, rotation_index_mcp_spread, 0f) * currentRotation_index_mcp;
        targetRotation_index_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_index_mcp_stretch) * currentRotation_index_mcp;
        targetRotation_index_pip = Quaternion.Euler(0f, 0f, rotation_index_pip) * currentRotation_index_pip;
        targetRotation_index_dip = Quaternion.Euler(0f, 0f, rotation_index_dip) * currentRotation_index_dip;

        //targetRotation_index_mcp_spread = Quaternion.Slerp(currentRotation_index_mcp, targetRotation_index_mcp_spread * targetRotation_index_mcp_stretch, t);
        //targetRotation_index_pip = Quaternion.Slerp(currentRotation_index_pip, targetRotation_index_pip, t);
        //targetRotation_index_dip = Quaternion.Slerp(currentRotation_index_dip, targetRotation_index_dip, t);
        

        // 손 관절의 회전 값을 업데이트
        skeleton.CustomBones[6].localRotation = targetRotation_index_mcp_spread; //slerp 제거하면서 수정
        skeleton.CustomBones[6].localRotation = targetRotation_index_mcp_stretch;
        skeleton.CustomBones[7].localRotation = targetRotation_index_pip;
        skeleton.CustomBones[8].localRotation = targetRotation_index_dip;

        //관절별 회전 각도
        float angleDifference = Quaternion.Angle(currentRotation_index_mcp,targetRotation_index_mcp_spread);
        float angleDifference_pip = Quaternion.Angle(currentRotation_index_pip,targetRotation_index_pip);
        float angleDifference_dip = Quaternion.Angle(currentRotation_index_dip,targetRotation_index_dip);
        //print("angelDiff : " + angleDifference);
        //print("pipDiff : " + angleDifference_pip);
        //print("dipDiff : " + angleDifference_dip);



        /*middle finger*/
        rotation_middle_mcp_spread = -(float.Parse(current_pose_data[8]) - float.Parse(previous_pose_data[8]));
        rotation_middle_mcp_stretch =  -(float.Parse(current_pose_data[9]) - float.Parse(previous_pose_data[9]));
        rotation_middle_pip = -(float.Parse(current_pose_data[10]) - float.Parse(previous_pose_data[10]));
        rotation_middle_dip = -(float.Parse(current_pose_data[11]) - float.Parse(previous_pose_data[11]));

        currentRotation_middle_mcp = skeleton.CustomBones[9].localRotation;
        currentRotation_middle_pip = skeleton.CustomBones[10].localRotation;
        currentRotation_middle_dip = skeleton.CustomBones[11].localRotation;

        targetRotation_middle_mcp_spread = Quaternion.Euler(0f, rotation_middle_mcp_spread, 0f) * currentRotation_middle_mcp;
        targetRotation_middle_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_middle_mcp_stretch) * currentRotation_middle_mcp;
        targetRotation_middle_pip = Quaternion.Euler(0f, 0f, rotation_middle_pip) * currentRotation_middle_pip;
        targetRotation_middle_dip = Quaternion.Euler(0f, 0f, rotation_middle_dip) * currentRotation_middle_dip;

        //targetRotation_middle_mcp_spread = Quaternion.Slerp(currentRotation_middle_mcp, targetRotation_middle_mcp_spread * targetRotation_middle_mcp_stretch, t);

        skeleton.CustomBones[9].localRotation = targetRotation_middle_mcp_spread ;   //slerp 제거하면서 수정
        skeleton.CustomBones[9].localRotation = targetRotation_middle_mcp_stretch ;
        skeleton.CustomBones[10].localRotation = targetRotation_middle_pip;
        skeleton.CustomBones[11].localRotation = targetRotation_middle_dip;



        /*ring finger*/
        rotation_ring_mcp_spread = -(float.Parse(current_pose_data[12]) - float.Parse(previous_pose_data[12]));
        rotation_ring_mcp_stretch =  -(float.Parse(current_pose_data[13]) - float.Parse(previous_pose_data[13]));
        rotation_ring_pip = -(float.Parse(current_pose_data[14]) - float.Parse(previous_pose_data[14]));
        rotation_ring_dip = -(float.Parse(current_pose_data[15]) - float.Parse(previous_pose_data[15]));

        currentRotation_ring_mcp = skeleton.CustomBones[12].localRotation;
        currentRotation_ring_pip = skeleton.CustomBones[13].localRotation;
        currentRotation_ring_dip = skeleton.CustomBones[14].localRotation;

        targetRotation_ring_mcp_spread = Quaternion.Euler(0f, rotation_ring_mcp_spread, 0f) * currentRotation_ring_mcp;
        targetRotation_ring_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_ring_mcp_stretch) * currentRotation_ring_mcp;
        targetRotation_ring_pip = Quaternion.Euler(0f, 0f, rotation_ring_pip) * currentRotation_ring_pip;
        targetRotation_ring_dip = Quaternion.Euler(0f, 0f, rotation_ring_dip) * currentRotation_ring_dip;

        //targetRotation_ring_mcp_spread = Quaternion.Slerp(currentRotation_ring_mcp, targetRotation_ring_mcp_spread * targetRotation_ring_mcp_stretch, t);

        skeleton.CustomBones[12].localRotation = targetRotation_ring_mcp_spread;      //slerp 제거하면서 수정
        skeleton.CustomBones[12].localRotation = targetRotation_ring_mcp_stretch;
        skeleton.CustomBones[13].localRotation = targetRotation_ring_pip;
        skeleton.CustomBones[14].localRotation = targetRotation_ring_dip;



        /*pinky finger*/
        rotation_pinky_mcp_spread = -(float.Parse(current_pose_data[16]) - float.Parse(previous_pose_data[16]));
        rotation_pinky_mcp_stretch =  -(float.Parse(current_pose_data[17]) - float.Parse(previous_pose_data[17]));
        rotation_pinky_pip = -(float.Parse(current_pose_data[18]) - float.Parse(previous_pose_data[18]));
        rotation_pinky_dip = -(float.Parse(current_pose_data[19]) - float.Parse(previous_pose_data[19]));

        currentRotation_pinky_mcp = skeleton.CustomBones[16].localRotation;
        currentRotation_pinky_pip = skeleton.CustomBones[17].localRotation;
        currentRotation_pinky_dip = skeleton.CustomBones[18].localRotation;

        targetRotation_pinky_mcp_spread = Quaternion.Euler(0f, rotation_pinky_mcp_spread, 0f) * currentRotation_pinky_mcp;
        targetRotation_pinky_mcp_stretch = Quaternion.Euler(0f, 0f, rotation_pinky_mcp_stretch) * currentRotation_pinky_mcp;
        targetRotation_pinky_pip = Quaternion.Euler(0f, 0f, rotation_pinky_pip) * currentRotation_pinky_pip;
        targetRotation_pinky_dip = Quaternion.Euler(0f, 0f, rotation_pinky_dip) * currentRotation_pinky_dip;

        //targetRotation_pinky_mcp_spread = Quaternion.Slerp(currentRotation_pinky_mcp, targetRotation_pinky_mcp_spread * targetRotation_pinky_mcp_stretch, t);

        skeleton.CustomBones[16].localRotation = targetRotation_pinky_mcp_spread ;    //slerp 제거하면서 수정
        skeleton.CustomBones[16].localRotation = targetRotation_pinky_mcp_stretch ;
        skeleton.CustomBones[17].localRotation = targetRotation_pinky_pip;
        skeleton.CustomBones[18].localRotation = targetRotation_pinky_dip;


        rotation_complete = true;
    }

    //종료
    public void Quit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}



