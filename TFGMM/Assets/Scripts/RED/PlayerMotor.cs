using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class PlayerMotor : EntityEventListener<IPlayerState>
{
    [SerializeField]
    private Camera _cam = null;
    [SerializeField]
    private Canvas _can = null;
    private Rigidbody _rigidbody = null;

    [SerializeField]
    private RectTransform ammoCanvas = null;

    [SerializeField]
    float movementLimit = 0.05f;

    [SerializeField]
    Transform playerBall;

    //[SerializeField] //VA RAPIDISIMO NO SE PORQUE. VALOR CABLEADO EN LA LINEA 125 (APROX)
    private float _speed = 3.0f; 

    private Vector3 _lastServerPos = Vector3.zero;
    private bool _firstState = true;

    [SerializeField]
    private int _totalLife = 2500;
    private int _actualLife = 2500;
    private Vector3 _spawnPos;
    public Vector3 SpawnPos { get => _spawnPos; set => _spawnPos = value; }

    private int _blueScore = 0;
    private int _redScore = 0;
    private int _timeMatch = 90;
    private bool isd = false;

    private int team = -1;
    private int id = 0;
    public void setID(int i) { id = i; }

    public int TotalLife { get => _totalLife; }

    public int ActualLife { get => _actualLife; set => _actualLife = value; }

    public int BlueScore { get => _blueScore; set => _blueScore = value; }
    public int RedScore { get => _redScore; set => _redScore = value; }
    public int TimeMatch { get => _timeMatch; set => _timeMatch = value; }

    public void BuffSpeed(float buff)
    {
        Debug.Log("Speed buffed");
        //_speed += buff;
    }
    public void DeBuffSpeed(float buff)
    {
        Debug.Log("Speed Debuffed");
        //_speed -= buff;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(bool isMine)
    {
        if (isMine)
        {
            _cam.gameObject.SetActive(true);
            _can.gameObject.SetActive(true);
            ammoCanvas.gameObject.SetActive(true);
            _spawnPos = transform.position;
            //if (gameObject.tag.Equals("Blue"))
            //{
            //    Debug.Log("Motor TAG BLUE");
            //    team = 1;
            //}
        }
        else
        {
            _spawnPos = transform.position;
        }
    }

    public void Respawn()
    {
        isd = true;
        _firstState = true;
        SetState(_spawnPos, 0f);
        _lastServerPos = _spawnPos;
        RespawnEvent evnt1 = RespawnEvent.Create(GlobalTargets.OnlyServer);
        evnt1.id = id;
        evnt1.Send();

    }
    public State ExecuteCommand(float horizontal, float vertical)
    {
        Vector3 movingDir = Vector3.zero;

        //Mover jugador
        if (horizontal > movementLimit || -movementLimit > horizontal || vertical > movementLimit || -movementLimit > vertical)
        {
            //Debug.Log("Mover");

            float value = 1; //En funcion de la distancia mas o menos velocidad
            Vector3 forward = _cam.transform.forward * vertical;
            Vector3 right = _cam.transform.right * horizontal;
            movingDir += forward + right;
            movingDir = new Vector3(movingDir.x, 0, movingDir.z);


            movingDir = movingDir * Time.deltaTime * value /** team*/;
        }
        else
        {
            //Debug.Log("MovementLimit " + movementLimit);
            //Debug.Log("Joystick " + horizontal + " " + vertical);
        }


        //Mover bola inferior
        //playerBall.position = new Vector3(horizontal + transform.position.x, playerBall.position.y, vertical + transform.position.z);
        playerBall.position = (Vector3.Distance(playerBall.localPosition, Vector3.zero) < 0.1f && horizontal == 0 && vertical == 0 ) ?
            new Vector3(transform.position.x ,playerBall.position.y, transform.position.z) : 
            Vector3.Lerp(playerBall.position, new Vector3(movingDir.normalized.x + transform.position.x, playerBall.position.y, movingDir.normalized.z + transform.position.z), 0.3f);

        //Mirar hacia adelante
        transform.LookAt(new Vector3(playerBall.position.x, 0, playerBall.position.z));
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);



        //if (forward ^ backward)
        //{
        //    movingDir += forward ? transform.forward : -transform.forward;
        //}
        //if (left ^ right)
        //{
        //    movingDir += right ? transform.right : -transform.right;
        //}

        movingDir.Normalize();
        //movingDir *= _speed;
        _rigidbody.velocity = movingDir * _speed;

        State stateMotor = new State();
        stateMotor.position = transform.position;
        //NO HACE NADA (Y ADEMAS SOLO FUNCIONARIA EN EL CONTROLLER)
        if (isd)
        {
            stateMotor.position = _spawnPos;
            //_rigidbody.position = _spawnPos;
            isd = false;
            _firstState = false;
        }

        return stateMotor;
    }

    public void SetState(Vector3 position, float rotation)
    {
        if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        if (_firstState)
        {
            if (position != Vector3.zero)
            {
                transform.position = position;
                _firstState = false;
                _lastServerPos = Vector3.zero; //0 si estas actualizado?
            }
        }
        else
        {
            if (position != Vector3.zero)
            {
                _lastServerPos = position;
            }

            //HEMOS COMENTADO ESTA LINEA DE GRATIS
            //transform.position += (_lastServerPos - transform.position) * 0.5f;
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }

    public void SetTeam(int t)
    {
        Debug.Log("SOY TEAM ANTES MOTOR: " + team);
        team = t;
        Debug.Log("SOY TEAM DESPUES MOTOR: " + team);
    }

    public int GetTeam()
    {
        return team;
    }
}