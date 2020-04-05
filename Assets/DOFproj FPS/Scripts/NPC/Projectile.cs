

using UnityEngine;


    /// <summary>
    /// The Projectile component moves a Destructible object along the specified path. Can apply damage at the collision point.
    /// </summary>
    public class Projectile : MonoBehaviour
    {


        /// <summary>
        /// Initializes the object. This will be called from an object creating the projectile (such as a weapon).
        /// </summary>
        /// <param name="velocity">The velocity to apply.</param>
        /// <param name="torque">The torque to apply.</param>
        /// <param name="damageAmount">The amount of damage to apply to the hit object.</param>
        /// <param name="impactForce">The amount of force to apply to the hit object.</param>
        /// <param name="impactForceFrames">The number of frames to add the force to.</param>
        /// <param name="impactLayers">The layers that the projectile can impact with.</param>
        /// <param name="impactStateName">The name of the state to activate upon impact.</param>
        /// <param name="impactStateDisableTimer">The number of seconds until the impact state is disabled.</param>
        /// <param name="surfaceImpact">A reference to the Surface Impact triggered when the object hits an object.</param>
        /// <param name="originator">The object that instantiated the trajectory object.</param>
        //public override void Initialize(Vector3 velocity, Vector3 torque, float damageAmount, float impactForce, int impactForceFrames, LayerMask impactLayers,
        //                             string impactStateName, float impactStateDisableTimer, SurfaceImpact surfaceImpact, GameObject originator)
        //{
        //    // The projectile can deactivate after it comes in contact with another object or after a specified amount of time. Do the scheduling here to allow
        //    // it to activate after a set amount of time.
        //    m_ScheduledDeactivation = Scheduler.Schedule(m_Lifespan, Deactivate);

        //    base.Initialize(velocity, torque, damageAmount, impactForce, impactForceFrames, impactLayers, impactStateName, impactStateDisableTimer, surfaceImpact, originator);
        //}

        /// <summary>
        /// The projectile has reached its lifespan.
        /// </summary>
        private void Deactivate()
        {
            //OnCollision(null);
        }



    }
