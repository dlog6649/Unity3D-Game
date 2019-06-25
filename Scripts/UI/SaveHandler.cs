using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHandler : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SavePoint")
        {
            Camera.main.SendMessage("WriteToFile");
            Destroy(other.gameObject);
        }
    }
}
