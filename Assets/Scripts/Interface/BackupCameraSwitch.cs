using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script will switch the camera view when reversing
///     used as a backup camera 
/// </summary>

public class BackupCameraSwitch : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject backupCam;
    public GameObject armCam;

    private float reverseStatus;

    void Start()
    {
        backupCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        reverseStatus = Input.GetAxisRaw("Vertical");

        if (reverseStatus < 0)
        {
            
            backupCam.SetActive(true);
            mainCam.SetActive(false);
            armCam.SetActive(false);
        }
        
        else
        {
            mainCam.SetActive(true);
            backupCam.SetActive(false);
            armCam.SetActive(false);
        }
    }
}
