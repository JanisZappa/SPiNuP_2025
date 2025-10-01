using System.Collections.Generic;
using System.Text;
using GeoMath;
using TMPro;
using UnityEngine;
using House;
using Cell = HouseGen.Cell;

#if UNITY_STANDALONE || UNITY_EDITOR


public class HouseDraw : MonoBehaviour
{      
    public bool draw;
    
	public Color gridColor;
    public Color gridColor2;

    [Space(10)] 
    public TextMeshProUGUI ui;
    public HouseDebugCam cam;

    private readonly StringBuilder uiBuilder = new StringBuilder(10000);

    private HouseGen gen;
    private HouseRules rules;
    private HouseOcclusion occlusion;
    private HousePiecePlacer piecer;
    private HouseMesher mesher;
   

    private bool drawRects = true, drawGrid = true, showBuilding = true, 
                 showZones, showElements, showBlockedCells, showOcclusion, showSeams, showZoneSteps;

    
    private const float cellSize = HouseGen.cellSize;
    private Vector2 center;
    
    
    private void Awake()
    {
        gen       = GetComponent<HouseGen>();
        rules     = GetComponent<HouseRules>();
        piecer    = GetComponent<HousePiecePlacer>();
        occlusion = GetComponent<HouseOcclusion>();
        mesher    = GetComponent<HouseMesher>();
    }


    public void LateUpdate()
    {
        if (gen == null || !draw)
            return;
        
        
        drawGrid         =         drawGrid.KeySwitch(KeyCode.F1);
        showBuilding     =     showBuilding.KeySwitch(KeyCode.F2);
        drawRects        =        drawRects.KeySwitch(KeyCode.F3);
        showZoneSteps    =    showZoneSteps.KeySwitch(KeyCode.F4);
        showElements     =     showElements.KeySwitch(KeyCode.F5);
        showBlockedCells = showBlockedCells.KeySwitch(KeyCode.F6);
        showOcclusion    =    showOcclusion.KeySwitch(KeyCode.F7);
        showSeams        =        showSeams.KeySwitch(KeyCode.F8);
        showZones        =        showZones.KeySwitch(KeyCode.F9);
        
        
    //  Cam Controll  //
        if(Input.GetKeyDown(KeyCode.Mouse2))
            cam.SwitchSide();
        if(Input.GetKeyDown(KeyCode.T))
            cam.FrameBuilding(gen.houseBounds);
        if(Input.GetKeyDown(KeyCode.Z))
            cam.FrameHorizontal(gen.houseBounds);
        if(Input.GetKeyDown(KeyCode.U))
            cam.FrameVertical(gen.houseBounds);
        if (center != gen.houseBounds.Center)
        {
            center = gen.houseBounds.Center;
            cam.FrameBuilding(gen.houseBounds);
        }
        
        cam.UpdatePos();
        
       
        bool front = !cam.back;
        float side = cam.back ? -1 : 1;
        
        int mouseCellIndex = gen.CellPosToIndex(((cam.mousePos - gen.cellMin) * HouseGen.cellSizeMulti).Vector2IntFloor());
        UI_Update(front, mouseCellIndex);
        
        
        bool showMouseCell = mouseCellIndex >= 0 && mouseCellIndex < gen.cellCount;
        if(showMouseCell)
            gen.IndexToCellBounds(mouseCellIndex).Draw().SetColor(Color.black.A(.4f)).SetDepth(6 * -side);
        
        Color grid = cam.back ? gridColor2 : gridColor;
        Color cellColor = Color.Lerp(Color.black, grid, .3f);
        
        if(cam.OrthoSize < 70 && showMouseCell)
            gen.DrawCornerIDs(!cam.back, showZoneSteps, mouseCellIndex);
        
        //gen.DrawHouseCells(side);
        DRAW.Vector(Vector3.right * -1000, Vector3.right * 2000).SetColor(Color.black);
        //gen.DrawTotalBounds();
        
        //piecer.DrawCornerDirection(side);
        //piecer.DrawEdgeDirection(side);
        
    //  Occlusion  //
        if (showOcclusion)
            occlusion.DrawBounds(side);
        
        if (showBlockedCells)
            rules.DrawElementCells(side);
       
        
        
        if (drawRects)
        {
            gen.DrawRects(showBuilding, !cam.back, cam.OrthoSize > 70 && cam.OrthoSize < 140);
            gen.DrawBadCorners(front, side);
        }
        else
        {
            DrawPieces(piecer.fills, cellColor, front, false);
            
            DrawPieces(piecer.edges,   cellColor, front);
            DrawPieces(piecer.corners, cellColor, front);
            
            DrawPieces(piecer.seams,            cellColor, front, false);
            DrawPieces(piecer.seamFrontCorners, cellColor, front, false);
        }
        
        
		
        if(showZones)
            rules.DrawZones(side);

        if(drawGrid)
            DRAW.Grid(new Vector3(0, gen.size.y * .5f), gen.size, gen.xCells + 1, gen.yCells + 1, HouseDebugCam.CamBounds).
                SetColor(grid).SetDepth(2 * side);
        
        if(showSeams)
            piecer.DrawSeams(side);

		
        DrawPillars(cellColor, side);
		
        if (showElements)
            rules.DrawElements(side);
    }


