﻿namespace BinarySerializer.Klonoa.DTP
{
    public abstract class BaseEnemyData : BinarySerializable
    {
        public EnemyObject Pre_EnemyObj { get; set; }
        public LevelData2D Pre_LevelData2D { get; set; }

        public int DespawnDistance { get; set; } = -1; // The distance from Klonoa before despawning
        public EnemyWaypoint[] Waypoints { get; set; } = new EnemyWaypoint[0];
    }
}