﻿using System.Linq;

namespace BinarySerializer.KlonoaDTP
{
    public class LevelData2D : BinarySerializable
    {
        public Pointer[] DataPointers { get; set; }

        // Serialized from pointers
        public EnemyObjectIndexTables EnemyObjectIndexTables { get; set; }
        public EnemyObject[] EnemyObjects { get; set; }

        public ushort[] CollectibleSectorIndices { get; set; }
        public DreamStonesCollectiblesInfo[] DreamStonesCollectibleInfos { get; set; }
        public CollectibleObject[] CollectibleObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointers = s.SerializePointerArrayUntil(DataPointers, x => x == null, name: nameof(DataPointers));

            s.DoAt(DataPointers[0], () => EnemyObjectIndexTables = s.SerializeObject<EnemyObjectIndexTables>(EnemyObjectIndexTables, name: nameof(EnemyObjectIndexTables)));

            var enemyIndices = EnemyObjectIndexTables.IndexTables.SelectMany(x => x).ToArray();
            var enemyObjsCount = enemyIndices.Any() ? enemyIndices.Max() + 1 : 0;
            s.DoAt(DataPointers[40], () => EnemyObjects = s.SerializeObjectArray<EnemyObject>(EnemyObjects, enemyObjsCount, name: nameof(EnemyObjects)));

            s.DoAt(DataPointers[43], () => CollectibleSectorIndices = s.SerializeArray<ushort>(CollectibleSectorIndices, 11, name: nameof(CollectibleSectorIndices)));

            s.DoAt(DataPointers[44], () => DreamStonesCollectibleInfos = s.SerializeObjectArray<DreamStonesCollectiblesInfo>(DreamStonesCollectibleInfos, 10, name: nameof(DreamStonesCollectibleInfos)));

            var collectibleObjsCount = CollectibleSectorIndices[10];

            s.DoAt(DataPointers[42], () => CollectibleObjects = s.SerializeObjectArray<CollectibleObject>(CollectibleObjects, collectibleObjsCount, name: nameof(CollectibleObjects)));
        }
    }
}