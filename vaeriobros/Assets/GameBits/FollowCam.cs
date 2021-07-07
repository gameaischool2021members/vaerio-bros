using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform Target { get; set; }
    Transform thisTransform;

    void Start()
    {
        thisTransform = this.transform;
    }

    void FixedUpdate()
    {
        if(Target == null)
        {
            return;
        }

        var desiredPos = Target.position;
        desiredPos.z = thisTransform.position.z;

        thisTransform.position = Vector3.Lerp(thisTransform.position, desiredPos, Time.deltaTime * 20);
    }
}