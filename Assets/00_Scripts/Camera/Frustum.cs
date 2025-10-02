using Generation;
using GeoMath;
using UnityEngine;


public enum CellVis { None, Some, All }


public class Frustum
{
    public const int cells = 8, cellCount = cells * cells;
    
    private readonly Camera    cam;
    private readonly Transform camTransform;

    public  Vector3 focusPoint;
    
    
    private readonly Vector3[] camRays = new Vector3[4];

    public Bounds2D frontBounds, backBounds;
    
    private readonly bool[] frontCellCheck = new bool[cellCount], 
                            backCellCheck  = new bool[cellCount];
    
    private readonly CellVis[] frontVis = new CellVis[cellCount],
        backVis = new CellVis[cellCount];
    
    private readonly int[] min = new int[cells], max = new int[cells];

    private bool front, isValid;
    public Vector2Int minCell;

    private readonly float[] quadDepths = { -Level.BorderDepth, -Level.WallDepth, Level.WallDepth, Level.BorderDepth };
    private readonly Quad[]  quads = CollectionInit.Array<Quad>(4);
    private readonly Quad shadowQuad = new Quad();
    private Color[] colors;
    
    
//  Hull & Wrapping  //
    private static int hullCount;
    private readonly Vector2[] hull = new Vector2[20];
   
    private static readonly Line[] wrapLines = new Line[6];
    
    
    public Frustum(Camera cam)
    {
        this.cam     = cam;
        camTransform = cam.transform;
    }


    public void Update()
    {
        Vector3 camPos     = camTransform.position;
        Vector3 camForward = camTransform.forward;
        Quaternion camRot  = camTransform.rotation;
        
        front = camPos.z < 0;

        //  Create Frustum Rays  //
        float yFac = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * .5f);
        float xFac = yFac * cam.aspect;

        camRays[0] = camRot * new Vector3( xFac,  yFac, 1);
        camRays[1] = camRot * new Vector3( xFac, -yFac, 1);
        camRays[2] = camRot * new Vector3(-xFac, -yFac, 1);
        camRays[3] = camRot * new Vector3(-xFac,  yFac, 1);

        //  If any Ray crosses the Center Plane -> Frustum isn't valid  //
        for (int i = 0; i < 4; i++)
            if (camRays[i].z > 0 != front)
            {
                isValid = false;
                return;
            }
        
        isValid = true;

        //  Update Quads and Fill Front and Back Bounds  //
        for (int i = 0; i < 4; i++)
        {
            Quad  quad   = quads[i];
            float depth  = quadDepths[i];
            float toQuad = Mathf.Abs(camPos.z - depth);

            for (int e = 0; e < 4; e++)
            {
                int rayIndex = front ? e : 3 - e;
                quad[e] = camPos + camRays[rayIndex] * (toQuad / Mathf.Abs(camRays[rayIndex].z));
            } 
            
            quad.CalcBounds();
            
            const float maxDist = 300;
            quad.bounds = quad.bounds.Clamp(camPos.x - maxDist, camPos.x + maxDist, camPos.y - maxDist, camPos.y + maxDist);
        }
            
        //  Update Focus Point and Light / Shadow Maps //
        focusPoint = camForward * (-camPos.z.SignAdd(-Level.WallDepth) / camForward.z) + camPos;
        
        MapCam.UpdateMapQuads(quads[front? 1 : 2].bounds, quads[0].bounds.Add(quads[3].bounds));
        
        //  Calculate Min Cell  //
        minCell = Cell.ToCellPos((Mathf.Floor((focusPoint.x + Level.CellHalfSize) / Level.CellSize) + cells * -.5f) * Level.CellSize, 
            (Mathf.Round( focusPoint.y / Level.CellSize)                       + cells * -.5f) * Level.CellSize);

        //  Fill Vis Cells  //
        frontCellCheck.Clear();
        backCellCheck.Clear();
       
        FillVisCells(!front);
        FillVisCells(front);
        
        frontBounds = quads[0].bounds.Add(quads[1].bounds);
        backBounds  = quads[2].bounds.Add(quads[3].bounds);

