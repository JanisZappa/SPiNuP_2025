using System.Collections;
using Generation;
using GeoMath;
using LevelElements;
using UnityEngine;


public partial class Level : Singleton<Level>
{
    static Level()
    {
        items  = new Item[Item.TotalCount];
        tracks = new Track[MaxActiveElements];
        
        oldItems = new Item[Item.TotalCount];

        addedItems   = new Item[MaxActiveElements];
        removedItems = new Item[MaxActiveElements];
        
        elementCheck = new int[Item.TotalCount + Track.TotalCount];
        pieceCheck   = new int[10000];
        
        frustumCells = new FrustumCell[Frustum.cellCount];
        
        pieces        = new int[10000];
        oldPieces     = new int[10000];
        addedPieces   = new int[10000];
        removedPieces = new int[10000];
    }
    
    
    public enum LevelType { HandMade }
    
    
    public LevelType levelType;
    
    
    public class FrustumCell
    {
        public Vector2Int cellPos;
        
        public Cell cell;
        public CellVis frontVis, backVis;
        
        public bool visible => frontVis != CellVis.None || backVis != CellVis.None;


        public void SetCellPos(Vector2Int cellPos)
        {
            this.cellPos = cellPos;
            cell = CellMaster.GetCellAt(cellPos);
        }
    }
    
    
    public static readonly FrustumCell[] frustumCells;
    
    public static Item           StartStick;
    public static ILevelGenerator Generator;

    public static readonly Item[] items;
    private static readonly Item[] oldItems, addedItems, removedItems;
    public static int itemCount;
    
    public static readonly Track[] tracks;
    public static int trackCount;

    private static readonly int[] pieces, oldPieces, addedPieces, removedPieces;
    private static int pieceCount;
    
    private static readonly int[] elementCheck, pieceCheck;

    
    #region Setup

    public static void GameStart()
    {
        Generator.StartGame();
        
        HouseGen.GameStart();
        
        itemCount  = 0;
        trackCount = 0;
        pieceCount = 0;
    }


    public static void ClearPieces()
    {
        if (pieceCount == 0)
            return;
        
        PlacerMeshes.UpdateVisiblePieces(addedPieces, 0, pieces, pieceCount);
        pieceCount = 0;
    }


    public static void ClearLevel()
    {
        LevelSaveLoad.NewLevel();
        Element.ClearAll();
        Refresh();
    }

    
    public static IEnumerator GameLoad()
    {
        for ( int i = 0; i < Frustum.cellCount; i++ )
            frustumCells[i] = new FrustumCell();

        switch(Inst.levelType)
        {
            case LevelType.HandMade:
                Generator = Inst.GetComponent<LevelSaveLoad>();
                break;
        }
        
        ActorAnimator.GameLoad();

        yield return Generator.LoadGame();

        HouseGen.GameLoad();
    }

    #endregion
    
    
    #region Frustum Range


