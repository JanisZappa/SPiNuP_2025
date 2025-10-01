using System.Collections.Generic;
using System.Text;
using ActorAnimation;
using Clips;
using Generation;
using GeoMath;
using LevelElements;
using UnityEngine;


public static class LevelDebug 
{
    private static readonly BoolSwitch showItemInfo        = new("Elements/Item Info", false);
    private static readonly BoolSwitch showFluffInfo       = new("Elements/Fluff Info", false);
    private static readonly BoolSwitch showCollectableInfo = new("Elements/Collectable Info", false);
    private static readonly BoolSwitch showTrackInfo       = new("Elements/Track Info", false);
    private static readonly BoolSwitch showTracks          = new("Elements/Tracks", false);
    private static readonly BoolSwitch showLinks           = new("Elements/Links", false);
    private static readonly BoolSwitch markStartStick      = new("Elements/StartStick", false);
    
    private static readonly BoolSwitch showHeightPlanes    = new("Level/Show Height Planes", false);
    
    private static readonly BoolSwitch showBounds     = new ("Bounds/Elements", false);
    private static readonly BoolSwitch showCells      = new ("Bounds/Cells", false);
    private static readonly BoolSwitch showCellItems  = new ("Bounds/Cell Item IDs", false);
    private static readonly BoolSwitch showCellTracks = new ("Bounds/Cell Track IDs", false);
    
    private static readonly BoolSwitch showSubCells   = new ("Bounds/Filled Sub Cells", false); 
    private static readonly BoolSwitch showOccluders  = new ("Bounds/Occluder", false);
    private static readonly BoolSwitch subBounds      = new ("Bounds/Track Sub", false);
   
    private static readonly BoolSwitch outOfBoundsItem  = new("Dev/Out Of Bounds", false);
    private static readonly BoolSwitch showGenRects     = new("Gen/Show Rects", false);

    
    public static void DebugUpdate()
    {
        if (!GameManager.Running)
            return;
        
        if (showItemInfo && Level.items != null)
            ShowItemIDS(Mask.IsItem);
        
        if (showFluffInfo && Level.items != null)
            ShowItemIDS(Mask.IsFluff);
        
        if (showCollectableInfo && Level.items != null)
            ShowItemIDS(Mask.IsCollectable);
        
        if (showBounds && Level.items != null)
            ShowItemBounds();
        
        
        if(ElementEdit.element != null && !Creator.currentFilter.Fits(ElementEdit.elementType))
            ElementEdit.element.Draw();
        

        if (showTracks || GameManager.IsCreator && Creator.currentFilter == Mask.IsTrack)
            ShowTracks();


        if (GameManager.IsCreator && (Creator.currentFilter == Mask.IsItem || 
                                      Creator.currentFilter == Mask.IsFluff || 
                                      Creator.currentFilter == Mask.IsCollectable))
            ShowItems();
        
        
        if(showTrackInfo)
            ShowTrackInfo();
        
        if (showBounds && Level.tracks != null)
            ShowTrackBounds();
        
        if(subBounds && Level.tracks != null)
            ShowSubBounds();
        
        if(showLinks || GameManager.IsCreator)
            ShowLinks();
        
        if (showHeightPlanes)
            ShowFloor();
        
        if (showCells)
            ShowCells();

        if (showSubCells)
            ShowSubCells();
        
        if(showCellItems || showCellTracks)
            ShowCellElementIDs();

        if (showOccluders)
            ShowOccluders();
        
        if(markStartStick || GameManager.IsCreator)
            ShowStartStick();

        if (outOfBoundsItem && GTime.Paused)
            ShowOutOfBounds();
        
        if(showGenRects)
            ShowHouseGenRects();
    }


    private static void ShowOccluders()
    {
        int count = OccluderPacking.cellPosBounds.Length;
        for (int i = 0; i < count; i++)
            OccluderPacking.cellPosBounds[i].Draw();
    }


