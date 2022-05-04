using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify different players
    public Rigidbody m_Shell;                   // Prefab of shell
    public Transform m_FireTransform;           // Child of tank where shells are spawned
    public Slider m_AimSlider;                  // Child of tank that displays current launch force
    public AudioSource m_ShootingAudio;         // Reference to audio source used to play shooting audio; different from movement audio source
    public AudioClip m_ChargingClip;            // Audio plays when shot is charging up
    public AudioClip m_FireClip;                // Audio plays when shot is fired
    public float m_MinLaunchForce = 15f;        // Force given to shell if fire button is not held
    public float m_MaxLaunchForce = 30f;        // The force given to shell if fire button is held for max charge time
    public float m_MaxChargeTime = 0.75f;       // How long shell can charge before it is auto fired at max force
    public bool m_IsDummy = false;
    public float shootDelay = 0.5f;                 //  Delay between shots


    private string m_FireButton;                // Input axis used for launching shells
    private float m_CurrentLaunchForce;         // Force that will be given to shell when fire button is released
    private float m_ChargeSpeed;                // How fast launch force increases, based on max charge time
    private bool m_Fired;                       // Whether or not shell has been launched with this button press
    private float lastTimeShot;                 // Records last time shot


    private void OnEnable(){
        // When tank is turned on, reset launch force and UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start(){
        // Fire axis is based on player number
        if (GameSettingsManager.gamemode == "multiplayer"){
            m_FireButton = "Fire1";
        } else {
            m_FireButton = "Fire" + m_PlayerNumber;
        }

        // Rate that launch force charges up is the range of possible forces by the max charge time
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        lastTimeShot = Time.time;
    }

    // Track current state of fire button and make decisions based on current launch force
    private void Update(){

        if (!m_IsDummy && lastTimeShot+shootDelay<Time.time){
            m_AimSlider.value = m_MinLaunchForce;

            // Max charge but not fired
            if (m_CurrentLaunchForce>=m_MaxLaunchForce && !m_Fired) {
                // Use max force and launch shell
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
            // If button is pressed first time
            else if (Input.GetButtonDown(m_FireButton)) {
                // Reset fired flag and launch force
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change clip to charging clip and start playing
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // Holding fire button but not fired
            else if (Input.GetButton(m_FireButton) && !m_Fired) {
                // Increment launch force and update slider
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Released button, having not fired yet
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired) {
                //launch shell
                Fire();
                lastTimeShot=Time.time;
            }
        }
    }


    // Instantiate and launch shell
    private void Fire(){
        // Set fired flag so Fire is only called once
        m_Fired = true;

        // Create instance of shell and store a reference to it's rigidbody
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set shell's velocity to launch force in fire position's forward direction
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        if (GameSettingsManager.gamemode == "multiplayer"){
            GameSettingsManager.room.Send("fire", m_CurrentLaunchForce);
        }

        // Change clip to firing clip and play
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset launch force; precaution in case of missing button events
        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    public void DummyFire(float launchForce){
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = launchForce * m_FireTransform.forward;

        // Change clip to firing clip and play
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}