using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class dollyManager : MonoBehaviour
{
    public bool fakeTwoTouches = false;
    public Cinemachine.CinemachineDollyCart dollyY;
    public Cinemachine.CinemachineDollyCart dollyX;
    public Vector3 startPoint;
    public Vector3 currentPoint;
    public Vector3 offset;
    [FormerlySerializedAs("speedMultiplier")] public float speedMultiplierY = 1f;
    public float speedMultiplierX = 1f;
    public float magnitude;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool twoTouches;
        if (fakeTwoTouches)
        {
            twoTouches = Input.touchCount == 1;
            if(twoTouches){
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    startPoint = Input.touches[0].position;

                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    currentPoint = Input.touches[0].position;
                }
                else if ((Input.touches[0].phase == TouchPhase.Ended))
                {
                    startPoint = Vector3.zero;
                    currentPoint = Vector3.zero;
                    offset = Vector3.zero;
                    dollyY.m_Speed = 0;
                    dollyX.m_Speed = 0;
                    magnitude = 0;
                }
            }
            else
            {
                startPoint = Vector3.zero;
                currentPoint = Vector3.zero;
                offset = Vector3.zero;
                dollyY.m_Speed = 0;
                dollyX.m_Speed = 0;
                magnitude = 0;
            }
            
            
            
        }
        else
        {
            twoTouches = Input.touchCount == 2;
            if(twoTouches)
            {
                if (Input.touches[0].phase == TouchPhase.Began ||
                    Input.touches[1].phase == TouchPhase.Began)
                {
                    startPoint = Input.touches[0].position + Input.touches[1].position;
                    startPoint /= 2;
                }
                else if (Input.touches[0].phase == TouchPhase.Moved &&
                         Input.touches[1].phase == TouchPhase.Moved)
                {
                    currentPoint = Input.touches[0].position + Input.touches[1].position;
                    currentPoint /= 2;
                }
                else if ((Input.touches[0].phase == TouchPhase.Ended ||
                          Input.touches[1].phase == TouchPhase.Ended))
                {
                    startPoint = Vector3.zero;
                    currentPoint = Vector3.zero;
                    offset = Vector3.zero;
                    dollyY.m_Speed = 0;
                    dollyY.m_Speed = 0;
                    magnitude = 0;
                }
            }
            else
            {
                startPoint = Vector3.zero;
                currentPoint = Vector3.zero;
                offset = Vector3.zero;
                dollyY.m_Speed = 0;
                dollyY.m_Speed = 0;
                magnitude = 0;
            }
        }
        


        if (startPoint != Vector3.zero && currentPoint != Vector3.zero)
        {
            offset = currentPoint - startPoint;
            magnitude = offset.magnitude;
            offset = offset.normalized;
            dollyY.m_Speed = magnitude * (speedMultiplierY/1000) * offset.y;
            dollyX.m_Speed = magnitude * (speedMultiplierY/1000) * offset.x;

            float dollyYPos = dollyY.m_Position;
            float maxDollyXPosAbs = (1 - dollyYPos) / 2;
            if(dollyX.m_Position > .5f + maxDollyXPosAbs) 
            {
                dollyX.m_Position = .5f + maxDollyXPosAbs;
                dollyX.m_Speed = 0;
            }
            else if(dollyX.m_Position < .5f - maxDollyXPosAbs)
            {
                dollyX.m_Position = .5f - maxDollyXPosAbs;
                dollyX.m_Speed = 0;
            }

            if (maxDollyXPosAbs < .2f)
            {
                dollyX.m_Speed = 0;
                dollyX.m_Position = .5f;
            }


        }
    }
}
