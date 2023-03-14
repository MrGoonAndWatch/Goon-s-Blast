using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class MatchEndMenu : MonoBehaviour
{
    public void ReturnToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel((int)GameConstants.LevelIndexes.MainMenu);
    }
}
