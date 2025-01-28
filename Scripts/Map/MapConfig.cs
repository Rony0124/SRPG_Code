using System.Collections;
using System.Collections.Generic;
using OneLine;
using UnityEngine;

namespace TSoft.Map
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        public List<NodeBlueprint> nodeBlueprints;
        [Tooltip("Nodes that will be used on layers with Randomize Nodes > 0")]
        public List<NodeType> randomNodes = new List<NodeType>
            {NodeType.Store, NodeType.Treasure, NodeType.MinorEnemy};
        public int GridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

        public Sprite stageBackground;
        
        public IntMinMax numOfPreBossNodes;
        public IntMinMax numOfStartingNodes;

        [Tooltip("Increase this number to generate more paths")]
        public int extraPaths;
        public List<MapLayer> layers;
    }
}
