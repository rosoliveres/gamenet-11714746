using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject FpsModel;
    public GameObject NonFpsModel;

    public GameObject PlayerUiPrefab;
    public PlayerMovementController PlayerMovementController;
    public Camera FpsCamera;

    private Animator animator;
    public Avatar FpsAvatar, NonFpsAvatar;

    private Shooting shooting;
    public TextMeshProUGUI PlayerNameText;

    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        PlayerNameText.text = photonView.Owner.NickName;
        PlayerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();

        FpsModel.SetActive(photonView.IsMine);      // fps model (hands only) is active when it's yours
        NonFpsModel.SetActive(!photonView.IsMine);  // other model is active when it's others
        animator.SetBool("isLocalPlayer", photonView.IsMine);

        animator.avatar = photonView.IsMine ? FpsAvatar : NonFpsAvatar;

        shooting = this.GetComponent<Shooting>();

        if (photonView.IsMine)
        {
            // If local player, instantiate player ui and setup the player controller
            GameObject playerUi = Instantiate(PlayerUiPrefab);
            PlayerMovementController.FixedTouchField = playerUi.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            PlayerMovementController.Joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            FpsCamera.enabled = true;

            playerUi.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            // if it isn't the local player, dont enable player ui + fps controller - should only control ours
            PlayerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            FpsCamera.enabled = false;
        }

        unit = this.GetComponent<Unit>();

        GameManager.instance.PlayersInGame.Add(unit);
        GameManager.instance.OnAddPlayer(shooting);
    }
}
