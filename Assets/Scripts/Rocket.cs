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

    [SerializeField] bool collisionDetection = true;

    float levelLoadDelay = 2f;

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
        if (Input.GetKey("escape"))
        {
            // go back to menu
            SceneManager.LoadScene(0);
        }

        // if in debug build, respond to debug keys
        if (Debug.isDebugBuild)
        {
            if (Input.GetKey(KeyCode.N)) { LoadNextScene(); }
            if (Input.GetKey(KeyCode.C)) { collisionDetection = !collisionDetection; }
        }

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
                if (collisionDetection) { StartDeathSequence(); }
                break;
            case "Target":
                // TODO check if rocket is stable on landing pad
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
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        engineParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(Explosion);
        explosionParticles.Play();
        state = State.Dying;
        currentLevel = 0;
        Invoke("ReloadScene", levelLoadDelay);
    }

    private void LoadNextScene()
    {
        state = State.Alive;

        int numLevel = SceneManager.sceneCountInBuildSettings - 1;
        print(numLevel);
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = (currentLevel % numLevel) + 1;
        SceneManager.LoadScene(nextLevel);
    }

    private void ReloadScene()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel);
    }

    // currently unused
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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Rotate(Time.deltaTime * rcsRotate);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Rotate(Time.deltaTime * -rcsRotate);
        }
    }

    private void Rotate(float rotateThisFrame)
    {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotateThisFrame);
        rigidBody.freezeRotation = false;
    }
}
