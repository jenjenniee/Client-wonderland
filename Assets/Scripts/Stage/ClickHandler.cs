using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject polaroid;

    public void ClickPolaroid()
    {
        if (polaroid != null)
        {
            //Debug.Log($"polaroid On");
            polaroid.SetActive(true);
        }
    }
}
