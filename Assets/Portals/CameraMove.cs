using UnityEngine;

public class CameraMove : MonoBehaviour {

    public Transform player;

    void Update() {
        transform.position = player.transform.position;
    }
}
