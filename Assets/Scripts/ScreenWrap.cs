using UnityEngine;

namespace Skyjoust
{
    /// Wraps an object horizontally across the camera bounds — fly off one
    /// edge, reappear on the other. Attach to the player, drones and orbs.
    public class ScreenWrap : MonoBehaviour
    {
        [SerializeField] float margin = 0.6f;

        Camera cam;

        void Start()
        {
            cam = Camera.main;
        }

        void LateUpdate()
        {
            if (cam == null)
            {
                cam = Camera.main;
                if (cam == null) return;
            }

            float halfWidth = cam.orthographicSize * cam.aspect;
            float left = cam.transform.position.x - halfWidth;
            float right = cam.transform.position.x + halfWidth;

            Vector3 p = transform.position;
            if (p.x < left - margin) p.x = right + margin;
            else if (p.x > right + margin) p.x = left - margin;
            transform.position = p;
        }
    }
}
