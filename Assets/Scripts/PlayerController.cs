using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject playerObject; // do I need to do this?
    private GameObject ground;
    private Dictionary<KeyCode, Vector3> movement;
    private Dictionary< HashSet<KeyCode>, Vector3> rotationTargets;
    
    private bool performRotation = false;
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

        rotationTargets = new Dictionary<HashSet<KeyCode>, Vector3>(HashSet<KeyCode>.CreateSetComparer());

        rotationTargets.Add(setOf(new[] { KeyCode.W }), new Vector3(0f,270f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S }), new Vector3(0f,90f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.D }), new Vector3(0f,0f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.A }), new Vector3(0f,180f,0f));

        // W+D should be 305f
        // S+D should be 45f
        // S+A should be 135f
        // W+A should be 215f

        rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.D }), new Vector3(0f,305f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.D }), new Vector3(0f,45f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.A }), new Vector3(0f,135f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.A }), new Vector3(0f,215f,0f));

        // If you're holding more than 2 keys,
        // I mean on one level you're a monster
        // But on the other level, we can just figure that out easily enough

        rotationTargets.Add(setOf(new[] { KeyCode.W,KeyCode.A,KeyCode.D }), new Vector3(0f,270f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S,KeyCode.A,KeyCode.D }), new Vector3(0f,90f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.D, KeyCode.W, KeyCode.S }), new Vector3(0f,0f,0f));
        rotationTargets.Add(setOf(new[] { KeyCode.A, KeyCode.W, KeyCode.S }), new Vector3(0f,180f,0f));
    }

    // Update is called once per frame
    void Update()
    {
        HashSet<KeyCode> usedKeys = new HashSet<KeyCode>();
        
        foreach (var k in movement) {
            if (Input.GetKey(k.Key)) {
                transform.Translate((k.Value * Time.deltaTime) * speed, ground.transform);
                usedKeys.Add(k.Key);
            }
        }
        // Okay we have KEYS and A LOOKUP TABLE
        // WOO
        
        if (usedKeys.Count > 0) {
            Debug.Log("keys used " + usedKeys.Count);
            Debug.Log(usedKeys);
            Vector3 rotationTarget;
            rotationTargets.TryGetValue(usedKeys, out rotationTarget);
            transform.eulerAngles = rotationTarget;
            // if (currentEuler != rotationTarget) {
                
            // }
        }
    }
    private HashSet<KeyCode> setOf (KeyCode[] keys) {
        HashSet<KeyCode> keyCodes =  new HashSet<KeyCode>();
        foreach (KeyCode k in keys) {
            keyCodes.Add(k);
        }
        return keyCodes;
    }
}
