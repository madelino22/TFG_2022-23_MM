using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
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

    private float _speed = 7f;

    private Vector3 _lastServerPos = Vector3.zero;
    private bool _firstState = true;

    [SerializeField]
    private int _totalLife = 2600;

    private int _blueScore = 0;
    private int _redScore = 0;
    private int _timeMatch = 90;

    private int team = -1;

    public int TotalLife { get => _totalLife; }

    public int BlueScore { get => _blueScore; set => _blueScore = value; }
    public int RedScore { get => _redScore; set => _redScore = value; }
    public int TimeMatch { get => _timeMatch; set => _timeMatch = value; }

    public void BuffSpeed(float buff)
    {
        Debug.Log("Speed buffed");
        _speed += buff;
    }
    public void DeBuffSpeed(float buff)
    {
        Debug.Log("Speed Debuffed");
        _speed -= buff;
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
            //if (gameObject.tag.Equals("Blue"))
            //{
            //    Debug.Log("Motor TAG BLUE");
            //    team = 1;
            //}
        }
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


            movingDir = movingDir * Time.deltaTime * _speed * value /** team*/;
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
        _rigidbody.velocity = movingDir;

        State stateMotor = new State();
        stateMotor.position = transform.position;

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
                _lastServerPos = Vector3.zero;
            }
        }
        else
        {
            if (position != Vector3.zero)
            {
                _lastServerPos = position;
            }

            transform.position += (_lastServerPos - transform.position) * 0.5f;
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
}