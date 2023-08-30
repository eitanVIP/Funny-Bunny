using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Shooter : MonoBehaviour
{
    [HideInInspector] public bool Loop;
    [HideInInspector] public int loopStartDelay;
    [SerializeField] GameObject bulletPrefab;
    [HideInInspector] public bool shootingAnimation;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float animationSpeed;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float bulletSpeed;
    [SerializeField] Vector2 bulletDir;
    [SerializeField] Transform bulletSpawnOffset;
    [SerializeField] float shootTime;
    [HideInInspector] public bool timeRandomness;
    [HideInInspector] public float minRandomness;
    [HideInInspector] public float maxRandomness;
    float originalShootTime;
    float Shots = 0;

    void Awake()
    {
        originalShootTime = shootTime;
    }

    IEnumerator Start()
    {
        if(animator)
            GetComponent<Animator>().speed = 1 / shootTime * animationSpeed;
        yield return new WaitForSeconds(shootTime);
        Shots++;
        if(timeRandomness)
            shootTime = originalShootTime + Random.Range(minRandomness, maxRandomness);

        if (Loop & Shots >= loopStartDelay)
            Shoot();

        StartCoroutine(Start());
    }

    public void Shoot()
    {
        if (!GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
        {
            if (audioSource)
            {
                audioSource.volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
                audioSource.Play();
            }
            GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnOffset.position, Quaternion.identity);
            newBullet.GetComponent<Rigidbody2D>().velocity = (transform.right * bulletDir.x + transform.up * bulletDir.y) * bulletSpeed;
            Destroy(newBullet, 10);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Shooter))]
public class Shooter_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        Shooter script = (Shooter)target;

        // draw checkbox for the bool
        script.Loop = EditorGUILayout.Toggle("Loop", script.Loop);
        if (script.Loop) // if bool is true, show other fields
            script.loopStartDelay = EditorGUILayout.IntField("Loop Start Delay", script.loopStartDelay);

        script.shootingAnimation = EditorGUILayout.Toggle("Shooting Animation", script.shootingAnimation);
        if (script.shootingAnimation) // if bool is true, show other fields
        {
            script.animator = EditorGUILayout.ObjectField("Animator", script.animator, typeof(Animator)) as Animator;
            script.animationSpeed = EditorGUILayout.FloatField("Animation Speed", script.animationSpeed);
        }

        script.timeRandomness = EditorGUILayout.Toggle("Time Randomness", script.timeRandomness);
        if (script.timeRandomness) // if bool is true, show other fields
        {
            script.minRandomness = EditorGUILayout.FloatField("Randomness Min", script.minRandomness);
            script.maxRandomness = EditorGUILayout.FloatField("Randomness Max", script.maxRandomness);
        }
    }
}
#endif