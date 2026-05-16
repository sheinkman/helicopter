using UnityEngine;

namespace Skyjoust
{
    /// An enemy aircraft. Flies on a Rigidbody2D, chases the player's altitude,
    /// and drops a power orb when out-jousted.
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyDrone : MonoBehaviour
    {
        [Header("Flight")]
        [SerializeField] float thrustImpulse = 5f;
        [SerializeField] float steerImpulse = 7f;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxRiseSpeed = 9f;
        [SerializeField] float maxFallSpeed = 11f;

        [Header("AI")]
        [SerializeField] float minThinkInterval = 0.30f;
        [SerializeField] float maxThinkInterval = 0.70f;

        [Header("Rewards")]
        [SerializeField] PowerOrb orbPrefab;
        [SerializeField] int scoreValue = 100;
        [SerializeField] ParticleSystem destroyVfx;

        [Header("Visuals")]
        [SerializeField] SpriteRenderer body;

        static readonly Color[] TierColors =
        {
            new Color(1f, 0.42f, 0.42f),  // tier 0
            new Color(1f, 0.62f, 0.30f),  // tier 1
            new Color(0.83f, 0.42f, 1f),  // tier 2
        };

        Rigidbody2D rb;
        WaveSpawner spawner;
        int tier;
        float thinkTimer;
        bool dead;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// Called by WaveSpawner right after instantiation.
        public void Initialize(int tier, WaveSpawner spawner)
        {
            this.tier = Mathf.Clamp(tier, 0, TierColors.Length - 1);
            this.spawner = spawner;
            if (body != null) body.color = TierColors[this.tier];
        }

        void FixedUpdate()
        {
            if (dead) return;

            thinkTimer -= Time.fixedDeltaTime;
            AircraftController player = GameManager.Instance != null
                ? GameManager.Instance.Player : null;

            if (thinkTimer <= 0f && player != null && player.isActiveAndEnabled)
            {
                thinkTimer = Mathf.Max(0.12f,
                    Random.Range(minThinkInterval, maxThinkInterval) - tier * 0.05f);

                bool wantsHeight = player.transform.position.y > transform.position.y - 0.3f;
                if (wantsHeight || Random.value < 0.4f)
                    rb.AddForce(Vector2.up * (thrustImpulse + tier * 0.6f), ForceMode2D.Impulse);

                float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
                rb.AddForce(Vector2.right * (dir * (steerImpulse + tier * 1.5f)),
                            ForceMode2D.Impulse);
            }

            ClampVelocity();
            FaceMovement();
        }

        void ClampVelocity()
        {
            float cap = maxSpeed + tier;
            Vector2 v = rb.linearVelocity;
            v.x = Mathf.Clamp(v.x, -cap, cap);
            v.y = Mathf.Clamp(v.y, -maxFallSpeed, maxRiseSpeed);
            rb.linearVelocity = v;
        }

        void FaceMovement()
        {
            if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) return;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (rb.linearVelocity.x < 0f ? -1f : 1f);
            transform.localScale = s;
        }

        /// Out-jousted by the player: award score and drop an orb.
        public void Defeat()
        {
            if (dead) return;
            dead = true;
            GameManager.Instance.AddScore(scoreValue);
            if (orbPrefab != null)
                Instantiate(orbPrefab, transform.position, Quaternion.identity);
            FinishDestroy();
        }

        /// Destroyed by the lava: no score, no orb.
        public void Incinerate()
        {
            if (dead) return;
            dead = true;
            FinishDestroy();
        }

        void FinishDestroy()
        {
            if (destroyVfx != null)
                Instantiate(destroyVfx, transform.position, Quaternion.identity);
            if (spawner != null) spawner.NotifyDroneDestroyed(this);
            Destroy(gameObject);
        }
    }
}
