using System.Collections.Generic;
using UnityEngine;
using House;

using Piece     = HouseGen.Piece;
using CornerIDs = HouseGen.CornerIDs;
using PieceType = HouseGen.PieceType;
using Cell      = HouseGen.Cell;


public partial class HousePiecePlacer : MonoBehaviour 
{
    public const int investigate = 0; //30387;
    
	public readonly List<Piece> edges   = new List<Piece>(10000),
		                        corners = new List<Piece>(10000),
		                        fills   = new List<Piece>(10000);
	
    public readonly List<Piece> seams            = new List<Piece>(10000),
                                seamFrontCorners = new List<Piece>(10000),
                                seamBackCorners  = new List<Piece>(10000);

    private bool[] fillList;
    private int[] flush, justBlocked;

    private List<int> frontUsed, backUsed;
    
    private bool[] connectionCheck;
    private int[] connectionFrontier, connectionInput;
    private int frontierCount, inputCount;
    
    public List<Vector3Int> unused;
    
    private HouseGen   gen;
    private HouseRules rules;
    
    private readonly OrderRandomizer randomizer = new OrderRandomizer(10000);
    
    private const int maxSideLength = 7;
    
    
    public void Init()
    {
        gen   = GetComponent<HouseGen>();
        rules = GetComponent<HouseRules>();
        
        frontUsed          = new List<int>(gen.cellCount);
        backUsed           = new List<int>(gen.cellCount);
        
        connectionCheck    = new bool[gen.cellCount];
        connectionFrontier = new int[gen.cellCount];
        connectionInput    = new int[gen.cellCount];
        
        flush       = new int[gen.cellCount];
        justBlocked = new int[gen.cellCount];
        
        fillList     = new bool[gen.cellCount];
        
        
        unused = new List<Vector3Int>();

        for (int i = 0; i < 6; i++)
        {
            int z = 1 + i;

            for (int x = 0; x < maxSideLength; x++)
            for (int y = 0; y < maxSideLength; y++)
                unused.Add(new Vector3Int(z, x + 1, y + 1));
        }
    }

    
    public void PieceItTogether()
    {
        Timer.Start(Timer.Entry.Piecer);

        FindUsed();
        
        seamFrontCorners.Clear();
        
        CollectEdges();
        
        CollectSeams();
        
        CollectFills();
        
        Analyze();
        
        Timer.End(Timer.Entry.Piecer);
    }


    private void FindUsed()
    {
        Timer.Start(Timer.Entry.FindUsedCells);
        
        frontUsed.Clear();
        for (int i = 0; i < gen.cellCount; i++)
            if (gen.cells[i].Is(Cell.House))
                frontUsed.Add(i);
        
        
        frontierCount = 0;
        inputCount = 0;
        connectionCheck.Clear();
        for (int i = 0; i < gen.cellCount; i++)
            if (gen.cells[i].Is(Cell.House) && !gen.frontIDs[i].IsOnEdge)
            {
                connectionCheck[i] = true;
                connectionInput[inputCount++] = i;
            }
        

        int islands = -1;
        while (inputCount > 0)
        {
            if (frontierCount == 0)
            {
                int index = -1;

                while (true)
                {
                    inputCount--;
                    
                    if(inputCount == -1)
                        break;
                        
                    index = connectionInput[inputCount];
                    if (connectionCheck[index])
                    {
                        connectionCheck[index] = false;
                        break;
                    }
                }

                if (index != -1)
                {
                    connectionFrontier[frontierCount++] = index;
                    islands++;
                }
                else
                    break;
            }

            while (frontierCount > 0)
            {
                frontierCount--;
                int frontier = connectionFrontier[frontierCount];
                
                Vector2Int pos = gen.IndexToCellPos(frontier);

                for (int x = -1; x < 2; x++)
                for (int y = -1; y < 2; y++)
                {
                    if(x == 0 && y == 0)
                        continue;
                    
                    Vector2Int checkPos = pos.Add(x, y);
                    
                    if(!gen.ValidCellPos(checkPos))
                        continue;
                    
                    int index = gen.CellPosToIndex(checkPos);

                    if (connectionCheck[index])
                    {
                        connectionCheck[index] = false;
                        connectionFrontier[frontierCount++] = index;
                    }   
                }
            }
        }
    
        if(islands > 1)
            Debug.Log(islands + " Islands");
        
        Timer.End(Timer.Entry.FindUsedCells);
    }
    

