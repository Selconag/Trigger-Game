using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
	// If the touch is longer than MAX_SWIPE_TIME, we dont consider it a swipe
	public const float MAX_SWIPE_TIME = 0.5f;

	// Factor of the screen width that we consider a swipe
	// 0.17 works well for portrait mode 16:9 phone
	public const float MIN_SWIPE_DISTANCE = 0.17f;

	[Range(0.0f, 5f)]
	[SerializeField] public float moveAmount = 2.0f;

    [Range(0.0f,10f)]
	[SerializeField] public float moveSpeed = 2.0f;

	public static bool swipedRight = false;
	public static bool swipedLeft = false;
	public static bool swipedUp = false;
	public static bool swipedDown = false;


	public bool debugWithArrowKeys = true;

	Vector2 startPos;
	float startTime;
	private int left = 1; private int right = 1;


    private void Start()
    {
		Debug.Log("Right: " + right + "Left: " + left);
    }

    private void FixedUpdate()
    {
		transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);
    }
    public void Update()
	{
		swipedRight = false;
		swipedLeft = false;
		swipedUp = false;
		swipedDown = false;

		if (Input.touches.Length > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
				startTime = Time.time;
			}
			if (t.phase == TouchPhase.Ended)
			{
				if (Time.time - startTime > MAX_SWIPE_TIME) // press too long
					return;

				Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

				Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

				if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
					return;

				if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
				{ // Horizontal swipe
					if (swipe.x > 0)
					{
						swipedRight = true;
						
						
						if (right < 2)
						{
							right++;
							left--;

							transform.position += new Vector3(moveAmount, 0, 0);
							Debug.Log("Right: " + right + " Left: " + left);
						}
					}
					else
					{
						swipedLeft = true;
						Debug.Log("Right: " + right + " Left: " + left);

						if (left < 2)
						{
							left++; right--;
							transform.position -= new Vector3(moveAmount, 0, 0);
							Debug.Log("Right: " + right + " Left: " + left);
						} 

					}
				}
				else
				{ // Vertical swipe
					if (swipe.y > 0)
					{
						swipedUp = true;
						Debug.Log("Swiped Up");
					}
					else
					{
						swipedDown = true;
						Debug.Log("Swiped Down");
					}
				}
			}
		}

		if (debugWithArrowKeys)
		{
			swipedDown = swipedDown || Input.GetKeyDown(KeyCode.DownArrow);
			swipedUp = swipedUp || Input.GetKeyDown(KeyCode.UpArrow);
			swipedRight = swipedRight || Input.GetKeyDown(KeyCode.RightArrow);
			swipedLeft = swipedLeft || Input.GetKeyDown(KeyCode.LeftArrow);
			
		}

	}



}