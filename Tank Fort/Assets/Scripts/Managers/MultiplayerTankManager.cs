using System;
using UnityEngine;

[Serializable] // When you have a instance of this class, it will show on the inspector
public class MultiplayerTankManager:TankManager
{
    public void Setup()
    {
        Debug.Log("hello");
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
    }
}