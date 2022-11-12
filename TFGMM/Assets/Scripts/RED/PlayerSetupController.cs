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

    [SerializeField]
    private Canvas canvas;

    public Camera SceneCamera { get => _sceneCamera; }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
            evnt.Send();

            StartMatchEvent evnt2 = StartMatchEvent.Create(GlobalTargets.OnlyServer);
            evnt2.Send();
        }
    }

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawn.transform.position, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
    }

    public override void OnEvent(StartMatchEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0,0,0), Quaternion.identity);
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt.Send();
    }
}