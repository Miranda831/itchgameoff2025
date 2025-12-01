using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Trap : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private const string RAISE_PARAM = "Raise";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           
            anim.SetTrigger(RAISE_PARAM);
        }
    }
}