using UnityEngine;
using Photon.Bolt;

public class PlayerSetupController : GlobalEventListener
{
    [SerializeField]
    private Camera _sceneCamera;

    [SerializeField]
    private GameObject _setupPanel;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject[] spawners;

    public Camera SceneCamera { get => _sceneCamera; }

    private int contador = 3;

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
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[contador].transform.position, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
        contador++;
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