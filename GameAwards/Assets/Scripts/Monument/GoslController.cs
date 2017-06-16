    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoslController : Monument {

    [SerializeField]
    Monument[] checkPoints;

    [SerializeField]
    GameObject windObj;

    [SerializeField]
    CameraAndMask mainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    bool open;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0;i<checkPoints.Length; i++)
        {
            if (i == 0) open = checkPoints[i].IsOn;
            else
            {
                open &= checkPoints[i].IsOn;
            }
            if (open == false) break;
        }
        if (open == true && windObj.activeInHierarchy == true) windObj.SetActive(false);
    }
}