    private static void ShowItemIDS(ElementMask mask)
    {
        float depth = mask == Mask.IsFluff ? Z.W : Z.P;
        
        for (int i = 0; i < Level.itemCount; i++)
        {
            Item item = Level.items[i];
            if (Level.items[i].side.IsCamSide && mask.Fits(item.elementType))
            {
                Color c = ActorAnim.DebugTextColor(item);
                DRAW.Text(item.ID.ToString("D3"),   item.GetLagPos(GTime.Now), c, 1.5f, offset: V2.down *  3, z: depth);
                DRAW.Text(item.elementType.ToString(), item.GetLagPos(GTime.Now), c, 1.5f, offset: V2.down *  5, z: depth);
                
                /*DRAW.GUI_Text(item.ID.ToString("D3") + "\n" + item.elementType.ToString(),      item.GetLagPos(GTime.Now).V3(depth), c, 1.5f);*/
            } 
        }
    }

    
    private static void ShowItemBounds()
    {
        for (int i = 0; i < Level.itemCount; i++)
        {
            if(!Level.items[i].side.IsCamSide || Level.items[i].parent != null)
                continue;
                
            Color c = Level.items[i].elementType.DebugColor();
            Level.items[i].bounds.Draw().SetColor(c).SetDepth(Z.W10);
        }
    }

    
    private static void ShowTrackBounds()
    {
        for (int i = 0; i < Level.trackCount; i++)
        {
            if(!Level.tracks[i].side.IsCamSide)
                continue;

            Track track = Level.tracks[i];
            Color c = track.elementType.DebugColor();
            track.bounds.Draw().SetColor(c).SetDepth(Z.W10);
        }
    }


    private static void ShowTrackInfo()
    {
        for (int i = 0; i < Level.trackCount; i++)
            if(Level.tracks[i].side.IsCamSide)
            {
                Track track =  Level.tracks[i];
                DRAW.Text(track.ID + " Speed: " + track.speed.ToString("F0"), track.center, Color.white, 1.5f, offset: V2.down * 3, z: Z.P);
                DRAW.Text("Items: " + track.itemCount + (track.side.front? " F" : " B"), track.center, Color.white, 1.5f, offset: V2.down * 5, z: Z.P);
            }
    }