    public static void Refresh()
    {
    //  Update Frustum  //
        Frustum frustum = GameCam.frustum;
        frustum.Update();
        
    //  Set Frustum Cell Positions  //
        {
            int index = 0;
            for (int y = 0; y < Frustum.cells; y++)
                for (int x = 0; x < Frustum.cells; x++)
                    frustumCells[index++].SetCellPos(new Vector2Int(x, y) + frustum.minCell);
        }
        
        
    //  Check Frustum Cell Visibility  //
        for (int i = 0; i < Frustum.cellCount; i++)
            frustum.SetCellVis(frustumCells[i]);
        
        
    //  Save old State  //
        int oldItemCount = 0;
        for (int i = 0; i < itemCount; i++)
        {
            Item item = items[i];
            int id = item.ID;
            
            //TODO? Really???
            if (elementCheck[id] != -1)
                oldItems[oldItemCount++] = item;
        }
        elementCheck.Clear();

        int oldPieceCount = pieceCount;
        for (int i = 0; i < pieceCount; i++)
        {
            int id = pieces[i];
            oldPieces[i] = id;
        }
        pieceCheck.Clear();
            
        
    //  Collect Items, Tracks and Connections  //
        itemCount  = 0;
        trackCount = 0;
        pieceCount = 0;

         
        Bounds2D camFront = frustum.frontBounds;
        Bounds2D camBack  = frustum.backBounds;

        for (int i = 0; i < Frustum.cellCount; i++)
        {
            if (!frustumCells[i].visible || frustumCells[i].cell == null)
                continue;
            
            
            
            CellVis frontVis = frustumCells[i].frontVis;
            CellVis backVis  = frustumCells[i].backVis;

        //  Items & Fluff  //
            Cell cell = frustumCells[i].cell;

            for (int side = 0; side < 2; side++)
            {
                Cell.Content content = side == 0 ? cell.front : cell.back;
                CellVis checkVis = side == 0 ? frontVis : backVis;
                Bounds2D checkBounds = side == 0 ? camFront : camBack;

            //  Collect Solo Items  //
                int cellItemCount = content.soloItemCount;
                for (int e = 0; e < cellItemCount; e++)
                {
                    Item item = content.soloItems[e];

                    if (!item.elementType.ShowThis())
                        continue;

                    if (elementCheck[item.ID] == 0 && (checkVis == CellVis.All || checkVis == CellVis.Some && item.bounds.Intersects(checkBounds)) &&
                        IsNotOccluded(item))
                    {
                        items[itemCount++] = item;
                        elementCheck[item.ID] = 1;
                    }
                }


            //  Tracks  //
                bool camSide = (GameCam.CurrentSide == Side.Front) == (side == 0);

                int cellTrackCount = content.trackCount;
                for (int e = 0; e < cellTrackCount; e++)
                {
                    Track track = content.tracks[e];

                    if (!camSide && track.allSubsOccluded || 
                        elementCheck[track.ID] == 1 || 
                        checkVis == CellVis.None || 
                        !track.bounds.Intersects(checkBounds))
                        continue;

                    
                    tracks[trackCount++] = track;
                    elementCheck[track.ID] = 1;

                    if (checkBounds.Contains(track.bounds) && (camSide || track.allSubsVisible))
                    {
                        for (int iI = 0; iI < track.itemCount; iI++)
                        {
                            Item item = track.items[iI];

                            if (!item.elementType.ShowThis() || elementCheck[item.ID] == 1 || !IsNotOccluded(item))
                                continue;

                            items[itemCount++] = item;
                            elementCheck[item.ID] = 1;
                        }
                    }
                    else
                    {
                        track.FillSubBounds(GTime.Now, Mask.AnyThing);
                        int gotItems = 0;

                        for (int sub = 0; sub < track.subBoundCount; sub++)
                        {
                            Track.SubBound sB = track.GetSubBound(sub);
                            if (sB.Intersects(checkBounds) && (camSide || !track.subOcclusion[sub]))
                            {
                                for (int iI = 0; iI < sB.itemCount; iI++)
                                {
                                    Item item = sB.items[iI];

                                    if (!item.elementType.ShowThis())
                                        continue;

                                    if (elementCheck[item.ID] == 0 && IsNotOccluded(item))
                                    {
                                        items[itemCount++] = item;
                                        elementCheck[item.ID] = 1;
                                        gotItems++;
                                    }
                                }
                            }

                            if (gotItems >= track.itemCount)
                                break;
                        }
                    }
                }    
            }
            
        //  Both Sides  //
            {
                int camSide = (int)GameCam.CurrentSide.Sign;
                
                for (int e = 0; e < cell.bgPieceCount; e++)
                {
                    Cell.SidePiece piece = cell.bgPieces[e];
                    int id = piece.index;
                    
                    if (pieceCheck[id] == 0 && (piece.side == camSide || piece.side == 0))
                    {
                        pieceCheck[id] = 1;
                        pieces[pieceCount++] = id;
                    }
                }
            }
        }
            
        

    //  Collect Items that were added / removed  //
        {
            int addCount    = 0;
            int removeCount = 0;
        
            for (int i = 0; i < oldItemCount; i++)
            {
                Item item = oldItems[i];
                if (elementCheck[item.ID] == 0)
                    removedItems[removeCount++] = item;
                else
                    elementCheck[item.ID] = 0;
            }

            for (int i = 0; i < itemCount; i++)
            {
                Item item = items[i];
                if (elementCheck[item.ID] == 1)
                    addedItems[addCount++] = item;
            }
                
            
        //  Update Actors if there are any Changes  //
            if (addCount > 0 || removeCount > 0)
                ActorAnimator.UpdateVisibleActors(addedItems, addCount, removedItems, removeCount);    
        }
        
        
        
    //  Collect BGMeshes that were added / removed  //
        {
            int addCount    = 0;
            int removeCount = 0;

            for (int i = 0; i < oldPieceCount; i++)
            {
                int id = oldPieces[i];
                if (pieceCheck[id] == 0)
                    removedPieces[removeCount++] = id;
                else
                    pieceCheck[id] = 0;
            }
            
            for (int i = 0; i < pieceCount; i++)
            {
                int id = pieces[i];
                if (pieceCheck[id] == 1)
                    addedPieces[addCount++] = id;
            }

            if (addCount > 0 || removeCount > 0)
                PlacerMeshes.UpdateVisiblePieces(addedPieces, addCount, removedPieces, removeCount);    
        }
    }


