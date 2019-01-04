﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float rcsRotate = 100f;
    [SerializeField] int currentLevel = 0;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip SuccessfulLanding;
    [SerializeField] AudioClip Explosion;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            ResponseToThrustInput();
            ResponseToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Dangerous":
                StartDeathSequence();
                break;
            case "Target":
                StartSuccessSequence();
                break;
            case "Fuel":
                print("Refueling");
                break;
            default:
                break;
        }
    }

    private void StartSuccessSequence()
    {
        engineParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(SuccessfulLanding);
        successParticles.Play();
        state = State.Transcending;
        if (currentLevel != 2) { currentLevel++; }
        Invoke("LoadNextScene", 2f);
    }

    private void StartDeathSequence()
    {
        engineParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(Explosion);
        explosionParticles.Play();
        state = State.Dying;
        currentLevel = 0;
        Invoke("LoadFirstLevel", 2f);
    }

    private void LoadNextScene()
    {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void ResponseToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * Time.deltaTime * rcsThrust);
            if (!audioSource.isPlaying) { audioSource.Play(); }
            if (!engineParticles.isPlaying) { engineParticles.Play(); }
        }
        else
        {
            audioSource.Stop();
            engineParticles.Stop();
        }
    }

    private void ResponseToRotateInput()
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

        rigidBody.freezeRotation = false; // resume physics rotation
    }

}