    private void CollectEdges()
    {
    //  Corners  //
        corners.Clear();

        int usedCellCount = frontUsed.Count;
        for (int i = 0; i < usedCellCount; i++)
        {
            int index = frontUsed[i];
            
            Vector2Int pos = gen.IndexToCellPos(index);
			
            CornerIDs corner = gen.frontIDs[index];

            if (corner.IsEdgeCorner)
            {
                /*if(index == 22276)
                    Debug.Log(index.ToString().B_Purple());*/
                
                corners.Add(new Piece(pos, Vector2Int.one, corner, PieceType.Detail, 0));
                gen.cells.Set(Cell.Corner, frontUsed[i]);
            }
        }
        
        CellListRemoval(frontUsed, Cell.Corner);
        
        int cornersCount = corners.Count;

        
    //  Edges  //
        edges.Clear();
        int edgeCount = CreateEdges(corners, edges, false);

        
    //  Remove "Used" Cells that are now Edges  //
        CellListRemoval(frontUsed, Cell.Edge);
        
        
    //  Free Seam Corners again //
        for (int i = 0; i < cornersCount; i++)
        {
            Piece piece = corners[i];
            int index = gen.CellPosToIndex(piece.pos);
            if (gen.cells[index].Any(Cell.FrontSeam | Cell.BackSeam))
            {
                corners.RemoveAt(i);
                i--;
                cornersCount--;
                
                seamFrontCorners.Add(piece);
            }
        }
        
            
    //  Try to grow Corners  //
        {
            for (int growStep = 0; growStep < 3; growStep++)
            {
                randomizer.Randomize(cornersCount, gen.random);

                for (int i = 0; i < cornersCount; i++)
                    if (gen.random.Chance(4, 6))
                    {
                        int index = randomizer[i];
                        corners[index] = GrowCorner(corners[index]);
                    }


            //  Remove "Used" Cells that are now Bigger Corners  //
                CellListRemoval(frontUsed, Cell.Corner);

                for (int i = 0; i < edgeCount; i++)
                {
                    Piece edge = edges[i];

                    Piece trimmed = TrimmEdgeIfInCorner(edge);
                    
                    if (trimmed.sqrArea == 0)
                    {
                        edges.RemoveAt(i);
                        i--;
                        edgeCount--;
                    }
                    else
                        edges[i] = trimmed;
                }
            }
        }
        
		
    //  Divide Edges if too long  //
         EdgeDivide(edges, 6);
         edgeCount = edges.Count;
        
        
    //  Divide Edges if on Seam  //
        for (int i = 0; i < edgeCount; i++)
        {
            Piece edge = edges[i];
            
            int length = edge.sqrArea;

            if(length == 1)
                continue;

            if (edge.size.x > edge.size.y)
            {
                for (int j = 0; j < length; j++)
                {
                    Vector2Int pos = edge.pos.Add(j, 0);
                    int index = gen.CellPosToIndex(pos);
                    
                    CornerIDs cornerIDS = gen.frontIDs[index];
                    if (cornerIDS.HasZoneChange)
                    {
                        IndexDebug(index, "Found Seam on Edge");
                        
                        edges.RemoveAt(i);
                        i--;
                        edgeCount--;

                        if (j > 0)
                        {
                            edges.Add(new Piece(edge.pos, edge.size.Clamp(j), edge.cornerIDs, PieceType.Detail, 0));
                            edgeCount++;
                        }

                        seamFrontCorners.Add(new Piece(pos, new Vector2Int(1, 1), cornerIDS, PieceType.Detail, 0));

                        if (j < length - 1)
                        {
                            edges.Add(new Piece(pos.Add(1, 0), edge.size.Clamp(length - (j + 1)), edge.cornerIDs, PieceType.Detail, 0));
                            edgeCount++;
                        }
                        
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < length; j++)
                {
                    Vector2Int pos = edge.pos.Add(0, j);
                    int index = gen.CellPosToIndex(pos);
                    
                    CornerIDs cornerIDS = gen.frontIDs[index];
                    if (cornerIDS.HasZoneChange)
                    {
                        IndexDebug(index, "Found Seam on Edge");
                        
                        edges.RemoveAt(i);
                        i--;
                        edgeCount--;

                        if (j > 0)
                        {
                            edges.Add(new Piece(edge.pos, edge.size.Clamp(j), edge.cornerIDs, PieceType.Detail, 0));
                            edgeCount++;
                        }

                        seamFrontCorners.Add(new Piece(pos, new Vector2Int(1, 1), cornerIDS, PieceType.Detail, 0));

                        if (j < length - 1)
                        {
                            edges.Add(new Piece(pos.Add(0, 1), edge.size.Clamp(length - (j + 1)), edge.cornerIDs, PieceType.Detail, 0));
                            edgeCount++;
                        }
                        
                        break;
                    }
                }
            }
        }
     
     
    //  Try to Grow Edges  //
        for (int growStep = 0; growStep < 2; growStep++)
        {
            randomizer.Randomize(edgeCount, gen.random);
    
            for (int i = 0; i < edgeCount; i++)
                if (gen.random.Chance(3, 10))
                {
                    int index = randomizer[i];
                    
                    edges[index] = GrowEdge(edges[index]);
                }
        }

        
    //  Remove "Used" Cells that are Edges  //
        CellListRemoval(frontUsed, Cell.Edge);
        
        
        
        
    //  Clean Up    //
        SetRightZone(corners);
        SetRightZone(edges);
        
        
    //  Copy Front to Back  //
        seamBackCorners.CopyFrom(seamFrontCorners);
               backUsed.CopyFrom(frontUsed);
    }


    private void SetRightZone(List<Piece> pieces)
    {
        int count = pieces.Count;

        for (int i = 0; i < count; i++)
            pieces[i] = SetRightZone(pieces[i]);
    }
    

    private Piece SetRightZone(Piece piece)
    {
        int value = piece.cornerIDs.cornerValue;
        
        CornerIDs[] cornerIDs = piece.side == 1? gen.backIDs : gen.frontIDs;
        
        for (int y = 0; y < piece.size.y; y++)
        for (int x = 0; x < piece.size.x; x++)
        {
            Vector2Int p = piece.pos.Add(x, y);
            int index = gen.CellPosToIndex(p);
            
            CornerIDs cIds = cornerIDs[index];
            if (cIds.cornerValue == value)
                return new Piece(piece.pos, piece.size, cIds, piece.type, piece.side);
        }
        
        return piece;
    }
	
	
	private Piece GrowCorner(Piece piece)
    { 
        if(gen.cells[gen.CellPosToIndex(piece.pos)].Any(Cell.FrontSeam | Cell.BackSeam))
            return piece;
        
        Piece grown = piece.Grow(piece.cornerIDs.GrowDir());
        
        if(grown.pos.x < 0 || grown.pos.x + grown.size.x >= gen.xCells ||
           grown.pos.y < 0 || grown.pos.y + grown.size.y >= gen.yCells)
            return piece;
        
        
    //  Get CornerPos  //
        Vector2Int cornerPos = piece.pos;
        for (int y = 0; y < grown.size.y; y++)
        for (int x = 0; x < grown.size.x; x++)
        {
            Vector2Int p = grown.pos.Add(x, y);
            int index = gen.CellPosToIndex(p);
            if (gen.frontIDs[index].Equals(piece.cornerIDs))
            {
                cornerPos = p;
                goto FoundCornerPos;
            }
        }
        
        
        FoundCornerPos:
        
        CornerIDs cornerPosIDs = gen.frontIDs[gen.CellPosToIndex(cornerPos)];
        
        for (int y = 0; y < grown.size.y; y++)
        for (int x = 0; x < grown.size.x; x++)
        {
            Vector2Int p = grown.pos.Add(x, y);
            
            if(piece.Contains(p) || p == cornerPos)
                continue;
            
            int index = gen.CellPosToIndex(p);
            
            Cell cell = gen.cells[index];
            
            if(cell.Any(Cell.Corner | Cell.FrontSeam | Cell.BackSeam | Cell.Blocked))
                return piece;
            
            Vector2Int dirToCell = (p - cornerPos).Normalized();
            
            CornerIDs cIDs = gen.frontIDs[index];
            if((dirToCell.x == 0 || dirToCell.y == 0) && cIDs.IsEdgeCorner)
                return piece;
            
            bool isAir = !cell.Is(Cell.House);
            
            if(!cornerPosIDs.FitsDir(cIDs, dirToCell, isAir))
                return piece;
        }
        
    
        piece = grown;
        gen.SetAreaCells(piece.pos, piece.size, Cell.Corner);

        return piece;
    }


    private Piece TrimmEdgeIfInCorner(Piece piece)
    {
        bool rootIsChecked = gen.cells[gen.CellPosToIndex(piece.pos)].Is(Cell.Corner);

    //  Piece is Gone  //
        if (rootIsChecked && piece.sqrArea == 1)
            return new Piece(piece.pos, Vector2Int.zero, piece.cornerIDs, piece.type, 0);
            
    //  Horizontal  //
        if (piece.size.x > piece.size.y)
        {
            if(rootIsChecked)
                piece = new Piece(new Vector2Int(piece.pos.x + 1, piece.pos.y), new Vector2Int(piece.size.x - 1, piece.size.y), piece.cornerIDs, piece.type, 0);
            
            Vector2Int tip = piece.pos.Add(piece.size.x - 1, 0);
            if(gen.cells[gen.CellPosToIndex(tip)].Is(Cell.Corner))
                piece = new Piece(piece.pos, piece.size.Add(-1, 0), piece.cornerIDs, piece.type, 0);
        }
        else
        {
            if(rootIsChecked)
                piece = new Piece(new Vector2Int(piece.pos.x, piece.pos.y + 1), new Vector2Int(piece.size.x, piece.size.y - 1), piece.cornerIDs, piece.type, 0);
            
            Vector2Int tip = piece.pos.Add(0, piece.size.y - 1);
            if(gen.cells[gen.CellPosToIndex(tip)].Is(Cell.Corner))
                piece = new Piece(piece.pos, piece.size.Add(0, -1), piece.cornerIDs, piece.type, 0);
        }
        
        return piece;
    }


    private Piece GrowEdge(Piece piece)
    {
        if(piece.sqrArea == 1 || gen.cells[gen.CellPosToIndex(piece.pos)].Any(Cell.FrontSeam | Cell.BackSeam))
            return piece;
        
        Piece grown = piece.Grow(piece.cornerIDs.GrowDir().Reverse());
        
        for (int y = 0; y < grown.size.y; y++)
        for (int x = 0; x < grown.size.x; x++)
        {
            Vector2Int p = grown.pos.Add(x, y);
            
            if(piece.Contains(p))
                continue;
            
            int index = gen.CellPosToIndex(p);
            
            Cell cell = gen.cells[index];
            
            if(cell.Any(Cell.Edge | Cell.FrontSeam | Cell.BackSeam | Cell.Corner | Cell.Blocked))
                return piece;
        }
        
        piece = grown;
        gen.SetAreaCells(piece.pos, piece.size, Cell.Edge);
        
        return piece;
    }


    private void CollectSeams()
    {
        seams.Clear();

        
        for (int side = 0; side < 2; side++)
        {
            bool front = side == 1;
            
            List<int>   used        = front? frontUsed        : backUsed;
            List<Piece> seamCorners = front? seamFrontCorners : seamBackCorners;
            
            Cell seamValue   = front? Cell.FrontSeam       : Cell.BackSeam;
            Cell cornerValue = front? Cell.FrontSeamCorner : Cell.BackSeamCorner;
            
            CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
            
            int pieceSide = front? -1 : 1;
            
        //  Corners  //
            {
                int useCount = used.Count;
                for (int i = 0; i < useCount; i++)
                {
                    int index = used[i];
                    
                    if(index == investigate)
                        Debug.Log("Found Seam Corner");
                    
                    if (gen.cells[index].Is(seamValue))
                    {
                        CornerIDs cIDs = cornerIDs[index];
                
                        if (cIDs.IsZoneCorner)
                        {
                            gen.cells.Set(cornerValue, index);
                    
                            Vector2Int pos = gen.IndexToCellPos(index);
                    
                            seamCorners.Add(new Piece(pos, Vector2Int.one, cIDs, PieceType.Flat, pieceSide));
                            
                            if(index == investigate)
                                Debug.Log("Added Seam Corner");
                        }
                    }
                }
            
                CellListRemoval(used, cornerValue);
            }   
            
        //  Seams  //
            CreateEdges(seamCorners, seams, true, front);
        }
        
        EdgeDivide(seams, 3);
        CellListRemoval(frontUsed, Cell.FrontSeam);
        CellListRemoval(backUsed,  Cell.BackSeam);
        
        SetRightZone(seamFrontCorners);
        SetRightZone(seamBackCorners);
        SetRightZone(seams);
    }
    
    
    private void CollectFills()
    {
         fills.Clear();  
         
         
    //  Block both Front And Back and make "Holes"  //
        {
            int useCount = frontUsed.Count;
            int flushCount = 0;
            for (int i = 0; i < useCount; i++)
            {
                int index = frontUsed[i];
                if(!gen.cells[index].Any(Cell.Blocked | Cell.FrontSeam | Cell.BackSeam))
                    flush[flushCount++] = index;
            }
		   
            FillArea(fills, flush, flushCount, PieceType.Hole, 0);

            
        //  Only keep Hole Pieces that are Square and bigger than 2  //
            int holeFillCount = fills.Count;
            for (int i = 0; i < holeFillCount; i++)
            {
                Piece piece = fills[i];
                if (piece.size.x > 3 && piece.size.y == piece.size.x)
                {
                    for (int y = 0; y < piece.size.x; y++)
                    for (int x = 0; x < piece.size.x; x++)
                    {
                        int index = gen.CellPosToIndex(piece.pos.Add(x, y));
                        gen.cells.Set(Cell.Hole, index);
                    }
                }
                else
                {
                    fills.RemoveAt(i);
                    i--;
                    holeFillCount--;
                }
            }
            
            CellListRemoval(frontUsed, Cell.Hole);
            CellListRemoval(backUsed,  Cell.Hole);
        }
        
        
    //  Front and Back  //
        for (int side = 0; side < 2; side++)
        {
            bool front = side == 0;
            int sideValue = front? -1 : 1;
            
            List<int> used = front? frontUsed : backUsed;
            
            Cell block = front? Cell.FrontBlock : Cell.BackBlock;
            
            
        //  Flat  //
            int blockCount = 0;
            int useCount = used.Count;
            for (int i = 0; i < useCount; i++)
            {
                int index = used[i];
                
                if(gen.cells[index].Is(block))
                    justBlocked[blockCount++] = index;
            }
            
            CellListRemoval(used, block);
	   
            FillArea(fills, justBlocked, blockCount, PieceType.Flat, sideValue);
            
            
        //  Windows  //
            int count = used.Count;
            
            CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
            for (int i = 0; i < count; i++)
            {
                int index = used[i];

                Vector2Int pos = gen.IndexToCellPos(index);

                if (pos.x % 3 == 0 && pos.y % 5 == 0 && GrowWindow(pos, front))
                    fills.Add(new Piece(pos, new Vector2Int(2, 3), cornerIDs[index], PieceType.Window, sideValue));
            }
            
            count = CellListRemoval(used, front? Cell.FrontWindow: Cell.BackWindow);
            
            
        //  The Rest  //
            int flushCount = 0;
            for (int i = 0; i < count; i++)
                flush[flushCount++] = used[i]; 
            
            
            FillArea(fills, flush, flushCount, PieceType.Detail, sideValue);
        }
        
        
        /*Vector2Int maxSize = Vector2Int.zero;
        for (int i = 0; i < fills.Count; i++)
            maxSize =Vector2Int.Max(maxSize, fills[i].size);
        
        Debug.Log(maxSize);*/
    }
    
    
    private void FillArea(List<Piece> pieceList, int[] areaList, int areaCount, PieceType surfaceType, int sideValue)
    {
        bool front = sideValue != 1;
        
    //  Randomize AreaList Order //
        areaList.RandomizeRange(gen.random, areaCount);
        
        fillList.Clear();
        for (int i = 0; i < areaCount; i++)
            fillList[areaList[i]] = true;
        
        CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
		
        while (true)
        {
            int index;
            while (true)
            {
                areaCount--;

                if (areaCount < 0)
                    goto Done;
				
                
                index = areaList[areaCount];
                if (fillList[index])
                {
                    fillList[index] = false;
                    break;
                }
            }

        //  New Piece  //
            Piece p = new Piece(gen.IndexToCellPos(index), Vector2Int.one, cornerIDs[index], surfaceType, sideValue);

        //  Grow as often as possible  //
            while (true)
            {
                Piece grown = p.Grow(HouseHelp.AxisDir(gen.random.Range(0, 4)));
            //  Check if Pad Postions are Valid and inside Fill Area  //
                for (int yP = 0; yP < grown.size.y; yP++)
                for (int xP = 0; xP < grown.size.x; xP++)
                {
                    Vector2Int checkPos = grown.pos.Add(xP, yP);
                    
                    if(p.Contains(checkPos))
                        continue;
                    
                    if (!gen.ValidCellPos(checkPos) || !fillList[gen.CellPosToIndex(checkPos)])
                        goto NotGrowing;
                }     
                
                
            //  Stop Growing if too big  //
                if (grown.size.x > maxSideLength || grown.size.y > maxSideLength)
                {
                    pieceList.Add(p);
                    break;
                }

                p = grown;
                
            //  Remove Pad from Fill Area  //
                gen.SetAreaBools(grown.pos, grown.size, fillList, false);
                continue;
                
                
                NotGrowing:
                pieceList.Add(p);
                break;
            }
            continue;
            
            
            Done: break;
        }
    }
    
    
    private bool GrowWindow(Vector2Int pos, bool front)
    {
        CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
        
        int i = 0;
        for (int y = 0; y < 3; y++)
        for (int x = 0; x < 2; x++)
        {
            i++;
            
            if(i == 1)
                continue;

            Vector2Int checkPos = pos.Add(x, y);
            if (!gen.ValidCellPos(checkPos))
                return false;
            
            int checkIndex = gen.CellPosToIndex(checkPos);
            
            CornerIDs cID = cornerIDs[checkIndex];
            if(!cID.AllTheSame || !rules.GetZone(cID.ArrayZone).windows)
                return false;
            
            Cell cell = gen.cells[checkIndex];
            
            if(!cell.Is(Cell.House)  || 
                cell.Any(Cell.Corner | Cell.Edge | (front? Cell.FrontSeam : Cell.BackSeam) | Cell.Hole | Cell.Blocked))
                return false;
        }
        
        gen.SetAreaCells(pos, new Vector2Int(2, 3), front? Cell.FrontWindow: Cell.BackWindow);

        return true;
    }

    
    private int CreateEdges(List<Piece> corners, List<Piece> edges, bool seams, bool front = true)
    {
        PieceType type = seams? PieceType.Flat : PieceType.Detail;
        
        int cornerCount = corners.Count;
        for (int i = 0; i < cornerCount; i++)
        {
            Piece piece = corners[i];
            
            CornerIDs[] cornerIDs = front? gen.frontIDs : gen.backIDs;
            Cell edgeValue = seams? front? Cell.FrontSeamEdge : Cell.BackSeamEdge : Cell.Edge;
            
            int side = seams? front? -1 : 1 : piece.side;

            for (int dir = 0; dir < 4; dir++)
            {
                CornerIDs next = piece.cornerIDs.ProjectDir(dir);
                int cornerValue  = seams? next.ZoneCornerValue : next.cornerValue;
                
                if(dir % 2 == 0? cornerValue != 0110 && cornerValue != 1001 : 
                                 cornerValue != 0011 && cornerValue != 1100)
                    continue;
                
                int edgeLength = 0;
                Vector2Int growDir;
                switch (dir)
                {
                    default: growDir = Vector2Int.up;    break;
                    case 1:  growDir = Vector2Int.right; break;
                    case 2:  growDir = Vector2Int.down;  break;
                    case 3:  growDir = Vector2Int.left;  break;
                }
                
                while (true)
                {
                    Vector2Int checkPos = piece.pos + growDir * (edgeLength + 1);
                    if (!gen.ValidCellPos(checkPos))
                        break;

                    int       checkIndex = gen.CellPosToIndex(checkPos);
                    CornerIDs checkIDs   = cornerIDs[checkIndex];
                    
                    if(gen.cells[checkIndex].Is(edgeValue))
                        break;

                    if (seams? !checkIDs.IsOnEdge && checkIDs.ZoneCornerValue == cornerValue : 
                                checkIDs.cornerValue == cornerValue)
                    {
                        gen.cells.Set(edgeValue, checkIndex);
                        edgeLength++;
                    }
                    else
                        break;
                }

                if (edgeLength == 0)
                    continue;

                switch (dir)
                {
                    case 0: edges.Add(new Piece(piece.pos.Add(0, 1),           new Vector2Int(1, edgeLength), next, type, side)); break;
                    case 1: edges.Add(new Piece(piece.pos.Add(1, 0),           new Vector2Int(edgeLength, 1), next, type, side)); break;
                    case 2: edges.Add(new Piece(piece.pos.Add(0, -edgeLength), new Vector2Int(1, edgeLength), next, type, side)); break;
                    case 3: edges.Add(new Piece(piece.pos.Add(-edgeLength, 0), new Vector2Int(edgeLength, 1), next, type, side)); break;
                } 
            }
        }
        
        return edges.Count;
    }


    private void EdgeDivide(List<Piece> pieces, int maxEdgeLength)
    {
        int edgeCount = pieces.Count;
        for (int i = 0; i < edgeCount; i++)
        {
            Piece edge = pieces[i];
            
            int length = edge.sqrArea;
            if (length > maxEdgeLength)
            {
                pieces.RemoveAt(i);
                i--;
                int pieceLength = gen.random.Range(1, maxEdgeLength + 1);
                pieces.Add(new Piece(edge.pos, edge.size.Clamp(pieceLength), edge.cornerIDs, edge.type, edge.side));

                
                Vector2Int pos = edge.pos + edge.size.OnlyLongest() * pieceLength;
                pieces.Add(new Piece(pos, edge.size.Clamp(length - pieceLength), edge.cornerIDs, edge.type, edge.side));
                
                edgeCount++;
            }
        }
    }
    
    
    private int CellListRemoval(List<int> cells, Cell info)
    {
        int count = cells.Count;
        for (int i = 0; i < count; i++)
            if (gen.cells[cells[i]].Is(info))
            {
                cells.RemoveAt(i);
                i--;
                count--;
            }
        
        return count;
    }


    private static void IndexDebug(int index, string message)
    {
        if(index == investigate)
            Debug.Log(message);
    }


    private void Analyze()
    {
        for (int i = 0; i < fills.Count; i++)
        {
            Piece p = fills[i];
            int z = p.cornerIDs.a;
            Vector2Int s = p.size;
            
            Vector3Int check = new Vector3Int(z, s.x, s.y);
            
            unused.Remove(check);
        }
    }
}
