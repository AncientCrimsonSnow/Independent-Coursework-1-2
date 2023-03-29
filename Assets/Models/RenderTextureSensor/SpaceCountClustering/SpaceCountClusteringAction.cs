using Extensions.Math;
using Extensions.Ml_Agents;
using Extensions.Ml_Agents.VectorSensor;
using Unity.Mathematics;
using Unity.MLAgents.Actuators;

namespace Models.RenderTextureSensor.SpaceCountClustering
{
    public class SpaceCountClusteringAction : Action<
        SpaceCountClusteringAction,
        SpaceCountClusteringActionData,
        EmptyVectorObservation,
        SpaceCountClusteringAgent>
    {
        
        public SpaceCountClusteringAction(ActionBuffers actions, SpaceCountClusteringAgent agent) : base(actions, agent)
        { }

        public SpaceCountClusteringAction()
        {
            
        }


        public override int GetNumberOfContinuesActions()
        {
            return 0;
        }

        public override int[] GetBranchSizes(SpaceCountClusteringAgent agent)
        {
            var textureSize = agent.Config.GetTextureSize();
            var result = new int[textureSize.x * textureSize.y];
            for(var i = 0; i != result.Length; i++)
            {
                result[i] = agent.ClusterCount;
            }
            return result;
        }

        public override void ApplyAction(SpaceCountClusteringAgent agent)
        {
            var reward = CalculateReward(data.actionTextureData, agent.LastObservationData, agent.ClusterCount);
            agent.SetReward(reward);
            agent.RenderTextureAgentInOutView.SetTextureOutput(data.actionTextureData);
        }

        protected override SpaceCountClusteringActionData GetData(float[] continuesActions, int[] discreteActions, SpaceCountClusteringAgent agent)
        {
            var textureSize = agent.Config.GetTextureSize();
            return new SpaceCountClusteringActionData(discreteActions.Convert1DTo2D(textureSize.x, textureSize.y));
        }
        
        public float CalculateReward(in int[,] actions, in int[,] observations, int clusterCount)
        {
            var rewardParameter = CalculateRewardParameter(actions, observations, clusterCount);
            
            var highestCount = 0;
            var lowestCount = int.MaxValue;
            var reward = 0;
            var totalReward = 0f;
            
            for (var i = 0; i != clusterCount; i++)
            {
                if(rewardParameter.counts[i] < lowestCount)
                    lowestCount = rewardParameter.counts[i];
                if(rewardParameter.counts[i] > highestCount)
                    highestCount = rewardParameter.counts[i];
                
                reward += rewardParameter.neighbourBonus[i];
                totalReward -= rewardParameter.averageDeviationsSquared[i]  * 3;
                totalReward -= rewardParameter.highestDeviationsSquared[i]  * 3;
            }
            var diff = highestCount-lowestCount;
            if (diff > 1)
            {
                reward /= diff;
            }
            totalReward += reward;
            
            return totalReward;
        }
        
        private RewardParameter CalculateRewardParameter(in int[,] actions, in int[,] observations, int clusterCount)
        {
            var averageDeviationsSquared = new float[clusterCount];
            var highestDeviationsSquared = new float[clusterCount];
            var counts = new int[clusterCount];
            var neighbourBonus = new int[clusterCount];

            var sumsPosPerCluster = new int2[clusterCount];
            var countCellsPerCluster = new int[clusterCount];
            
            for(var y = 0; y < actions.GetLength(0); y++)
            {
                for(var x = 0; x < actions.GetLength(1); x++)
                {
                    var clusterIndex = actions[y,x];
                    var cellPos = new int2(x, y);
                    
                    neighbourBonus[clusterIndex] += CalcNeighbourBonusForCell(clusterIndex, cellPos, actions);
                    counts[clusterIndex] += observations.GetElementAt(x,y);
                    
                    sumsPosPerCluster[clusterIndex] += cellPos;
                    countCellsPerCluster[clusterIndex]++;
                }
            }
            
            var averagePos = CalcAverageClusterPos(sumsPosPerCluster, countCellsPerCluster, clusterCount);
            
            var sumsDeviationsSquared = new float[clusterCount];
            
            for(var y = 0; y < actions.GetLength(0); y++)
            {
                for(var x = 0; x < actions.GetLength(1); x++)
                {
                    var observationElement = observations.GetElementAt(x,y);
                    if(observationElement == 0)
                        continue;
                    
                    var clusterIndex = actions[y,x];
                    var deviationSquared = math.distancesq(averagePos[clusterIndex], new int2(x,y));
                    
                    if(deviationSquared > highestDeviationsSquared[clusterIndex])
                        highestDeviationsSquared[clusterIndex] = deviationSquared;
                    
                    sumsDeviationsSquared[clusterIndex] += deviationSquared * observationElement;
                }
            }
            
            for (var i = 0; i != clusterCount; i++)
                if(counts[i] != 0 )
                    averageDeviationsSquared[i] = sumsDeviationsSquared[i]/counts[i];
            
            return new RewardParameter(
                averageDeviationsSquared,
                highestDeviationsSquared,
                counts,
                neighbourBonus
            );
        }
        
        private float2[] CalcAverageClusterPos(in int2[]sumsPosPerCluster, in int[] countCellsPerCluster, int clusterCount)
        {
            var result = new float2[clusterCount];
            for (var i = 0; i != clusterCount; i++)
            {
                if(countCellsPerCluster[i] != 0 )
                    result[i] = (float2)sumsPosPerCluster[i]/countCellsPerCluster[i];
            }

            return result;
        }
        
        private int CalcNeighbourBonusForCell(in int clusterIndex, in int2 cellPos, in int[,] actions)
        {
            var result = 0;
            
            var dimensionsLengths = new int2
            {
                x = actions.GetLength(1),
                y = actions.GetLength(0)
            };
            
            for (var y = -1; y != 2; y++)
            {
                for (var x = -1; x != 2; x++)
                {
                    if(x == 0 && y == 0)
                        continue;
                    
                    var neighbourCellPos = new int2
                    {
                        x = cellPos.x + x,
                        y = cellPos.y + y
                    };
                    
                    if(!IsInBounds(neighbourCellPos, dimensionsLengths))
                        continue;
                    
                    var neighbourClusterIndex = actions[neighbourCellPos.y, neighbourCellPos.x];
                    if (x == 0 || y == 0)
                    {
                        if(clusterIndex == neighbourClusterIndex)
                            result += 2;
                    }
                    else
                    {
                        if(clusterIndex == neighbourClusterIndex)
                            result += 1;
                    }
                }
            }
            return result;
        }
        
        private bool IsInBounds(in int2 pos, in int2 dimensionLengths)
        {
            return pos.x >= 0 &&
                   pos.y >= 0 &&
                   pos.x < dimensionLengths.x &&
                   pos.y < dimensionLengths.y;
        }

        public struct RewardParameter
        {
            public readonly float[] averageDeviationsSquared;
            public readonly float[] highestDeviationsSquared;
            public readonly int[] counts;
            public readonly int[] neighbourBonus;

            public RewardParameter(float[] averageDeviationsSquared, float[] highestDeviationsSquared, int[] counts, int[] neighbourBonus)
            {
                this.averageDeviationsSquared = averageDeviationsSquared;
                this.highestDeviationsSquared = highestDeviationsSquared;
                this.counts = counts;
                this.neighbourBonus = neighbourBonus;
            }
        }
    }
}