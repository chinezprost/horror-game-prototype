using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectManager : NetworkBehaviour
{

    [SerializeField] private NetworkPrefabsList NetworkPrefabs;

    void Update()
    {
        if (!IsOwner) return;
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                var ObjectTransform = Instantiate(NetworkPrefabs.PrefabList[1].Prefab, new Vector3(0, 0, 0),
                    Quaternion.identity);
                ObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            }*/
        }
        
    }
}
