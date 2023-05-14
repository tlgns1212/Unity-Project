using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 myPos = transform.position;
        switch (transform.tag)
        {
            case "Ground":
                float difX = playerPos.x - myPos.x;
                float difY = playerPos.y - myPos.y;
                float dirX = difX < 0 ? -1 : 1;
                float dirY = difY < 0 ? -1 : 1;
                difX = Mathf.Abs(difX);
                difY = Mathf.Abs(difY);

                if (difX > difY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (difX < difY)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2);
                }          
                    

                break;
        }
    }
}
