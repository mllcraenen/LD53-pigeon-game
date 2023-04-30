using UnityEngine;
using UnityEditor;

public class BoidMovement : MonoBehaviour {
	public BoidSettings settingsFile;
	public GameObject[] boids;

	Vector3 velocity = new Vector3();

	//Make all this be done by an external window controller, and done better
	private bool isWrappingX = false;
	private bool isWrappingY = false;
	float rotation = 0;

	//Debug
	Vector3 gizmoMovement = new Vector3();
	Vector3 gizmoForward = new Vector3();
	Vector3 gizmoCollisionAvoid = new Vector3();
	public bool isDebugBoid = false;
	//End debug

	Camera cam;

	public void Init(BoidSettings settings, Transform target) {
		//settingsFile = settings;
		float startSpeed = (settingsFile.minSpeed + settingsFile.maxSpeed) / 2;
		velocity = transform.up * startSpeed;
	}

	void Awake() {
		cam = Camera.main;
		rotation = Random.Range(0, 360);
		transform.Rotate(0, 0, rotation);
		float startSpeed = (settingsFile.minSpeed + settingsFile.maxSpeed) / 2;
		velocity = transform.up * startSpeed;
	}

	private void Start() {
		boids = GameObject.FindGameObjectsWithTag("Boid");
	}


	void Update() {
		Move();
	}

	/**
	 * In the future make every boid only receive their cav, coh, and ali vectors from an external controller that loops over the boids.
	 */
	void Move() {
		Vector3 acceleration = new Vector3();
		// Forward vector
		Vector3 forwardVector = steerTowards(transform.up, settingsFile.Kfow);

		// Avoidance vector
		Vector3 obstacleAvoid = onCollisionCourse() ? steerTowards(getObstacleAvoidanceVector(), settingsFile.Kcav) : Vector3.zero;

		// Separation vector
		Vector3 separation = steerTowards(getSeparationVector(), settingsFile.Ksep);

		// Alignment vector
		Vector3 alignment = steerTowards(getAlignmentVector(), settingsFile.Kali);

		// Cohesion vector
		Vector3 cohesion = steerTowards(getCohesionVector(), settingsFile.Kcoh);

		acceleration += forwardVector;
		acceleration += separation;
		acceleration += obstacleAvoid;
		acceleration += alignment;
		acceleration += cohesion;
		// Mouse targeting vector
		if (settingsFile.EnableMouseTargeting) acceleration += getMouseTargetVector();

		//DEBUG gizmos
		gizmoCollisionAvoid = separation;
		gizmoForward = forwardVector;

		// The previous frame's velocity vector plus the acceleration calculated above is used 
		velocity += acceleration * Time.deltaTime;
		float speed = velocity.magnitude;
		Vector3 dir = velocity / speed;
		speed = Mathf.Clamp(speed, settingsFile.minSpeed, settingsFile.maxSpeed);
		velocity = dir * speed;

		transform.position += velocity * Time.deltaTime;
		transform.up = dir;

		gizmoMovement = acceleration;
		//transform.position += acceleration;
		//transform.up = transform.up * (1 - s.rotationSpeed) + acceleration * (s.rotationSpeed);
	}

	Vector3 steerTowards(Vector3 vector) {
		Vector3 v = vector.normalized * settingsFile.maxSpeed - velocity;
		v.z = 0;
		return Vector3.ClampMagnitude(v, settingsFile.maxSteerForce);
	}
	Vector3 steerTowards(Vector3 vector, float parameter) {
		Vector3 v = vector.normalized * (settingsFile.maxSpeed * parameter) - velocity;
		v.z = 0;
		return Vector3.ClampMagnitude(v, settingsFile.maxSteerForce);
	}

