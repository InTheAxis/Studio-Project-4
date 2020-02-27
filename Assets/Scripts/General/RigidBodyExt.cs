using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RigidBodyExt
{
    //some simple shortcuts to acting on multiple rigidbodies

    public static Rigidbody[] GetRigiBodiesInSphere(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, mask);
        return GetRigidBodies(colliders);
    }
    public static Rigidbody[] GetRigiBodiesInCapsule(Vector3 origin, Vector3 dest, float radius, LayerMask mask)
    {
        Collider[] colliders = Physics.OverlapCapsule(origin, dest, radius, mask);
        return GetRigidBodies(colliders);
    }

    public static void ResetVel(Rigidbody[] rbs)
    { 
        foreach(Rigidbody rb in rbs)
        {
            NetworkRigidbodyController.instance.setVel(rb, Vector3.zero);
        }
    }
    public static void SlowVel(Rigidbody[] rbs, Vector3 slowFactor)
    {
        foreach (Rigidbody rb in rbs)
        {
            NetworkRigidbodyController.instance.setVel(rb, new Vector3(rb.velocity.x * slowFactor.x, rb.velocity.y * slowFactor.y, rb.velocity.z * slowFactor.z));
            //rb.velocity.Set(rb.velocity.x * slowFactor.x, rb.velocity.y * slowFactor.y, rb.velocity.z * slowFactor.z);
        }
    }
    public static void SlowVel(Rigidbody[] rbs, float slowFactor)
    {
        foreach (Rigidbody rb in rbs)
        {
            NetworkRigidbodyController.instance.setVel(rb, rb.velocity * slowFactor);
            //rb.velocity *= slowFactor;
        }
    }

    public static void PushInDirection(Rigidbody[] rbs, Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        foreach (Rigidbody rb in rbs)
        {
            NetworkRigidbodyController.instance.applyForce(rb, force, mode);
            //rb.AddForce(force, mode);
        }
    }

    public static void PullTowards(Rigidbody[] rbs, Vector3 origin, float forceMag, ForceMode mode = ForceMode.Impulse)
    {
        Vector3 disp;
        foreach (Rigidbody rb in rbs)
        {
            disp = (origin - rb.position);
            if (disp.sqrMagnitude > Mathf.Epsilon * Mathf.Epsilon)
                NetworkRigidbodyController.instance.applyForce(rb, disp.normalized * forceMag, mode);
                //rb.AddForce(disp.normalized * forceMag, mode);
        }
    }

    private static Rigidbody[] GetRigidBodies(Collider[] colliders)
    {
        List<Rigidbody> ret = new List<Rigidbody>();
        foreach (Collider c in colliders)
        {
            if (c.attachedRigidbody != null)
                ret.Add(c.attachedRigidbody);
        }
        return ret.ToArray();
    }
}
