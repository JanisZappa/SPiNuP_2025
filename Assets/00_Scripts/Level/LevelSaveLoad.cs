using System.Collections;
using System.Collections.Generic;
using System.IO;
using LevelElements;
using UnityEngine;


public class LevelSaveLoad : MonoBehaviour, ILevelGenerator
{
    private const float checkTime = 20;

    
    public class LevelData
    {
        private LevelData(int levelID)
        {
            this.levelID = levelID;
        }
        
        private readonly int levelID;

        private Vector2 startPos  = new Vector2(0, 10);
        private Side    startSide = Side.Front;

        private Vector2 camPos;
        private float   camZoom;
        private Side    camSide;

        private struct ItemData
        {
            private readonly elementType itemType;
            private readonly Side        side;
            private readonly Vector2     pos;

            
            public ItemData(BinaryReader reader)
            {
                itemType = (elementType) reader.ReadInt32();
                side     = reader.ReadSide();
                pos      = reader.ReadVector2();
            }
            
            
            public ItemData(elementType itemType, Side side, Vector2 pos)
            {
                this.itemType = itemType;
                this.side     = side;
                this.pos      = pos;
            }
            
            
            public void SetupItem()
            {
                Item.GetFreeItem.SetType(itemType).SetRootPos(pos).SetSide(side).Refresh();
            }
        }
        
        
        private readonly List<ItemData> itemData = new List<ItemData>();


        private struct TrackData
        {
            private readonly Vector2       pos;
            private readonly Side          side;
            private readonly elementType   trackType;
            private readonly float         angle, size, offset, speed, completion;
            private readonly elementType[] itemTypes;
            

            public TrackData(BinaryReader reader)
            {
                int itemCount = reader.ReadInt32();
                itemTypes = new elementType[itemCount];
                for (int e = 0; e < itemCount; e++)
                    itemTypes[e] = (elementType) reader.ReadInt32();
                
                
                pos        = reader.ReadVector2();
                side       = reader.ReadSide();
                trackType  = (elementType) reader.ReadInt32();
                angle      = reader.ReadSingle();
                size       = reader.ReadSingle();
                offset     = reader.ReadSingle();
                speed      = reader.ReadSingle();
                completion = reader.ReadSingle();
            }
            
            
            public void SetupTrack()
            {
                if (itemTypes.Length == 0)
                    return;
                
                Track track = Track.GetNewTrack(trackType);
        
                track.SetRootPos(pos);
                track.angle  = angle;
                track.size   = size;
                track.offset = offset;
                track.side   = side;
                track.speed  = speed;
                track.growth = completion;
        
                for (int i = 0; i < itemTypes.Length; i++)
                {
                    Item item = Item.GetFreeItem;
                         item.SetType(itemTypes[i]);
                         
                    track.ParentThis(item);
                }

                track.ScanAndRefesh();
                track.Refresh();
            }
        }
        
        
        private readonly List<TrackData> trackData = new List<TrackData>();


        private struct LinkData
        {
            private readonly linkType type;

            private readonly bool    aIsItem, bIsItem;
            private readonly Vector2 aPos,  bPos;
            private readonly Side    aSide, bSide;

            
            public LinkData(BinaryReader reader)
            {
                type = (linkType) reader.ReadInt32();
                
                aIsItem = reader.ReadBoolean();
                aPos    = reader.ReadVector2();
                aSide   = reader.ReadSide();
                
                bIsItem = reader.ReadBoolean();
                bPos    = reader.ReadVector2();
                bSide   = reader.ReadSide();
            }


