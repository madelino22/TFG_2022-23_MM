using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PROTOTIPO ==> NO USAR
public class PlayerAttacking : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    Joystick attackJoystick;

    [SerializeField]
    Transform attackLookPoint;

    [SerializeField]
    float trailDistance = 30;

    [SerializeField]
    float deadZone = 0.5f;

    [SerializeField]
    Transform player;

    private RaycastHit hit;

    //SHOOT BULLETS
    bool shoot = false;
    [SerializeField]
    GameObject bulletPrefab;
    int numBullets = 10;

    //GETS
    public float getTrailDistance() { return trailDistance; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(attackJoystick.Horizontal) > 0.1f || Mathf.Abs(attackJoystick.Vertical) > 0.1f)
        {
            //Hacerlo visible
            lineRenderer.gameObject.SetActive(true);

            transform.position = new Vector3(player.transform.position.x, 1.46f, player.transform.position.z);

            //Copy PlayerMov
            attackLookPoint.position = new Vector3(attackJoystick.Horizontal + player.position.x, 1.46f, attackJoystick.Vertical + player.position.z);
            transform.LookAt(new Vector3(attackLookPoint.position.x, 0, attackLookPoint.position.z));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            
            lineRenderer.SetPosition(0, transform.position);
            //???
            //Vector3 dir = new Vector3(attackLookPoint.position.x, 0, attackLookPoint.position.z);
            Vector3 dir = new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical);
            dir = dir.normalized;

            if (Physics.Raycast(transform.position, dir, out hit, trailDistance))
            {
                Vector3 movBlocked = new Vector3(hit.point.x, 0, hit.point.z);
                lineRenderer.SetPosition(1, transform.position + movBlocked);
            }
            else
            {
                lineRenderer.SetPosition(1, transform.position + dir * trailDistance);
            }

            //SHOOT
            if (!shoot)
            {
                shoot = true;
            }

        }

        else if (shoot && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Shoot");
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            shoot = false;
        }
        else if(lineRenderer.gameObject.activeInHierarchy)
        {
            this.gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            attackLookPoint.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            //Hacer desaparecer el camino
            lineRenderer.gameObject.SetActive(false);
        }

        //???????????????????????
        IEnumerator ShootBullet()
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            
            for(int i = 0; i < numBullets-1; i++)
            {
                yield return new WaitForSeconds(0.2f);
                Instantiate(bulletPrefab, transform.position, transform.rotation);

            }
            //StartCoroutine(ShootBullet());
        }
    }
}
