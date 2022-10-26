using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview_DiverAndSubmarine : MonoBehaviour {

	public List<GameObject> models = new List<GameObject> ();
	public List<GameObject> menus = new List<GameObject> ();
	public Transform container;
	private string currentAnim = "Idle";
	private Animator current;
	private bool isRotating = false;
	private float rotDir = 0f;

	private int index = 0;

	void Awake() {
		ShowModel (index);
	}
		
	public void ShowModel(int x) {
		if (current != null) {
			Destroy (current.gameObject);
		}
		index = x;
		current = Instantiate (models [index], container.position, Quaternion.identity).GetComponent<Animator> ();
		current.transform.SetParent (container, false);

		for (int i = 0; i < menus.Count; i++) {
			if (menus [i] != null) {
				menus [i].SetActive (false);
			}
		}
		if (menus [x] != null) {
			menus [x].SetActive (true);
		}

		BTN ("Idle");
	}

	public void BTN(string s) {
		currentAnim = s;
		current.Play (s);

		if (s == "Idle") {
			current.SetBool ("isWalking", false);
			current.SetBool ("isRunning", false);
			current.SetBool ("isBurrowed", false);
		}

		if (s == "Walk") {
			current.SetBool ("isWalking", true);
			current.SetBool ("isRunning", false);
			current.SetBool ("isBurrowed", false);
		}
		if (s == "Run") {
			current.SetBool ("isWalking", false);
			current.SetBool ("isRunning", true);
			current.SetBool ("isBurrowed", false);
		}
		if (s == "Burrow") {
			current.SetBool ("isWalking", false);
			current.SetBool ("isRunning", false);
			current.SetBool ("isBurrowed", true);
		}
		if (s == "Unburrow") {
			current.SetBool ("isWalking", false);
			current.SetBool ("isRunning", false);
			current.SetBool ("isBurrowed", false);
		}
	}

	public void ROTATE(float direc) {
		isRotating = true;
		rotDir = direc;
	}

	public void ROTCANCEL() {
		isRotating = false;
		rotDir = 0f;
	}

	public void Update() {
		if (isRotating == true) {
			container.Rotate (new Vector3(0f, rotDir * Time.deltaTime, 0f));
		}
	}
}
