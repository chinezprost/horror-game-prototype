using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;
public class RpcManager : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabsList NetworkPrefabs;
    public static RpcManager Singleton { get; set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    IEnumerator DespawnCorountine(NetworkObject networkObjectScript)
    {
        yield return new WaitForSeconds(5f);
        networkObjectScript.Despawn();

    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateFootstepServerRpc(Vector3 position, int type, bool isRunning)
    {
        //SendFootstepClientRpc(position, type, index);
        var instantiatedFootstepGameobject = Instantiate(NetworkPrefabs.PrefabList[2].Prefab, position, quaternion.identity);
        var objectNetworkScript = instantiatedFootstepGameobject.GetComponent<NetworkObject>();
        objectNetworkScript.Spawn();
        SendFootstepClientRpc(new NetworkObjectReference(objectNetworkScript), type, isRunning);
        StartCoroutine(DespawnCorountine(objectNetworkScript));

    }

    [ClientRpc]
    public void SendFootstepClientRpc(NetworkObjectReference objectID, int type, bool isRunning)
    {
        NetworkObject gameObject;
        objectID.TryGet(out gameObject);
        var footstepAudio = gameObject.GetComponent<AudioSource>();
        if (!isRunning)
        {
            footstepAudio.maxDistance = 7.5f;
        }
        else
        {
            footstepAudio.maxDistance = 15;
        }
        if (type == 0)
        {
            footstepAudio.clip = ResourceManager.Singleton.Footstep_SFX_Left[Random.Range(0, ResourceManager.Singleton.Footstep_SFX_Left.Count)];
        }
        else
        {
            footstepAudio.clip = ResourceManager.Singleton.Footstep_SFX_Right[Random.Range(0, ResourceManager.Singleton.Footstep_SFX_Right.Count)];
        }
        footstepAudio.Play();
    }
    
    
    
}
