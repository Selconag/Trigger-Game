using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Lean.Pool;
using System;
using System.Threading.Tasks;
public class Player : MonoBehaviour
{
    [Header("Movement Attributes")]
	[Tooltip("Determines movement speed.")]
	[Range(0.0f, 50f)]
	[SerializeField] private float m_ForwardMovementSpeed = 5f;
	[Range(0.0f, 50f)]
	[SerializeField] private float m_HorizontalMovementSpeed = 5f;
	[Range(0.0f, 5f)]
	[SerializeField] private float m_BoundariesLimit = 3f;
	[Range(1.0f, 60f)]
	[Tooltip("Determines rotation speed.")]
	[SerializeField] private float rotateSpeed = 5.0f;

	[Header("Other Attributes")]
	[Range(0f, 1f)]
	[SerializeField] public int AmmoAmount,ArmourAmount;
	
	[SerializeField] public bool isAlive;

	GameObject _player;
	GameObject endScreen;

	[SerializeField] private GameObject m_BulletObject;
	[SerializeField] private Transform m_BulletSpawnPoint;
	//[SerializeField] private Transform m_BulletPool;

	private static Player _instance;
	private Joystick m_Joystick;
	private Rigidbody m_Rigid;
	private Animator m_Animator;
	private bool gameStarted = false;
	private bool fingerRelease = true;

	#region Getter-Setters
	public static Player Instance
	{
		get { return _instance; }
	}

	private void GameStarted()
	{
		gameStarted = true;
	}

	public Vector3 PlayerPosition
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	#endregion

	#region MonoBehaviour
	private void Start()
	{
		_instance = this;
		m_Joystick = Joystick.Instance;
		m_Rigid = GetComponent<Rigidbody>();
		m_Animator = GetComponent<Animator>();
		_player = GameObject.FindGameObjectWithTag("Player");
		//m_BulletPool = GameObject.Find("BulletPool").transform;
	}

	void FixedUpdate()
	{
		endScreen.SetActive(!_player.activeInHierarchy);

		//if (m_BulletPool == null) m_BulletPool = GameObject.Find("BulletPool").transform;
		if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !gameStarted) gameStarted = true;
		if (!gameStarted) return;


		if (Input.touchCount > 0 || Input.GetMouseButton(0) && fingerRelease)
		{
			fingerRelease = false;

			if (m_Joystick.Horizontal > 0.6f || m_Joystick.Horizontal < -0.6f)
            {
				NewMovementSystem(m_Joystick.Horizontal);
			}
		}
        else
        {
			fingerRelease = true;
			float vertical = Time.deltaTime * m_ForwardMovementSpeed;
			Vector3 m_NewLocation = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z + vertical);
			this.transform.position = m_NewLocation;
		}

		////REMOVED PART
		//if (Input.touchCount > 0 || Input.GetMouseButton(0))
		//{
		//	float horizontal = m_Joystick.Horizontal * Time.deltaTime * m_HorizontalMovementSpeed;
		//	float vertical = Time.deltaTime * m_ForwardMovementSpeed;
		//	Vector3 m_NewLocation = new Vector3(
		//		Mathf.Clamp(this.gameObject.transform.position.x + horizontal, -m_BoundariesLimit, m_BoundariesLimit),
		//		this.gameObject.transform.position.y,
		//		this.gameObject.transform.position.z + vertical);
		//	this.transform.position = m_NewLocation;
		//}
		//else
		//{
		//	float vertical = Time.deltaTime * m_ForwardMovementSpeed;
		//	Vector3 m_NewLocation = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z + vertical);
		//	this.transform.position = m_NewLocation;
		//}
	}
	
	private void NewMovementSystem(float horizontal)
    {
		float vertical = Time.deltaTime * m_ForwardMovementSpeed;
		float horizontalMovement = 0f;

		//Player will skip to the next path
		if (horizontal > 0f)
        {
			horizontalMovement = 2.0f;
		}
		else if (horizontal == 0f)
        {
			horizontalMovement = 0f;
		}
        else
        {
			horizontalMovement = -2.0f;
		}
		Vector3 m_NewLocation = new Vector3(
				Mathf.Clamp(this.gameObject.transform.position.x + horizontalMovement, -m_BoundariesLimit, m_BoundariesLimit),
				this.gameObject.transform.position.y,
				this.gameObject.transform.position.z + vertical);
		this.transform.position = m_NewLocation;

	}

	private void isKilled()
    {
		if(!isAlive)
        {
			Destroy(this);
        }
    }
	//private void SpawnBullets()
	//{
	//	AbilityManager AbilityM = AbilityManager.Instance;
	//	GameObject go = LeanPool.Spawn(m_BulletObject, m_BulletSpawnPoint.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), m_BulletPool);
	//	Bullet bullet = go.GetComponent<Bullet>();
	//	bullet.IsInstantiatable = true;
	//	bullet.PierceLevel = AbilityM.PiercingLevel;
	//	bullet.BounceLevel = AbilityM.BouncingLevel;
	//	go.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
	//	//go.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	//}
	#endregion
}