    private static void ShowSubBounds()
    {
        Color a = COLOR.green.lime;
        Color b = COLOR.red.tomato;
        float multi = Mth.SmoothPP(.25f, 1, GTime.Now * 2f);
        
        for (int i = 0; i < Level.trackCount; i++)
        {
            if(!Level.tracks[i].side.IsCamSide)
                continue;
            
            Track track = Level.tracks[i];

            int highlightBound;
                if(track.itemCount == 0) 
                    highlightBound = (int)Mathf.Repeat(Mathf.Floor(GTime.Now * .25f), track.subBoundCount);
                else
                    highlightBound = track.SubIndex(track.SubStep(track.items[0], GTime.Now));
            
            
            for (int e = 0; e < track.subBoundCount; e++)
            {
                Bounds2D drawBounds = track.GetSubBound(e).bounds;
                bool isHighlighted = highlightBound == e; 
                Color    color      = isHighlighted? Color.white : track.subOcclusion[e]? b : a;
                drawBounds.Draw().SetColor(color).SetDepth(Z.W10).Fill(isHighlighted? .1f :  0);
            }

            track.FillSubBounds(GTime.Now, Mask.AnyThing);
            for (int e = 0; e < track.itemCount; e++)
            {
                Item item = track.items[e];
                if (track.ItemIsInSubBound(item, highlightBound))
                {
                    Vector2 pos = item.GetPos(GTime.Now);

                    float radius = item.radius;
                    DRAW.Circle(pos, radius, 20).SetColor(Color.white).SetDepth(Z.W30);
                    DRAW.Circle(pos, radius * (1 + multi * 2), 20).SetColor(Color.white.A(.5f)).SetDepth(Z.W30).Fill(.1f);
                    DRAW.Circle(pos, radius * (1 + multi * 4), 20).SetColor(Color.white.A(.25f)).SetDepth(Z.W30);
                }
            }
        }
    }

    
    private static void ShowFloor()
    {
        Vector2 camP = GameCam.CurrentPos;

        const float multi = Height.Factor * 100;
        
        float min = Mathf.Floor(camP.y / multi) * multi;
        float max = Mathf.Ceil( camP.y / multi) * multi;

        for (int i = 0; i < 2; i++)
        {
            float y = i == 0 ? min : max;
            
            Vector2 a = new Vector3(-500, y, 0), dir = V3.right * 1000;
            for (int e = 0; e < 5; e++)
            {
                DRAW.Vector(a, dir).SetColor(Color.Lerp(COLOR.orange.coral,  COLOR.yellow.fresh,    e * (1f / 6))).SetDepth( Z.WallZ(e + 1));
                DRAW.Vector(a, dir).SetColor(Color.Lerp(COLOR.purple.violet, COLOR.blue.cornflower, e * (1f / 6))).SetDepth(-Z.WallZ(e + 1));
            }
        }
    }

    
    private static void ShowCells()
    {
        Vector2Int camCellPos = Cell.ToCellPos(GameCam.frustum.focusPoint);
        Cell.GetBounds(camCellPos).Pad(-.4f).Draw().SetColor(Color.magenta).SetDepth(Z.W20);
        
        CellMaster.GetCellAt(camCellPos)?.DrawBGBounds();
        return;
        
        for (int i = 0; i < Frustum.cellCount; i++)
        {
            if (CellMaster.GetCellAt(Level.frustumCells[i].cellPos) == null || Level.frustumCells[i].cell == null)
                continue;
            
            Cell cell = Level.frustumCells[i].cell;
            Cell.Content content = GameCam.CurrentSide.front ? cell.front : cell.back;

            CellVis cellVis = GameCam.CurrentSide.front ? Level.frustumCells[i].frontVis : 
                                                          Level.frustumCells[i].backVis;

            Color color;
            switch (cellVis)
            {
                default:           color = COLOR.yellow.fresh;          break;
                case CellVis.Some: color = COLOR.green.spring.A(.5f);   break;
                case CellVis.All:  color = COLOR.purple.orchid.A(.75f); break;
            }
            
            Vector2Int cellPos = Level.frustumCells[i].cellPos;
            Bounds2D cellBounds = Cell.GetBounds(cellPos);

            if (content.itemCount > 0) 
                cellBounds.Draw(.95f).SetColor(color).SetDepth(Z.W05).Fill(cellVis != CellVis.None? 0 : .25f);
            else
                DRAW.GapRectangle(cellBounds.Center, cellBounds.Size * .78f, 1).
                    SetColor(color).SetDepth(Z.W05);
        }
    }
    
    
    private static void ShowSubCells()
    {
        for (int i = 0; i < Frustum.cellCount; i++)
        {
            if (CellMaster.GetCellAt(Level.frustumCells[i].cellPos) == null|| Level.frustumCells[i].cell == null)
                continue;
            
            Cell cell = Level.frustumCells[i].cell;
            Cell.Content content = GameCam.CurrentSide.front ? cell.front : cell.back;

            if (content.itemCount == 0)
                continue;
            
            Vector2 cellPos = Level.frustumCells[i].cellPos;
            new Bounds2D(cellPos * Level.CellSize).
                Add((cellPos + V2.one) * Level.CellSize).
                DrawSubDiv(7, 7, .95f).
                SetColor(Color.white.PerlinColor(cellPos.x, cellPos.y, .3f)).
                SetDepth(Z.W + Mathf.Sign(Z.W) * 1).
                Fill(.75f, true);
        }
    }

    
    private static void ShowCellElementIDs()
    {
        InitStrings();

        Vector2 drawOffset = Vector2.one * Level.CellSize * .5f;
        for (int i = 0; i < Frustum.cellCount; i++)
        {
            if (CellMaster.GetCellAt(Level.frustumCells[i].cellPos) == null || Level.frustumCells[i].cell == null)
                continue;

            Cell cell = Level.frustumCells[i].cell;
            Cell.Content content = GameCam.CurrentSide.front ? cell.front : cell.back;
            
            cellStrings[i].Append("<color=white>").Append(Level.frustumCells[i].cellPos).Append("</color>\n");
            
            int rows = 1;

            if (showCellItems)
            {
                cellStrings[i].Append("<color=yellow>");
                Item[] items = content.soloItems;
                int count = content.soloItemCount;
                for (int j = 0; j < count; j++)
                {
                    if (items[j].side != GameCam.CurrentSide)
                        continue;

                    bool newRow = (j + 1) % 5 == 0 && j != 0;
                    bool createSpace = !newRow && j != count - 1;

                    cellStrings[i].Append(items[j].ID.ToString("D3"));

                    if (createSpace)
                        cellStrings[i].Append("  ");

                    if (newRow)
                    {
                        cellStrings[i].Append(System.Environment.NewLine);
                        rows++;
                    }
                }

                cellStrings[i].Append("</color>");
                cellStrings[i].Append(System.Environment.NewLine);
                
                if(showCellTracks)
                    rows++;
            }

            if (showCellTracks)
            {
                cellStrings[i].Append("<color=orange>");
                Track[] tracks = content.tracks;
                int trackCount = content.trackCount;
                for (int j = 0; j < trackCount; j++)
                {
                    if (tracks[j].side != GameCam.CurrentSide)
                        continue;

                    bool newRow = (j + 1) % 5 == 0 && j != 0;
                    bool createSpace = !newRow && j != trackCount - 1;

                    cellStrings[i].Append(tracks[j].ID.ToString("D3"));

                    if (createSpace)
                        cellStrings[i].Append("  ");

                    if (newRow)
                    {
                        cellStrings[i].Append(System.Environment.NewLine);
                        rows++;
                    }
                }

                cellStrings[i].Append("</color>");
            }

            DRAW.Text(cellStrings[i].ToString(), Level.frustumCells[i].cellPos * Level.CellSize + drawOffset, Color.white, 1.65f, 
                offset: V2.up * (-3 + rows / 1.65f * .55f), z: Z.W10);
        }
    }


