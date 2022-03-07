using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public List<GameObject> SpawnPts = new List<GameObject>();

    public static SpawnPointManager instance;

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

    public Vector3 GetRandomSpawnPt()
    {
        int randIndex = Random.Range(0, SpawnPts.Count);
        Debug.Log("Spawning in pt " + randIndex);
        return new Vector3(SpawnPts[randIndex].transform.position.x, 0, SpawnPts[randIndex].transform.position.z);
    }
}