        for (int i = 0; i < cellCount; i++)
        {
            bool frontIsVisible = frontCellCheck[i];
            bool backIsVisible  =  backCellCheck[i];

            Bounds2D b = new Bounds2D();
            if (frontIsVisible || backIsVisible)
            {
                Vector2 cellPos = new Vector2((i % cells + minCell.x) * Level.CellSize + Level.CellHalfSize, 
                    (Mathf.Floor((float)i / cells) + minCell.y) * Level.CellSize + Level.CellHalfSize);
                
                b = new Bounds2D(cellPos).Pad(Level.CellHalfSize);
            }
            
            frontVis[i] = frontIsVisible? frontBounds.Contains(b) ? CellVis.All : CellVis.Some : CellVis.None;
            backVis[i]  = backIsVisible?   backBounds.Contains(b) ? CellVis.All : CellVis.Some : CellVis.None;
        }
    }
    
    
    private void FillVisCells(bool frontQuads)
    {
        Quad far     = quads[frontQuads ? 0 : 3];
        Quad surface = quads[frontQuads ? 1 : 2];
        
        //  Extend frontside Surface Quad into ShadowCast Direction  //
        if (frontQuads && front)
        {
            //  Ready Shadow Quad  //
            for (int i = 0; i < 4; i++)
                shadowQuad[i] = surface[i];
            
            //  Extend Sides  //
            Vector2 shadowDir = -LightingSet.ShadowDir.SetZ(0).normalized;
            for (int i = 0; i < 4; i++)
            {
                Vector2 p1 = shadowQuad[i];
                Vector2 p2 = shadowQuad[i.IndexUp(4)];

                Vector2 dir = (p2 - p1).Rot90().normalized;
                float dot = Mathf.Clamp01(Vector2.Dot(shadowDir, dir));
                dir = dir.SetLength(3 * dot);
                
                wrapLines[i] = new Line(p1, p2).Move(dir);
            }
            
            //  Update Surface Quad  //
            for (int i = 0; i < 4; i++)
            {
                Vector2 intersection;
                if (wrapLines[i.IndexDown(4)].Contact(wrapLines[i], out intersection, true))
                    surface[i] = intersection;
            }
            
            surface.CalcBounds();
        }
        
        hullCount = ConvexHull.GetHull(far, surface, hull);
        

        //  Use Hull Points to mark crossed Cells as visible  //
        for (int i = 0; i < 8; i++)
        {
            min[i] = int.MaxValue;
            max[i] = int.MinValue;
        }
    
        for (int i = 0; i < hullCount; i++)
        {
            const float multi = 1f / Level.CellSize;
            int count;
            Vector2Int[] cellPositions = SuperCover.GetLineCells(hull[i] * multi, hull[(i + 1) % hullCount] * multi, out count);
            for (int c = 0; c < count; c++)
            {
                Vector2Int cellPos = cellPositions[c];
                int row = cellPos.y -minCell.y;
                if (row >= 0 && row < cells)
                {
                    int x = cellPos.x - minCell.x;
                    min[row] = Mathf.Min(min[row], x);
                    max[row] = Mathf.Max(max[row], x);
                }
            } 
        }

        bool[] vis = frontQuads ? frontCellCheck : backCellCheck;
        for (int y = 0; y < cells; y++)
        {
            int start = min[y], end = max[y] + 1;

            if (start < cells || end > 0)
            {
                start = Mathf.Max(0, start);
                end   = Mathf.Min(cells, end);

                for (int x = start; x < end; x++)
                    vis[y * cells + x] = true;
            }
        }
    }

    /// <summary> Used by MoveCam for Warp and By Spinner for visibility </summary>
    public bool InFrustum(Bounds2D checkBounds, bool front)
    {
        if (!isValid)
            return false;
            
        if(front && frontBounds.Intersects(checkBounds))
            for (int i = 0; i < 2; i++)
                if (quads[i].Intersects(checkBounds))
                    return true;
           
        if(!front && backBounds.Intersects(checkBounds))
            for (int i = 2; i < 4; i++)
                if (quads[i].Intersects(checkBounds))
                    return true;

        return false;
    }


    public void SetCellVis(Level.FrustumCell frustumCell)
    {
        int cellIndex = GetCameraCellIndex(frustumCell.cellPos);
        frustumCell.frontVis = frontVis[cellIndex];
        frustumCell.backVis  =  backVis[cellIndex];
    }


    private int GetCameraCellIndex(Vector2Int cellPos)
    {
        return (cellPos.y - minCell.y) * cells + (cellPos.x - minCell.x);
    }
    
    
    public void DebugDraw()
    {
        //  Prep Colors  //
        if(colors == null)
            colors = new []
            {
                COLOR.red.tomato.A(.5f),
                Color.Lerp(COLOR.yellow.fresh, COLOR.red.tomato, .5f).A(.5f),
                COLOR.purple.maroon.A(.5f),
                COLOR.purple.orchid.A(.5f)
            };
        
        
        // Hull  //
        for (int i = 0; i < hullCount; i++)
            DRAW.Vector(hull[i], hull[(i + 1) % hullCount] - hull[i]).SetColor(COLOR.blue.cornflower).SetDepth(Z.W05);

        
        for (int i = 0; i < 4; i++)
        {
            DRAW.Shape shape = DRAW.Shape.Get(5);

            for (int e = 0; e < 5; e++)
                shape.Set(e, quads[i][e % 4]);
            
            shape.SetColor(colors[i]).SetDepth(quadDepths[i].SignAdd(.1f));
        }
        
        frontBounds.Draw().SetColor(Color.white).SetDepth(-(Level.WallDepth + Level.PlaneOffset));
        backBounds.Draw().SetColor(Color.white).SetDepth(  Level.WallDepth + Level.PlaneOffset);
        
        
        

        /*for (int i = 0; i < 4; i++)
        {
            DRAW.Vector(quads[0][i].V3(quadDepths[0].SignAdd(.1f)), quads[1][i].V3(quadDepths[1].SignAdd(.1f)) - quads[0][i].V3(quadDepths[0].SignAdd(.1f))).SetColor(colors[0]);
            DRAW.Vector(quads[2][i].V3(quadDepths[2].SignAdd(.1f)), quads[3][i].V3(quadDepths[3].SignAdd(.1f)) - quads[2][i].V3(quadDepths[2].SignAdd(.1f))).SetColor(colors[3]);
        }*/

        
        for (int i = 0; i < 4; i++)
            DRAW.GapVector(quads[front? 0 : 3][i].V3(quadDepths[front? 0 : 3].SignAdd(.1f)), cam.transform.position - quads[front? 0 : 3][i].V3(quadDepths[front? 0 : 3].SignAdd(.1f)), 5).SetColor(colors[front? 1 : 2]);
       
        
        DRAW.GapVector(focusPoint, cam.transform.position - focusPoint, 20).SetColor(colors[front? 0 : 3]);

        
    }
}