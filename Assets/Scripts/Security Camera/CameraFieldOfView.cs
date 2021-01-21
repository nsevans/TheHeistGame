using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraFieldOfView : MonoBehaviour {

	public float viewRadius;			//Radius of the view
	[Range(0,360)]
	public float viewAngle;				//Angle of the view from 0 degrees to 360 degrees

	public LayerMask targetMask;		//The layer that targets will be in
	public LayerMask obstacleMask;		//The layer that obstacles will be in

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();	//A list of all visible targets (Guards and Player)

	public float meshResolution;		//Number of rays cast per degree
	public int edgeResolveIterations;
	public float edgeDistanceThreshold;

	public MeshFilter viewMeshFilter;
	private Mesh viewMesh;

	void Start(){
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
		//StartCoroutine("FindTargetsWithDelay", 0.2f);
	}

	IEnumerator FindTargetsWithDelay(float delay){
		while(true){
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void LateUpdate(){
		FindVisibleTargets();
		DrawFieldOfView();
	}

	void FindVisibleTargets(){
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for(int i=0; i< targetsInViewRadius.Length; i++){
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2){
				float distToTarget = Vector3.Distance(transform.position, target.position);
				if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask)){
					//No obstacles in the way, can see target
					Debug.DrawLine(transform.position, target.position, Color.blue);
					visibleTargets.Add(target);

				}
			}
		}
	}

	void DrawFieldOfView(){
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();
		ViewCastInfo oldViewCast = new ViewCastInfo();

		for(int i=0; i<= stepCount; i++){
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);

			if(i > 0){
				bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
				if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded)){
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if(edge.pointA != Vector3.zero){
						viewPoints.Add(edge.pointA);
					}else if(edge.pointB != Vector3.zero){
						viewPoints.Add(edge.pointB);
					}
				}
			}

			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for(int i=0; i<vertexCount-1; i++){
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if(i < vertexCount - 2){
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}
		viewMesh.Clear();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();

	}

	EdgeInfo FindEdge(ViewCastInfo min, ViewCastInfo max){
		float minAngle = min.angle;
		float maxAngle = max.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for(int i=0; i<edgeResolveIterations; i++){
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

			bool edgeDistThresholdExceeded = Mathf.Abs(min.dist - newViewCast.dist) > edgeDistanceThreshold;
			if(newViewCast.hit == min.hit && !edgeDistThresholdExceeded){
				minAngle = angle;
				minPoint = newViewCast.point;
			}else{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}
		return new EdgeInfo(minPoint, maxPoint);
	}

	ViewCastInfo ViewCast(float globalAngle){
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)){
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}else{
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
		if(!angleIsGlobal){
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo{
		public bool hit;
		public Vector3 point;
		public float dist;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle){
			hit = _hit;
			point = _point;
			dist = _dist;
			angle = _angle;
		}
	}

	public struct EdgeInfo{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB){
			pointA = _pointA;
			pointB = _pointB;
		}
	}

	//Return a list of all Transforms of visible targets from this objects position
	public List<Transform> GetTargetsInView(){
		return visibleTargets;
	}

	//Return a list of all Collidets in this object's given radius 
	public Collider[] GetTargetsInRadius(){
		return Physics.OverlapSphere(transform.position, viewRadius, targetMask);
	}
}
