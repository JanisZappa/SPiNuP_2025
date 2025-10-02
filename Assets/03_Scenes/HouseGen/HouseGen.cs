using System.Collections.Generic;
using GeoMath;
using House;
using UnityEngine;
using PadRules = HouseZone.PadRules;


public partial class HouseGen : Singleton<HouseGen>
{
    public Vector2 size;

    [Space] 
    [Range(0, 1000000)]
    public int seed;
    public bool randomSeed;
    
    [Space]
    public float areaMulti = 1;
	
    [HideInInspector] public int xCells, yCells, cellCount;
    [HideInInspector] public Vector2 cellMin, cell;
    
    [HideInInspector] public CornerIDs[] frontIDs, backIDs;
    [HideInInspector] public System.Random random;
    
    [HideInInspector] public Cell[] cells;
    
    [HideInInspector] public Bounds2D houseBounds;
    
    [Header("Rect Info")]
    [SerializeField]    private int allRectCount;
    [SerializeField]    private int useRectCount;
    
    public const float cellSize      = 2.5f, 
                       cellSizeMulti = 1f / cellSize;
    
    public const int totalRects = 100000;
    
    
//  Modules  //
    private HouseRules       rules;
    private HouseOcclusion   occlusion;
    private HousePiecePlacer piecer;
    private HouseMesher      mesher;
	
    private List<RectZone> divideZones;
    
    private RectZone[] allRectZones, useRectZones;
    
    public List<Vector2> badFrontCorners, badBackCorners;
    
    public static bool ForcedUpdate;
    
    public static int Seed => Inst? Inst.seed : 0;


