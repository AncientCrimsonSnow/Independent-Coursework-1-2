using Extensions.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Ml_Agents.RenderTextureSensor.RenderTextureToRenderTexture
{
    public class RenderTextureAgentInOutView
    {
        private readonly RawImage _input;
        private readonly RawImage _output;
        private readonly Color[] _clusterColors;
        private readonly Vector2Int _textureSize;

        public RenderTextureAgentInOutView(RawImage input, RawImage output, int colorsCount, Vector2Int textureSize)
        {
            _input = input;
            _output = output;
            _clusterColors = Utilities.GetMaxDistributedColors(colorsCount);
            _textureSize = textureSize;
            PositionImages();
        }
        
        public void SetTextureOutput(int[,] data)
        {
            if(!Application.isEditor) return;
            
            var texture = new Texture2D(_textureSize.x ,_textureSize.y);
            _output.texture = texture;
            
            for(var y = 0; y != _textureSize.y; y++)
            {
                for (var x = 0; x != _textureSize.x; x++)
                {
                    var clusterIndex = data.GetElementAt(x,y);
                    texture.SetPixel(x, y, _clusterColors[clusterIndex]);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();
        }
        private void PositionImages()
        {
            _input.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _input.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            
            _output.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _output.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            
            var xRatio = Screen.width/(float)_textureSize.x;
            var yRatio = Screen.height/(float)_textureSize.y;

            if (xRatio > yRatio)
            {
                //nebeneinander
                _input.rectTransform.pivot = new Vector2(1f,0.5f);
                _output.rectTransform.pivot = new Vector2(0f,0.5f);
                
                xRatio = Screen.width/(_textureSize.x * 2f);
            }
            else
            {
                //übereinander
                _input.rectTransform.pivot = new Vector2(0.5f,0f);
                _output.rectTransform.pivot = new Vector2(0.5f,1f);
                yRatio = Screen.height/(_textureSize.y * 2f);
                
            }
            
            if (xRatio > yRatio)
            {
                _input.rectTransform.sizeDelta = new Vector2(_textureSize.x, _textureSize.y) * yRatio; 
                _output.rectTransform.sizeDelta = new Vector2(_textureSize.x, _textureSize.y) * yRatio;
            }
            else
            {
                _input.rectTransform.sizeDelta = new Vector2(_textureSize.x, _textureSize.y) * xRatio; 
                _output.rectTransform.sizeDelta = new Vector2(_textureSize.x, _textureSize.y) * xRatio;
            }
            
            _input.rectTransform.anchoredPosition = Vector2.zero;
            _output.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}