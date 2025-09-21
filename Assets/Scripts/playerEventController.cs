using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEventController : MonoBehaviour
{
    public void onReload()
    {
        PlayerController.instance.finishReloadAnim();
    }

    public void die()
    {
        PlayerController.instance.died();
    }
}
