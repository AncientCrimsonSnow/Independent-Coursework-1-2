using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Extensions.Ml_Agents.RenderTextureSensor
{
    [RequireComponent(typeof(BehaviorParameters))]
    [RequireComponent(typeof(RenderTextureSensorComponent))]
    public abstract class RenderTextureAgent<T, O, A, D> : Agent<T, O, A, D>
        where T : RenderTextureAgent<T, O, A, D>
        where O : struct, IObservation<O>
        where A : Action<A, D, O, T>, new()
        where D : struct, IActionData<D>
    {
        public RenderTextureConfig Config => _config;

        [SerializeField]
        RenderTextureConfig _config;
        [SerializeField]
        private RenderTexture _observationTexture;
        
        private RenderTextureSensorComponent _renderTextureSensor;

        protected internal override void SetupInternal()
        {
            _renderTextureSensor = GetComponent<RenderTextureSensorComponent>();
            var textureSize = _config.GetTextureSize();
            _observationTexture.Release();
            _observationTexture.height = textureSize.y;
            _observationTexture.width = textureSize.x;
            _observationTexture.Create();
            _renderTextureSensor.RenderTexture = _observationTexture;
        }
        
        protected RenderTexture GetObservationTexture()
        {
            return _observationTexture;
        }
    }
}