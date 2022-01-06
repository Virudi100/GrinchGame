using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gifts : MonoBehaviour
{
    [SerializeField] private GameObject level;
    [SerializeField] private GameObject live1;
    [SerializeField] private GameObject live2;
    [SerializeField] private GameObject live3;

    public int liveCount;

    private void Start()
    {
        liveCount = 3;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Quand le cadeau entre en contact avec le sol, le cadeau est détruit et on perd une vie

        if (collision.gameObject.CompareTag("Target"))
        {
            level.GetComponent<LevelManager>().maxGift--;

            if(liveCount == 3)
            {
                live3.SetActive(false);
            }
            else if(liveCount == 2)
            {
                live2.SetActive(false);
            }
            else
            {
                live1.SetActive(false);
            }

            liveCount--;
            Destroy(collision.gameObject);
        }
    }

    public void Restart()
    {
        //Réinitialise les vies au restart

        liveCount = 3;
        live3.SetActive(true);
        live2.SetActive(true);
        live1.SetActive(true);
    }
}
