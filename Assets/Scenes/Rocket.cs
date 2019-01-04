using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float rcsRotate = 100f;

    Rigidbody rigidBody;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Dangerous":
                print("Dead");
                break;
            case "Target":
                print("Successful Landing");
                break;
            case "Fuel":
                print("Refueling");
                break;
            default:
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * Time.deltaTime * rcsThrust);
            if (!audioSource.isPlaying) { audioSource.Play(); }
        }
        else { audioSource.Stop(); }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rcsRotate);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * Time.deltaTime * rcsRotate);
        }
        /*
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.left);
        }
        */
        rigidBody.freezeRotation = false; // resume physics rotation
    }

}
