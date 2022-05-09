using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBehaviour : MonoBehaviour
{


    [SerializeField]
    private int layerValue;

    const float DIST_FROM_OBJECT = 0.2f;
    const float MOVE_SPEED = 1;
    const float ROTATION_SPEED = 50;
    const float TIME_BETWEEN_DECISIONS = 5.0f;

    const float MIN_X = 7;
    const float MAX_X = -7;
    const float MIN_Y = 2;
    const float MAX_Y = 5;
    const float MIN_Z = -9;
    const float MAX_Z = 3;

    private List<GameObject> allTargets = new List<GameObject>();
    GameObject target;
    Vector3 randomPlace;

    float timer = 0.0f;
    bool arrived = false;

    // Start is called before the first frame update
    void Start()
    {
        target = null;
        SelectRandomLocation();
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var item in allObjects)
        {
            if (item.layer == layerValue)
            {
                allTargets.Add(item);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > TIME_BETWEEN_DECISIONS)
        {
            DefineBehaviour();
            timer = 0.0f;
        }

        // If there's no target
        if (target == null)
        {
            // Wander around
            Vector3 distVector = randomPlace - transform.position;
            if (distVector.magnitude > DIST_FROM_OBJECT)
            {
                // Go to it
                transform.Translate(distVector.normalized * Time.deltaTime * MOVE_SPEED, Space.World);
            }
            else
            {
                SelectRandomLocation();
            }
        }
        else
        {
            // Get the vector from base to slightly above the target
            Vector3 distVector = (target.transform.position + Vector3.up * 0.1f) - transform.position;
            // If the target is too far
            if (distVector.magnitude > DIST_FROM_OBJECT)
            {
                // Go to it
                transform.Translate(distVector.normalized * Time.deltaTime * MOVE_SPEED, Space.World);
                transform.LookAt(target.transform);
                // Adaptation due to gameobject
                transform.localEulerAngles = transform.localEulerAngles + Vector3.left * 90;
                arrived = false;
            }
            else
            {
                if (!arrived)
                    transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, transform.localEulerAngles.z - 90);
                transform.RotateAround(target.transform.position, Vector3.up, ROTATION_SPEED * Time.deltaTime);
                arrived = true;
            }
        }
    }

    private void DefineBehaviour()
    {
        int r = Random.Range(1, 100);
        Debug.LogFormat("Random behaviour {0}", r);
        if (target == null)
        {
            // 80 % of probability to continue the wandering
            // 20 % of chance to select a target
            if (r > 20)
            {
                // Select target
                Debug.Log("Selected target");
                SelectTarget();
            }
        }
        else
        {
            // 20 % of chance to drop the target
            // 20 % of chance to change target
            // 60 % of chance to keep the same target
            if (r > 80)
            {
                Debug.Log("Droped target");
                target = null;
                SelectRandomLocation();
            }
            else if (r > 60)
            {
                Debug.Log("Changed target");
                SelectTarget();
            }
        }
    }

    private void SelectTarget()
    {
        int r = Random.Range(0, allTargets.Count - 1);
        target = allTargets[r];
        arrived = false;
    }

    private void SelectRandomLocation()
    {
        randomPlace = new Vector3(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y), Random.Range(MIN_Z, MAX_Z));
        transform.LookAt(randomPlace);
        // Adaptation due to gameobject
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * 90;
    }
}
