using Generation;
using GeoMath;
using UnityEngine;


public static class CellMaster
{
    static CellMaster()
    {
        Cells = CollectionInit.Array<Cell>(Level.cellCount);
    }
    
    private static readonly Cell[] Cells;

    
    public static void ClearCells()
    {
        for (int i = 0; i < Level.cellCount; i++)
            Cells[i].Clear();
    }
    
    
    public static Cell GetCellAt(Vector2Int cellPos)
    {
        return ValidCell(cellPos)? 
            Cells[cellPos.x - Level.cellXmin + (cellPos.y - Level.cellYmin) * Level.xCells] : 
            null;
    }


    private static bool ValidCell(Vector2Int cellPos)
    {
        return !(cellPos.x >= Level.cellXmax || cellPos.x < Level.cellXmin || 
                 cellPos.y >= Level.cellYmax || cellPos.y < Level.cellYmin);
    }
    
    
    public static void ClearOccluders()
    {
        for (int i = 0; i < Level.cellCount; i++)
            Cells[i].ClearOccluders();
    }
    
    
    public static int GetBoundCells(Bounds2D bounds, Cell[] boundCells)
    {
        const float multi = 1f / Level.CellSize;
        int minX = Mathf.Max(Level.cellXmin, Mathf.FloorToInt(bounds.minX * multi));
        int maxX = Mathf.Min(Level.cellXmax, Mathf.FloorToInt(bounds.maxX * multi));
        
        int minY = Mathf.Max(Level.cellYmin, Mathf.FloorToInt(bounds.minY * multi));
        int maxY = Mathf.Min(Level.cellYmax, Mathf.FloorToInt(bounds.maxY * multi));

        int xRange = maxX - minX + 1;
        int yRange = maxY - minY + 1;

        int count = 0;

        for (int y = 0; y < yRange; y++)
        for (int x = 0; x < xRange; x++)
        {
            Cell cell = GetCellAt(new Vector2Int(minX + x, minY + y));
            if (cell != null)
                boundCells[count++] = cell;
        }
        
        return count;
    }
}
