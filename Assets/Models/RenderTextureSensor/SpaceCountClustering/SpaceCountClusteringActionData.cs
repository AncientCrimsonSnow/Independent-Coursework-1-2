using Extensions.Ml_Agents;

namespace Models.RenderTextureSensor.SpaceCountClustering
{
    public struct SpaceCountClusteringActionData : IActionData<SpaceCountClusteringActionData>
    {
        public readonly int[,] actionTextureData;

        public SpaceCountClusteringActionData(int[,] actionTextureData)
        {
            this.actionTextureData = actionTextureData;
        }
    }
}