using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                        // Used to filter what explosion affects, should be set to "Players"
    public ParticleSystem m_ExplosionParticles;         // Reference to particles that play on explosion
    public AudioSource m_ExplosionAudio;                // Reference to audio that play on explosion
    public float m_MaxDamage = 100f;                    // Amount of damage done if explosion is centered on tank
    public float m_ExplosionForce = 1000f;              // Amount of force added to a tank at center of explosion
    public float m_MaxLifeTime = 2f;                    // Time in secs before shell is removed
    public float m_ExplosionRadius = 5f;                // Max distance away from explosion tanks can be and are still affected


    private void Start(){
        // If it isn't destroyed by then, destroy shell after it's lifetime
        Destroy(gameObject, m_MaxLifeTime);
    }


    // Find all tanks in area around the shell and damage them
    // On Trigger will allow it to run this whenever it hits anything
    private void OnTriggerEnter(Collider other){
        // Collect all colliders in a sphere from shell's current pos to a radius of the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        // Go through all colliders and find their rigidbody
        for (int i=0; i<colliders.Length; i++) {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have rigidbody, go on to next collider
            if (!targetRigidbody)
                continue;

            // Add explosion force
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find TankHealth script associated with rigidbody
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // If no TankHealth script attached to gameobject, go on to next collider
            if (!targetHealth)
                continue;

            // Calculate amount of damage target should take based on distance from shell
            float damage = CalculateDamage(targetRigidbody.position);

            // Deal this damage to the tank
            targetHealth.TakeDamage(damage);
        }
        // Unparent particles from the shell
        // allows the explosion sound and particles to continue to happen after shell game object is deleted
        m_ExplosionParticles.transform.parent = null;

        // Play particle system and explosion sound effect
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Once particles have finished, destroy gameobject they are on
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Destroy shell
        Destroy(gameObject);
    }


    // Calculate amount of damage target should take based on it's position
    private float CalculateDamage(Vector3 targetPosition){
        // Creating vector from shell to target
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate distance from shell to target
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate proportion of max distance(explosionRadius) target is away
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as proportion of max possible damage
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that min damage is always 0
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}