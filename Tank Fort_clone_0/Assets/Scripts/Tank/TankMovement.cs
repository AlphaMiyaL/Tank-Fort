using UnityEngine;

public class TankMovement : MonoBehaviour{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player; This is set by this tank's manager
    public float m_Speed = 12f;                 // How fast the tank moves forward and back
    public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second
    public AudioSource m_MovementAudio;         // Reference to audio source used to play engine sounds; different to the shooting audio source
    public AudioClip m_EngineIdling;            // Audio played when the tank isn't moving
    public AudioClip m_EngineDriving;           // Audio played when the tank is moving
    public float m_PitchRange = 0.2f;           // Amount by which the pitch of the engine noises can vary
    public ParticleSystem m_leftDustTrail;      // Reference to left dust particles; used to set active or not based on movement
    public ParticleSystem m_rightDustTrail;     //Reference to right dust particles; used to set active or not based on movement


    private string m_MovementAxisName;          // Name of the input axis for moving forward and back
    private string m_TurnAxisName;              // Name of the input axis for turning
    private Rigidbody m_Rigidbody;              // Reference used to move the tank
    private float m_MovementInputValue;         // Current value of the movement input
    private float m_TurnInputValue;             // Current value of the turn input
    private float m_OriginalPitch;              // Pitch of the audio source at the start of scene   


    //Runs once at very start of game
    private void Awake(){
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable (){
        // When tank is on, make sure it's not kinematic so it can move
        m_Rigidbody.isKinematic = false;

        // Reset the input values
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable (){
        // When tank is off, set to kinematic so it stops moving
        m_Rigidbody.isKinematic = true;
    }


    private void Start(){
        // The axes names are based on player number
        if (GameSettingsManager.gamemode == "multiplayer"){
            m_MovementAxisName = "Vertical1";
            m_TurnAxisName = "Horizontal1";
        } else {
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;
        }

        // Store the original pitch of the audio source
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update(){
        // Store the player's input and make sure the audio for the engine is playing
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();

        //Fixing Particle Sytem so that it works when going forward instead of just turning
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) {
            _updateParticleSystem(m_leftDustTrail, false);
            _updateParticleSystem(m_rightDustTrail, false);
        }
        else {
            _updateParticleSystem(m_leftDustTrail, true);
            _updateParticleSystem(m_rightDustTrail, true);
        }
    }


    private void EngineAudio(){
        // If no input (Stationary)
        if (Mathf.Abs(m_MovementInputValue)<0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f){
            // If the audio source is playing the driving clip, change clip to idling and play
            //   need to call the play again since it stops automatically when changed
            if (m_MovementAudio.clip == m_EngineDriving) {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        // If the tank is moving and if the idling clip is currently playing, change clip to driving and play
        else {
            if (m_MovementAudio.clip == m_EngineIdling) {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void _updateParticleSystem(ParticleSystem particleSystem, bool shouldBePlaying) {
        if (particleSystem.isPlaying && !shouldBePlaying) {
            // Stop if currently playing and it shouldn't be
            particleSystem.Stop();
        }
        else if (!particleSystem.isPlaying && shouldBePlaying) {
            // Play if currently not playing and it should be
            particleSystem.Play();
        }
    }

    //Runs every in-game Physics step
    private void FixedUpdate(){
        // Move and turn the tank.
        Move();
        Turn();
        GameSettingsManager.room.Send("move", new {
                    xPos = Mathf.Round(transform.position.x * 1000000.0f) * 0.000001f,
                    yPos = Mathf.Round(transform.position.y * 1000000.0f) * 0.000001f,
                    zPos = Mathf.Round(transform.position.z * 1000000.0f) * 0.000001f,
                    xRot = transform.rotation.x,
                    yRot = transform.rotation.y,
                    zRot = transform.rotation.z,
                    wRot = transform.rotation.w,
                });
    }


    private void Move(){
        // Adjust the position of the tank based on the player's input

        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames
        // *deltaTime makes it proportional to seconds rather than frames
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn(){
        // Adjust the rotation of the tank based on the player's input

        // Determine the number of degrees to be turned based on the input, speed and time between frames
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        //Rotations are stored with Quarternions in Unity rather than Vector3 or floats
        // Make turn into a rotation in the y axis
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); //x, y ,z

        // Apply this rotation to the rigidbody's rotation
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}