using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

using Platformer.Mechanics;

public class JumperAgent : Agent
{

    public PlatformCreation myPlatformCreation;
    public PlayerController myController;

    Rigidbody2D rBody;

    float trigger = -7;
    float rewardTrigger;
    //int fallChecker;
    int counter;
    int glitchChecker;
    int curPlat;
    float curPlatTrig; 

    Vector3 startLocation;


    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        //fallChecker = 0;
        curPlat = 0;
        curPlatTrig = -7;
        glitchChecker = 0;
        counter = 0;
        startLocation = new Vector3(-9, 0, 0);
        rewardTrigger = -7;
    }

    // Update is called once per frame
    public override void OnEpisodeBegin()
    {
        rBody.position = new Vector3(-9, 4);
        //fallChecker = 0;
        curPlat = 0;
        curPlatTrig = -7;
        glitchChecker = 0;
        rewardTrigger = -9;
        counter = 0;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        

        //player position
        sensor.AddObservation(rBody.transform.localPosition);

        //current platform location and width
        Vector3 curPlatform = new Vector3(myPlatformCreation.locationX[counter], myPlatformCreation.locationY[counter], 0);
        float curWidth = myPlatformCreation.widths[counter];
        sensor.AddObservation(curPlatform);
        sensor.AddObservation(curWidth);


        //next platform location and width
        Vector3 nextPlatform = new Vector3(myPlatformCreation.locationX[counter + 1], myPlatformCreation.locationY[counter + 1], 0);
        float nextWidth = myPlatformCreation.widths[counter + 1];
        sensor.AddObservation(nextPlatform);
        sensor.AddObservation(nextWidth);

        //next platform locaiton and width


        //sensor.AddObservation((Vector3)myPlatformCreation.locations[counter]);
        //sensor.AddObservation((float)myPlatformCreation.widths[counter]);



        // option 1 
        //get platform position
        //get platform width

        // get platform position in front of cur
        // same for width 


        // option 2 
        // get first part of platform at current or previous column
        // get last part of platform at current or next column

        // get first platform in front of player
        // get last platform in front of previous observe




        // or make platform objects
        // get current platfrom
        // get next platform


        //create two arrays
        // one is lcoations and second is widths
        //create a trigger variable at end of cur platform
        // add to counter every time pas trigger
        // sensor.addObservation(locationArray[counter]);
        // sensor.addObservation(widthArray[counter]);
        // sensor.addObservation(locationArray[counter + 1]);
        // sensor.addObservation(widthArray[counter + 1]);
        // counter++;



    }





    public float forceMultiplier = 100;
    public override void OnActionReceived (ActionBuffers actionBuffers)
    {

        //the farther along, the farther it has to go to get a reward 
        if (rBody.position.x > rewardTrigger)
        {
            SetReward(1.0f);

            float num = rBody.position.x - (rBody.position.x % 10);

            rewardTrigger += (num / 10) + 1;
            glitchChecker = 0;
        }

        if (rBody.transform.localPosition.x > curPlatTrig)
        {
            float platformDiff = myPlatformCreation.locationX[counter + 1] - myPlatformCreation.locationX[counter];
            float wided = myPlatformCreation.widths[counter];
            curPlatTrig += platformDiff + wided;
            curPlat++;
        }
        
        if (rBody.transform.localPosition.x > trigger)
        {
            float platformDiff = myPlatformCreation.locationX[counter + 1] - myPlatformCreation.locationX[counter];
            float wided = myPlatformCreation.widths[counter];
            counter++;
            trigger += platformDiff + wided;
            //SetReward(1.0f);
        }

        /*Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];*/
        myController.updateMovement(actionBuffers.ContinuousActions[0], actionBuffers.ContinuousActions[1] >= 0, actionBuffers.ContinuousActions[2] >= 0);

        //rBody.AddForce(controlSignal * forceMultiplier);

        //print(actionBuffers.ContinuousActions[0] + " Y: " + actionBuffers.ContinuousActions[1]);

        //print(rBody.position.y);

        if (rBody.position.y < myPlatformCreation.locationY[curPlat] - 8)
        {
            EndEpisode();
        }

        /*if (rBody.position.y < -5)
        {
            EndEpisode();
        }*/

        /*
        if (rBody.velocity.y > 0)
        {
            fallChecker++;
        }
        else
        {
            fallChecker = 0;
        }


        if (fallChecker >= 1150)
        {
            EndEpisode();
        }
        */

        
        glitchChecker++;
        if (glitchChecker > 2000)
        {
            EndEpisode();
        }

    }


    public override void Heuristic (in ActionBuffers actionsOut)
    {
        
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        if (Input.GetButtonUp("Jump"))
        {
            continuousActionsOut[1] = 1;
        }
        else
        {
            continuousActionsOut[1] = 0;
        }


        //continuousActionsOut[1] = Input.GetButtonUp("Jump");
    }

}
