using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicators : MonoBehaviour
{
    [SerializeField] GameObject onePlayerInd;
    [SerializeField] GameObject twoPlayerInd;



    void Update()
    {
        onePlayerInd.transform.position = PlayerManager.GetInstance().p1RobotGO.transform.position;
        twoPlayerInd.transform.position = PlayerManager.GetInstance().p2RobotGO.transform.position;    
    }
}
