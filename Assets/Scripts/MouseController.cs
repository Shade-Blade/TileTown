using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D result = Physics2D.GetRayIntersection(cameraRay);
            if (result == null || result.collider == null)
            {
                return;
            }
            MouseTarget target = result.collider.GetComponent<MouseTarget>();
            if (target != null)
            {
                target.OnClick();
            }

        }
    }
}
