using UnityEngine;

public class DynamicTerrain2D : MonoBehaviour {

	public Vector2[] SurfacePoints;
	public int LevelOfDetail = 1;
	public float Height = 1;

	public void RefreshTerrain ()
	{
		if (LevelOfDetail < 1)
			return;
		
		Mesh mesh;
		mesh = new Mesh ();
		gameObject.GetComponent<MeshFilter> ().mesh = mesh;

		Vector3[] upper_verts = new Vector3[(SurfacePoints.Length-1)*LevelOfDetail+1];
		Vector3[] bottom_verts = new Vector3[upper_verts.Length];


		for (int i = 0; i < SurfacePoints.Length-1; i++) {
			for (int j = 0; j <= LevelOfDetail; j++) {
				Vector3 nextPosDir = SurfacePoints [i + 1] - SurfacePoints [i];

				//Smooth y axie of points
				float yc = easeInOutSine (SurfacePoints [i].y, SurfacePoints [i + 1].y,  (float)(j) / LevelOfDetail);
				nextPosDir *= (float)(j) / LevelOfDetail;
				nextPosDir = new Vector3 (nextPosDir.x, yc, nextPosDir.z);

				upper_verts [i * LevelOfDetail + j] = new Vector3( SurfacePoints [i].x,SurfacePoints [i].y,0) + nextPosDir;
			}
		}

		for(int i = 0; i < upper_verts.Length; i++){
			bottom_verts [i] = new Vector3 (upper_verts[i].x,upper_verts[i].y-Height,upper_verts[i].z);
		}


		int[] tris = new int[upper_verts.Length*3*2];
		for (int i = 0,j=0; i < upper_verts.Length-1; i++,j+=6) {
			tris [j] = i;
			tris [j+1] = i+upper_verts.Length+1;
			tris [j+2] = i+upper_verts.Length;

			tris [j+3] = i;
			tris [j+4] = i+1;
			tris [j+5] = i+upper_verts.Length+1;
		}

		Vector3[] verts = new Vector3[upper_verts.Length+bottom_verts.Length];
		Vector2[] uvs = new Vector2[verts.Length];

		upper_verts.CopyTo (verts,0);
		bottom_verts.CopyTo (verts,upper_verts.Length);

		for (int i=0; i < uvs.Length;i++) {
			uvs [i] = new Vector2 (verts [i].x, verts [i].y);
		}

		mesh.Clear ();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();


		// Generate poltgon colloder
		Vector2[] colPoints = new Vector2[upper_verts.Length+bottom_verts.Length];
		for (int i = 0,j=bottom_verts.Length-1; i < upper_verts.Length; i++,j--) {
			//upper points
			colPoints [i] = new Vector2 (upper_verts [i].x, upper_verts [i].y);
			//bottom points
			colPoints [upper_verts.Length+i] = new Vector2 (bottom_verts [j].x, bottom_verts [j].y);
		}
		gameObject.GetComponent<PolygonCollider2D> ().points = colPoints;
	}


	float easeInOutSine(float start, float end, float val){
		end -= start;
		return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1);
	}

}
