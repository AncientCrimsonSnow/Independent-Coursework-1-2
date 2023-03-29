using Extensions.Math;
using Extensions.Ml_Agents.RenderTextureSensor;
using Extensions.Ml_Agents.RenderTextureSensor.RenderTextureToRenderTexture;
using Extensions.Ml_Agents.VectorSensor;
using Unity.Mathematics;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Models.RenderTextureSensor.SpaceCountClustering
{
    public class SpaceCountClusteringAgent : RenderTextureAgent<
        SpaceCountClusteringAgent,
        EmptyVectorObservation,
        SpaceCountClusteringAction,
        SpaceCountClusteringActionData>
    {
        public int ClusterCount => Mathf.CeilToInt(_totalEntityCount/(float)_entitiesPerCluster);
        public RenderTextureAgentInOutView RenderTextureAgentInOutView => _renderTextureAgentInOutView;
        public int[,] LastObservationData { get; private set; }

        [SerializeField]
        private int _totalEntityCount;
        [SerializeField]
        private int _entitiesPerCluster;
        [SerializeField]
        private RawImage _inputView;
        [SerializeField]
        private RawImage _outputView;

        
        private RenderTextureAgentInOutView _renderTextureAgentInOutView;

        protected override void Setup()
        {
            _renderTextureAgentInOutView = new RenderTextureAgentInOutView(_inputView, _outputView, _entitiesPerCluster, Config.GetTextureSize());
        }

        protected override void OnBeforeCollectObservation()
        {
            LastObservationData = GetSrcTextureData(_totalEntityCount, Config.GetTextureSize());
            LastObservationData.PrintOnRenderTexture(GetObservationTexture());
        }

        protected override SpaceCountClusteringActionData GetHeuristicData()
        {
            var textureSize = Config.GetTextureSize();
            var heuristicTextureData = new int[textureSize.y, textureSize.x];
            for (var y = 0; y != textureSize.y; y++)
            {
                for (var x = 0; x != textureSize.x; x++)
                {
                    heuristicTextureData.SetElementAt(x, y , Random.Range(0, ClusterCount));
                }
            }
            return new SpaceCountClusteringActionData(heuristicTextureData);
        }

        protected override void Heuristic(SpaceCountClusteringActionData data, in ActionSegment<float> continuesActions,
            in ActionSegment<int> discreteActions)
        {
            var textureData = data.actionTextureData;
            for (var y = 0; y < textureData.GetLength(1); y++)
            {
                for (var x = 0; x < textureData.GetLength(0); x++)
                {
                    discreteActions[y * textureData.GetLength(0) + x] = textureData[x, y];
                }
            }
        }

        protected override void OnAfterApplyAction(SpaceCountClusteringAction action)
        {
            MyEndEpisode();
            StartLearning();
        }

        private static int[,] GetSrcTextureData(int totalEntityCount, Vector2Int textureSize)
        {
            var result = new int[textureSize.y, textureSize.x];
            
            var perlinSeed = new float2
            {
                x = Random.Range(0, 1000000),
                y =  Random.Range(0, 1000000)
            };
            for (var i = 0; i != totalEntityCount; i++)
            {
                while (true)
                {
                    var randX = Random.Range(0, textureSize.x);
                    var randY = Random.Range(0, textureSize.y);
                    var setChance = Mathf.PerlinNoise(perlinSeed.x + randX, perlinSeed.y + randY);
                    var chance = Random.value;
                    if (chance <= setChance)
                    {
                        var moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                        moveVector = moveVector.normalized * Random.Range(0f, (textureSize.x + textureSize.y) / 4f);
                        var x =  (int)Mathf.Clamp(randX + moveVector.x,0, textureSize.x - 1);
                        var y =  (int)Mathf.Clamp(randY + moveVector.y,0, textureSize.y - 1);
                        result[y, x]++;
                        break;
                    }
                }
            }
            return result;
        }

        protected override EmptyVectorObservation GetObservation()
        {
            return new EmptyVectorObservation();
        }

        protected override SpaceCountClusteringAction GetAction(ActionBuffers actions)
        {
            return new SpaceCountClusteringAction(actions, this);
        }
    }
}