using System;
using UnityEngine;

[Serializable] // When you have a instance of this class, it will show on the inspector
public class TankManager
{
    // This class manages settings on a tank
    // Works with GameManager class to control how tanks behave and whether players have control of their tank in different phases of the game

    public Color m_PlayerColor;                             // Color of tank
    public Transform m_SpawnPoint;                          // Position and direction tank will have when it spawns
    public CameraFollow m_Camera;                           // Reference to camera following player
    public GameObject m_CameraTransform;                    // Reference to camera location
    [HideInInspector] public int m_PlayerNumber;            // Specifies which player manager is for
    [HideInInspector] public string m_ColoredPlayerText;    // String that represents player with their number colored to match tank
    [HideInInspector] public GameObject m_Instance;         // Reference to instance of tank when it is created
    [HideInInspector] public double m_Wins;                 // Number of wins player has so far
    [HideInInspector] public int coinPoint;                 // Amount of coins collected
    [HideInInspector] public bool underdogUsed = false;     // Whether the player has gotten an underdog point before


    private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable/enable control
    private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable/enable control
    private GameObject m_CanvasGameObject;                  // Used to disable world space UI during Starting and Ending phases of each round

    public void Setup()
    {
        // Get references to components
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        // Set player numbers to be consistent across scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        // Create string using correct color that says 'PLAYER 1' etc based on tank's color and player's number
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // Get all of the renderers of tank
        // Tank is composed of an bunch of mesh renderers
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        // Go through all renderers, set material color to color specific to tank
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        //Set Camera target to current Tank
        m_CameraTransform = m_Instance.transform.Find("CameraTransform").gameObject;
        Transform target = m_CameraTransform.transform;
        m_Camera.target = target;
    }

    // Used during phases of game where player shouldn't be able to control the tank
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    // Used during phases of game where player should be able to control the tank
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    // Used at start of each round to put tank into it's default state
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
