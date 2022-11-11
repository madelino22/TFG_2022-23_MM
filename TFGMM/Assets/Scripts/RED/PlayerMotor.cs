using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera _cam = null;
    private Rigidbody _rigidbody = null;

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

    public int TotalLife { get => _totalLife; }

    public int BlueScore { get => _blueScore; set => _blueScore = value; }
    public int RedScore { get => _redScore; set => _redScore = value; }
    public int TimeMatch { get => _timeMatch; set => _timeMatch = value; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(bool isMine)
    {
        if (isMine)
            _cam.gameObject.SetActive(true);
    }

    public State ExecuteCommand(float horizontal, float vertical)
    {
        Vector3 movingDir = Vector3.zero;

        //Mover bola inferior
        playerBall.position = new Vector3(horizontal + transform.position.x, transform.position.y, vertical + transform.position.z);

        //Mirar hacia adelante
        transform.LookAt(new Vector3(playerBall.position.x, 0, playerBall.position.z));
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        Debug.Log("Rotar");

        //Mover jugador
        if (horizontal > movementLimit || -movementLimit > horizontal || vertical > movementLimit || -movementLimit > vertical)
        {
            Debug.Log("Mover");

            float value = 1; //En funcion de la distancia mas o menos velocidad
            movingDir += new Vector3(horizontal, 0, vertical);

            movingDir = movingDir * Time.deltaTime * _speed * value;
            //transform.Translate(Vector3.forward * Time.deltaTime * _speed * value);
        }
        else
        {
            Debug.Log("MovementLimit " + movementLimit);
            Debug.Log("Joystick " + horizontal + " " + vertical);
        }
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
}