    private void UI_Update(bool front, int cellIndex)
    {
        Color a = COLOR.red.tomato, b = COLOR.green.forest;
        
        HouseGen.CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
        bool inRange = cellIndex >= 0 && cellIndex < gen.cellCount;
        
        uiBuilder.Length = 0;
        uiBuilder.
            AppendLine("F1 : drawGrid".        B_Switch(a, b, drawGrid)).
            AppendLine("F2 : showBuilding".    B_Switch(a, b, showBuilding)).
            AppendLine("F3 : drawRects".       B_Switch(a, b, drawRects)).
            AppendLine("F4 : showZoneSteps".   B_Switch(a, b, showZoneSteps)).
            AppendLine("F5 : showElements".    B_Switch(a, b, showElements)).
            AppendLine("F6 : showBlockedCells".B_Switch(a, b, showBlockedCells)).
            AppendLine("F7 : showOcclusion".   B_Switch(a, b, showOcclusion)).
            AppendLine("F8 : showSeams".       B_Switch(a, b, showSeams)).
            AppendLine("F9 : showZones".       B_Switch(a, b, showZones)).
            NewLine().
            AppendLine("Generate:    Minus".B_Black()).
            AppendLine("Frame Cam:   T".    B_Black()).
            AppendLine("Frame Cam H: Z".    B_Black()).
            AppendLine("Frame Cam V: U".    B_Black()).
            AppendLine("Switch Side: MB 2". B_Black()).
            NewLine().
            Append("MouseCell: ".B_Black()).Append(cellIndex.ToString().B_Black()).NewLine().
            Append("Corners: "  .B_Black()).Append((inRange? cornerIDs[cellIndex].Log() : "-").B_Black()).NewLine().
            Append((inRange? gen.cells[cellIndex].ToString() : "-").B_Black());
        
        ui.text = uiBuilder.ToString();
    }
    
    
    private void DrawPieces(List<HouseGen.Piece> pieces, Color cellColor, bool front, bool cornerColors = true)
    {
        bool placeMeshes = mesher != null && mesher.placeMeshes;
        
        int pieceCount = pieces.Count;
        for (int i = 0; i < pieceCount; i++)
            if(cornerColors)
                pieces[i].DrawCornerColor(cellColor, front, gen, placeMeshes);
            else
                pieces[i].DrawSurfaceColor(cellColor, front, gen, placeMeshes);
    }
    
    
    private void DrawPillars(Color cellColor, float side)
    {
        for (int y = 0; y < gen.yCells; y++)
        for (int x = 0; x < gen.xCells; x++)
        {
            int index = y * gen.xCells + x;
            Vector2 cellPos = gen.cellMin + new Vector2(x * cellSize, y * cellSize);
			
            Cell cell = gen.cells[index];
            if (cell.Is(Cell.Pillar))
            {
                Color pillarColor = y % 2 == 0 ? Color.Lerp(cellColor, COLOR.blue.cornflower, .7f) : Color.Lerp(cellColor, COLOR.turquois.seagreen, .5f);
				
                if(!cell.Is(Cell.House))
                    DRAW.Rectangle(cellPos, new Vector2(cellSize * .35f, cellSize)).SetColor(pillarColor).Fill(.5f, true).SetDepth(-2 * side);
                else
                {
                    if(gen.frontIDs[index].cornerValue == 1100)
                        DRAW.Rectangle(cellPos + new Vector2(0, cellSize * -.25f), new Vector2(cellSize, cellSize * .5f)).SetColor(pillarColor).Fill(.5f, true).SetDepth(-2 * side);
                    else
                        DRAW.Rectangle(cellPos + new Vector2(0, cellSize * +.25f), new Vector2(cellSize, cellSize * .5f)).SetColor(pillarColor).Fill(.5f, true).SetDepth(-2 * side);
                }
            }
        }
    }
}