            public void SetupLink()
            {
                Element a = aIsItem ?
                    Search.ClosestItem(aPos, checkTime, aSide, Mask.AnyThing) :
                    (Element)Search.ClosestTrack(aPos, checkTime, aSide);
            
                Element b = bIsItem ?
                    Search.ClosestItem(bPos, checkTime, bSide, Mask.AnyThing) :
                    (Element)Search.ClosestTrack(bPos, checkTime, bSide);

                if (a == null || b == null)
                {
                    Debug.Log("Link Setup !!!".B_Red());
                    Debug.LogFormat("{0} and {1}", a == null ? "A is Null" : "A is Fine", b == null ? "B is Null" : "B is Fine");
                    Debug.LogFormat("{0} and {1}", aIsItem ? "A is Item" : "A is Track",  bIsItem ? "B is Item" : "B is Track");
                }
                
                Link.Create(type, a, b);
            }
        }
        private readonly List<LinkData> linkData = new List<LinkData>();

        
        public static LevelData New
        {
            get
            {
                LevelData levelData  = new LevelData(UI_LevelList.levelList.FirstFreeID);
                
                levelData.itemData.Add(new ItemData(elementType.Stick, levelData.startSide, levelData.startPos));
                
                return levelData;
            }
        }
        
        
        public static LevelData Create(int levelID, byte[] bytes)
        {
            LevelData load = new LevelData(levelID);

            using ( MemoryStream m = new MemoryStream(bytes) )
                using ( BinaryReader reader = new BinaryReader(m) )
                {
                    int itemCount = reader.ReadInt32();
                    for (int i = 0; i < itemCount; i++)
                        load.itemData.Add( new ItemData(reader));

                    
                    int trackCount = reader.ReadInt32();
                    for (int i = 0; i < trackCount; i++)
                        load.trackData.Add( new TrackData(reader));
        
                    
                    int connectionCount = reader.ReadInt32();
                    for (int i = 0; i < connectionCount; i++)
                        load.linkData.Add( new LinkData(reader));
        
                    
                    load.startPos  = reader.ReadVector2();
                    load.startSide = reader.ReadSide();
                    
                    
                    load.camPos  = reader.ReadVector2();
                    load.camZoom = reader.ReadSingle();
                    
                    load.camSide = reader.ReadSide();
                }

            return load;
        }

        
        public void InitLevel()
        {
            LevelSeed.LevelID = levelID;
        
            for (int i = 0; i < itemData.Count;  i++)
                itemData[i].SetupItem();

        
            for (int i = 0; i < trackData.Count; i++)
                trackData[i].SetupTrack();


            for (int i = 0; i < linkData.Count; i++)
                linkData[i].SetupLink();
        
            
            Level.StartStick = Search.ClosestItem(startPos, checkTime, startSide, Mask.AnyThing);
            
        //  Cam Values  //
            GameCam.SetStartValues(camPos, camZoom, camSide);
        }
        
        
        public static byte[] Save(Item startStick, Item[] items, Track[] tracks, Link[] links, Vector3 camPos, float zoom, Side side)
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                //  Items //
                    writer.Write(items.Length);
                    for (int i = 0; i < items.Length; i++)
                    {
                        writer.Write((int) items[i].elementType);
                        writer.Write(items[i].side);
                        writer.Write(items[i].rootPos);
                    }
                    
                //  Tracks // 
                    writer.Write(tracks.Length);
                    for (int i = 0; i < tracks.Length; i++)
                    {
                        writer.Write(tracks[i].itemCount);

                        for (int e = 0; e < tracks[i].itemCount; e++)
                            writer.Write((int) tracks[i].items[e].elementType);


                        writer.Write(tracks[i].rootPos);
                        writer.Write(tracks[i].side);
                        writer.Write((int) tracks[i].elementType);
                        writer.Write(tracks[i].angle);
                        writer.Write(tracks[i].size);
                        writer.Write(tracks[i].offset);
                        writer.Write(tracks[i].speed);
                        writer.Write(tracks[i].growth);
                    }

                //  Links  //
                    writer.Write(links.Length);
                    for (int i = 0; i < links.Length; i++)
                    {
                        writer.Write((int) links[i].linkType);
                        
                        writer.Write(Mask.AnyThing.Fits(links[i].a.elementType));
                        writer.Write(links[i].a.GetPos(checkTime));
                        writer.Write(links[i].a.side);
                        
                        writer.Write(Mask.AnyThing.Fits(links[i].b.elementType));
                        writer.Write(links[i].b.GetPos(checkTime));
                        writer.Write(links[i].b.side);
                    }

                    
                    writer.Write(startStick.GetPos(checkTime));
                    writer.Write(startStick.side);
                    
                    writer.Write(camPos);
                    writer.Write(zoom);
                    writer.Write(side);
                }

