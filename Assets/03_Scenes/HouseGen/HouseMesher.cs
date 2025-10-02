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
    
    
    
    private void Awake()
    {
        gen    = GetComponent<HouseGen>();
        piecer = GetComponent<HousePiecePlacer>();
    }

    
	public void PlaceMeshes()
    {
        if (!placeMeshes)
            return;
        
        Level.ClearPieces();
        PlacerMeshes.Clear(cellSize);

        PlacePieces(piecer.fills);
        PlacePieces(piecer.corners);
        PlacePieces(piecer.edges);
        PlacePieces(piecer.seamFrontCorners);
        PlacePieces(piecer.seamBackCorners);
        PlacePieces(piecer.seams);

        //PlacerMeshes.Report();
    }


    private void PlacePieces(List<Piece> pieces)
    {
        int fillCount = pieces.Count;
        for (int i = 0; i < fillCount; i++)
        {
            Piece piece = pieces[i];
            bool foundPiece = true;
            PlacerMeshes.Place(piece, gen, piece.side, ref foundPiece);
        } 
    }
}
