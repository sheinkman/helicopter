using UnityEngine;

namespace Skyjoust
{
    /// The player's aircraft. Thrust-to-climb flight on a Rigidbody2D, plus
    /// joust resolution against enemy drones.
    [RequireComponent(typeof(Rigidbody2D))]
    public class AircraftController : MonoBehaviour
    {
        [Header("Flight")]
        [SerializeField] float thrustImpulse = 6f;       // sharp pop on each tap
        [SerializeField] float thrustHoldForce = 16f;    // gentle lift while held
        [SerializeField] float horizontalForce = 30f;
        [SerializeField] float maxHorizontalSpeed = 8f;
        [SerializeField] float maxRiseSpeed = 9f;
        [SerializeField] float maxFallSpeed = 12f;

        [Header("Joust")]
        [Tooltip("How much higher than a drone the craft must be to win the joust.")]
        [SerializeField] float joustHeightAdvantage = 0.35f;

        [Header("Feedback")]
        [SerializeField] ParticleSystem thrustVfx;
        [SerializeField] ParticleSystem hitVfx;
        [SerializeField] GameObject shieldVisual;

        Rigidbody2D rb;
        float invulnTimer;
        float shieldTimer;
        bool thrustQueued;
        float horizontalInput;

        public bool IsInvulnerable => invulnTimer > 0f;
        public bool IsShielded => shieldTimer > 0f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (invulnTimer > 0f) invulnTimer -= Time.deltaTime;
            if (shieldTimer > 0f)
            {
                shieldTimer -= Time.deltaTime;
                if (shieldTimer <= 0f && shieldVisual != null) shieldVisual.SetActive(false);
            }

            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                horizontalInput = 0f;
                return;
            }

            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horizontalInput -= 1f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horizontalInput += 1f;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.W))
            {
                thrustQueued = true;
            }

            UpdateFacing();
        }

        void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            if (thrustQueued)
            {
                rb.AddForce(Vector2.up * thrustImpulse, ForceMode2D.Impulse);
                thrustQueued = false;
                if (thrustVfx != null) thrustVfx.Play();
            }

            bool holding = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) ||
                           Input.GetKey(KeyCode.W);
            if (holding) rb.AddForce(Vector2.up * thrustHoldForce);

            rb.AddForce(Vector2.right * (horizontalInput * horizontalForce));

            Vector2 v = rb.linearVelocity;
            v.x = Mathf.Clamp(v.x, -maxHorizontalSpeed, maxHorizontalSpeed);
            v.y = Mathf.Clamp(v.y, -maxFallSpeed, maxRiseSpeed);
            rb.linearVelocity = v;
        }

        void UpdateFacing()
        {
            if (Mathf.Abs(horizontalInput) < 0.01f) return;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (horizontalInput < 0f ? -1f : 1f);
            transform.localScale = s;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            EnemyDrone drone = col.collider.GetComponentInParent<EnemyDrone>();
            if (drone == null) return;

            if (IsShielded)
            {
                drone.Defeat();
                return;
            }
            if (IsInvulnerable) return;

            float dy = transform.position.y - drone.transform.position.y;
            if (dy > joustHeightAdvantage)
            {
                drone.Defeat();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 3f);
            }
            else if (dy < -joustHeightAdvantage)
            {
                Kill();
            }
            // even altitude: the Rigidbody2D bounce settles it, nobody loses
        }

        /// Destroy the craft (joust loss or lava). Routes through GameManager.
        public void Kill()
        {
            if (IsInvulnerable) return;
            if (hitVfx != null) Instantiate(hitVfx, transform.position, Quaternion.identity);
            GameManager.Instance.PlayerKilled();
        }

        public void Respawn(Vector3 position, float invulnerability)
        {
            transform.position = position;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            invulnTimer = invulnerability;
            shieldTimer = 0f;
            if (shieldVisual != null) shieldVisual.SetActive(false);
            gameObject.SetActive(true);
        }

        public void GrantShield(float duration)
        {
            shieldTimer = Mathf.Max(shieldTimer, duration);
            if (shieldVisual != null) shieldVisual.SetActive(true);
        }
    }
}
