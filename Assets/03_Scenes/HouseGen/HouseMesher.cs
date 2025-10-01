using System.Collections.Generic;
using UnityEngine;

using Piece     = HouseGen.Piece;
using CornerIDs = HouseGen.CornerIDs;


public class HouseMesher : MonoBehaviour 
{
    public bool placeMeshes;
    public PlacerMeshes meshes;

    [HideInInspector] public HouseGen gen;
    private HousePiecePlacer piecer;
    
    private const float cellSize = HouseGen.cellSize;
    
#if UNITY_EDITOR
    private readonly List<Piece> failedPieces = new List<Piece>();
    private readonly List<CornerIDs> failedCornerIds = new List<CornerIDs>();
    private int failedPiecePick;
#endif
    
    
    private void Awake()
    {
        gen    = GetComponent<HouseGen>();
        piecer = GetComponent<HousePiecePlacer>();
    }

    
	public void PlaceMeshes()
    {
        if (!placeMeshes)
            return;
        
#if UNITY_EDITOR
        failedPieces.Clear();
        failedPiecePick = 0;
#endif
        Level.ClearPieces();
        PlacerMeshes.Clear(cellSize);

        PlacePieces(piecer.fills);
        PlacePieces(piecer.corners);
        PlacePieces(piecer.edges);
        PlacePieces(piecer.seamFrontCorners);
        PlacePieces(piecer.seamBackCorners);
        PlacePieces(piecer.seams);

#if UNITY_EDITOR
        int failedCount = failedPieces.Count;
        if (failedCount > 0)
        {
            Debug.LogFormat("Couldn't mesh {0} Pieces !".B_Red(), failedCount);
            
            failedCornerIds.Clear();

            for (int i = 0; i < failedCount; i++)
            {
                CornerIDs cIDs = failedPieces[i].cornerIDs;
                if (!failedCornerIds.Contains(cIDs))
                {
                    failedCornerIds.Add(cIDs);
                    
                    Debug.Log(cIDs.Log().B_Orange());
                }
            }  
        }
#endif
		
        PlacerMeshes.Report();
    }


    private void PlacePieces(List<Piece> pieces)
    {
        int fillCount = pieces.Count;
        for (int i = 0; i < fillCount; i++)
        {
            Piece piece = pieces[i];
            
            bool foundPiece = true;
            
            PlacerMeshes.Place(piece, gen, piece.side, ref foundPiece);
            
        #if UNITY_EDITOR
            if(!foundPiece)
                failedPieces.Add(piece);
        #endif
        } 
    }

    
    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Piece piece = failedPieces[failedPiecePick];
            GameCam.CheckThisOut(piece.GetBounds(gen).Center, piece.side == 0? GameCam.CurrentSide.front : piece.side == -1);
            EditorCamControll.SetEditorFocus();
            
            failedPiecePick = (failedPiecePick + 1).Repeat(failedPieces.Count);
            
            Debug.Log(piece.Log().B_Red());
        }
    }
    #endif
}