                return m.ToArray();
            }
        }
    }
    
    
    private static readonly BoolSwitch FirstLevel  = new("Level/First", true);
    private static readonly BoolSwitch RandomLevel = new ("Level/Random", false);

    private static LevelList levelList;
    public static LevelList LevelList => levelList ? levelList : levelList = Resources.Load("SavedLevels/_LevelList") as LevelList;

    public  static string CurrentLevel = "";
    private static string LastLevel    = "";
    public bool IsNew { private set; get; }
    private static LevelData data;

    
    public IEnumerator LoadGame()
    {
        if (!GameManager.IsCreator)
        {
            if (RandomLevel)
            {
                CurrentLevel = LevelList.GetOtherLevel("").name;
                yield break;
            }

            if (FirstLevel)
            {
                CurrentLevel = LevelList.GetFirstLevel().name;
                yield break;
            }
        }


        UI_Manager.ShowUI(UIType.LevelList, true);
        
        while (CurrentLevel == "")
            yield return null;
        
        UI_Manager.ShowUI(UIType.LevelList, false);
    }
    

    public void StartGame()
    {
        if (LastLevel == CurrentLevel)
        {
            IsNew = false;
            return;
        }
        
        IsNew = true;
           
        Element.ClearAll();
        
        CellMaster.ClearCells();
        
        
        if (CurrentLevel == UI_LevelList.NewLevel)
        {
            LevelData.New.InitLevel();
            return;
        }


        if (LastLevel != CurrentLevel)
        {
            LevelSaveFile saveFile = LevelList.GetLevel(CurrentLevel);
            data = LevelData.Create(saveFile.levelID, saveFile.bytes);
        }

        data.InitLevel();
        
        LastLevel = CurrentLevel;
    }


    public static void NewLevel()
    {
        CellMaster.ClearCells();
        CurrentLevel = "";
    }


//  Save  //
    public static void SaveLevel(string saveName)
    {
    //  Getting Just solo Items  //
        List<Item> activeItems = Item.active;
       
        int iCount = activeItems.Count;
        for (int i = 0; i < iCount; i++)
            if (activeItems[i].parent != null)
            {
                activeItems.RemoveAt(i);
                iCount--;
                i--;
            }
        
        
    //  Tracks  //
       List<Track> activeTracks = Track.active;
       List<Link>  allLinks     = Link.GetAllToSave;
        
        Debug.LogFormat("{0} Items Total". B_Orange(), Item.Count);
        Debug.LogFormat("{0} Items Saved". B_Orange(), activeItems.Count);
        Debug.LogFormat("{0} Tracks Saved".B_Yellow(), activeTracks.Count);
        Debug.LogFormat("{0} Links Saved". B_Pink(),   allLinks.Count);
        Debug.LogFormat("Cam Pos: {0}".    B_Salmon(), GameCam.CurrentPos);
        Debug.LogFormat("Cam Zoom: {0}".   B_Salmon(), GameCam.CurrentDolly);
        Debug.LogFormat("Cam Side: {0}".   B_Salmon(), GameCam.CurrentSide);


        List<string> bounds = new List<string>();
        for (int i = 0; i < iCount; i++)
        {
            Item item = activeItems[i];
            
            if(Mask.Debug.Fits(item.elementType))
                bounds.Add(item.bounds.GetString() + item.GetIdAndSideInfo());
        }
            
        
        int tCount = activeTracks.Count;
        for (int i = 0; i < tCount; i++)
        {
            Track track = activeTracks[i];

            for (int e = 0; e < track.itemCount; e++)
                if (Mask.Debug.Fits(track.items[e].elementType))
                    goto GetSubbounds;
                
            continue;
            
            
            GetSubbounds:
            
            string trackInfo = track.GetIdAndSideInfo();

            for (int s = 0; s < track.subBoundCount; s++)
                bounds.Add(track.GetSubBound(s).bounds.GetString() + trackInfo);
        }
                
        
        ResourceTxt.Write("AllBounds", bounds.ToArray());
        
        levelList.SaveLevel(saveName, LevelData.Save(Level.StartStick, activeItems.ToArray(), activeTracks.ToArray(),  allLinks.ToArray(), GameCam.CurrentPos, GameCam.CurrentDolly, GameCam.CurrentSide));
    }

    
    public static bool NewSaveFile(string levelName)
    {
        return levelList.IsNewLevelSave(levelName);
    }
}