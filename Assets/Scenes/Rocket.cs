using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 75;
    [SerializeField] float rcsRotate = 100;

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
        //rigidBody.freezeRotation = true;

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
        //rigidBody.freezeRotation = false; // resume physics rotation
    }

}
