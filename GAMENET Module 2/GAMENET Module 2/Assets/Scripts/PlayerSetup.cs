using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject FpsModel;
    public GameObject NonFpsModel;

    public GameObject PlayerUiPrefab;
    public PlayerMovementController PlayerMovementController;
    public Camera FpsCamera;

    private Animator animator;
    public Avatar FpsAvatar, NonFpsAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();

        FpsModel.SetActive(photonView.IsMine);      // fps model (hands only) is active when it's yours
        NonFpsModel.SetActive(!photonView.IsMine);  // other model is active when it's others
        animator.SetBool("isLocalPlayer", photonView.IsMine);

        animator.avatar = photonView.IsMine ? FpsAvatar : NonFpsAvatar;

        if (photonView.IsMine)
        {
            // If local player, instantiate player ui and setup the player controller
            GameObject playerUi = Instantiate(PlayerUiPrefab);
            PlayerMovementController.FixedTouchField = playerUi.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            PlayerMovementController.Joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            FpsCamera.enabled = true;
        }
        else
        {
            // if it isn't the local player, dont enable player ui + fps controller - should only control ours
            PlayerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            FpsCamera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
