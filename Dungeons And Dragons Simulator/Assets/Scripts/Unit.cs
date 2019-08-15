using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour
{

    private int unitID { get; set; }

	private int maxActions { get; set; }

	private int maxHealth { get; set; }

	private int currentHealth { get; set; }

	private int damage { get; set; }

	private int attack { get; set; }

	private int range { get; set; }

	private string name { get; set; }

	private int remainingActions { get; set; }

	private Hex Location;

	private bool moving;
	private Vector3 targetLocation;
	private Vector3 velocity;
	float smoothDistance = 0.01f;
	float smoothTime = 0.5f;


	void Awake ()
	{
		moving = false;
	}



	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moving) {
			if (Vector3.Distance (this.transform.position, targetLocation) < smoothDistance) {
				moving = false;
			} else {
				this.transform.position = Vector3.SmoothDamp (
					this.transform.position, targetLocation,
					ref velocity, smoothTime);
			}
		}
	}

	public void ChangeHex (Hex hex)
	{
		Location = hex;
		targetLocation = hex.Position ();
		moving = true;
	}

	/// <summary>
	/// Sets the max actions.
	/// Guarantees remaining actions <= new max.
	/// </summary>
	/// <param name="actions">Actions.</param>
	public void setMaxActions (int actions)
	{
		maxActions = actions;
		if (remainingActions > actions) {
			remainingActions = actions;
		}
	}

	/// <summary>
	/// Sets the remaining actions.
	/// Cannot exceed max actions.
	/// </summary>
	/// <param name="actions">Actions.</param>
	public void setRemainingActions (int actions)
	{
		if (maxActions < actions) {
			Debug.Log ("Unit cannot have movement = " + actions + ", max movement is " + maxActions);
			return;
		}
		remainingActions = actions;
	}

	/// <summary>
	/// Sets the max health.
	/// Guarantees current health <= new max.
	/// </summary>
	/// <param name="health">Health.</param>
	public void setMaxHealth (int health)
	{
		maxHealth = health;
		if (currentHealth > health) {
			currentHealth = health;
		}
	}

	/// <summary>
	/// Sets the current health.
	/// Wont exceed max health.
	/// </summary>
	/// <param name="health">Health.</param>
	public void setCurrentHealth (int health)
	{
		if (maxHealth < health) {
			Debug.Log ("Unit cannot have health = " + health + ", max health is " + maxHealth);
			return;
		}
		currentHealth = health;
	}

	/// <summary>
	/// Changes the health. If damage, make negative.
	/// Wont exceed max health.
	/// </summary>
	/// <param name="change">Change.</param>
	public void changeHealth (int change)
	{
		currentHealth += change;
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}
	}

	/// <summary>
	/// Changes the actions. If using actions, make negative.
	/// Wont exceed max actions.
	/// </summary>
	/// <param name="change">Change.</param>
	public void changeActions (int change)
	{
		remainingActions += change;
		if (remainingActions > maxActions) {
			remainingActions = maxActions;
		}
	}

	public void setUnitID (int ID)
	{
		if (unitID != null) {
			Debug.Log ("Unit already has ID of " + unitID + ". It can only be set once."); 
			return;
		}

		unitID = ID;
	}




	public class EqualityComparer : IEqualityComparer<Unit>
	{
		public bool Equals (Unit x, Unit y)
		{
			return (x.unitID == y.unitID);
		}

		public int GetHashCode (Unit x)
		{
			return x.unitID;
		}
	}
}