    public static void CantShowItem(Item item)
    {
        elementCheck[item.ID] = -1;
    }

    private static readonly BoolSwitch HideBackSide = new("Dev/Hide BackSide", false);
    
    private static bool IsNotOccluded(Item item)
    {
        return item.side == GameCam.CurrentSide || !HideBackSide && !item.isOccluded;
    }

    #endregion
    
    
    #region Cells and Bounds
    public static void AddElementToCells(Element element)
    {
        Search.Cells(element.bounds);
        
        for (int i = 0; i < Search.cellCount; i++)
            Search.boundCells[i].Add(element);
    }
    
    
    public static void RemoveElementFromCells(Element element)
    {
        Search.Cells(element.bounds);

        for (int i = 0; i < Search.cellCount; i++)
            Search.boundCells[i].Remove(element);
    }
    
    
    public static void AddBGMeshToCells(int bgMeshIndex, Bounds2D bounds, int side)
    {
        /*Cell bestCell = Search.BestCell(bounds);
        
        if(bestCell != null)
            bestCell.Add(bgMeshIndex, side);*/
        
        Search.Cells(bounds);
        
        for (int i = 0; i < Search.cellCount; i++)
            Search.boundCells[i].Add(bgMeshIndex, side);
    }
    
    
    public static void RemoveBGMeshFromCells(int bgMeshIndex, Bounds2D bounds, int side)
    {
        Search.Cells(bounds);

        for (int i = 0; i < Search.cellCount; i++)
            Search.boundCells[i].Remove(bgMeshIndex, side);
    }
    
    #endregion
    
    
    public static void AddWallBound(Bounds2D bound, bool solid)
    {
        if (Generator == null)
            return;
        
        Search.Cells(bound);
        
        for (int i = 0; i < Search.cellCount; i++)
            Search.boundCells[i].AddBound(bound, solid);
    }
}




public interface ILevelGenerator
{
    IEnumerator LoadGame();
    void StartGame();
    bool IsNew { get; }
}


public static class Search
{
    static Search()
    {
        boundCells  = new Cell[100];
        boundItems  = new Item[200];
        boundTracks = new Track[100];
        
        callCheck = new bool[Item.TotalCount + Track.TotalCount];
    }
    
    
    public static readonly Cell[]  boundCells;
    public static readonly Item[]  boundItems;
    public static readonly Track[] boundTracks;

