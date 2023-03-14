using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class Startup : MonoBehaviour
{
    void Update()
    {
        // TODO: Do we need to wait for anything to finish initializing first?
        PhotonNetwork.LoadLevel((int)GameConstants.LevelIndexes.MainMenu);
    }
}
