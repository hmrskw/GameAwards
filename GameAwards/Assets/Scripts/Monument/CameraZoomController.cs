using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour {
    [System.Serializable]
    public struct ZoomData
    {
        public int startFrame;
        public int endFrame;
        public float targetFoV;
    }
    [SerializeField]
    Animator cutAnim;
    [SerializeField]
    Camera camera;
    [SerializeField]
    ZoomData[] zoomData;
    [SerializeField]
    float totalFrame;

    void OnEnable() {
        StartCoroutine(Func());
	}
	
    IEnumerator Func()
    {
        while(cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("New State"))
        {
            yield return null;
        }
        float startFov = camera.fieldOfView;
        float startTime = Time.timeSinceLevelLoad;
        float diff = (Time.timeSinceLevelLoad - startTime) * 30f;
        float total;
        float nowFrame = cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrame;

        for (int i = 0; i < zoomData.Length; i++) {
            total = zoomData[i].endFrame - zoomData[i].startFrame;
            while (diff <= 1)
            {
                Debug.Log("now" + cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrame);
                nowFrame = (cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime * totalFrame) - zoomData[i].startFrame;

                diff = nowFrame/totalFrame;
                camera.fieldOfView = Mathf.Lerp(startFov, zoomData[i].targetFoV, diff);
                yield return null;
            }
        }
        yield return null;
    }
}