	private Vector3 getMouseTargetVector() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 targetVector = (mousePos - transform.position).normalized * settingsFile.mouseTargetingSpeed;
		targetVector.z = 0;
		if (isDebugBoid) {
			Debug.DrawLine(transform.position, mousePos, Color.red);
		}
		return targetVector;
	}

	private bool onCollisionCourse() {
		float twoRSquared = (2 * Mathf.Pow(settingsFile.visionRange, 2));
		float visionConeWidth = Mathf.Sqrt(twoRSquared - twoRSquared * Mathf.Cos(settingsFile.visionAngle));
		if (Physics2D.CircleCast(transform.position, visionConeWidth, transform.up, settingsFile.visionRange, 1 << settingsFile.ObstacleLayer)) {
			getObstacleAvoidanceVector();
			return true;
		}
		return false;
	}
	private Vector3 getObstacleAvoidanceVector() {
		float twoRSquared = (2 * Mathf.Pow(settingsFile.visionRange, 2));
		float visionConeWidth = Mathf.Sqrt(twoRSquared - twoRSquared * Mathf.Cos(settingsFile.visionAngle));
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, visionConeWidth, transform.up, settingsFile.visionRange, 1 << settingsFile.ObstacleLayer);
		Vector3 sumVector = new Vector3();
		for (int i = 0; i < hits.Length; i++) {
			Vector3 v = (transform.position - new Vector3(hits[i].point.x, hits[i].point.y, 0));
			// Set the magnitude of the vector to be the hit distance fraction of the max distance (RaycastHit2D.fraction is 1 at max distance).
			v = v.normalized; //* (1 - hits[i].fraction)
			sumVector += v;

			if (isDebugBoid) {
				Debug.DrawLine(transform.position, hits[i].point, new Color(255, 0, 0, 50));
			}
		}

		return sumVector;
	}

	/**
	 * ( 1 ) Get Separation Vector.
	 * First of the three main forces that make flocking behaviour.
	 * Steers to avoid crowding local flockmates.
	 */
	private Vector3 getSeparationVector() {
		//make vision cone
		////naive solution: iterate over each boid and test their distance from this boid. Then calculate the angle between them to see if they are within the "vision cone".
		//each boid in vision cone adds an opposing vector whose size is based on the distance to the boid
		Vector3 totalEvasionVector = new Vector3();

		foreach (GameObject boid in boids) {
			if (boid == this.gameObject) continue;
			float dist = Vector3.Distance(boid.transform.position, transform.position);
			//If the boid is within vision range
			if (dist <= settingsFile.visionRange) {
				if (isDebugBoid)
					Debug.DrawLine(transform.position, boid.transform.position, Color.magenta);
				float angle = Vector3.SignedAngle(transform.up, (boid.transform.position - transform.position), Vector3.forward);
				if ((angle >= 0 && angle <= settingsFile.visionAngle) || (angle <= 0 && angle >= -settingsFile.visionAngle)) {
					if (isDebugBoid)
						Debug.DrawLine(transform.position, boid.transform.position, Color.yellow);

					Vector3 boidEvasionDirection = (transform.position - boid.transform.position);
					//The magnitude of the evasion gets larger as the opposing boid is closer.
					float evasionMagnitude = ((settingsFile.visionRange - boidEvasionDirection.magnitude));
					float mag = boidEvasionDirection.magnitude;
					//Set the magnitude of the evasion vector of this from the target boid
					boidEvasionDirection = boidEvasionDirection.normalized * evasionMagnitude;
					//Add the magnitude adjusted vector to the total sum vector
					totalEvasionVector += boidEvasionDirection;
				}
			}
		}

		// Clamp the magnitude of the sum vector to the max magnitude
		//totalEvasionVector = totalEvasionVector.normalized * (s.Ksep * s.maxSpeed) - velocity;
		//Vector3.ClampMagnitude(totalEvasionVector, s.maxSteerForce);
		return totalEvasionVector;
	}

	/**
	 * ( 2 ) Get Alignment Vector.
	 * Second of the three main forces that make flocking behaviour.
	 * Steers towards the average heading of local flockmates.
	 */
	private Vector3 getAlignmentVector() {
		Vector3 alignment = new Vector3();
		foreach (GameObject boid in boids) {
			if (boid == gameObject) continue;
			float dist = Vector3.Distance(boid.transform.position, transform.position);

			// If the boid is in the vision cone
			if (dist <= settingsFile.visionRange) {
				float angle = Vector3.SignedAngle(transform.up, (boid.transform.position - transform.position), Vector3.forward);
				if ((angle >= 0 && angle <= settingsFile.visionAngle) || (angle <= 0 && angle >= -settingsFile.visionAngle)) {
					// Add the direction it is facing to the alignment summation.
					alignment += boid.transform.up.normalized;
				}
			}
		}

		return alignment;
	}

	/**
	 * ( 3 ) Get Cohesion Vector.
	 * Third of the three main forces that make flocking behaviour.
	 * Steers towards the average position of local flockmates.
	 */
	private Vector3 getCohesionVector() {
		Vector3 averagePosition = new Vector3();
		int neighbours = 0;
		foreach (GameObject boid in boids) {
			if (boid == gameObject) continue;
			float dist = Vector3.Distance(boid.transform.position, transform.position);

			// If the boid is in the vision cone
			if (dist <= settingsFile.visionRange) {
				float angle = Vector3.SignedAngle(transform.up, (boid.transform.position - transform.position), Vector3.forward);
				if ((angle >= 0 && angle <= settingsFile.visionAngle) || (angle <= 0 && angle >= -settingsFile.visionAngle)) {
					// Add the boid position to the average position summation.
					averagePosition += (boid.transform.position - transform.position);
					neighbours++;
				}
			}
		}

		// Divide the summation by the amount of boids in range.
		return averagePosition / neighbours;
	}

	private void OnBecameInvisible() {
		ScreenWrap();
	}

	private void OnBecameVisible() {
		isWrappingX = false;
		isWrappingY = false;
	}


	//Wraps the boid around to the other side of the screen.
	private void ScreenWrap() {
		//This line prevents errors being thrown at shutdown.
		if (cam == null) return;

		//If we are wrapping do not proceed. (Prevents infinite wrapping/leaving screen permanently)
		if (isWrappingX && isWrappingY) return;

		//Get the viewport coordinates to test where actor is outside view.
		var viewportPosition = cam.WorldToScreenPoint(transform.position);


		Vector3 newPosition = transform.position;
		//If the actor is outside the camera FOV on the right or left invert the X axis so it'll appear on the other side.
		if (!isWrappingX && (viewportPosition.x > Screen.width || viewportPosition.x < 0)) {
			newPosition.x = -newPosition.x;
			isWrappingX = true;
			if (isDebugBoid) {
				Debug.Log(viewportPosition);
				Debug.Log(Screen.width + " : " + Screen.height);

			}
		}
		//Same for Y.
		if (!isWrappingY && (viewportPosition.y > Screen.height || viewportPosition.y < 0)) {
			newPosition.y = -newPosition.y;
			isWrappingY = true;
		}
		//Set position to wrapped position.
		transform.position = newPosition;
	}

	private void OnDrawGizmos() {
		if (isDebugBoid == true) {
			//Draw the vision arc
			Handles.color = new Color(.5f, .5f, .5f, .1f);
			Handles.DrawSolidArc(transform.position, transform.forward, transform.up, settingsFile.visionAngle, settingsFile.visionRange);
			Handles.DrawSolidArc(transform.position, -transform.forward, transform.up, settingsFile.visionAngle, settingsFile.visionRange);

			Handles.color = Color.blue;
			Handles.DrawLine(transform.position, transform.position + gizmoMovement);
			Handles.color = Color.red;
			Handles.DrawLine(transform.position, transform.position + gizmoForward);
			Handles.color = Color.green;
			Handles.DrawLine(transform.position, transform.position + gizmoCollisionAvoid);
		}

	}

	public void SetColor(Color col) {
		if (transform.GetComponent<SpriteRenderer>() != null) {
			transform.GetComponent<SpriteRenderer>().color = col;
		}
	}
}