public partial class HouseGen
{
    public void DrawCornerIDs(bool front, bool zoneSteps, int index)
    {
        Color c = Color.Lerp(Color.white, Color.black, .8f).A(.8f);
        const float offset = cellSize * -.2f;
        CornerIDs[] corners = front? frontIDs : backIDs;
        
        Bounds2D b = IndexToCellBounds(index);

        if (HouseDebugCam.IsVisible(b))
            corners[index].DrawIDs(b.Pad(offset), c, zoneSteps);
    }


    public partial struct CornerIDs
    {
        public void DrawIDs(Bounds2D bounds, Color color, bool showZoneCornerValues = false)
        {
            if (showZoneCornerValues)
            {
                byte4 zones = ZoneCorners;
                
                DRAW.GUI_Text(zones.a.ToString(), bounds.TL, color);
                DRAW.GUI_Text(zones.b.ToString(), bounds.TR, color);
                DRAW.GUI_Text(zones.c.ToString(), bounds.BR, color);
                DRAW.GUI_Text(zones.d.ToString(), bounds.BL, color);
                
                return;
            }
            
            DRAW.GUI_Text(a.ToString(), bounds.TL, color);
            DRAW.GUI_Text(b.ToString(), bounds.TR, color);
            DRAW.GUI_Text(c.ToString(), bounds.BR, color);
            DRAW.GUI_Text(d.ToString(), bounds.BL, color);
        }
    }
    
    
    public void DrawRects(bool showBuilding, bool front, bool zoneIds)
    {
        RectZone[] rectArr = showBuilding ? useRectZones : allRectZones;
        int        count   = showBuilding ? useRectCount : allRectCount;
        
        for (int i = 0; i < count; i++)
        {
            RectZone rectZone = rectArr[i];
            Bounds2D rect = GetRectZoneBounds(rectZone);
            
            if(!HouseDebugCam.IsVisible(rect))
                continue;
				
            Vector2 center = rect.Center;
            float a = .3f + Mathf.PingPong((center.x + center.y) * 33.52113f, .7f);
            
            bool bothSides = rectZone.BothExtruded;
            bool extruded  = front? rectZone.FrontExtruded : rectZone.BackExtruded;
            
            float pad = bothSides || extruded? cellSize * -.25f : 0;
            Color color = rules.GetZoneColor(rectZone.ArrayZone(front));
            if(bothSides)
                color = Color.Lerp(color, Color.white, Mathf.SmoothStep(0, .5f, Mathf.PingPong(Time.realtimeSinceStartup * 10, 1)));
            
            GetRectZoneBounds(rectZone).Pad(pad).Draw().SetColor(color).Fill(a, true);
				
            if(zoneIds)
                DRAW.GUI_Text(rectZone.Zone(front).PrepString(), rect.Center);
        }
    }


