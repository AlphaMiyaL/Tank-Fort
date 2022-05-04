using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // Amount of hp each tank starts with
    public Slider m_Slider;                             // Slider to represent how much hp the tank currently has
    public Image m_FillImage;                           // Image component of the slider
    public Color m_FullHealthColor = Color.green;       // Color of hp bar when full hp
    public Color m_ZeroHealthColor = Color.red;         // Color of hp bar when no hp
    public GameObject m_ExplosionPrefab;                // Prefab that will be instantiated(spawn) in Awake, then used whenever the tank dies


    private AudioSource m_ExplosionAudio;               // The audio source to play when tank explodes
    private ParticleSystem m_ExplosionParticles;        // The particle system that will play when tank is destroyed
    private float m_CurrentHealth;                      // How much hp the tank currently has
    private bool m_Dead;                                // Boolean check if tank is dead



    private void Awake(){
        // Instantiate explosion prefab and get reference to particle system on it
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get reference to audio source on the instantiated prefab
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable prefab so it can be activated when required
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable(){
        // When tank is enabled, reset tank's hp and boolean check for dead
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the hp slider's value and color
        SetHealthUI();
    }

    // Adjust tank's current hp, update UI based on new hp and check whether tank is dead or not
    public void TakeDamage(float amount){
        // Reduce current hp by amount of damage done
        m_CurrentHealth -= amount;

        // Change the UI elements appropriately
        SetHealthUI();

        // If current health is at/below zero not been registered dead, call OnDeath
        if (m_CurrentHealth <= 0f && !m_Dead) {
            OnDeath();
        }
    }


    // Adjust the value and color of the slider
    private void SetHealthUI(){
        // Set slider's value appropriately
        m_Slider.value = m_CurrentHealth;

        // Interpolate color of the bar between the choosen colours based on current % of the starting hp
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    // Play the effects for the death of the tank and deactivate it
    private void OnDeath(){
        m_Dead = true;

        // Move instantiated explosion prefab to tank's position and turn it on
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play particle system and tank explosion sound
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        //Turn tank off
        gameObject.SetActive(false);
    }
}