    public static int cellCount, itemCount, trackCount;

    private static readonly bool[] callCheck;
    
    
    public static void Cells(Bounds2D bounds)
    {
        cellCount = CellMaster.GetBoundCells(bounds, boundCells);
    }

    
    public static void ItemsAndTracks(Bounds2D bounds, Side side, ElementMask filter)
    {
        callCheck.Clear();
        itemCount = trackCount = 0;
        bool all = filter == Mask.AnyThing;
        
        Cells(bounds);
        for (int i = 0; i < cellCount; i++)
        {    
        //  Items & Fluff  //
            Cell.Content content = side.front? boundCells[i].front : boundCells[i].back;

            int cellItemCount = content.soloItemCount;
            for (int e = 0; e < cellItemCount; e++)
            {
                Item item = content.soloItems[e];
                
                if (!callCheck[item.ID] &&
                    (all || filter.Fits(item.elementType)) && item.bounds.Intersects(bounds))
                {
                    boundItems[itemCount++] = item;
                    callCheck[item.ID] = true;
                }
            }
            
            int cellTrackCount = content.trackCount;
            for (int e = 0; e < cellTrackCount; e++)
                if (all || content.tracks[e].AnyItemMatchesMask(filter))
                {
                    if (!callCheck[content.tracks[e].ID] &&
                        content.tracks[e].bounds.Intersects(bounds))
                    {
                        boundTracks[trackCount++] = content.tracks[e];
                        callCheck[content.tracks[e].ID] = true;
                    }
                }
        }
    }


    public static void Items(Bounds2D bounds, Side side, float time, ElementMask filter)
    {
        callCheck.Clear();
        itemCount = 0;
        bool all = filter == Mask.AnyThing;
        
        Cells(bounds);
        for (int i = 0; i < cellCount; i++)
        {    
        //  Solo Items  //
            Cell.Content content = side.front? boundCells[i].front : boundCells[i].back;
            
            int cellItemCount = content.soloItemCount;
            for (int e = 0; e < cellItemCount; e++)
            {
                Item item = content.soloItems[e];
                
                if (!callCheck[item.ID] && 
                    (all || filter.Fits(item.elementType)) && item.bounds.Intersects(bounds))
                {
                    boundItems[itemCount++] = item;
                    callCheck[item.ID] = true;
                }
            }
            
        //  Track Items  //
            int cellTrackCount = content.trackCount;
            for (int e = 0; e < cellTrackCount; e++)
            {
                Track track = content.tracks[e];
                
                if (!callCheck[track.ID] &&
                    (all || track.AnyItemMatchesMask(filter)) &&
                    track.bounds.Intersects(bounds))
                {
                    callCheck[content.tracks[e].ID] = true;
                    

                    if (track.subBoundCount == 1)
                        for (int f = 0; f < track.itemCount; f++)
                        {
                            Item item = track.items[f];
                            if (filter.Fits(item.elementType) && !callCheck[item.ID])
                            {
                                boundItems[itemCount++] = item;
                                callCheck[item.ID] = true;
                            }
                        }
                    else
                    {
                        bool prepearedTrack = false;
                        int gotItems = 0;

                        for (int subBound = 0; subBound < track.subBoundCount; subBound++)
                        {
                            Track.SubBound sB = track.GetSubBound(subBound);
                            if (sB.Intersects(bounds))
                            {
                                if (!prepearedTrack)
                                {
                                    track.FillSubBounds(time, filter);
                                    prepearedTrack = true;
                                }

                                for (int subI = 0; subI < sB.itemCount; subI++)
                                {
                                    Item item = sB.items[subI];
                                    if (!callCheck[item.ID])
                                    {
                                        boundItems[itemCount++] = item;
                                        callCheck[item.ID] = true;
                                        gotItems++;
                                    }
                                }
                            }

                            if (gotItems >= track.itemCount)
                                break;
                        }
                    }
                }
            }
        }
    }


