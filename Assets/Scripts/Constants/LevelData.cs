using System.Collections.Generic;

public class LevelData
{
    public LevelData()
    {
        Tiles = new List<TileData>();
    }

    public LevelSong Song { get; set; }
    public LevelSkin Skin { get; set; }
    public List<TileData> Tiles { get; set; }
}

public class TileData
{
    public TileType Type { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public string Properties { get; set; }
}

public enum LevelSong
{
    None = 0,
    Song1 = 1,
    Song2 = 2
}

public enum LevelSkin
{
    Sky = 0,
    Desert = 1,
    Grass = 2,
    HauntHouse = 3
}

public enum TileType
{
    None = 0,
    PlayerSpawn = 1,
    BreakableWallBrick = 2,
    UnbreakableFloor1 = 3,

    //LevelExit = 100,
}