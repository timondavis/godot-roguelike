using System;
using System.Collections.Generic;
using Godot;
using Roguelike.Script.Map.Model.Direction.Pattern.HexTile;
using Roguelike.Script.Map.Model.Direction.Pattern.RectangleTile;

namespace Roguelike.Script.Map.Model.Direction.Pattern;

public class DirectionalPatternFactory
{
    public DirectionalPattern Generate(TileSet.TileShapeEnum tileShape, DirectionalPatternEnum pattern)
    {
        DirectionalPattern selectedPattern;
        switch (tileShape)
        {
            case (TileSet.TileShapeEnum.Hexagon):
                switch (pattern)
                {
                    case(DirectionalPatternEnum.Plus):
                        selectedPattern = new HexPatternPlus();
                        break;
                    case(DirectionalPatternEnum.Star):
                        selectedPattern = new HexPatternStar();
                        break;
                    case(DirectionalPatternEnum.Surround):
                        selectedPattern = new HexPatternSurround();
                        break;
                    default:
                        throw new ArgumentException("Invalid patternname parameter: " + pattern.ToString());
                }
                break;
            case (TileSet.TileShapeEnum.Square):
                switch (pattern)
                {
                    case(DirectionalPatternEnum.Plus):
                        selectedPattern = new SquarePatternPlus();
                        break;
                    case(DirectionalPatternEnum.Star):
                        selectedPattern = new SquarePatternStar();
                        break;
                    case(DirectionalPatternEnum.Surround):
                        selectedPattern = new SquarePatternSurround();
                        break;
                    default:
                        throw new ArgumentException("Invalid patternname parameter: " + pattern.ToString());
                }

                break;
        default:
                throw new Exception("Invalid tileshape parameter: " + tileShape.ToString());
        }

        return selectedPattern;
    }
}