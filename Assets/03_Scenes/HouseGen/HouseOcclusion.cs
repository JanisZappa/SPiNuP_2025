using System.Collections.Generic;
using GeoMath;
using House;
using UnityEngine;
using UnityEngine.Profiling;
using Piece = HouseGen.Piece;
using CornerIDs = HouseGen.CornerIDs;
using Cell = HouseGen.Cell;


public partial class HouseOcclusion : MonoBehaviour
{
    private HouseGen gen;
    
    private bool[] occlusionGen, occlusionGenB;
    
    private readonly List<Bounds2D> occludedBounds    = new List<Bounds2D>(1000), 
                                    notOccludedBounds = new List<Bounds2D>(1000);

    private UniqueList occluded, notOccluded;
    
    
    public void Init()
    {
        gen = GetComponent<HouseGen>();
        
        occlusionGen  = new bool[gen.cellCount];
        occlusionGenB = new bool[gen.cellCount];
        
        occluded    = new UniqueList(gen.cellCount);
        notOccluded = new UniqueList(gen.cellCount);
    }
    
    
    public void CalculateOcclusion()
    {
        Timer.Start(Timer.Entry.Occlusion);
        
        Profiler.BeginSample("Occlusion_FindOutline");
        
        int count = gen.cellCount;
        for (int i = 0; i < count; i++)
            occlusionGen[i] = gen.cells[i].Is(Cell.House);
        
        notOccluded.Clear();
        for (int e = 0; e < 2; e++)
        {
            for (int y = 0; y < gen.yCells; y++)
            for (int x = 0; x < gen.xCells; x++)
            {
                Vector2Int cellPos = new Vector2Int(x, y);
                int index = gen.CellPosToIndex(cellPos);
                
                if (!occlusionGen[index])
                {
                    occlusionGenB[index] = false;
                    continue;
                }

                if (gen.cells[index].Is(Cell.Hole))
                {
                    notOccluded.Add(index);
                    occlusionGenB[index] = false;
                    continue;
                }
                    

                bool solid = true;
                for (int i = 0; i < 8; i++)
                {
                    Vector2Int neighbour = cellPos + HouseHelp.Dir(i);
                    if (gen.ValidCellPos(neighbour))
                    {
                        int neighbourIndex = gen.CellPosToIndex(neighbour);
                        if (!occlusionGen[neighbourIndex] || gen.cells[neighbourIndex].Is(Cell.Hole))
                        {
                            solid = false;
                            break;
                        }  
                    }
                }

                if(solid)
                    occlusionGenB[index] = true;
                else
                {
                    notOccluded.Add(index);
                    occlusionGenB[index] = false;
                }
            }
            
            occlusionGen.CopyFrom(occlusionGenB);
        }

        occluded.Clear();
        for (int i = 0; i < count; i++)
            if (occlusionGen[i])
                occluded.Add(i);

        Profiler.EndSample();
        
        
        Profiler.BeginSample("Occlusion_OccludedBounds");
        
        occludedBounds.Clear();
        while (occluded.Length > 0)
        {
            int index = occluded.RemoveAt(gen.random.Range(0, occluded.Length));
            
            occlusionGen[index] = false;
            
            Piece p = new Piece(gen.IndexToCellPos(index), Vector2Int.one, new CornerIDs(1, 1, 1, 1), 0, 0);
            
        //  Grow as often as possible  //
            while (true)
            {
                Piece grown = p.Grow(HouseHelp.AxisDir(gen.random.Range(0, 4)));
               
                for (int yP = 0; yP < grown.size.y; yP++)
                for (int xP = 0; xP < grown.size.x; xP++)
                {
                    Vector2Int checkPos = grown.pos.Add(xP, yP);
                    
                    if(p.Contains(checkPos))
                        continue;
                    
                    if (!gen.ValidCellPos(checkPos) || !occlusionGen[gen.CellPosToIndex(checkPos)])
                        goto NotGrowing;
                }
                
                for (int yP = 0; yP < grown.size.y; yP++)
                for (int xP = 0; xP < grown.size.x; xP++)
                {
                    Vector2Int checkPos = grown.pos.Add(xP, yP);

                    if (p.Contains(checkPos))
                        continue;

                    int checkIndex = gen.CellPosToIndex(checkPos);

                    occlusionGen[checkIndex] = false;

                    occluded.Remove(checkIndex);
                }
                
                p = grown;
                
                continue;
               
                
                NotGrowing:
                occludedBounds.Add(p.GetBounds(gen));
                break;
            }
        }

        Profiler.EndSample();
       
        
        Profiler.BeginSample("Occlusion_NonOccludedBounds");
        
        occlusionGenB.Clear();
        for (int i = 0; i < notOccluded.Length; i++)
            occlusionGenB[notOccluded[i]] = true;
        
        notOccludedBounds.Clear();
        
        while (notOccluded.Length > 0)
        {
            int index = notOccluded.RemoveAt(gen.random.Range(0, notOccluded.Length));
            
            occlusionGen[index] = false;
            Piece p = new Piece(gen.IndexToCellPos(index), Vector2Int.one, new CornerIDs(1,1,1,1), 0, 0);
            
        //  Grow as often as possible  //
            while (true)
            {
                Piece grown = p.Grow(HouseHelp.AxisDir(gen.random.Range(0, 4)));
               
                for (int yP = 0; yP < grown.size.y; yP++)
                for (int xP = 0; xP < grown.size.x; xP++)
                {
                    Vector2Int checkPos = grown.pos.Add(xP, yP);
                    
                    if(p.Contains(checkPos))
                        continue;
                    
                    if (!gen.ValidCellPos(checkPos) || !occlusionGenB[gen.CellPosToIndex(checkPos)])
                        goto NotGrowing;
                }
               
                
                for (int yP = 0; yP < grown.size.y; yP++)
                for (int xP = 0; xP < grown.size.x; xP++)
                {
                    Vector2Int checkPos = grown.pos.Add(xP, yP);

                    if (p.Contains(checkPos))
                        continue;

                    int checkIndex = gen.CellPosToIndex(checkPos);

                    occlusionGenB[checkIndex] = false;

                    notOccluded.Remove(checkIndex);
                }
                
                p = grown;
                
                continue;
                
                
                NotGrowing:
                notOccludedBounds.Add(p.GetBounds(gen));
                break;
            }
        }
        
        Profiler.EndSample();
        
        
        Timer.End(Timer.Entry.Occlusion);
        
        Timer.Start(Timer.Entry.TellLevelAboutOcclusion);
        
        CellMaster.ClearOccluders();

        for (int i = 0; i < occludedBounds.Count; i++)
            Level.AddWallBound(occludedBounds[i], true);
        
        for (int i = 0; i < notOccludedBounds.Count; i++)
            Level.AddWallBound(notOccludedBounds[i], false);
        
        Timer.End(Timer.Entry.TellLevelAboutOcclusion);
    }
}