using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;


/// <summary>
///     This script detects mouse position
///     use it to control the active camera
/// </summary>
public class JointJoystick : MonoBehaviour
{
    public float mouseSensitivity;

    public ArticulationBody shoulderJoint;
    public ArticulationBody elbowJoint;
    public ArticulationBody wristJoint1;
    public ArticulationBody wristJoint2;

    private ArticulationBody selectedJoint1;
    private ArticulationBody selectedJoint2;

    public ArticulationBody leftGripper1;
    public ArticulationBody leftGripper2;
    public ArticulationBody rightGripper1;
    public ArticulationBody rightGripper2;


    public ArticulationJointController jointController;

    public float speed = 1.0f;

    private float mouseX;
    private float mouseY;

    private float gripperHome = 0f;
    private float gripperOn = 25f;
    private float r1Home;
    private float r2Home;

    private float yawRotation = 0f;
    private float pitchRotation = 0f;

    public float angleLimit = 90f;

    public float yawOffset = 0f;
    public float pitchOffset = 0f;
    private float yawOffsetDeg;
    private float pitchOffsetDeg;
    private bool jointSetting;
    private bool gripperSetting = false;

    private float prevTrigger;

        [Tooltip("Color to highlight the currently selected join")]
        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);
        public Color undoColor; //= new Color(0.0f, 1.0f, 1.0f, 1.0f);
        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]
        public int selectedIndex;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // from MouseCameraControl.cs
        // since its 0s, I think this can be ignored, but not willing to risk it
        yawOffsetDeg = yawOffset * Mathf.Rad2Deg;
        pitchOffsetDeg = pitchOffset * Mathf.Rad2Deg;
        yawRotation = yawOffsetDeg;
        pitchRotation = pitchOffsetDeg;

        jointSetting = true;

        //set the proper colors     
        selectedJoint1 = wristJoint1;
        selectedJoint2 = wristJoint2;
        Highlight(undoColor);

        selectedJoint1 = shoulderJoint;
        selectedJoint2 = elbowJoint;
        Highlight(highLightColor);
        
        prevTrigger = Input.GetAxis("Trigger");
        
    }

    // Update is called once per frame
    void Update()
    {
        bool SelectionInput1 = Input.GetKeyDown("joystick button 4");
        bool SelectionInput2 = Input.GetKeyDown("joystick button 5");

        // change what joints are selected if the bumpers are hit
        if (SelectionInput2 | SelectionInput1)
        {
            jointSetting = !jointSetting;

            // un-highlight the old joints
            Highlight(undoColor);

            // switch to new joints
            if (jointSetting)
            {
                selectedJoint1 = shoulderJoint;
                selectedJoint2 = elbowJoint;
            }
            else
            {
                selectedJoint1 = wristJoint1;
                selectedJoint2 = wristJoint2;
            }
 
            // highlight new joints
            Highlight(highLightColor);
        }

        mouseX = Input.GetAxis("DPadVertical");
        mouseY = Input.GetAxis("DPadHorizontal");

    }

    void FixedUpdate()
    {
        float trigger = Input.GetAxis("Trigger");
        float gripperTarget;

        if (prevTrigger != trigger && trigger > 0)
        {
            gripperSetting = !gripperSetting;
            Debug.Log("switch");
        }

        prevTrigger = trigger;
        if (gripperSetting)
        {
            gripperTarget = gripperOn;
        }
        else 
        {
            gripperTarget = gripperHome;
        }

        jointController.SetJointTarget(leftGripper1, gripperTarget, speed);
        jointController.SetJointTarget(leftGripper2, gripperTarget, speed);
        jointController.SetJointTarget(rightGripper1, gripperTarget, speed);
        jointController.SetJointTarget(rightGripper2, gripperTarget, speed);


        // if no input, don't move
        if (mouseX == 0 && mouseY == 0)
            return;

        // math to get the targets to move (taken from MouseCameraControl.cs)
        yawRotation = selectedJoint1.xDrive.target;
        pitchRotation = selectedJoint2.xDrive.target;
        yawRotation -= mouseX * mouseSensitivity * Time.fixedDeltaTime;
        pitchRotation += mouseY * mouseSensitivity * Time.fixedDeltaTime;
        yawRotation = Mathf.Clamp(yawRotation, 
                                  yawOffsetDeg-angleLimit, yawOffsetDeg+angleLimit);
        pitchRotation = Mathf.Clamp(pitchRotation, 
                                    pitchOffsetDeg-angleLimit, pitchOffsetDeg+angleLimit);

        // actually send to that position
        jointController.SetJointTarget(selectedJoint1, yawRotation, speed/2);
        jointController.SetJointTarget(selectedJoint2, pitchRotation, speed/2);

        
    }

    /// <summary>
    /// Highlights the color of the robot by changing the color of the part to a color set by the user in the inspector window
    /// </summary>
    /// <param name="selectedIndex">Index of the link selected in the Articulation Chain</param>
    /// edited function from tutorials
    private void Highlight(Color highLightColor)
    {
        Renderer[] rendererList1 = selectedJoint1.transform.GetChild(0).GetComponentsInChildren<Renderer>();
        Renderer[] rendererList2 = selectedJoint2.transform.GetChild(0).GetComponentsInChildren<Renderer>();

        // set the color of the selected join meshes to the highlight color
        foreach (var mesh in rendererList1)
        {
	    MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
        }
        // set the color of the selected join meshes to the highlight color
        foreach (var mesh in rendererList2)
        {
	    MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
        }
    }

    // edited from MouseCameraControl.cs
    public void HomeGripper()
    {
        StartCoroutine(HomeGripperCoroutine());
    }
    private IEnumerator HomeGripperCoroutine()
    {
        yield return new WaitUntil(() => HomeGripperAndCheck() == true);
    }
    private bool HomeGripperAndCheck()
    {
        bool left1Homed = leftGripper1.xDrive.target == gripperHome;
        bool left2Homed = leftGripper2.xDrive.target == gripperHome;
        bool right1Homed = rightGripper1.xDrive.target == gripperHome;
        bool right2Homed = rightGripper2.xDrive.target == gripperHome;

        if (!left1Homed)
            jointController.SetJointTarget(leftGripper1, gripperHome, speed);
        if (!left2Homed)
            jointController.SetJointTarget(leftGripper2, gripperHome, speed);
        if (!right1Homed)
            jointController.SetJointTarget(rightGripper1, gripperHome, speed);
        if (!right2Homed)
            jointController.SetJointTarget(rightGripper2, gripperHome, speed);
        if (left1Homed && left2Homed && right1Homed && right2Homed)
            return true;
        return false;
    }



       
}
