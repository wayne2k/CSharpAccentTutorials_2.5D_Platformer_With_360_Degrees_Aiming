using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour 
{
	public Transform shoulderT;
	public Transform rightShoulder;
	public Vector3 lookPos;
	GameObject _rsp;

	[SerializeField] float _maxSpeed = 15f;
	[SerializeField] float _acceleration = 32f;
	[SerializeField] float _jumpSpeed = 5f;
	[SerializeField] float _jumpDuration = 150f;

	float _horizontal;
	float _vertical;
	bool _jumpInput; //float _jumpInput;

	bool _onGround = true;
	float _jumpTimer;
	bool _jumpKeyDown;
	bool _canVariableJump = false;
	float _movement_anim;


	Vector3 _directionPos;
	Vector3 _storeDir;

	Transform _model;
	LayerMask _layerMask;
	Rigidbody _rb;
	Animator _anim;

	void Awake ()
	{
		SetupAnimator();

		_rb = GetComponent<Rigidbody>();
		_layerMask = ~(1<<8);

		_rsp = new GameObject(transform.root.name + " Right Shoulder IK Helper");
	}

	void Update ()
	{

	}

	void FixedUpdate ()
	{
		InputHandler();
		UpdateRigidbodyValues();
		MovementHandler();
		HandleRotation();
		HandleAimingPosition();
		HandleAnimations();
		HandleShoulder();
	}

	bool IsOnGround ()
	{
		bool retVal = false;
		float lengthToSearch = 1.5f;

		Vector3 lineStart = transform.position + Vector3.up;
		Vector3 vectorToSearch = -Vector3.up;

		RaycastHit hit;
		if (Physics.Raycast(lineStart, vectorToSearch, out hit, lengthToSearch, _layerMask))
		{
			retVal = true;
		}

		return retVal;
	}

	void InputHandler ()
	{
		_horizontal = Input.GetAxis("Horizontal");
		_vertical = Input.GetAxis("Vertical");
		_jumpInput = Input.GetButton("Fire2");
	}

	void UpdateRigidbodyValues ()
	{
		if (_onGround)
		{
			_rb.drag = 4f;
		}
		else
		{
			_rb.drag = 0f;
		}
	}

	void MovementHandler ()
	{
		_onGround = IsOnGround();

		if (_horizontal < -0.1f) 
		{
			if (_rb.velocity.x > -_maxSpeed) {
				_rb.AddForce (new Vector3(-_acceleration, 0f, 0f));
			}
			else {
				_rb.velocity = new Vector3 (-_maxSpeed, _rb.velocity.y, 0f);
			}
		}
		else if (_horizontal > 0.1f) 
		{
			if (_rb.velocity.x < _maxSpeed) {
				_rb.AddForce (new Vector3(_acceleration, 0f, 0f));
			}
			else {
				_rb.velocity = new Vector3 (_maxSpeed, _rb.velocity.y, 0f);
			}
		}

		if (_jumpInput) //if (_jumpInput > 0.1f)
		{
			if (_jumpKeyDown == false) {
				_jumpKeyDown = true;

				if (_onGround) {
					_rb.velocity = new Vector3(_rb.velocity.x, _jumpSpeed, 0f);
					_jumpTimer = 0f;
				}
			}
			else if (_canVariableJump)
			{
				_jumpDuration += Time.deltaTime;

				if (_jumpTimer < _jumpDuration / 1000) {
					_rb.velocity = new Vector3(_rb.velocity.x, _jumpSpeed, 0f);
				}
			}
		}
		else
		{
			_jumpKeyDown = false;
		}
	}

	void HandleAimingPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			Vector3 lookpos = hit.point;
			lookpos.z = transform.position.z;
			this.lookPos = lookpos;
		}
	}

	void HandleRotation ()
	{
		Vector3 directionToLook = lookPos - transform.position;
		directionToLook.y = 0f; //we want to look in the direction of the x-axis only.

		Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
	}

	void HandleAnimations ()
	{
		_anim.SetBool("OnAir", _onGround == false);
		float animvalue = _horizontal;

		if (lookPos.x < transform.position.x) { //If we are aiming to the left of the character
			animvalue = -animvalue; //toggle input value.
		}

		_anim.SetFloat("Movement", animvalue, .1f, Time.deltaTime);
	}

	void HandleShoulder ()
	{
		shoulderT.LookAt(lookPos);

		Vector3 rightShoulderPos = rightShoulder.TransformPoint(Vector3.zero); //armature rightshoulder to worldpos.
		_rsp.transform.position = rightShoulderPos;
		_rsp.transform.parent = transform;

		shoulderT.position = _rsp.transform.position;
	}

	void SetupAnimator ()
	{
		_anim = GetComponent<Animator> ();

		foreach (var childAnim in GetComponentsInChildren<Animator>()) {

			if (childAnim != _anim)
			{
				_anim.avatar = childAnim.avatar;
				Destroy(childAnim);
				break;
			}
		}
	}
}
