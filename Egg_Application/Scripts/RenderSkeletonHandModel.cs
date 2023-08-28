using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Hand Skeleton Model의 joint, sphere의 position을 설정
skeleton.CustomBones[] 의 global position을 가져옴
joint : 각 joint별 sphere를 생성하고, sphere의 position을 skeleton.CustomBones[]의 position으로 설정
joint 잇는 선 : 각 endpoint의 LineRenderer 컴포넌트로 등록
*/
public class RenderSkeletonHandModel : MonoBehaviour
{
    public OVRCustomSkeleton skeleton;

    //joint 위치 업데이트용
    Vector3[] newPositions_thumb = new Vector3[5]; 
    Vector3[] newPositions_index = new Vector3[5]; 
    Vector3[] newPositions_middle = new Vector3[5]; 
    Vector3[] newPositions_ring = new Vector3[5]; 
    Vector3[] newPositions_pinky = new Vector3[5]; 

    /*
    name : SetJointPosition
    parameter : 
    return : 

    각 손가락별 enpoint의 LineRenderer 가져와 관절에 맞춰 position 설정
    */
    private void SetJointPosition()
    {
        GameObject thumb_endpoint,index_endpoint,middle_endpoint,ring_endpoint,pinky_endpoint;
        thumb_endpoint = GameObject.FindWithTag("Thumb_endpoint");
        index_endpoint = GameObject.FindWithTag("Index_finger_endpoint");
        middle_endpoint = GameObject.FindWithTag("middle_finger_endpoint");
        ring_endpoint = GameObject.FindWithTag("ring_finger_endpoint");
        pinky_endpoint = GameObject.FindWithTag("pinky_finger_endpoint");

        //각 손가락별 선 가져옴
        LineRenderer renderer_thumb = thumb_endpoint.GetComponent<LineRenderer>();
        LineRenderer renderer_index = index_endpoint.GetComponent<LineRenderer>();
        LineRenderer renderer_middle = middle_endpoint.GetComponent<LineRenderer>();
        LineRenderer renderer_ring = ring_endpoint.GetComponent<LineRenderer>();
        LineRenderer renderer_pinky = pinky_endpoint.GetComponent<LineRenderer>();


        //thumb joint position 가져옴
        Vector3 position_wrist = skeleton.CustomBones[0].position;
        Vector3 position_thumb1 = skeleton.CustomBones[3].position;
        Vector3 position_thumb2 = skeleton.CustomBones[4].position;
        Vector3 position_thumb3 = skeleton.CustomBones[5].position;
        Vector3 position_thumb4 = skeleton.CustomBones[19].position;

        newPositions_thumb[0] = position_wrist;
        newPositions_thumb[1] = position_thumb1;
        newPositions_thumb[2] = position_thumb2;
        newPositions_thumb[3] = position_thumb3;
        newPositions_thumb[4] = position_thumb4;


        //index joint position 가져옴
        Vector3 position_index1 = skeleton.CustomBones[6].position;
        Vector3 position_index2 = skeleton.CustomBones[7].position;
        Vector3 position_index3 = skeleton.CustomBones[8].position;
        Vector3 position_index4 = skeleton.CustomBones[20].position;

        newPositions_index[0] = position_wrist;
        newPositions_index[1] = position_index1;
        newPositions_index[2] = position_index2;
        newPositions_index[3] = position_index3;
        newPositions_index[4] = position_index4;


        //middle joint position 가져옴
        Vector3 position_middle1 = skeleton.CustomBones[9].position;
        Vector3 position_middle2 = skeleton.CustomBones[10].position;
        Vector3 position_middle3 = skeleton.CustomBones[11].position;
        Vector3 position_middle4 = skeleton.CustomBones[21].position;

        newPositions_middle[0] = position_wrist;
        newPositions_middle[1] = position_middle1;
        newPositions_middle[2] = position_middle2;
        newPositions_middle[3] = position_middle3;
        newPositions_middle[4] = position_middle4;

        //ring joint position 가져옴
        Vector3 position_ring1 = skeleton.CustomBones[12].position;
        Vector3 position_ring2 = skeleton.CustomBones[13].position;
        Vector3 position_ring3 = skeleton.CustomBones[14].position;
        Vector3 position_ring4 = skeleton.CustomBones[22].position;

        newPositions_ring[0] = position_wrist;
        newPositions_ring[1] = position_ring1;
        newPositions_ring[2] = position_ring2;
        newPositions_ring[3] = position_ring3;
        newPositions_ring[4] = position_ring4;

        //pinky joint position 가져옴
        Vector3 position_pinky1 = skeleton.CustomBones[16].position;
        Vector3 position_pinky2 = skeleton.CustomBones[17].position;
        Vector3 position_pinky3 = skeleton.CustomBones[18].position;
        Vector3 position_pinky4 = skeleton.CustomBones[23].position;

        newPositions_pinky[0] = position_wrist;
        newPositions_pinky[1] = position_pinky1;
        newPositions_pinky[2] = position_pinky2;
        newPositions_pinky[3] = position_pinky3;
        newPositions_pinky[4] = position_pinky4;

        //각 joint 의 position update
        for(int i=0; i<5; i++)
        {
            renderer_thumb.SetPosition(i,newPositions_thumb[i]);
            renderer_index.SetPosition(i,newPositions_index[i]);
            renderer_middle.SetPosition(i,newPositions_middle[i]);
            renderer_ring.SetPosition(i,newPositions_ring[i]);
            renderer_pinky.SetPosition(i,newPositions_pinky[i]);
        }
    }

    void Start()
    {
        SetJointPosition();        
    }

    void Update()
    {
        SetJointPosition();
    }
}
