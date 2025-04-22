using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_animator_controller_turn : MonoBehaviour
{
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			StopAllCoroutines();
			StartCoroutine(STIMULATE());
		}
	}

	public GameObject guard;
	[Range(0.1f, 5f)]
	public float animator_speed = 1f;

	IEnumerator STIMULATE()
	{
		#region frame_rate
		QualitySettings.vSyncCount = 2;
		yield return null;
		#endregion

		/*
		v2 a = new v2(0, 1); 
		v2 b = (+1, 10);
		v2 c = '>';

		console.log(a, b, c);
		*/

		Animator _animator = guard.GetComponent<Animator>();
		Transform _transform = guard.GetComponent<Transform>();


		_animator.SetBool("bool-walk", true);
		yield return new WaitForSeconds(1f);
		_animator.SetBool("bool-turnright", true);
		// Wait until the “TurnRight” state has played through once
		yield return new WaitUntil(() =>
		{
			AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
			return info.IsName("Right Turn")
				   && info.normalizedTime >= 1f
				   && !_animator.IsInTransition(0);
		});
		Debug.Log("turning right complete from stright....");

		_animator.SetBool("bool-turnright", false);
		_transform.eulerAngles = new Vector3(0f, 90f, 0f);
		yield return null;

		yield return new WaitForSeconds(10f);
		//








		float[] anim_time = new float[]
		{
			1.033f - 0.3f, // turn right
			1.033f - 0.3f, // turn left
		};

		string instr = "^>v<^>";

		char prev_char = instr[0];
		foreach(char curr_char in instr)
		{
			v2 dir = curr_char;
			Debug.Log(dir);

			if(curr_char == prev_char)
			{
				_transform.forward = ((v2)curr_char).getVec3;
				_animator.SetBool("bool-walk", true);
				_animator.SetBool("bool-wait", false);

				// play walking-anim(looped) for 1sec
				yield return new WaitForSeconds(1f);
			}
			else
			{
				// turn
				float area = U.area(curr_char, prev_char);
				console.log(area, curr_char, prev_char);

				if(area > 0f)
				{
					// turn
					_animator.SetBool("bool-turnright", true);
					yield return new WaitForSeconds(anim_time[0]);
					_animator.SetBool("bool-turnright", false);
					yield return null;
					_transform.forward = ((v2)curr_char).getVec3;

					// walk
					_animator.SetBool("bool-walk", true);
					_animator.SetBool("bool-wait", false);

					// play walking-anim(looped) for 1sec
					yield return new WaitForSeconds(1f);
				}
				else if(area < 0f)
				{
					// turn
					_animator.SetBool("bool-turnleft", true);
					yield return new WaitForSeconds(anim_time[1]);
					_animator.SetBool("bool-turnleft", false);
					yield return null;
					_transform.forward = ((v2)curr_char).getVec3;

					// walk
					_animator.SetBool("bool-walk", true);
					_animator.SetBool("bool-wait", false);

					// play walking-anim(looped) for 1sec
					yield return new WaitForSeconds(1f);
				}
			}
			//
			prev_char = curr_char;
		}
		yield return null;
	}


	public class v2
	{
		public float x;
		public float y;

		public v2(float x, float y)
		{ this.x = x; this.y = y; }

		// used as: v2 a = (+1, +10);
		public static implicit operator v2(char _char)
		{
			if (_char == '>') return new v2(+1 , 0 );
			if (_char == '^') return new v2( 0 , +1);
			if (_char == '<') return new v2(-1 , 0 );
			if (_char == 'v') return new v2( 0 , -1);
			else return new v2(100, 100);
		}
		public static implicit operator v2((int X, int Y) t) => new v2(t.X, t.Y);
		public override string ToString()
		{
			return $"[{this.x}, {this.y}]";
		}

		public Vector3 getVec3 { get { return new Vector3(this.x, 0f, this.y); } }
	}

	public class U
	{
		// area
		public static float area(v2 a, v2 b)
		{
			return a.x * b.y - b.x * a.y;
		}

		// dot
		public static float dot(v2 a, v2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		// round
		public static int round(float x)
		{
			if (x > 0f)
			{
				int x_I = (int)x;
				float x_dF = x - x_I;
				if (x_dF > 0.5f)
					return x_I + 1;
				else
					return x_I;			 
			}
			else
			{
				int x_I = (int)x;
				float x_dF = x - x_I;
				if (x_dF < -0.5f)
					return x_I - 1;
				else
					return x_I;
			}
				
		}
	}

}