    private static StringBuilder[] cellStrings;
    private static void InitStrings()
    {
        if (cellStrings == null)
        {
            cellStrings = new StringBuilder[Frustum.cellCount];
            for (int i = 0; i < Frustum.cellCount; i++)
                cellStrings[i] = new StringBuilder(200);
        }

        for (int i = 0; i < cellStrings.Length; i++)
            cellStrings[i].Length = 0;
    }


    private static void ShowItems()
    {
        for (int i = 0; i < Level.itemCount; i++)
        {
            Item item = Level.items[i];
            if (item.side.IsCamSide && (Creator.currentFilter.Fits(item.elementType) || LevelCheck.Overlapping(item)))
                item.Draw();
        }
    }
    
    
    private static void ShowTracks()
    {
        for (int i = 0; i < Level.trackCount; i++)
        {
            Track track = Level.tracks[i];
            if (track.side.IsCamSide)
                track.Draw();
        }
    }

    
    private static void ShowLinks()
    {
        Clip clip = Spinner.CurrentFocusClip;

        if (clip == null)
            return;
        
        Item swingItem = clip.Type.IsAnySwing()? ((Swing) clip).GetStick(GTime.Now).Item : null;

        List<Link> links = GameCam.CurrentSide.front ? Link.frontLinks : Link.backLinks;
        int count = links.Count;
        for (int i = 0; i < count; i++)
            links[i].Draw(swingItem);
    }


    private static void ShowStartStick()
    {
        if (Level.StartStick != null && Level.StartStick.side.IsCamSide)
        {
            Vector2 pos    = Level.StartStick.GetPos(GTime.Now);
            DRAW.ZappCircle(pos, 1f, .8f, 10, Time.realtimeSinceStartup * 60).SetDepth(Z.W05).Fill(.2f);
        }
    }


    private static void ShowOutOfBounds()
    {
        for (int i = 0; i < Level.trackCount; i++)
        {
            Track track = Level.tracks[i];
            if (track.side.IsCamSide)
                for (int j = 0; j < track.itemCount; j++)
                {
                    Vector2 pos = track.items[j].GetLagPos(GTime.Now);
                    Bounds2D b = new Bounds2D(pos).Pad(track.items[j].radius);
                    
                    if (!track.items[j].bounds.Contains(b))
                    {
                        float r = track.items[j].radius;
                        DRAW.MultiCircle(pos, r * 10, 4, r * 2, 16).SetColor(Color.red).SetDepth(Z.P);
                    }
                }
        }
    }


    public static void ShowHitPoint()
    {
        if (UI_Manager.HitUI || Input.GetKey(KeyCode.LeftAlt))
            return;

        Color c = LevelCheck.ClosestElement != null? LevelCheck.GetColor(LevelCheck.ClosestElement) : Color.white; 
        
        float dotRadius = Mathf.SmoothStep(.1f, .3f, Mathf.PingPong(Time.realtimeSinceStartup * 2, 1));
        DRAW.Circle(Level.HitPoint, dotRadius, 20).SetColor(c).SetDepth(Z.W05).Fill(1);

        if (LevelCheck.ClosestElement != null)
        {
            Vector2 dir    = LevelCheck.ElementPoint - Level.HitPoint;
            DRAW.Arrow(Level.HitPoint, dir, .2f).SetColor(c).SetDepth(Z.W05).Fill(1);
        }

        if (!LevelCheck.ClosestLink.Equals(Link.None))
        {
            Vector2 dir    = LevelCheck.LinkPoint - Level.HitPoint;
            DRAW.GapVector(Level.HitPoint, dir, 5).SetColor(Color.white).SetDepth(Z.W05).Fill(1);
        }
    }


    private static void ShowHouseGenRects()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        
        HouseGen gen = HouseGen.Inst;
        HouseRules rules = gen.gameObject.GetComponent<HouseRules>();
        
        int count;
        HouseGen.RectZone[] zones = gen.GetUseZones(out count);

        for (int i = 0; i < count; i++)
        {
            HouseGen.RectZone zone = zones[i];
            Bounds2D b = gen.GetRectZoneBounds(zone);
            
            if(GameCam.frustum.InFrustum(b, GameCam.CurrentSide.front))
                b.Pad(-.1f).Draw().SetColor(rules.GetZoneColor(zone.ArrayZone(GameCam.CurrentSide.front))).SetDepth(Z.W20);
        }
        
        #endif
    }
}
