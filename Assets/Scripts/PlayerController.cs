using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(Animator))]
public class PlayerController : Singleton<PlayerController>
{
    // Start is called before the first frame update

    private GameObject playerObject; // do I need to do this?
    public GameObject ground;
    private Dictionary<KeyCode, Vector3> movement;
    private Dictionary<HashSet<KeyCode>, Vector3> rotationTargets;

    public float speed = 1.0f;
    public float rotationSpeed = 45.0f; // 45 degrees per tick?

    private Vector3 currentEuler;
    public Vector3 defaultRotation = new Vector3(0f, 0f, 0f);

    private bool continueRotate = false;
    public Vector3 rotationTarget;
    private Vector3 absoluteTarget;

    public int Energy = 100;
    public int MaxEnergy = 100;

    public bool bWorking = false;
    public GameObject WorkParticles;

    public GridTile CurrentTile;

    public delegate void EnergyUpdated(float newPercent);
    public static event EnergyUpdated EnergyUpdatedEvent;

    [EventRef]
    public string TreadFX;

    protected override void Awake()
    {
        base.Awake();

    }
    void Start()
    {
        // playerObject = GameObject.Find("PlayerTestObject");
        UpdateCurrentTile();

        currentEuler = defaultRotation;

        movement = new Dictionary<KeyCode, Vector3>();

        movement.Add(KeyCode.W, Vector3.forward);
        movement.Add(KeyCode.A, Vector3.left);
        movement.Add(KeyCode.S, Vector3.back);
        movement.Add(KeyCode.D, Vector3.right);

        rotationTargets = new Dictionary<HashSet<KeyCode>, Vector3>(
            HashSet<KeyCode>.CreateSetComparer());

        // Normal WASD

        rotationTargets.Add(setOf(new[] { KeyCode.W }), new Vector3(0f, 270f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.A }), new Vector3(0f, 180f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S }), new Vector3(0f, 90f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.D }), new Vector3(0f, 0f, 0f));

        // Pressing multiple keys at once

        // W+D should be 305f
        // S+D should be 45f
        // S+A should be 135f
        // W+A should be 215f

        rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.D }), new Vector3(0f, 305f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.D }), new Vector3(0f, 45f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.A }), new Vector3(0f, 135f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.A }), new Vector3(0f, 215f, 0f));

        // If you're holding more than 2 keys,
        // I mean on one level you're a monster
        // But on the other level, we can just figure that out easily enough

        rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.A, KeyCode.D }), new Vector3(0f, 270f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.A, KeyCode.D }), new Vector3(0f, 90f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.D, KeyCode.W, KeyCode.S }), new Vector3(0f, 0f, 0f));
        rotationTargets.Add(setOf(new[] { KeyCode.A, KeyCode.W, KeyCode.S }), new Vector3(0f, 180f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (continueRotate)
        {
            // Keep rotating!

            if (currentEuler == rotationTarget)
            {
                // Update the current euler towards the rotation target
                continueRotate = false;
                // we may be in a state where currentEuler doesn't actually
                // correspond to reality, since we're screwing with the rotation values above.
                // So, we should reset the currentEuler to our expected rotation space.
                currentEuler = absoluteTarget;
            }
            else
            {
                // Hardcoding rotational axis, for now

                currentEuler = Vector3.RotateTowards(currentEuler, rotationTarget, 1, rotationSpeed);
                transform.eulerAngles = currentEuler;

            }
        }
        if (bWorking)
        {
            return;
        }

        UpdateCurrentTile();

        HashSet<KeyCode> usedKeys = new HashSet<KeyCode>();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Debug.Log("Horizontal" + horizontal);
        //Debug.Log("Vertical" + vertical);

        // I don't feel good about this but it'll work
        // possibly?



        if (horizontal > 0 && horizontal <= 1)
        {
            usedKeys.Add(KeyCode.D);
        }

        if (horizontal < 0 && horizontal >= -1)
        {
            usedKeys.Add(KeyCode.A);
        }

        if (vertical > 0 && vertical <= 1)
        {
            usedKeys.Add(KeyCode.W);
        }

        if (vertical < 0 && vertical >= -1)
        {
            usedKeys.Add(KeyCode.S);
        }

        foreach (var k in usedKeys)
        {
            Vector3 direction;
            movement.TryGetValue(k, out direction);
            transform.Translate((direction * Time.deltaTime) * speed, ground.transform);
            RuntimeManager.PlayOneShot(TreadFX, transform.position);
        }

        if (usedKeys.Count > 0)
        {
            rotationTargets.TryGetValue(usedKeys, out rotationTarget);
            absoluteTarget = rotationTarget;
            if (currentEuler != rotationTarget)
            {
                // Update the current euler towards the rotation target

                if (currentEuler.y >= 270 && (rotationTarget.y >= 0 || rotationTarget.y <= 90))
                {
                    // If we are over 270 and we're trying to get to 0 or 90,
                    rotationTarget.y += 360.0f;
                }

                if (currentEuler.y <= 90 && rotationTarget.y >= 270)
                {
                    currentEuler.y += 360.0f;
                }
                continueRotate = true;
            }
        }


    }
    private HashSet<KeyCode> setOf(KeyCode[] keys)
    {
        HashSet<KeyCode> keyCodes = new HashSet<KeyCode>();
        foreach (KeyCode k in keys)
        {
            keyCodes.Add(k);
        }
        return keyCodes;
    }

    public void StartNewDay(EWeather weather)
    {
        Energy = MaxEnergy;
    }

    private void OnEnable()
    {
        DayNightManager.EndDayEvent += StartNewDay;
        EnergyUpdatedEvent += checkLowEnergyWarnings;
    }
    private void OnDisable()
    {
        DayNightManager.EndDayEvent -= StartNewDay;
    }

    public bool CanWork()
    {
        return Energy > 0;
    }


    private void UpdateCurrentTile()
    {
        GridTile _tile = GridManager.Instance.GetTile(transform.position);

        if (_tile != null)
        {
            if (_tile != CurrentTile)
            {
                CurrentTile = _tile;
                PlayerMouseinput.Instance.HighlightWorkableTiles();
            }
        }
    }
    public void StartWork(GridTile workTarget , float workTime)
    {
        if (bWorking)
        {
            return;
        }
        Energy -= 10;
        bWorking = true;

        EnergyUpdatedEvent?.Invoke((float)Energy / MaxEnergy);
        // Figure out where the work tile is in relation to the player tile
        int direction = TileKey.GetDirectionIndex(CurrentTile.Key, workTarget.Key);
        // Coerce the direction into wasd cardinal space
        Debug.Log(direction);

        //         rotationTargets.Add(setOf(new[] { KeyCode.W }), );
        // rotationTargets.Add(setOf(new[] { KeyCode.A }), new Vector3(0f, 180f, 0f));
        // rotationTargets.Add(setOf(new[] { KeyCode.S }), );
        // rotationTargets.Add(setOf(new[] { KeyCode.D }), );

        // // Pressing multiple keys at once

        // // W+D should be 305f
        // // S+D should be 45f
        // // S+A should be 135f
        // // W+A should be 215f

        // rotationTargets.Add(setOf(new[] { KeyCode.S, KeyCode.A }), new Vector3(0f, 135f, 0f));
        // rotationTargets.Add(setOf(new[] { KeyCode.W, KeyCode.A }), new Vector3(0f, 215f, 0f));

        if (direction == 0) {
            // w
            rotationTarget = new Vector3(0f, 270f, 0f);
        }
        else if (direction == 1) {
            //wd
            rotationTarget = new Vector3(0f, 305f, 0f);
        }
        else if (direction == 2) {
            //d
            rotationTarget = new Vector3(0f, 0f, 0f);
        }
        else if (direction == 3) {

            //ds
            rotationTarget = new Vector3(0f, 45f, 0f);
        }
        else if (direction == 4) {
            // s
            rotationTarget = new Vector3(0f, 90f, 0f);
        }
        else if (direction == 5) {
            //sa
            rotationTarget = new Vector3(0f, 135f, 0f);
        }
        else if (direction == 6) {
            // a
            rotationTarget = new Vector3(0f, 180f, 0f);
        }
        else if (direction == 7) {
            rotationTarget = new Vector3(0f, 215f, 0f);
        }
        continueRotate = true;

        StartCoroutine(WorkRoutine(workTime));

    }

    private void checkLowEnergyWarnings(float newPercent) {
        if (newPercent > 0.2 && newPercent <= 0.5) Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 low battery warning beep 𝅘𝅥𝅮", 2);
        if (newPercent <= 0.2) Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "𝅘𝅥𝅮 low battery urgent beep 𝅘𝅥𝅮", 2);
    }

    IEnumerator WorkRoutine(float workTime)
    {
        WorkParticles.SetActive(true);
        yield return new WaitForSeconds(workTime);
        WorkParticles.SetActive(false);
        bWorking = false;
    }
}
