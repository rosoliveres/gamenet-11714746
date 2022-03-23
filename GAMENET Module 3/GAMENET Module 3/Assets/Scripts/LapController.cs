using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapController : MonoBehaviour
{
    public List<GameObject> LapTriggers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in RacingGameManager.instance.LapTriggers)
        {
            LapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(LapTriggers.Contains(other.gameObject))
        {
            int indexOfTrigger = LapTriggers.IndexOf(other.gameObject); // get the index of the gameobject player collided with
            LapTriggers[indexOfTrigger].SetActive(false);   // set that lap trigger false
        }

        if(other.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().Camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;
    }
}