    public void DrawBadCorners(bool front, float side)
    {
        List<Vector2> badCorners = front? badFrontCorners : badBackCorners;
        
        int count = badCorners.Count;

        float lerp = Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.realtimeSinceStartup * 5, 1));
        Color c = Color.Lerp(COLOR.yellow.fresh, COLOR.blue.deepsky, lerp);
        float radius = Mathf.Lerp(1, 2, lerp);
    
        for (int i = 0; i < count; i++)
            DRAW.Circle(badCorners[i], radius, 16).SetColor(c).Fill(1, true).SetDepth(-2 * side);
    }
    

    public void DrawHouseCells(float side)
    {
        for (int i = 0; i < cellCount; i++)
            if (cells[i].Is(Cell.House))
            {
                Bounds2D b = IndexToCellBounds(i);

                if (HouseDebugCam.IsVisible(b))
                {
                    b.Draw().SetColor(COLOR.blue.deepsky).Fill(.4f).SetDepth(side);
                    b.Draw().SetDepth(-1 * side);
                }
            }
        
    }
    
    
    private Color GetZoneColor(Vector2Int pos)
    {
        return rules.GetZoneColor(frontIDs[CellPosToIndex(pos)].ArrayZone);
    }
    
    
    public partial struct Piece
    {
        private Color CornerColor(Color cellColor)
        {
            switch (cornerIDs.cornerValue)
            {
                default:    return Color.Lerp(cellColor, COLOR.green.spring, .7f); 
				
                case 0110:
                case 1001:
                case 0011:
                case 1100:
                case 0111:
                case 1011:
                case 1101:
                case 1110:
                case 1000: 
                case 0100:
                case 0010:
                case 0001:  return Color.Lerp(cellColor, COLOR.green.forest,  .7f);
					
                case 1111:  return Color.Lerp(cellColor, COLOR.purple.maroon, .7f);
            }
        }
		
		
        private Color SurfaceColor(Color cellColor, HouseGen gen)
        {
            switch (type)
            {
                default:                return Color.Lerp(cellColor, COLOR.grey.mid,        .7f); 
                case PieceType.Detail:  return Color.Lerp(cellColor, gen.GetZoneColor(pos), .85f);
                case PieceType.Hole:    return Color.Lerp(cellColor, Color.black,           .7f);
                case PieceType.Window:  return Color.Lerp(cellColor, COLOR.blue.cornflower, .7f);	
            }
        }
        
        
        private float SurfacePad
        {
            get
            {
                switch (type)
                {
                    default:              return .2f * -cellSize; 
                    case PieceType.Flat:  return 0;
                }
            }
        }


        public void DrawCornerColor(Color cellColor, bool front, HouseGen gen, bool justText = false)
        {
            DrawIt(CornerColor(cellColor), front, gen, justText);
        }
		
		
        public void DrawSurfaceColor(Color cellColor, bool front, HouseGen gen, bool justText = false)
        {
            DrawIt(SurfaceColor(cellColor, gen), front, gen, justText);
        }


        private void DrawIt(Color color, bool front, HouseGen gen, bool justText)
        {
            if(side != 0 && side != (front? -1 : 1))
                return;
            
            float depth = front? 2 : -2;
            
            Bounds2D b = GetBounds(gen);
            
            if(!HouseDebugCam.IsVisible(b))
                return;
            
            Vector2 center = b.Center;
			
            if(size.x > 1 && size.y > 1)
                DRAW.GUI_Text(sqrArea.PrepString(), center + Vector2.right * .25f, .75f);
            
            if(justText)
                return;
            
        //  Hole Get Special Mesh  //
            switch (type)
            {
                case PieceType.Hole:
                    Vector2 s = (size - new Vector2(.2f, .2f)) * cellSize;
                    Vector2 start = center + new Vector2(s.x * -.5f, 0);
                    DRAW.Shape shape = DRAW.Shape.Get(20);
                    shape.Set(0, start);
                    shape.Set(1, center + new Vector2(s.x * -.5f, s.y *  .5f));
                    shape.Set(2, center + new Vector2(s.x *  .5f, s.y *  .5f));
                    shape.Set(3, center + new Vector2(s.x *  .5f, s.y * -.5f));
                    shape.Set(4, center + new Vector2(s.x * -.5f, s.y * -.5f));
                    shape.Set(5, start);
                    
                    Vector2 pointer = new Vector2(Mathf.Min(s.x, s.y) * -.4f, 0);

                    for (int i = 0; i < 13; i++)
                        shape.Set(6 + i, (Vector3)center + Quaternion.AngleAxis(30 * i, Vector3.forward) * pointer);
                    
                    shape.Set(19, start);
                    shape.Reverse();
                    
                    shape.SetColor(color).TriFill(.5f, true).SetDepth(depth);
                    
                    return;
            }
            
            
            switch (cornerIDs.cornerValue)
            {
                case 0111:    if(sqrArea == 1)    goto Normal;
                              InwardEdge(center,   0, color, depth);    return;
                case 1011:    if(sqrArea == 1)    goto Normal;
                              InwardEdge(center,  90, color, depth);    return;
                case 1101:    if(sqrArea == 1)    goto Normal;
                              InwardEdge(center, 180, color, depth);    return;
                case 1110:    if(sqrArea == 1)    goto Normal;
                              InwardEdge(center, 270, color, depth);    return;
                    
                case 1000:    DrawOutwardsEdge(center, 180, color, depth);    return;
                case 0100:    DrawOutwardsEdge(center, 270, color, depth);    return;
                case 0010:    DrawOutwardsEdge(center,   0, color, depth);    return;
                case 0001:    DrawOutwardsEdge(center,  90, color, depth);    return;
            }
            
            Normal:
            b.Pad(SurfacePad).Draw().SetColor(color).Fill(.5f, true).SetDepth(depth);
        }


        private void DrawOutwardsEdge(Vector3 center, float angle, Color color, float depth)
        {
            Quaternion rot = Quaternion.AngleAxis(-angle, Vector3.forward);
            
            Vector2 s = (size - new Vector2(.4f, .4f)) * cellSize;
            Vector2 start = center + rot * new Vector2(s.x * .5f, s.y * .5f);
            
            DRAW.Shape shape = DRAW.Shape.Get(9);
            
            shape.Set(0, start);
            
            Vector3 pin  = center + rot * new Vector2(s.x * .5f, s.y * -.5f);
            shape.Set(1, pin);
            
            Vector3 curveStart = center + rot * new Vector2(s.x * -.5f, s.y * -.5f);
            shape.Set(2, curveStart);
            
            Vector3 dir   = curveStart - pin;
            
            shape.Set(3, pin + Quaternion.AngleAxis(-15, Vector3.forward) * dir);
            shape.Set(4, pin + Quaternion.AngleAxis(-30, Vector3.forward) * dir);
            shape.Set(5, pin + Quaternion.AngleAxis(-45, Vector3.forward) * dir);
            shape.Set(6, pin + Quaternion.AngleAxis(-60, Vector3.forward) * dir);
            shape.Set(7, pin + Quaternion.AngleAxis(-75, Vector3.forward) * dir);
            
            shape.Set(8, start);
            
            shape.Reverse();
                    
            shape.SetColor(color).TriFill(.5f, true).SetDepth(depth);
        }
        
        
        private void InwardEdge(Vector3 center, float angle, Color color, float depth)
        {
            Quaternion rot = Quaternion.AngleAxis(-angle, Vector3.forward);
            
            Vector2 s = (size - new Vector2(.4f, .4f)) * cellSize;
            Vector2 start = center + rot * new Vector2(s.x * .5f, s.y * .5f);
            
            DRAW.Shape shape = DRAW.Shape.Get(11);
            
            shape.Set(0, start);
            shape.Set(1, center + rot * new Vector2(s.x * .5f, s.y * -.5f));
            
            Vector3 curveStart = center + rot * new Vector2(s.x * -.5f, s.y * -.5f);
            shape.Set(2, curveStart);
            
            Vector3 pin = center + rot * new Vector2(s.x * -.5f, s.y * .5f);
            Vector3 dir = (curveStart - pin).normalized * ((size.x - 1) * cellSize);
                    
            shape.Set(3, pin + dir);
            
            shape.Set(4, pin + Quaternion.AngleAxis(15, Vector3.forward) * dir);
            shape.Set(5, pin + Quaternion.AngleAxis(30, Vector3.forward) * dir);
            shape.Set(6, pin + Quaternion.AngleAxis(45, Vector3.forward) * dir);
            shape.Set(7, pin + Quaternion.AngleAxis(60, Vector3.forward) * dir);
            shape.Set(8, pin + Quaternion.AngleAxis(75, Vector3.forward) * dir);
            shape.Set(9, pin + Quaternion.AngleAxis(90, Vector3.forward) * dir);
            
            shape.Set(10, start);
            
            shape.Reverse();
                    
            shape.SetColor(color).TriFill(.5f, true).SetDepth(depth);
        }
    }
}


