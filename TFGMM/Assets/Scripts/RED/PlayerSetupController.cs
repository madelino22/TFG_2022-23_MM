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

    private int contador = 2; // Team lejos (0,2) || Team cerca (3,5)

    private BoltEntity[] entity = new BoltEntity[6];

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
        if(contador<=2) //RED
        {
            entity[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player2, spawners[contador].transform.position, Quaternion.identity);
            entity[contador].AssignControl(evnt.RaisedBy);
            entity[contador].transform.Rotate(new Vector3(0, 180, 0));
        }
        else //BLUE
        {
            entity[contador] = BoltNetwork.Instantiate(BoltPrefabs.Player1, spawners[contador].transform.position, Quaternion.identity);
            entity[contador].AssignControl(evnt.RaisedBy);
        }
        //int team = 0;
        //if (contador >= 3)
        //    team = 1;
        //else
        //{
        //    entity[contador].transform.Rotate(new Vector3(0, 180, 0));
        //}
        //entity[contador].GetComponentInChildren<PlayerAttackTrail>().SetTeam(team);
        //entity[contador].GetComponentInChildren<PlayerMotor>().SetTeam(team);

        contador++;
    }

    public override void OnEvent(StartMatchEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Canvas, new Vector3(0,0,0), Quaternion.identity);
    }

    public override void OnEvent(deletePlayersEvent evnt)
    {
        Debug.Log("EN EVENTO");
        for (int i = 0; i < 6; i++)
        {
            Destroy(entity[i].gameObject);
        }
        contador = 3;
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer);
        evnt.Send();
    }
}