    private void Awake()
    {
        Palette.Load();
		
    //  Snap Size To CellSize  //
        size = new Vector2(Mathf.Round(size.x * cellSizeMulti) * cellSize, 
                           Mathf.Round(size.y * cellSizeMulti) * cellSize);
		
        xCells = Mathf.FloorToInt(size.x * cellSizeMulti) - 1;
        yCells = Mathf.FloorToInt(size.y * cellSizeMulti) - 1;
        cellCount = xCells * yCells;
		
        cellMin = new Vector2(size.x * -.5f + cellSize, cellSize - cellSize);
        cell    = new Vector2(cellSize, cellSize);
       
        frontIDs = new CornerIDs[cellCount];
        backIDs  = new CornerIDs[cellCount];
        
        allRectZones = new RectZone[totalRects];
        useRectZones = new RectZone[totalRects];
        
        cells = new Cell[cellCount];
		
        divideZones = new List<RectZone>(totalRects);

    //  Get Modules  //
        rules     = GetComponent<HouseRules>();
        occlusion = GetComponent<HouseOcclusion>();
        piecer    = GetComponent<HousePiecePlacer>();
        mesher    = GetComponent<HouseMesher>();
        
        
        badFrontCorners = new List<Vector2>(100);
        badBackCorners  = new List<Vector2>(100);
        
        
    //  Init Modules  //
            rules.Init();
        occlusion.Init();
           piecer.Init();
    }
    
    
    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Minus))
            Generate();
        
        if(Input.GetKeyDown(KeyCode.Period))
            randomSeed = !randomSeed;
    }
	
    
    public static void GameStart(bool forceUpdate = false)
    {
        ForcedUpdate = forceUpdate;
            
        if (Inst && Inst.gameObject.activeInHierarchy)
            Inst.Generate();
    }
	
    
    public static void GameLoad()
    {
        if(Inst && Inst.gameObject.activeInHierarchy && Inst.mesher)
            Inst.mesher.meshes.Setup(Inst.transform);
    }


    private void Generate()
    {
        CreateRects();
            
        cells.Clear();
        
        rules.DetectBlockedCells();
        
        DetectOccupiedCells();
        
        rules.UpdateElementDepths();
        
        piecer.PieceItTogether();
        
        occlusion.CalculateOcclusion();
        
        MakePillars();
		
        if(mesher)
            mesher.PlaceMeshes();
        
        
    //  Log  //
        if (!Application.isMobilePlatform && false)
        {
            Debug.Log("--------------");
            
            float combined = 0;
            
            Debug.Log(Timer.Log(Timer.Entry.CreateRects,             ref combined));
            Debug.Log(Timer.Log(Timer.Entry.DetectOccupiedCells,     ref combined));
            Debug.Log(Timer.Log(Timer.Entry.FindUsedCells,           ref combined));
            Debug.Log(Timer.Log(Timer.Entry.Piecer,                  ref combined));
            Debug.Log(Timer.Log(Timer.Entry.Occlusion,               ref combined));
            Debug.Log(Timer.Log(Timer.Entry.TellLevelAboutOcclusion, ref combined));
            //Debug.Log(Timer.Log(Timer.Entry.MakePillars,             ref combined));
            
            Debug.Log("All: " + combined.ToString("F4") + " | FPS: " + Mathf.FloorToInt(1f / combined));
            
            if(!Application.isEditor)
                DesktopTxt.Write("GenSpeed", new []{ "All: " + combined.ToString("F4") + " | FPS: " + Mathf.FloorToInt(1f / combined) });
        }
    }
    
    
    private void CreateRects()
    {
    //  Init  //
        Timer.Start(Timer.Entry.CreateRects);
        
        if(randomSeed)
            seed = Random.Range(0, 1000001);
        
        random = new System.Random(seed);
        rules.GetElementBounds();
        rules.SetupZones(cellMin, cellMin + new Vector2(xCells, yCells) * cellSize);

        allRectCount = 0;

        {
            Vector2Int m = Vector2Int.zero;
            Vector2Int s = new Vector2Int(xCells, yCells);
            
            divideZones.Add(rules.GetRectZone(new RectZone(m, s, 0)));
        }
        
		
        int checkRects = 1;
		
        while (checkRects > 0)
        {
            RectZone rectZone = divideZones.GetRemoveAt(random.Range(0, checkRects));        
            
            Vector2Int min = rectZone.min;
            Vector2Int siz = rectZone.size;
				
            checkRects--;

            float xMulti = rules.GetZone(rectZone.ArrayZone()).xShift;
            
            
            RectZone one, two;
            if (siz.x * xMulti > siz.y)
            {
                int block = random.Range(3, siz.x - 2);
                Vector2Int s1 = new Vector2Int(        block, siz.y);
                Vector2Int s2 = new Vector2Int(siz.x - block + 1, siz.y);
			
                one = new RectZone(min, s1, 0);
                two = new RectZone(min.Add(s1.x - 1, 0), s2, 0);
            }
            else
            {
                int block = random.Range(3, siz.y - 2);
                Vector2Int s1 = new Vector2Int(siz.x,         block);
                Vector2Int s2 = new Vector2Int(siz.x, siz.y - block + 1);
			
                one = new RectZone(min, s1, 0);
                two = new RectZone(min.Add(0, s1.y - 1), s2, 0);
            }


        //  If either of the  division rects is too narrow just keep the original rect  //
            if (one.size.x <= 2 || one.size.y <= 2 ||
                two.size.x <= 2 || two.size.y <= 2)
            {
                allRectZones[allRectCount++] = rectZone;
            } 
            else
                for (int i = 0; i < 2; i++)
                {
                    RectZone rect = i == 0 ? one : two;

                    RectZone splitZone = rules.GetRectZone(rect);

                    float areaThresh = rules.GetZone(splitZone.ArrayZone()).minArea * areaMulti;
                    if (rect.Area >= areaThresh)
                    {
                        divideZones.Add(splitZone);
                        checkRects++;
                    }
                    else
                        allRectZones[allRectCount++] = splitZone;
                }
        }
        

        Timer.End(Timer.Entry.CreateRects);
    }

    
    private void DetectOccupiedCells()
    {	
        Timer.Start(Timer.Entry.DetectOccupiedCells);
		
    //  Which Rects Are Part Of Building  //
        useRectCount = 0;
        
        Vector2Int boundMin = new( 100000,  100000),
                   boundMax = new(-100000, -100000);
        
    //  ...   //
        for (int i = 0; i < allRectCount; i++)
        {
            RectZone rectZone = allRectZones[i];
            
            Vector2Int min = rectZone.min, steps = rectZone.size;
            
            min   =   min.Add(-2, -2);
            steps = steps.Add( 4,  4);

            if (!rules.InBroadBound(min, min + steps.Add(-1, -1)))
            {
                allRectZones[i] = rectZone.SetZone(0);
                continue;
            }
            
            for (int x = 0; x < steps.x; x++)
            for (int y = 0; y < steps.y; y++)
            {
                Vector2Int checkPos = min.Add(x, y);
                
                if(!ValidCellPos(checkPos))
                    continue;
                
                int index = CellPosToIndex(checkPos);

                if (cells[index].Any(Cell.FrontBlock | Cell.BackBlock))
                    goto PartOfHouse;
            }
            
            allRectZones[i] = rectZone.SetZone(0);
            continue;
            
            PartOfHouse:
            useRectZones[useRectCount++] = rectZone;
            boundMin = Vector2Int.Min(boundMin, rectZone.min);
            boundMax = Vector2Int.Max(boundMax, rectZone.min + rectZone.size.Add(-1, -1));
        }

        houseBounds = IndexToCellBounds(CellPosToIndex(boundMin)).Add(IndexToCellBounds(CellPosToIndex(boundMax)));
        
        frontIDs.Clear();
        
        for (int i = 0; i < useRectCount; i++)
            RectZoneToHouse(useRectZones[i], true);
        
        
    //  Detect and Fix Bad Corners  //
        int cornerFixes = 0;
        int searchOffset = 0;
        while (true)
        {
            for (int i = 0; i < cellCount; i++)
            {
                int index = (searchOffset + i) % cellCount;
                
                if (cells[index].Is(Cell.House))
                {
                    CornerIDs cIDs = frontIDs[index];
                    if (!cIDs.BadCorners)
                        continue;

                    int fillZone = cIDs.RandomZonePick(random);

                    Vector2Int badPos = IndexToCellPos(index);

                    int offset = random.Range(0, 3);

                    for (int dir = 0; dir < 4; dir++)
                    {
                        Vector2Int checkPos = badPos + HouseHelp.DiagonalDir(dir + offset);

                        for (int e = 0; e < allRectCount; e++)
                        {
                            RectZone rZ = allRectZones[e];

                            if (rZ.Contains(checkPos) && rZ.Zone() == 0)
                            {
                                rZ = rZ.SetZone(fillZone);

                                allRectZones[e] = rZ;

                                useRectZones[useRectCount++] = rZ;
                                RectZoneToHouse(rZ, true);
                                
                                searchOffset = index + 1;

                                goto MoreToDo;
                            }
                        }
                    }
                }
            }

            break;
            
            MoreToDo:
            cornerFixes++;

            if (cornerFixes == 1000)
            {
                Debug.Log("Canceling CornerFixes");
                break;
            }  
        }
        
        
    //  Detect and Fix Bad Zone Corners  //
        int zoneCornerFixes = 0;
        searchOffset = 0;
        while (true)
        {
            for (int i = 0; i < cellCount; i++)
            {
                int index = (searchOffset + i) % cellCount;
                
                if (cells[index].Is(Cell.House))
                {
                    CornerIDs cIDs = frontIDs[index];
                    if (!cIDs.BadZoneCorners)
                        continue;

                    Vector2Int badPos = IndexToCellPos(index);

                    int offset = random.Range(0, 3);

                    for (int dir = 0; dir < 4; dir++)
                    {
                        Vector2Int checkPos = badPos + HouseHelp.DiagonalDir(dir + offset);

                        for (int e = 0; e < useRectCount; e++)
                        {
                            RectZone rZ = useRectZones[e];

                            if (rZ.Contains(checkPos))
                            {
                                byte setZone;
                                while (true)
                                {
                                    setZone = cIDs.RandomZonePick(random);
                                    if (setZone != rZ.Zone())
                                        break;
                                }

                                rZ = rZ.SetZone(setZone);

                                useRectZones[e] = rZ;
                                RectZoneToHouse(rZ, true);

                                searchOffset = index + 1;
                                
                                goto MoreToDo;
                            }
                        }
                    }
                }
            }

            break;
            
            MoreToDo:
            zoneCornerFixes++;
            
            if(zoneCornerFixes == 1000)
            {
                Debug.Log("Canceling ZoneCornerFixes");
                break;
            }  
        }

        
    //  Fix Bottom  //
    //  TODO? Check if this is needed ... 
        for (int x = 0; x < xCells; x++)
            if (cells[x].Is(Cell.House))
                frontIDs[x] = frontIDs[x].ProjectDir();
        
        
    //  Copy To Back  //
        backIDs.CopyFrom(frontIDs);
            
        DetectSeams();
        
        
    //  Which Rects could be Extruded  //
        for (int i = 0; i < useRectCount; i++)
        {
            RectZone rectZone = useRectZones[i];

            Vector2Int min = rectZone.min, steps = rectZone.size;

            PadRules pad = rules.GetZone(rectZone.ArrayZone()).extrudePadRules;

            Vector2Int padSteps = steps + new Vector2Int( pad.sides * 2, pad.top + pad.bottom);
            Vector2Int padMin   =   min + new Vector2Int(-pad.sides,    -pad.bottom);
            Vector2Int padMaxStart = new Vector2Int(padSteps.x - 1 - pad.sides, padSteps.y - 1 - pad.top);
            
            
            bool front = true, back = true;
            for (int y = 0; y < padSteps.y; y++)
            for (int x = 0; x < padSteps.x; x++)
            {
            //  Padded Edge  //
                if (!(x > pad.sides  && x < padMaxStart.x && y > pad.bottom && y < padMaxStart.y))
                {
                    Vector2Int realPos = padMin.Add(x, y);

                    if (!ValidExtrudeCellPos(realPos))
                    {
                        front = false;
                        back  = false;
                        goto DoneChecking;
                    }
                    
                    if(!ValidCellPos(realPos))
                        continue;

                    int  index = CellPosToIndex(realPos);
                    Cell c     = cells[index];

                    if (c.Any(Cell.FrontSeam | Cell.BackSeam))
                    {
                        front = false;
                        back  = false;
                        goto DoneChecking;
                    }

                    if (front && c.Is(Cell.FrontBlock))
                        front = false;

                    if (back && c.Is(Cell.BackBlock))
                        back = false;
                }

                if (!front && !back)
                    goto DoneChecking;
            }


            DoneChecking:

            if (!front && !back)
                continue;

            //  
            if (front != back)
            {
            //  Cant be a one sided rect on the House Edge  //
                for (int y = 0; y < steps.y; y++)
                for (int x = 0; x < steps.x; x++)
            //  Edge  //
                    if (x == 0 || x == steps.x - 1 ||
                        y == 0 || y == steps.y - 1)
                    {
                        Vector2Int realPos = min.Add(x, y);

                        if (realPos.y == 0)
                            continue;

                        int index = CellPosToIndex(realPos);

                        if (frontIDs[index].IsOnEdge)
                            goto NotCool;
                    }
            }
            
        //  Can be extruded  //
            if (front)
            {
                rectZone = rectZone.Extrude(true);
                RectZoneToHouse(rectZone, true);
            }

            if (back)
            {
                rectZone = rectZone.Extrude(false);
                RectZoneToHouse(rectZone, false);
            }  
            
            useRectZones[i] = rectZone;
            
            NotCool: ;
        }
        
        
    //  Detect and Fix Bad Corners  //
        badFrontCorners.Clear();
         badBackCorners.Clear();
         
        int extrudeFixes = 0;
        while (true)
        {
            for (int i = 0; i < cellCount; i++)
                if (cells[i].Is(Cell.House))
                {
                    bool front = frontIDs[i].BadZoneCorners, 
                         back  = backIDs[i].BadZoneCorners;
                    
                    if (!front && !back)
                        continue;

                    Vector2Int badPos = IndexToCellPos(i);
                    Vector2    bP     = IndexToCellCenter(i);

                    if (front)
                        badFrontCorners.Add(bP);
                    
                    if (back)
                        badBackCorners.Add(bP);
                    
                    //Debug.Log((front? "F" : " ") + ":" + (back? "B" : " "));
                    
                    
                    int offset = random.Range(0, 3);

                    for (int dir = 0; dir < 4; dir++)
                    {
                        int     dirPick  = (dir + offset) % 4;
                        
                        Vector2Int checkPos;
                        switch (dirPick)
                        {
                            default: checkPos = badPos.Add(-1, 1); break;
                            case 1 : checkPos = badPos.Add( 1, 1); break;
                            case 2 : checkPos = badPos.Add( 1, -1); break;
                            case 3 : checkPos = badPos.Add( -1, -1); break;
                        }

                        for (int e = 0; e < useRectCount; e++)
                        {
                            RectZone rZ = useRectZones[e];
                            
                            if (rZ.Contains(checkPos))
                            {
                                if (front && rZ.FrontExtruded)
                                {
                                    rZ = rZ.Level(true);
                                    RectZoneToHouse(rZ, true);
                                }

                                if (back && rZ.BackExtruded)
                                {
                                    rZ = rZ.Level(false);
                                    RectZoneToHouse(rZ, false);
                                }
                                
                                
                                useRectZones[e] = rZ;
                                
                                goto MoreToDo;
                            }
                        }
                    }
                }
            
            break;
            
            
            MoreToDo:

            extrudeFixes++;
            
            //Debug.Log("Extrude Fix " + extrudeFixes);
            
            if(extrudeFixes == 100)
            {
                Debug.Log("Canceling ExtrudeFixes");
                break;
            }  
        }

        int count = useRectCount;
        for (int i = 0; i < count; i++)
        {
            RectZone rZ = useRectZones[i];

            if (rZ.BothExtruded)
            {
                Vector2Int min = rZ.min, steps = rZ.size;
                
                int offset = random.Range(0, 4);

                for (int d = 0; d < 4; d++)
                {
                    int dir = offset + d;
                    int stepsCount = dir % 2 == 0? steps.x : steps.y;
                    int growSteps  = 0;

                    for (int e = 0; e < 3; e++)
                    {
                        for (int step = 0; step < stepsCount; step++)
                        {
                            Vector2Int checkPos = dir switch
                            {
                                1 => min.Add(steps.x + e, step),
                                2 => min.Add(step, -1 - e),
                                3 => min.Add(-1 - e, step),
                                _ => min.Add(step, steps.y + e)
                            };

                            if(checkPos.y < 0)
                                goto Done;
                        
                            int index = CellPosToIndex(checkPos);
                        
                            if(cells[index].Is(Cell.House))
                                goto Done;
                        }
                    
                        growSteps++;
                    }
                
                    Done:

                    growSteps--;

                    if (growSteps > 0)
                    {
                        growSteps = random.Range(1, growSteps + 1);
                    
                        RectZone newZone = rZ.SideMargin(growSteps, dir);
                        useRectZones[useRectCount++] = newZone;
                    
                        RectZoneToHouse(newZone, true);
                        RectZoneToHouse(newZone, false);
                        
                        break;
                    }   
                }
            }
        }
        
        
    //  Fix Bottom  //
        for (int x = 0; x < xCells; x++)
            if (cells[x].Is(Cell.House))
            {
                frontIDs[x] = frontIDs[x].ProjectDir();
                 backIDs[x] =  backIDs[x].ProjectDir();
            }
               
        DetectSeams();
        
			
        Timer.End(Timer.Entry.DetectOccupiedCells);
    }


    private void RectZoneToHouse(RectZone rectZone, bool front)
    {  
        Vector2Int min = rectZone.min, steps = rectZone.size;

        byte zone = rectZone.Zone(front);
        CornerIDs[] cornerIDs = front? frontIDs : backIDs;
        
        
        for (int y = 0; y < steps.y; y++)
        for (int x = 0; x < steps.x; x++)
        {
            Vector2Int cellPos = min.Add(x, y);
                
            int index = CellPosToIndex(cellPos);
                
            CornerIDs cIDs = cornerIDs[index];
                
            byte a = x > 0           && y < steps.y - 1? zone : cIDs.a;
            byte b = x < steps.x - 1 && y < steps.y - 1? zone : cIDs.b;
            byte c = x < steps.x - 1 && y > 0?           zone : cIDs.c;
            byte d = x > 0           && y > 0?           zone : cIDs.d;
        
            cornerIDs[index] = new CornerIDs(a, b, c, d);

            cells.Set(Cell.House, index);
        }
    }


    private void DetectSeams()
    {
        for (int i = 0; i < cellCount; i++)
            if (cells[i].Is(Cell.House))
            {
                {
                    if( frontIDs[i].HasZoneChange)
                        cells.Set(Cell.FrontSeam, i);
                    else
                        cells.UnSet(Cell.FrontSeam, i);
                }
                {
                    if(backIDs[i].HasZoneChange)
                        cells.Set(Cell.BackSeam, i);
                    else
                        cells.UnSet(Cell.BackSeam, i);
                }
            }
    }
    
    
    private void MakePillars()
    {
        Timer.Start(Timer.Entry.MakePillars);
        

        if (false)
        {
            for (int y = 0; y < yCells; y++)
            for (int x = 0; x < xCells; x++)
            {
                if ((x + y) % (y % 3 + 3) != 0)
                    continue;
                
                Vector2Int pos = new Vector2Int(x, y);

                int index = CellPosToIndex(pos);

                if (!cells[index].Is(Cell.House) || frontIDs[index].cornerValue != 1100)
                    continue;

                int pillarLength = 0;

                while (true)
                {
                    int checkPillar = pillarLength + 1;
                    Vector2Int checkPos = pos.Add(0, -checkPillar);
                    if (!ValidCellPos(checkPos))
                        break;

                    int checkIndex = CellPosToIndex(checkPos);

                    if (!cells[checkIndex].Is(Cell.House))
                    {
                        pillarLength++;
                    }
                    else
                    {
                        if (frontIDs[checkIndex].cornerValue != 0011)
                            pillarLength = 0;

                        break;
                    }
                }

                if (pillarLength == 0)
                    continue;

                pillarLength += 2;
                for (int i = 0; i < pillarLength; i++)
                {
                    Vector2Int checkPos = pos.Add(0, -i);
                    if(ValidCellPos(checkPos))
                        cells.Set(Cell.Pillar, CellPosToIndex(checkPos));
                }
            }
        }

        Timer.End(Timer.Entry.MakePillars);
    }

    
