using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_CONTROL_CHARACTER : MonoBehaviour
{

	bool start = true;
	private void Update()
	{
		/*
		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			StopAllCoroutines();
			StartCoroutine(STIMULATE());
		}
		*/

		if(start)
		{
			StartCoroutine(STIMULATE());
			start = false;
		}
	}

	public Animator guard_animator;
	public Transform guard_transform;

	IEnumerator STIMULATE()
	{
		#region frame_rate
		QualitySettings.vSyncCount = 2;
		yield return null; yield return null;
		#endregion

		/*
		vec3: input movement
		float: move_amount <- input_movement
		bool: is_crouching
		
		Blend-Tree
			Locomoiton
				||					with no exit time
			Locomotion-Crouch
		*/
		while (true)
		{
			//input_controller_0();
			input_controller_1();
			yield return null;
		}

	}

	bool is_crouching = false;
	void input_controller_0()
	{
			#region locomotion--idle, walk, jog
			Vector3 inp_movement = new Vector3()
			{
				x = Input.GetAxis("Horizontal"),
				y = 0f,
				z = Input.GetAxis("Vertical"),
			};

			float move_amount = Mathf.Abs(inp_movement.x) +
								Mathf.Abs(inp_movement.z);
			if (move_amount > 1f) move_amount = 1f;

			// translate
			guard_animator.SetFloat("float--move-amount", move_amount);

			// rotate
			if (move_amount > 0f)
			{
				guard_transform.rotation = Quaternion.RotateTowards(
					transform.rotation, // curr
					Quaternion.LookRotation(inp_movement), //
					500 * Time.deltaTime // delta_angle: depend on frame rate
				);
			}

			#endregion

			#region locomotion--crouch-idle, crouch-walk
			if(Input.GetKeyDown(KeyCode.C))
			{
				is_crouching = !is_crouching;
				guard_animator.SetBool("bool--crouch", is_crouching);

				#region ad transition through cross fade 
				/* // or same transtion could be achived by .CrossFade
				if(is_crouching)
					guard_animator.CrossFade("Locomotion-crouch Blend Tree", transition_duration: 0.25f);
				else
					guard_animator.CrossFade("Locomotion Blend Tree", transition_duration: 0.15f);
				*/
				#endregion
			}

			guard_animator.SetFloat("float--crouch-move-amount", move_amount);
			#endregion

			#region sprint
			if(move_amount > 0.5f + 0.3f) // is in walk locomotion, due to transition decrement of blend tree float val
			{
				guard_animator.SetBool("bool--sprint", Input.GetKey(KeyCode.LeftShift));

				bool is_running = Input.GetKey(KeyCode.LeftShift);
				if(is_running == true)
				{
					is_crouching = false;
					guard_animator.SetBool("bool--crouch", is_crouching);
				}
			}
			else
			{
				guard_animator.SetBool("bool--sprint", false);
			}
			#endregion
	}

	void input_controller_1()
	{
		/*
		flow
			- toggle couch
			- set move-amt
			- set bsprint, set bsouch
			- rotation
		*/

		#region input_axis
		Vector3 input_axis = new Vector3()
		{
			x = Input.GetAxis("Horizontal"),
			y = 0f,
			z = Input.GetAxis("Vertical"),
		}; 
		#endregion

		Vector2 amt_v2 = new Vector2(Mathf.Abs(input_axis.x), Mathf.Abs(input_axis.z));
		//float move_amt = (amt_v2.x  + amt_v2.y)					  // manhattan variation (0 to 2)
		float move_amt = (amt_v2.x * amt_v2.x + amt_v2.y * amt_v2.y); // parabolic variation (0 to 2)
		// Debug.Log($"move-amt: {move_amt}");

		ACTRL.animator = this.guard_animator;
		// couch >>
		if (Input.GetKeyDown(KeyCode.C)) ACTRL.toggle("bcouch");
		// << couch

		//// walk, jog, couch-walk, sprint, rotate  ////
		ACTRL.setf("fmove-amt", move_amt);
		ACTRL.setf("fmove-couch-amt", move_amt);

		// sprint
		float e = 1f / 100;
		if (move_amt < e) { ACTRL.setb("bsprint", false); return; /* exit sprint, rotate */ };
		if (Input.GetKey(KeyCode.LeftShift) && move_amt > 0.85f) { ACTRL.setb("bsprint", true); ACTRL.setb("bcouch", false); } // move-amt close to jog
		else													 { ACTRL.setb("bsprint", false); }

		// rotate >>
		ACTRL.transform = guard_transform;
		#region ad without cinemachine
		// without cinemachine
		// guard_transform.rotation = ACTRL.rotate_toward(input_axis, 500 * Time.deltaTime); // depend on frame rate
		#endregion
		
		// with cinemachine
		Vector3 cam_foward = Camera.main.transform.forward; cam_foward.y = 0f;
		Vector3 new_guard_forward = ACTRL.look(cam_foward) * input_axis;

		#region 180 turn
		// 180 turn >>
		float turn_angle = Mathf.Abs(Vector3.SignedAngle(guard_transform.forward, new_guard_forward, axis: Vector3.up));
		Debug.Log($"turn_angle: {turn_angle}");
		//ACTRL.setb("b180", turn_angle > 120f);
		// << 180 turn 
		#endregion

		guard_transform.rotation = ACTRL.rotate_toward(new_guard_forward, 500 * Time.deltaTime); // depend on frame rate, with cam align
		// << rotate
	}

	public static class ACTRL
	{
		public static Animator animator;
		public static Transform transform;

		public static bool getb(string parameter)
		{
			return animator.GetBool(parameter);
		}
		public static float getf(string parameter)
		{
			return animator.GetFloat(parameter);
		}

		public static void setb(string parameter, bool val)
		{
			animator.SetBool(parameter, val);
		}
		public static void setf(string parameter, float val)
		{
			animator.SetFloat(parameter, val);
		}

		public static void toggle(string parameter)
		{
			animator.SetBool(parameter, !animator.GetBool(parameter));
		}

		public static Quaternion rotate_toward(Vector3 look, float dt)
		{
			return Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(look), dt);
		}
		public static Quaternion look(Vector3 look_dir)
		{
			return Quaternion.LookRotation(look_dir);
		}
	}


	// TODO transition based on current state being played//
}
