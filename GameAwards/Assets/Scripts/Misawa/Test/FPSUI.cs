using UnityEngine;
using UnityEngine.UI;

public class FPSUI : MonoBehaviour {
    Text text;

	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = (1f / Time.deltaTime).ToString();
    }
}
