using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Bolt;

public class BasicShooter : EntityEventListener
{
    [SerializeField]
    bool testingScene = false;

    [SerializeField]
    Joystick attackJoystick;

    //[SerializeField]
    //GameObject bulletPrefab;

    [SerializeField]
    Transform attackLookPoint;

    [SerializeField]
    float spawnDistance = 1.5f;

    [SerializeField]
    ActivateSpecialModule specialAttack = null;

    [SerializeField]
    Image ammoBar = null;

    float ammoBarFullAmmount = 0;


    [SerializeField]
    float timeToReload = 2;
    float currentElapsedTime = 0f;

    [SerializeField]
    int bulletsNeededForSpecialAttack = 3;
    int numBullets = 0;
    int maxNumBullets = 3;
    bool shoot = false;
   

    // Start is called before the first frame update
    void Start()
    {
        if (specialAttack == null) 
            Debug.Log("Player doesn't have a SpecialAttackModule.");

        ammoBarFullAmmount = ammoBar.fillAmount;
        numBullets = 0;
        ammoBar.fillAmount = 0;
    }

    public void GetJoystick(Joystick a)
    {
        attackJoystick = a;
    }

    public void setSpecialModule(ActivateSpecialModule sa)
    {
        specialAttack = sa;
    }

    public void SetAttackLookPoint(Transform aLP)
    {
        attackLookPoint = aLP;
    }
    // Update is called once per frame
    void Update()
    {
        if (attackJoystick != null)
        {
            //PREPARE SHOOTER
            if (Mathf.Abs(attackJoystick.Horizontal) > 0.1f || Mathf.Abs(attackJoystick.Vertical) > 0.1f)
            {
                if (!shoot) shoot = true;
            }
            //JOYSTICK WAS RELEASED
            else if (shoot && Mathf.Abs(attackJoystick.Horizontal) <= 0.1f && Mathf.Abs(attackJoystick.Vertical) <= 0.1f/*Input.GetMouseButtonUp(0)*/ && numBullets > 0)
            {
                Debug.Log("Shoot");
                numBullets--;
                //ACTIVATE SPECIAL ATTACK
                if(numBullets == bulletsNeededForSpecialAttack && specialAttack != null)
                {
                    specialAttack.IncreaseAplha(bulletsNeededForSpecialAttack);
                    specialAttack.Activate(true);
                    numBullets = 0;
                }
                else if (specialAttack != null)
                {
                    specialAttack.IncreaseAplha(bulletsNeededForSpecialAttack);
                }
                
                //CALCULATE BULLET POS-------------------------------
                Vector3 bulletPos = transform.position;
                Vector3 desfase = attackLookPoint.position - transform.position;
                desfase.Normalize();
                bulletPos += desfase * spawnDistance;
                object[] array = { bulletPos, transform.rotation};
                ShootBullet(array);
                shoot = false;
                //PhotonNetwork.Instantiate(bulletPrefab.name, transform.position, transform.rotation);
            }


            if(numBullets < maxNumBullets)
            {
                //Debug.Log(ammoBarFullAmmount);
                currentElapsedTime += Time.deltaTime;
                if(currentElapsedTime >= timeToReload)
                {
                    numBullets++;
                    currentElapsedTime = 0;
                }

                ammoBar.fillAmount = ((currentElapsedTime / timeToReload + numBullets) * ammoBarFullAmmount / maxNumBullets);
            }
        }
    }

    private void ShootBullet(object[] arr)
    {
        ShootEvent evnt = ShootEvent.Create(entity, EntityTargets.OnlySelf);
        evnt.Position = (Vector3)arr[0];
        evnt.Rotation = (Quaternion)arr[1]; 
        evnt.Send();
    }

    public override void OnEvent(ShootEvent evnt)
    {
        Vector3 e = new Vector3(0, 1.7f, 0);
        BoltEntity entity;
        //DEPENDIENDO DEL EQUIPO DISPARA UNA BALA U OTRA
        if (this.gameObject.CompareTag("Red"))
            entity = BoltNetwork.Instantiate(BoltPrefabs.Bullet, evnt.Position+ e, evnt.Rotation);
        else 
            entity = BoltNetwork.Instantiate(BoltPrefabs.BlueBullet, evnt.Position+ e, evnt.Rotation);
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
