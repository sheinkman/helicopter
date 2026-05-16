using UnityEngine;

namespace Skyjoust
{
    public enum PowerOrbKind { Score, Shield, ExtraLife }

    /// A collectible dropped by a defeated drone. Falls under physics, rests on
    /// platforms, then fades out if not grabbed in time.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PowerOrb : MonoBehaviour
    {
        [Header("Effect values")]
        [SerializeField] int scoreValue = 250;
        [SerializeField] int bonusScore = 100;       // awarded by shield / life orbs
        [SerializeField] float shieldDuration = 6f;

        [Header("Lifetime")]
        [SerializeField] float lifetime = 8f;
        [SerializeField] float blinkThreshold = 2.5f;

        [Header("Pickup")]
        [SerializeField] float pickupRadius = 0.7f;
        [SerializeField] float popUpForce = 4f;

        [Header("Spawn weights")]
        [SerializeField] float scoreWeight = 0.62f;
        [SerializeField] float shieldWeight = 0.28f;
        [SerializeField] float lifeWeight = 0.10f;

        [Header("Visuals")]
        [SerializeField] SpriteRenderer body;
        [SerializeField] Color scoreColor = new Color(1f, 0.82f, 0.40f);
        [SerializeField] Color shieldColor = new Color(0.35f, 0.84f, 1f);
        [SerializeField] Color lifeColor = new Color(0.49f, 1f, 0.42f);

        public PowerOrbKind Kind { get; private set; }

        Rigidbody2D rb;
        float age;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            RollKind();
            rb.linearVelocity = new Vector2(Random.Range(-1.5f, 1.5f), popUpForce);
        }

        void RollKind()
        {
            float total = scoreWeight + shieldWeight + lifeWeight;
            float r = Random.value * total;
            if (r < scoreWeight) Kind = PowerOrbKind.Score;
            else if (r < scoreWeight + shieldWeight) Kind = PowerOrbKind.Shield;
            else Kind = PowerOrbKind.ExtraLife;

            if (body != null)
            {
                body.color = Kind == PowerOrbKind.Score ? scoreColor
                           : Kind == PowerOrbKind.Shield ? shieldColor
                                                         : lifeColor;
            }
        }

        void Update()
        {
            age += Time.deltaTime;
            if (age >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (body != null && age > lifetime - blinkThreshold)
            {
                Color c = body.color;
                c.a = Mathf.Sin(age * 18f) > 0f ? 1f : 0.25f;
                body.color = c;
            }

            AircraftController player = GameManager.Instance != null
                ? GameManager.Instance.Player : null;
            if (player != null && player.isActiveAndEnabled &&
                Vector2.Distance(transform.position, player.transform.position) <= pickupRadius)
            {
                Collect(player);
            }
        }

        void Collect(AircraftController player)
        {
            switch (Kind)
            {
                case PowerOrbKind.Score:
                    GameManager.Instance.AddScore(scoreValue);
                    break;
                case PowerOrbKind.Shield:
                    GameManager.Instance.AddScore(bonusScore);
                    player.GrantShield(shieldDuration);
                    break;
                case PowerOrbKind.ExtraLife:
                    GameManager.Instance.AddScore(bonusScore);
                    GameManager.Instance.AddLife();
                    break;
            }
            Destroy(gameObject);
        }
    }
}
