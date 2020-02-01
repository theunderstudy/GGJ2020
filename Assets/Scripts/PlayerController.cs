using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject playerObject; // do I need to do this?
    private GameObject ground;
    private Dictionary<KeyCode, Vector3> movement;
    private Dictionary<KeyCode, Vector3> rotationTargets;
    
    private bool performRotation = false;
    private Vector3 rotationTarget;
    public float speed = 1.0f;
    public float rotationSpeed = 45.0f; // 45 degrees per tick?

    public Vector3 target;
    private Vector3 currentEuler;
    void Start()
    {
        // playerObject = GameObject.Find("PlayerTestObject");
        ground = GameObject.Find("Ground");
        movement = new Dictionary<KeyCode, Vector3>();

        movement.Add(KeyCode.W, Vector3.forward);
        movement.Add(KeyCode.A, Vector3.left);
        movement.Add(KeyCode.S, Vector3.back);
        movement.Add(KeyCode.D, Vector3.right);

        rotationTargets = new Dictionary<KeyCode, Vector3>();

        // Hardcoded because that's easier right now
        rotationTargets.Add(KeyCode.W, new Vector3(0f,270f,0f));
        rotationTargets.Add(KeyCode.S, new Vector3(0f,90f,0f));
        rotationTargets.Add(KeyCode.D, new Vector3(0f,0f,0f));
        rotationTargets.Add(KeyCode.A, new Vector3(0f,180f,0f));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var k in movement) {
            if (Input.GetKey(k.Key)) {
                transform.Translate((k.Value * Time.deltaTime) * speed, ground.transform);
                performRotation = true;
                rotationTargets.TryGetValue(k.Key, out rotationTarget);
                transform.eulerAngles = rotationTarget;
            }
        }
        // if (performRotation) {
        //     // currentEulerAngles += new Vector3(0, , z) * Time.deltaTime * rotationSpeed;

        //     //apply the change to the gameObject
            
        // }
    }
}
