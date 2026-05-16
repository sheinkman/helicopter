using UnityEngine;

namespace Skyjoust
{
    /// The lava floor. Anything that touches its trigger collider is destroyed.
    /// Attach to a GameObject with a trigger Collider2D spanning the lava.
    public class LavaHazard : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            AircraftController player = other.GetComponentInParent<AircraftController>();
            if (player != null)
            {
                player.Kill();
                return;
            }

            EnemyDrone drone = other.GetComponentInParent<EnemyDrone>();
            if (drone != null)
            {
                drone.Incinerate();
                return;
            }

            PowerOrb orb = other.GetComponentInParent<PowerOrb>();
            if (orb != null)
                Destroy(orb.gameObject);
        }
    }
}
