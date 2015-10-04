using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;

	private Animator animator;
	private int Food;


	protected override void Start () 
	{
		animator = GetComponent<Animator> ();

		Food = GameManager.instance.playerFoodPoints;

		base.Start ();
	
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = Food;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;


		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall> (horizontal, vertical);

	
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		Food--;

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "food")
		{
			Food += pointsPerFood;
			other.gameObject.SetActive (false);
		} 
		else if (other.tag == "Soda")
		{
			Food += pointsPerSoda;
			other.gameObject.SetActive(false);
		}
	}

	protected override void onCantMove <T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseFood (int loss)
	{
		animator.SetTrigger ("playerHit");
		Food -= loss;
		CheckIfGameOver ();
	}

	private void CheckIfGameOver()
	{
		if (Food <= 0)
			GameManager.instance.GameOver ();

	}
}
