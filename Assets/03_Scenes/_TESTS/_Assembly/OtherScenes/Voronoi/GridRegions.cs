using System.Collections.Generic;
using UnityEngine;


namespace OldShit
{
    public class GridRegions : MonoBehaviour
    {
        public int xCells, yCells, maxRadius, points;
        public Material vertexColorMat;

        private List<PointArea> pointAreas;

        private Dictionary<Vector2, Cell> cellDict;
        private Dictionary<Vector2, MeshFilter> quads;

        private Vector2[] circleCells;

        
        private void Start()
        {
            // Prepare CircleStamp  ///////////////////////////////
            int sideLength = maxRadius * 2 + 1;
            int arrayLength = sideLength * sideLength;

            Vector2[] box = new Vector2[arrayLength];
            bool[] insideCircle = new bool[arrayLength];

            int nr = 0;
            int circleCount = 0;

            for ( int x = 0; x < sideLength; x++ )
                for ( int y = 0; y < sideLength; y++ )
                {
                    box[nr] = new Vector2(x - maxRadius, y - maxRadius);

                    if ( Vector2.Distance(box[nr], V2.zero) <= maxRadius )
                    {
                        insideCircle[nr] = true;
                        circleCount++;
                    }

                    nr++;
                }

            circleCells = new Vector2[circleCount];

            nr = 0;

            for ( int i = 0; i < arrayLength; i++ )
                if ( insideCircle[i] )
                {
                    circleCells[nr] = box[i];
                    nr++;
                }

            // Prepare QuadMeshes   //////////////////////////////////////
            quads = new Dictionary<Vector2, MeshFilter>();

            for ( int x = 0; x < xCells; x++ )
                for ( int y = 0; y < yCells; y++ )
                {
                    quads.Add(new Vector2(x, y), CreateQuad(x, y));
                }

        }

        
        private void ResetDictionarys()
        {
            bool newDictionary = (cellDict == null);

            if ( newDictionary )
                cellDict = new Dictionary<Vector2, Cell>();

            for ( int x = 0; x < xCells; x++ )
                for ( int y = 0; y < yCells; y++ )
                {
                    Vector2 key = new Vector2(x, y);

                    if ( newDictionary )
                        cellDict.Add(key, new Cell());


                    Cell cell = cellDict[key];
                    cell.areaID = -1;
                    cell.distance = 1000;

                    MeshFilter quadStuff = quads[key];
                    quadStuff.mesh = GetQuadMesh(Color.black, quadStuff.mesh);
                }

        }

        
        private MeshFilter CreateQuad(float x, float y)
        {
            GameObject newQuad = new GameObject();
            MeshFilter   mF = newQuad.AddComponent<MeshFilter>();
            MeshRenderer mR = newQuad.AddComponent<MeshRenderer>();
            
            mR.material = vertexColorMat;

            float xOffset = xCells - xCells * .5f;
            float yOffset = yCells - yCells * .5f;

            newQuad.transform.position = new Vector3(x - xOffset, y - yOffset, 0);
            newQuad.transform.parent = transform;

            newQuad.name = "Quad_" + x.ToString("F0") + "/" + y.ToString("F0");

            mF.mesh = new Mesh
            {
                vertices = new[] {new Vector3(.5f, .5f, 0), new Vector3(.5f, -.5f, 0), new Vector3(-.5f, -.5f, 0), new Vector3(-.5f, .5f, 0)}, 
                triangles = new[] {0, 1, 2, 3, 0, 2}
            };

            return mF;
        }


        private void Update()
        {
            if ( Input.GetKeyDown(KeyCode.Mouse0) )
            {
                float t = Time.realtimeSinceStartup;
                CreateGrid();
                Debug.Log(Time.realtimeSinceStartup - t);
            }
        }

        
        private void CreateGrid()
        {
            //! ResetDictionary  ////////////////////////////////
            ResetDictionarys();

            //! ADD POINTS AND ADD INFLUENCE     /////////////////////////////////////////////
            pointAreas = new List<PointArea>();
            Vector2 randomPoint = V2.zero;

            for ( int i = 0; i < points; i++ )
            {
                bool canSet = false;

                while ( !canSet )
                {
                    canSet = true;
                    randomPoint = new Vector2(Random.Range(0f, xCells), Random.Range(0f, yCells));

                    for ( int e = 0; e < pointAreas.Count; e++ )
                        if ( (randomPoint - pointAreas[e].areaPoint).sqrMagnitude < 4 )
                            canSet = false;
                }

                PointArea newArea = new PointArea(randomPoint, new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), 1));
                pointAreas.Add(newArea);

                SetInfluences(newArea, i);
            }

            //! Quads       /////////////////////////////////////////////////////////
            for ( int x = 0; x < xCells; x++ )
                for ( int y = 0; y < yCells; y++ )
                {
                    Vector2 key = new Vector2(x, y);
                    MeshFilter quadStuff = quads[key];
                    quadStuff.mesh = GetQuadMesh(CellArea(cellDict[key]), quadStuff.mesh);
                    //CellArea(cellDict[key]);
                }

        }

        
        private void SetInfluences(PointArea pointArea, int areaID)
        {
            Vector2 roundedPosition = new Vector2(Mathf.Round(pointArea.areaPoint.x), Mathf.Round(pointArea.areaPoint.y));

            for ( int i = 0; i < circleCells.Length; i++ )
            {
                Vector2 key = circleCells[i] + roundedPosition;

                if ( key.x >= 0 && key.y >= 0 && key.x < xCells && key.y < yCells )
                {
                    Cell cell = cellDict[key];

                    float distance = (key - pointArea.areaPoint).sqrMagnitude;

                    if ( distance < cell.distance )
                    {
                        cell.distance = distance;
                        cell.areaID = areaID;
                    }
                }
            }
        }


        private Color CellArea(Cell gridCell)
        {
            // Debug.Log(gridCell.areaID);

            return (gridCell.areaID != -1) ? pointAreas[gridCell.areaID].areaColor : Color.black;
        }

        
        private static Mesh GetQuadMesh(Color meshColor, Mesh m)
        {
            m.colors = new[]{
            meshColor,
            meshColor,
            meshColor,
            meshColor
            };

            return m;
        }
    }

    [System.Serializable]
    public class Cell
    {
        public int areaID;
        public float distance;
    }

    [System.Serializable]
    public class PointArea
    {
        public Vector2 areaPoint;
        public Color areaColor;

        public PointArea(Vector2 areaPoint, Color areaColor)
        {
            this.areaPoint = areaPoint;
            this.areaColor = areaColor;
        }
    }
}