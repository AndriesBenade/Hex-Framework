// PlayerController
using System.Collections;
using UnityEngine;

[System.Serializable]
public enum lookType
{
	lookX_PlayerY,
	lookXY_PlayerY
}

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Event Framework/Player Controller", 0)]
public class PlayerController : MonoBehaviour
{
	[Header("Player Controller")]
	public float normalSpeed = 0.2f;
	public float fastSpeed = 0.4f;
	public float jumpSpeed = 2.5f;
	public float jumpDuration = 1f;
	[Space(5f)]
	public bool allowJump = false;
	public float gravity = -9.8f;
	public bool includeDeltaTime = true;
	public bool gravityAcceleration;
	[Space(5)]
	public bool allowCrouch = false;
	public float crouchSpeedMultiplier = 0.4f;
	[Space(5)]
	public bool allowStamina = false;
	public float maxStamina = 100;
	public float cooldownTime = 4;
	public float subtraction = 10;
	[Space(5f)]
	public Transform look;
	public lookType lookBehaviour;
	public float maxLookAngle = 85f;
	public float sensitivityX = 10f;
	public float sensitivityY = 8f;
	public bool enableAimSensitivity;
	public float aimSensitivity = 3.5f;
	[Space(5)]
	public Camera cam;
	public bool force_pause = false;
	[Space(5)]
	public bool allowPhysInteraction = false;
	public float physMultiplier = 10;


	private CharacterController character;
	private bool halt;
	private float rotationX;
	private float rotationY;
	private float jumpPower;
	private Vector3 move;
	private bool crouch = false;
	private bool lastMoving = false;
	private float sprintTime = 0;
	private float staminaSubTime = 0;
	private float stamina = 100;
	private Vector3 moveForce = new Vector3(0, 0, 0);

	private void Start()
	{
		rotationX = base.transform.eulerAngles.x;
		rotationY = base.transform.eulerAngles.y;
		if (character == null)
		{
			character = GetComponent<CharacterController>();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			halt = ((!halt) ? true : false);
		}
		if (!halt  && !force_pause)
		{
			bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow);
			if (moving && getSprint())
            {
				if (!lastMoving)
                {
					staminaSubTime = Time.time;
					lastMoving = true;
				}
            }
			else
            {
				if (lastMoving)
                {
					sprintTime = Time.time;
					lastMoving = false;
                }
            }
			if (Input.GetKeyDown(KeyCode.Space) && character.isGrounded && allowJump)
			{
				StartCoroutine("Jump");
			}
			if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) && allowCrouch)
			{
				setCrouch(crouch ? false : true);
			}
			rotationY += Input.GetAxis("Mouse X") * ((!enableAimSensitivity) ? sensitivityY : (Input.GetMouseButton(1) ? aimSensitivity : sensitivityY));
			rotationX += Input.GetAxis("Mouse Y") * ((!enableAimSensitivity) ? sensitivityX : (Input.GetMouseButton(1) ? aimSensitivity : sensitivityX));
			rotationX = Mathf.Clamp(rotationX, 0f - maxLookAngle, maxLookAngle);
			switch (lookBehaviour)
			{
			case lookType.lookXY_PlayerY:
				look.eulerAngles = new Vector3(0f - rotationX, rotationY, 0f);
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, look.eulerAngles.y, base.transform.eulerAngles.z);
				break;
			case lookType.lookX_PlayerY:
				look.localEulerAngles = new Vector3(0f - rotationX, 0f, 0f);
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, rotationY, base.transform.eulerAngles.z);
				break;
			}
			if (allowStamina)
            {
				if (moving && getSprint())
                {
					if (Time.time - staminaSubTime >= 1)
                    {
						stamina -= subtraction;
						stamina = stamina < 0 ? 0 : stamina;
						staminaSubTime = Time.time;
                    }
                }
				else
                {
					if (Time.time - sprintTime >= cooldownTime && stamina != maxStamina)
                    {
						if (Time.time - staminaSubTime >= 1)
                        {
							stamina += subtraction;
							stamina = stamina > maxStamina ? maxStamina : stamina;
							staminaSubTime = Time.time;
						}
                    }
                }
            }
		}
	}

	private void setCrouch(bool b)
    {
		crouch = b;
		if (crouch)
		{
			look.localPosition = new Vector3(0, 2.5f, 0);
			character.height = 3;
			character.center = new Vector3(0, 1.5f, 0);
		}
		else
		{
			look.localPosition = new Vector3(0, 5.45f, 0);
			character.height = 6;
			character.center = new Vector3(0, 3, 0);
			//transform.position = new Vector3(transform.position.x, );
        }
    }

    private void FixedUpdate()
	{
		if (!halt && !force_pause)
		{
			move = (base.transform.forward * Input.GetAxis("Vertical") + base.transform.right * Input.GetAxis("Horizontal"));
			move *= crouch ? (normalSpeed * crouchSpeedMultiplier) : (getSprint() ? fastSpeed : normalSpeed);
			moveForce = move;
			if (allowJump)
				move.y += jumpPower + gravity * (includeDeltaTime ? Time.deltaTime : 1f) + (gravityAcceleration ? (gravity * (includeDeltaTime ? Time.deltaTime : 1f)) : 0f);
			character.Move(move);
			float verticalVelosity = 0;
			if (character.isGrounded)
			{
				verticalVelosity -= 0;
			}
			else
			{
				verticalVelosity -= 1;
			}
			move = new Vector3(0, verticalVelosity, 0);
			character.Move(move);
		}
	}

	public bool getCrouch()
	{
		return crouch;
	}

	public bool getSprint()
	{
		bool b = Input.GetKey(KeyCode.LeftShift);
		if (stamina <= 0 || crouch)
			b = false;
		return b;
	}

	public float getStamina()
    {
		return stamina;
    }

	private IEnumerator Jump()
	{
		_ = jumpSpeed;
		float[] changeFactor = new float[5]
		{
			jumpSpeed / -4f * 0f + jumpSpeed,
			jumpSpeed / -4f * 0.25f + jumpSpeed,
			jumpSpeed / -4f * 1f + jumpSpeed,
			jumpSpeed / -4f * 2.25f + jumpSpeed,
			jumpSpeed / -4f * 4f + jumpSpeed
		};
		jumpPower = 0f;
		for (int i = 0; i < 5; i++)
		{
			character.Move(new Vector3(0f, changeFactor[i], 0f));
			yield return new WaitForSeconds(jumpDuration / 5f);
		}
		jumpPower = 0f;
		yield return null;
	}

	public GameObject getCamera()
    {
		return cam.gameObject;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
		if (allowPhysInteraction)
        {
			Vector3 force = hit.moveDirection;
			force.x *= moveForce.x;
			force.y *= moveForce.y;
			force.z *= moveForce.z;
			force *= physMultiplier;
			if (hit.gameObject.GetComponent<Rigidbody>() != null)
				hit.gameObject.GetComponent<Rigidbody>().AddForce(force);
		}
    }

	public bool isTired()
    {
		return (stamina <= 0);
    }

	public bool isCrouching()
    {
		return crouch;
    }
}