public partial class HouseRules
{
    public void DrawElements(float side)
    {
        List<Bounds2D> elementArr = side < 0 ? backElements : frontElements;

        int count = elementArr.Count;
        for (int i = 0; i < count; i++)
        {
            Bounds2D b = elementArr[i];
            
            if(HouseDebugCam.IsVisible(b))
                b.Draw().Fill(.5f).SetDepth(-3 * side);
        }
    }


    public void DrawElementCells(float side)
    {
        Cell blocked = side < 0 ? Cell.BackBlock : Cell.FrontBlock;
        for (int i = 0; i < gen.yCells; i++)
            if (gen.cells[i].Is(blocked))
            {
                Bounds2D b = gen.IndexToCellBounds(i).Pad(cellSize * -.05f);
                if(HouseDebugCam.IsVisible(b))
                    b.Draw().SetColor(COLOR.red.blood).SetDepth(-4 * side);
            }
    }


    public void DrawZones(float side)
    {
        int zoneCount = zonePoints.Length;
        for (int i = 0; i < zoneCount; i++)
        {
            ZonePoint zP = zonePoints[i];
            DRAW.Circle(zP.pos, Mathf.Sqrt(1f / zP.multi), 100).SetColor(GetZoneColor(zP.zoneID)).Fill(.25f).SetDepth(-1 * side);
        }
    }


