using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPitch : MonoBehaviour {

    public float pitchMin = .5f;
    public float pitchMax = 2f;

    public float volChangeMin = -.2f;
    public float volChangeMax = .2f;

	// Use this for initialization
	void Start () {
        this.GetComponent<AudioSource>().pitch = Random.Range(pitchMin, pitchMax);
        this.GetComponent<AudioSource>().volume += Random.Range(volChangeMin, volChangeMax);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
