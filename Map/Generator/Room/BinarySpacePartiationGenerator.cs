using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
using Roguelike.Map.Generator;
using Roguelike.Map.Model;

public partial class BinarySpacePartiationGenerator : BasicRoomPlacementGenerator
{
    /*
    protected async Task PlaceRooms()
    {
        // Calculate Depth of BSP Algorithm such that it should faciltiate the chosen number of rooms.
        double preciseBinaryDepth = Math.Log2(NumberOfRooms);
        int minBinaryDepth = (int) Math.Ceiling(preciseBinaryDepth);
        
        TileType floorTileType = TileTypes.FindByName(TileType_Floor);

        for (int roomCounter = 0; roomCounter < NumberOfRooms; roomCounter++)
        {
            
        }
    }
    */

    private RectangleRoom[] Subdivide(RectangleRoom levelArea)
    {
        int dividerX = levelArea.TopLeft.X + (int) Math.Ceiling(levelArea.Size.X / 2.0);
        int dividerY = levelArea.TopLeft.Y + (int)Math.Ceiling(levelArea.Size.Y / 2.0);

        RectangleRoom[] divisions = { new RectangleRoom(), new RectangleRoom() }; 
        divisions[0].TopLeft = new Vector2I(levelArea.TopLeft.X, levelArea.TopLeft.Y);
        divisions[1].TopLeft = new Vector2I(dividerX + 1, dividerY + 1);

        divisions[0].Size = new Vector2I(dividerX - levelArea.TopLeft.X, dividerY - levelArea.TopLeft.Y);
        divisions[1].Size = new Vector2I(levelArea.Size.X - (dividerX + 1), levelArea.Size.Y - (dividerY + 1));

        return divisions;
    }


}
