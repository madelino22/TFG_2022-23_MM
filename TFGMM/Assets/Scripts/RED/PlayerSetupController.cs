using UnityEngine;
using Photon.Bolt;

public class PlayerSetupController : GlobalEventListener
{
    [SerializeField]
    private Camera _sceneCamera;

    [SerializeField]
    private GameObject _setupPanel;

    [SerializeField]
    private GameObject spawn;

    public Camera SceneCamera { get => _sceneCamera; }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt.Send();
        }
    }

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawn.transform.position, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt.Send();
    }
}