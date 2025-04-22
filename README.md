

![image](https://github.com/user-attachments/assets/19224b2e-272c-4fa7-a559-fb8d92ee2345)

https://github.com/user-attachments/assets/75b15503-d3bd-4e90-aa0b-2a41fd7500aa

```cs
	void input_animator_controller()
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
```

```
cinemachine.package installed prior, with component values
```
![image](https://github.com/user-attachments/assets/8334a081-7535-4478-913c-7608db82570d)
<br><br><br>
```
note: for each .anim
type => humanoid, model => copy from model
// to prevent player from rotating of to left or right => few changes
	- select animation.fbx
		- under root transform rotation
			bake into pose -> true (false if we rely on animation to totate character.... eg: run turn 180 )
			based upon -> original ((at start) if bake into pose false)
		- under root transform position(Y)
			bake into pose -> true
			based upon(at start) -> feet
		[apply]

[approach => apply root motion in animator]
```


