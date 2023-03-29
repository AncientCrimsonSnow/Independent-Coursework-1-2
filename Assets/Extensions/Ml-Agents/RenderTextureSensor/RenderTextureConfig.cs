using System;
using UnityEngine;

namespace Extensions.Ml_Agents.RenderTextureSensor
{
    [Serializable]
    public struct RenderTextureConfig
    {
        [SerializeField, Range(20f, 10000f)]
        private int _subdivisionsX;
        [SerializeField, Range(20f, 10000f)]
        private int _subdivisions_Y;
        
        public Vector2Int GetTextureSize()
        {
            return new Vector2Int(_subdivisionsX, _subdivisions_Y);
        }
    }
}