using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject WinPanel;
    public List<Unit> PlayersInGame = new List<Unit>();
    private int killCalls = 0;
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randomPointX = Random.Range(-10, 10);
            int randomPointZ = Random.Range(-10, 10);
            GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(randomPointX, 0, randomPointZ), Quaternion.identity);
        }
    }

    public void OnAddPlayer(Shooting shootingComponent)
    {
        Debug.Log("Adding to new player's shooting evt");
        shootingComponent.EvtOnWin.AddListener(OnWin);
        shootingComponent.EvtOnKill.AddListener(OnKill);
    }

    public void OnKill(string killerName, string victimName)
    {
        killCalls++;
        Debug.Log("Kill calls " + killCalls);
        foreach (Unit player in PlayersInGame)
        {
            StartCoroutine(SpawnPopUp(player.KillPanel, player.KillPanelParent, killerName, victimName));
        }
    }

    private IEnumerator SpawnPopUp(GameObject prefab, RectTransform parent, string killerName, string victimName)
    {
        float waitTime = 8f;
        while(killCalls > 0)
        {
            killCalls--;
            GameObject killPanel = Instantiate(prefab, parent);
            killPanel.transform.Find("KillText").GetComponent<TextMeshProUGUI>().text = killerName + " killed " + victimName;
            Destroy(killPanel.gameObject, 1.5f);

            while (waitTime > 0)
            {
                yield return new WaitForSeconds(1.0f);
                waitTime--;
            }
        }
    }

    public void OnWin(string winnerName)
    {
        foreach(Unit player in PlayersInGame)
        {
            player.WinPanel.transform.Find("WinText").GetComponent<TextMeshProUGUI>().text = winnerName + " wins!";
            player.WinPanel.SetActive(true);
        }
    }
}
