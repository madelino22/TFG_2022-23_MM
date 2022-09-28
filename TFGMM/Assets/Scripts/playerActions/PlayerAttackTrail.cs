using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrail : MonoBehaviour
{
    // [SerializeField] //GetComponentInChildren ==> void Start()
    LineRenderer lineRenderer;

    [SerializeField]
    Joystick attackJoystick;

    [SerializeField]
    Transform attackLookPoint;

    [SerializeField]
    float trailDistance = 30;

    //[SerializeField]
    //float deadZone = 0.5f;

    [SerializeField]
    Transform player;

    private RaycastHit hit;

    public void GetJoystick(Joystick a)
    {
        attackJoystick = a;
    }

    //GETS
    public float getTrailDistance() { return trailDistance; }

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackJoystick != null)
        {
            if (Mathf.Abs(attackJoystick.Horizontal) > 0.1f || Mathf.Abs(attackJoystick.Vertical) > 0.1f)
            {
                //Make Visible
                lineRenderer.gameObject.SetActive(true);

                //Update Position
                transform.position = new Vector3(player.transform.position.x, 1.46f, player.transform.position.z);

                //Rotate Trail
                attackLookPoint.position = new Vector3(attackJoystick.Horizontal + player.position.x, 1.46f, attackJoystick.Vertical + player.position.z);
                transform.LookAt(new Vector3(attackLookPoint.position.x, 0, attackLookPoint.position.z));
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                lineRenderer.SetPosition(0, transform.position);

                //Calculate Dir
                Vector3 dir = new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical);
                dir = dir.normalized;

                //Raicast collision ==> Shorter Trail
                if (Physics.Raycast(transform.position, dir, out hit, trailDistance))
                {
                    Vector3 movBlocked = new Vector3(hit.point.x, 0, hit.point.z);
                    lineRenderer.SetPosition(1, transform.position + movBlocked);
                }
                else
                {
                    lineRenderer.SetPosition(1, transform.position + dir * trailDistance);
                }
            }

            else if (lineRenderer.gameObject.activeInHierarchy)
            {
                this.gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
                attackLookPoint.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
                //Set Invisible
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }
}