//  Helper Functions  //
    public Vector2Int IndexToCellPos(int index)
    {
        return new Vector2Int(index % xCells, index / xCells);
    }
    
    
    public int CellPosToIndex(Vector2Int pos)
    {
        return pos.y * xCells + pos.x;
    }

    
    public bool ValidCellPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < xCells && pos.y >= 0 && pos.y < yCells;
    }

    
    public bool ValidExtrudeCellPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < xCells && pos.y < yCells;
    }
    
    
    public void SetAreaBools(Vector2Int root, Vector2Int size, bool[] bools, bool value)
    {
        for (int y = 0; y < size.y; y++)
        for (int x = 0; x < size.x; x++)
            bools[CellPosToIndex(root.Add(x, y))] = value;
    }
    
    
    public void SetAreaCells(Vector2Int root, Vector2Int size, Cell cellInfo)
    {
        for (int y = 0; y < size.y; y++)
        for (int x = 0; x < size.x; x++)
            cells.Set(cellInfo, CellPosToIndex(root.Add(x, y)));
    }
    
    
    public void BoundsToCellArea(Bounds2D rect, out Vector2Int min, out Vector2Int steps)
    {
        min = ((rect.BL - cellMin) * cellSizeMulti).Vector2IntFloor();
        Vector2Int max = ((rect.TR - cellMin) * cellSizeMulti).Vector2IntFloor();
        
        steps = new Vector2Int(max.x - min.x + 1, max.y - min.y + 1);
    }

    
    private Bounds2D CellAreaToBounds(Vector2Int pos, Vector2Int size)
    {
        Vector2 vSize = size;
        Vector2 c     = cellMin + (pos + vSize * .5f) * cellSize;
        Vector2 s     = vSize * cellSize;
        return new Bounds2D(c - s * .5f).Add(c + s * .5f);
    }


    private Vector2 IndexToCellCenter(int index)
    {
        Vector2Int cellPos = IndexToCellPos(index);
        return cellMin + new Vector2(cellPos.x * cellSize + cellSize * .5f, cellPos.y * cellSize + cellSize * .5f);
    }


    public Bounds2D GetRectZoneBounds(RectZone rectZone)
    {
        return new Bounds2D(IndexToCellCenter(CellPosToIndex(rectZone.min))).Add(IndexToCellCenter(CellPosToIndex(rectZone.Max)));
    }


    public Vector2 GetRectZoneCenter(RectZone rectZone)
    {
        Vector2 min = IndexToCellCenter(CellPosToIndex(rectZone.min));
        Vector2 max = IndexToCellCenter(CellPosToIndex(rectZone.Max));
        return (min + max) * .5f;
    }


    public int GetBoundZone(Bounds2D bounds, bool front)
    {
        CornerIDs[] ids = front? frontIDs : backIDs;
        
        int zone = int.MaxValue;

        BoundsToCellArea(bounds, out var min, out var steps);

        for (int y = 0; y < steps.y; y++)
        for (int x = 0; x < steps.x; x++)
            zone = Mathf.Min(zone, ids[CellPosToIndex(min.Add(x, y))].Zone);
        return zone;
    }
    
    
//  DRAW  //
    public Bounds2D IndexToCellBounds(int index)
    {
        Vector2 pos = cellMin + (Vector2)IndexToCellPos(index) * cellSize;
        
        return new Bounds2D(pos).Add(pos + cell);
    }


    public RectZone[] GetUseZones(out int count)
    {
        count = useRectCount;
        return useRectZones;
    }
}
