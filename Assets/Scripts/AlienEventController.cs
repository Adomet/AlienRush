using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienEventController : MonoBehaviour
{
    public AlienController alienController;
    public void giveDamage()
    {
        alienController.giveDamage();
    }
}
