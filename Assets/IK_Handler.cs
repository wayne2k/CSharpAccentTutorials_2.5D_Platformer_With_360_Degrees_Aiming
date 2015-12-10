using UnityEngine;
using System.Collections;

public class IK_Handler : MonoBehaviour 
{
	Animator anim;
	Vector3 lookPos;
	Vector3 ik_LookPos;
	Vector3	targetPos;

	PlayerController p1;

	public bool enableIK;

	public float lerpRate = 15f;
	public float updateLookPositionThreshold = 2f;
	public float lookWeight = 1f;
	public float bodyWeight = .9f;
	public float headWeight = 1f;
	public float clampWeight = 1f;

	public float rightHandWeight = 1f;
	public float leftHandWeight = 1f;

	public Transform rightHandTarget;
	public Transform rightElbowTarget;
	public Transform leftHandTarget;
	public Transform leftElbowTarget;

	void Awake ()
	{
		anim = GetComponent<Animator>();
		p1 = GetComponent<PlayerController>();
	}

	void OnAnimatorIK ()
	{
		if (enableIK == false)
		{
			anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, 0f);
			anim.SetIKHintPositionWeight (AvatarIKHint.LeftElbow, 0f);
			anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 0f);
			anim.SetIKHintPositionWeight (AvatarIKHint.RightElbow, 0f);
			anim.SetLookAtWeight (0f, 0f, 0f, 0f, 0f);
			return;
		}

		anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, leftHandWeight);
		anim.SetIKPosition (AvatarIKGoal.LeftHand, leftHandTarget.position);
		anim.SetIKRotation (AvatarIKGoal.LeftHand, leftHandTarget.rotation);
	
		anim.SetIKHintPositionWeight (AvatarIKHint.LeftElbow, leftHandWeight);
		anim.SetIKHintPosition (AvatarIKHint.LeftElbow, leftElbowTarget.position);

		anim.SetIKPositionWeight (AvatarIKGoal.RightHand, rightHandWeight);
		anim.SetIKPosition (AvatarIKGoal.RightHand, rightHandTarget.position);
		anim.SetIKRotation (AvatarIKGoal.RightHand, rightHandTarget.rotation);

		anim.SetIKHintPositionWeight (AvatarIKHint.RightElbow, rightHandWeight);
		anim.SetIKHintPosition (AvatarIKHint.RightElbow, rightElbowTarget.position);

		lookPos = p1.lookPos;
		lookPos.z = transform.position.z;

		float distaneFromplayer = Vector3.Distance(lookPos, transform.position);

		if (distaneFromplayer > updateLookPositionThreshold)
		{
			targetPos = lookPos;
		}

		ik_LookPos = Vector3.Lerp(ik_LookPos, targetPos, Time.deltaTime * lerpRate);

		anim.SetLookAtWeight (lookWeight, bodyWeight, headWeight, headWeight, clampWeight);
		anim.SetLookAtPosition (ik_LookPos);
	}

}
