using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script reads robot positions, velocities,
///     joint states, etc.
/// </summary>
public class Notice_StateReader : MonoBehaviour
{
    public GameObject robot;
    public int updateRate = 10;
    private float deltaTime;

    private Transform tf;
    private Rigidbody rb;
    public Vector3 position;
    private Quaternion rotation;
    public Vector3 eulerRotation;
    public Vector3 linearVelocity;
    public Vector3 angularVelocity;

    private ArticulationBody[] articulationChain;
    private int jointStateLength;
    public string[] names;
    public float[] positions;
    public float[] velocities;
    public float[] forces;
    ////////////////

    public GameObject r_notify;
    public GameObject l_notify;
    //right part of sensor
    public int numberOfRays=9;
    public float angle=90;
    public float rayRange=1.3f;

    void Start()
    {
        deltaTime = 1f/updateRate;
        InvokeRepeating("ReadState", 1f, deltaTime);

        // Get robot transform
        tf = robot.transform;
        rb = robot.GetComponent<Rigidbody>();
    
        // Get joints
        articulationChain = robot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(joint => joint.jointType 
                                                        != ArticulationJointType.FixedJoint).ToArray();
        jointStateLength = articulationChain.Length;
        names = new string[jointStateLength];
        positions = new float[jointStateLength];
        velocities = new float[jointStateLength];
        forces = new float[jointStateLength];

        for (int i = 0; i < jointStateLength; ++i)
            names[i] = articulationChain[i].name;
    }

    void Update()
    {
       //left sensor
        for (int i=0; i<numberOfRays; i++){
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis(-10+(i/((float)numberOfRays-1))*-angle, this.transform.up);
            var direction = rotation * rotationMod * Vector3.forward;
            var ray= new Ray(this.transform.position,direction);
            RaycastHit hitInfo;

            //if hit something
            if(Physics.Raycast(ray, out hitInfo, rayRange)){

                //able left_notify_object
               
                l_notify.SetActive(true);

            }
            else{
                //enable left_notify_object
                l_notify.SetActive(false);
            }
        }
        ReadState();
        this.transform.position = position;




        //right sensor
        var deltaPosition = Vector3.zero; 
        for (int i=0; i<numberOfRays; i++){
            var rotation_r = this.transform.rotation;
            var rotationMod_r = Quaternion.AngleAxis(10+(i/((float)numberOfRays-1))*angle, this.transform.up);
            var direction_r = rotation_r * rotationMod_r * Vector3.forward;
            var ray_r= new Ray(this.transform.position,direction_r);
            RaycastHit hitInfo_r;

            //if hit something
            if(Physics.Raycast(ray_r, out hitInfo_r, rayRange)){

                //able right_notify_object
                r_notify.SetActive(true);

            }
            else{
                r_notify.SetActive(false);
            }
        }
        ReadState();
        this.transform.position = position;
    }

    void ReadState()
    {
        // Pose and Velocity
        if (rb != null)
        {
            // lienar and angular velocity
            linearVelocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
            // transfer to local frame
            linearVelocity = tf.InverseTransformDirection(linearVelocity);
            angularVelocity = tf.InverseTransformDirection(angularVelocity);

            // position and orientation
            position = rb.position;
            rotation = rb.rotation;
            eulerRotation = rotation.eulerAngles;
        }
        else
        {
            // lienar and angular velocity
            linearVelocity = (tf.position - position) / deltaTime;
            angularVelocity = (tf.rotation.eulerAngles - eulerRotation) / deltaTime;
            // transfer to local frame
            linearVelocity = tf.InverseTransformDirection(linearVelocity);
            angularVelocity = tf.InverseTransformDirection(angularVelocity);

            // position and orientation
            position = tf.position;
            rotation = tf.rotation;
            eulerRotation = rotation.eulerAngles;
        }
        
        // Joint states
        for (int i = 0; i < jointStateLength; ++i)
        {   
            positions[i] = articulationChain[i].jointPosition[0];
            velocities[i] = articulationChain[i].jointVelocity[0];
            forces[i] = articulationChain[i].jointForce[0];
        }
        
    }
}