    public Color GetZoneColor(int zoneID)
    {
        return zones[zoneID].debugColor;
    }
}


public partial class HouseOcclusion
{
    public void DrawBounds(float side)
    {
        for (int i = 0; i < occludedBounds.Count; i++)
        {
            Bounds2D b = occludedBounds[i];
            if(HouseDebugCam.IsVisible(b))
                b.Draw().SetColor(Color.black).SetDepth(-5 * side);
        }

        Color c = Color.Lerp(COLOR.blue.navy, Color.white, .4f);
        for (int i = 0; i < notOccludedBounds.Count; i++)
        {
            Bounds2D b = notOccludedBounds[i];
            if(HouseDebugCam.IsVisible(b))
                b.Draw().SetColor(c).SetDepth(-5 * side);
        }
    }
}


public partial class HousePiecePlacer
{
    private const float cellSize = HouseGen.cellSize;
    
    
    public void DrawSeams(float side)
    {
        bool front = side > 0;
        
        Cell seamValue   = front? Cell.FrontSeam : Cell.BackSeam;
        Cell cornerValue = front? Cell.FrontSeamCorner : Cell.BackSeamCorner;
        
        //HouseGen.byte4[] extrudeSeams = front? gen.frontZones : gen.backZones;
        
        for (int i = 0; i < gen.cellCount; i++)
        {
            Cell cell = gen.cells[i];
            
            bool seam  = cell.Is(seamValue);
            bool xSeam = false; //!extrudeSeams[index].AllTheSame;
            
            if(!seam && !xSeam)
                continue;
            
            Bounds2D b = gen.IndexToCellBounds(i);
            if (!HouseDebugCam.IsVisible(b))
                continue;
            
            if (seam)
            {
                Color c = cell.Is(cornerValue)? COLOR.orange.coral : COLOR.red.hot;
                b.Draw().SetColor(c).Fill(.4f).SetDepth(side);
                b.Draw().SetDepth(-1 * side);
            }
        }
    }
    
    
    public void DrawCornerDirection(float side)
    {
        for (int i = 0; i < gen.cellCount; i++)
            if (gen.cells[i].Is(Cell.Corner))
            {
                Vector2 dir = gen.frontIDs[i].GrowDir();
                DRAW.Arrow(gen.IndexToCellBounds(i).Center, dir * cellSize * .4f, cellSize * .2f).SetColor(COLOR.turquois.bright).Fill(1).SetDepth(side * -10);
            }
    }
    
    
    public void DrawEdgeDirection(float side)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            HouseGen.Piece p = edges[i];
            Vector2 dir = edges[i].cornerIDs.GrowDir();
            DRAW.Arrow(p.GetBounds(gen).Center, dir * cellSize * 1.4f, cellSize * .2f).SetColor(COLOR.turquois.bright).Fill(1).SetDepth(side * -10);
        }
       
    }
}

#endif