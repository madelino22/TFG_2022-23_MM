using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpecialShooter : MonoBehaviour
{
    [SerializeField]
    bool testingScene = false;

    [SerializeField]
    Joystick specialJoystick;

    [SerializeField]
    GameObject specialBulletPrefab;

    [SerializeField]
    Transform attackLookPoint;

    [SerializeField]
    float spawnDistance = 1.5f;

    PhotonView view;

    int numBullets = 10;
    bool shoot = false;



    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void GetJoystick(Joystick a)
    {
        specialJoystick = a;
    }

    public void SetAttackLookPoint(Transform aLP)
    {
        attackLookPoint = aLP;
    }

    // Update is called once per frame
    void Update()
    {
        if (specialJoystick != null)
        {
            //PREPARE SHOOTER
            if (Mathf.Abs(specialJoystick.Horizontal) > 0.1f || Mathf.Abs(specialJoystick.Vertical) > 0.1f)
            {
                if (!shoot) shoot = true;
            }
            //JOYSTICK WAS RELEASED
            else if (shoot && Mathf.Abs(specialJoystick.Horizontal) <= 0.1f && Mathf.Abs(specialJoystick.Vertical) <= 0.1f/*Input.GetMouseButtonUp(0)*/)
            {
                Debug.Log("Shoot");
                //view.RPC("ShootBullet", RpcTarget.All, array);
                //CALCULATE BULLET POS-------------------------------
                Vector3 bulletPos = transform.position;
                Vector3 desfase = attackLookPoint.position - transform.position;
                desfase.Normalize();
                bulletPos += desfase * spawnDistance;
                object[] array = { bulletPos, transform.rotation };
                view.RPC("ShootBullet", RpcTarget.All, array);
                if (testingScene)
                    Instantiate(specialBulletPrefab, bulletPos/*transform.position*/, transform.rotation); //Test one player
                shoot = false;
                //PhotonNetwork.Instantiate(bulletPrefab.name, transform.position, transform.rotation);
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    if (attackJoystick != null)
    //    {
    //        //PREPARE SHOOTER
    //        if (Mathf.Abs(attackJoystick.Horizontal) > 0.1f || Mathf.Abs(attackJoystick.Vertical) > 0.1f)
    //        {
    //            if (!shoot) shoot = true;
    //        }
    //        //JOYSTICK WAS RELEASED
    //        else if (shoot && Input.GetMouseButtonUp(0))
    //        {
    //            object[] array = { transform.position, transform.rotation };
    //            Debug.Log("Shoot");
    //            view.RPC("ShootBullet", RpcTarget.All, array);
    //            shoot = false;
    //            //PhotonNetwork.Instantiate(bulletPrefab.name, transform.position, transform.rotation);
    //        }
    //    }
    //}

    [PunRPC]
    private void ShootBullet(object[] arr)
    {
        Instantiate(specialBulletPrefab, (Vector3)arr[0], (Quaternion)arr[1]);
    }

    //???????????????????????
    //IEnumerator ShootBullet()
    //{
    //    Instantiate(bulletPrefab, transform.position, transform.rotation);

    //    for (int i = 0; i < numBullets - 1; i++)
    //    {
    //        yield return new WaitForSeconds(0.2f);
    //        Instantiate(bulletPrefab, transform.position, transform.rotation);

    //    }
    //    //StartCoroutine(ShootBullet());
    //}
}