    public static void Tracks(Bounds2D bounds, Side side)
    {
        callCheck.Clear();
        trackCount = 0;
        
        Cells(bounds);
        
        for (int i = 0; i < cellCount; i++)
        {    
            Cell.Content content = side.front? boundCells[i].front : boundCells[i].back;
            
            int cellTrackCount = content.trackCount;
            for (int e = 0; e < cellTrackCount; e++)
                if (!callCheck[content.tracks[e].ID] &&
                    content.tracks[e].bounds.Intersects(bounds))
                {
                    boundTracks[trackCount++] = content.tracks[e];
                    callCheck[content.tracks[e].ID] = true;
                }
        }
    }
    
    
    public static Item ClosestItem(Vector2 pos, float time, Side side, ElementMask filter)
    {
        Bounds2D b = new Bounds2D(pos).Pad(Level.CellSize * .5f);
        
        Items(b, side, time, filter);
        
        float closestItemDistance = float.MaxValue;
        Item  closestItem         = null;
                
        for (int i = 0; i < itemCount; i++)
        {
            float sqrtDistance = (boundItems[i].GetLagPos(time) - pos).sqrMagnitude;
            if (sqrtDistance < closestItemDistance)
            {
                closestItemDistance = sqrtDistance;
                closestItem         = boundItems[i];
            }
        }

        return closestItem;
    }
    
    
    public static Track ClosestTrack(Vector2 pos, float time, Side side)
    {
        Bounds2D b = new Bounds2D(pos).Pad(Level.CellSize * .5f);
        
        Tracks(b, side);
        
        float closestTrackDistance = float.MaxValue;
        Track closestTrack         = null;
                
        for (int i = 0; i < trackCount; i++)
        {
            float sqrtDistance = (boundTracks[i].rootPos - pos).sqrMagnitude;
            if (sqrtDistance < closestTrackDistance)
            {
                closestTrackDistance = sqrtDistance;
                closestTrack         = boundTracks[i];
            }
        }

        return closestTrack;
    }
    
    
    /// <summary> Considering Item Radii - out closestPoint</summary>
    public static Element ClosestElement(Vector2 pos, float time, Side side, ElementMask mask, out Vector2 closestPoint)
    {
        closestPoint = pos;
        
        if (mask == null)
            return null;
        
        Bounds2D b = new Bounds2D(pos).Pad(Level.CellSize * .5f);

        if (mask == Mask.IsTrack)
        {
            Tracks(b, side);
        
            float closestTrackDistance = float.MaxValue;
            Track closestTrack         = null;
                
            for (int i = 0; i < trackCount; i++)
            {
                Vector2 closestTrackPoint = boundTracks[i].GetClosestPoint(pos);
                Vector2 dir   = pos - closestTrackPoint;
                float   dir_M = dir.magnitude;
                
                float distance = Mathf.Max(0, dir_M - boundTracks[i].maxItemRadius);
                if (distance < closestTrackDistance)
                {
                    closestTrackDistance = distance;
                    closestTrack         = boundTracks[i];
                    closestPoint = closestTrackPoint + dir * (1f / dir_M * boundTracks[i].maxItemRadius);
                }
            }

            return closestTrack;
        }

        
        Items(b, side, time, mask);
        
        float closestItemDistance = float.MaxValue;
        Item  closestItem         = null;
                
        for (int i = 0; i < itemCount; i++)
        {
            Vector2 itemPos = boundItems[i].GetPos(time);
            Vector2 dir = pos - itemPos;
            
            float distance = Mathf.Max(0, dir.magnitude - boundItems[i].radius);
            if (distance < closestItemDistance)
            {
                closestItemDistance = distance;
                closestItem         = boundItems[i];
                closestPoint = itemPos + dir.SetLength(boundItems[i].radius);
            }
        }

        return closestItem;
    }
}