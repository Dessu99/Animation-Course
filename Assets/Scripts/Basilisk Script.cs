using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AG3959
{
    public class BasiliskScript : MonoBehaviour
    {
        //HEAD TRACKING

        // Head Variables
        [SerializeField] Transform lookObject;
        [SerializeField] Transform headBone;
        [SerializeField] float headMaxTurnAngle = 70f;
        [SerializeField] float headTrackingSpeed = 8f;

        //EYE TRACKING

        // Eye Variables
        [SerializeField] Transform leftEyeBone;
        [SerializeField] Transform rightEyeBone;

        [SerializeField] float eyeTrackingSpeed;
        [SerializeField] float leftEyeMaxYRotation;
        [SerializeField] float leftEyeMinYRotation;
        [SerializeField] float rightEyeMaxYRotation;
        [SerializeField] float rightEyeMinYRotation;

        //LEG STEP

        // Calls to and from the Leg moving script
        [SerializeField] Leg_Step frontLeftLegStepper;
        [SerializeField] Leg_Step frontRightLegStepper;
        [SerializeField] Leg_Step midLeftLegStepper;
        [SerializeField] Leg_Step midRightLegStepper;
        [SerializeField] Leg_Step backLeftLegStepper;
        [SerializeField] Leg_Step backRightLegStepper;

    

        // How fast we can turn and move full throttle
        [SerializeField] float turnSpeed;
        [SerializeField] float moveSpeed;
        // How fast we will reach the above speeds
        [SerializeField] float turnAcceleration;
        [SerializeField] float moveAcceleration;
        // Try to stay in this range from the target
        [SerializeField] float minDistToTarget;
        [SerializeField] float maxDistToTarget;
        // If we are above this angle from the target, start turning
        [SerializeField] float maxAngToTarget;

        
        // World space velocity
        Vector3 currentVelocity;
        // We are only doing a rotation around the up axis, so we only use a float here
        float currentAngularVelocity;



        void RootMotionUpdate()
        {
            // Get the direction toward our target
            Vector3 towardTarget = lookObject.position - transform.position;
            // Vector toward target on the local XZ plane
            Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);
            // Get the angle from the gecko's forward direction to the direction toward toward our target
            // Here we get the signed angle around the up vector so we know which direction to turn in
            float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);

            float targetAngularVelocity = 0;

            // If we are within the max angle (i.e. approximately facing the target)
            // leave the target angular velocity at zero
            if (Mathf.Abs(angToTarget) > maxAngToTarget)
            {
                // Angles in Unity are clockwise, so a positive angle here means to our right
                if (angToTarget > 0)
                {
                    targetAngularVelocity = turnSpeed;
                }
                // Invert angular speed if target is to our left
                else
                {
                    targetAngularVelocity = -turnSpeed;
                }
            }

            // Use our smoothing function to gradually change the velocity
            currentAngularVelocity = Mathf.Lerp(
              currentAngularVelocity,
              targetAngularVelocity,
              1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
            );

            // Rotate the transform around the Y axis in world space, 
            // making sure to multiply by delta time to get a consistent angular velocity
            transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);

            Vector3 targetVelocity = Vector3.zero;

            // Don't move if we're facing away from the target, just rotate in place
            if (Mathf.Abs(angToTarget) < 90)
            {
                float distToTarget = Vector3.Distance(transform.position, lookObject.position);

                // If we're too far away, approach the target
                if (distToTarget > maxDistToTarget)
                {
                    targetVelocity = moveSpeed * towardTargetProjected.normalized;
                }
                // If we're too close, reverse the direction and move away
                else if (distToTarget < minDistToTarget)
                {
                    targetVelocity = moveSpeed * -towardTargetProjected.normalized;
                }
            }

            currentVelocity = Vector3.Lerp(
              currentVelocity,
              targetVelocity,
              1 - Mathf.Exp(-moveAcceleration * Time.deltaTime)
            );

            // Apply the velocity
            transform.position += currentVelocity * Time.deltaTime;
        }

        void Awake()
        {
            StartCoroutine(LegUpdateCoroutine());
        }

        // Only allow diagonal leg pairs to step together
        IEnumerator LegUpdateCoroutine()
        {
         // Run continuously
         while (true)
         {
         // Try moving one diagonal pair of legs
            do
            {
             frontLeftLegStepper.TryMove();
             midRightLegStepper.TryMove();
             backRightLegStepper.TryMove();
             // Wait a frame
             yield return null;
      
           // Stay in this loop while either leg is moving.
           // If only one leg in the pair is moving, the calls to TryMove() will let
           // the other leg move if it wants to.
          } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving || midRightLegStepper.Moving);

           // Do the same thing for the other diagonal pair
          do
          {
            frontRightLegStepper.TryMove();
            midLeftLegStepper.TryMove();
            backLeftLegStepper.TryMove();
           yield return null;
          } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving || midLeftLegStepper.Moving);
        }
}

        private void LateUpdate()
        {
            RootMotionUpdate();
            HeadTrackingUpdate();
            EyeTrackingUpdate();
        }

       
        void HeadTrackingUpdate()
        {
            // Changes the Quaternion local rotation to the headbone's

            Quaternion currentLocalRotation = headBone.localRotation;
            headBone.localRotation = Quaternion.identity;
            
            //Adds in the parent bone to help adjust the limits of head turn

            Vector3 targetWorldLookDir = lookObject.position - headBone.position;
            Vector3 targetLocalLookDir = headBone.parent.InverseTransformDirection(targetWorldLookDir);

            // Makes the headBone face towards the target

            Vector3 towardObjectFromHead = lookObject.position - headBone.position;
            headBone.rotation = Quaternion.LookRotation(towardObjectFromHead, transform.up);

            // angle limit

            targetLocalLookDir = Vector3.RotateTowards(Vector3.forward,targetLocalLookDir,Mathf.Deg2Rad * headMaxTurnAngle,0);

            Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

            // Changes the speed so it feels more natural

            Quaternion targetRotation = Quaternion.LookRotation(towardObjectFromHead, transform.up);
            headBone.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));

            
        }







        void EyeTrackingUpdate()
        {

            float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
            float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

            if (leftEyeCurrentYRotation > 180)
            {
                leftEyeCurrentYRotation -= 360;
            }
            if (rightEyeCurrentYRotation > 180)
            {
                rightEyeCurrentYRotation -= 360;
            }

            // Clamps the Y rotation
            float leftEyeClampedYRotation = Mathf.Clamp(
                leftEyeCurrentYRotation,
                leftEyeMinYRotation,
                leftEyeMaxYRotation
            );
            float rightEyeClampedYRotation = Mathf.Clamp(
                rightEyeCurrentYRotation,
                rightEyeMinYRotation,
                rightEyeMaxYRotation
            );

            // Applys the clamped y rotation, not the x and z
            leftEyeBone.localEulerAngles = new Vector3(
                leftEyeBone.localEulerAngles.x,
                leftEyeClampedYRotation,
                leftEyeBone.localEulerAngles.z
            );
            rightEyeBone.localEulerAngles = new Vector3(
                rightEyeBone.localEulerAngles.x,
                rightEyeClampedYRotation,
                rightEyeBone.localEulerAngles.z
            );

            Quaternion targetEyeRotation = Quaternion.LookRotation(
            lookObject.position - headBone.position, // toward target
            transform.up
            );

            leftEyeBone.rotation = Quaternion.Slerp(
              leftEyeBone.rotation,
              targetEyeRotation,
              1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );

            rightEyeBone.rotation = Quaternion.Slerp(
              rightEyeBone.rotation,
              targetEyeRotation,
              1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );
        }
    }

   
}