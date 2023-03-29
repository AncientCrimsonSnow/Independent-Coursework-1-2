using UnityEngine;

namespace Extensions.Ml_Agents
{
    public class Arena2 : MonoBehaviour
    {
        public Vector2 Size => new (width, height);
        public Vector2 Center => transform.position;
        
        [SerializeField]
        private float width;
        [SerializeField]
        private float height;
        
        [SerializeField]
        private Wall _wallPrefab;
        
        private void Awake()
        {
            var wallCollider = _wallPrefab.GetComponent<BoxCollider2D>();
            GetColliderDimensionsNormalized(wallCollider, out var length, out var thickness); 
            
            var horizontalLengthRatio = width/length;
            var verticalLengthRatio = height/length;
            
            var horizontalOffSet = (width + thickness) / 2;
            var verticalOffset = (height + thickness) / 2;
            
            var leftWall = Instantiate(wallCollider, transform);
            leftWall.name = "leftWall";
            var leftWallOffset = new Vector3(-horizontalOffSet, 0, 0);
            leftWall.transform.position = transform.position + leftWallOffset;
            RotateColliderToVertical(leftWall);
            ScaleObjectAtLength(leftWall, verticalLengthRatio);
            
            var rightWall = Instantiate(wallCollider, transform);
            rightWall.name = "rightWall";
            var rightWallOffset = new Vector3(horizontalOffSet, 0, 0);
            rightWall.transform.position = transform.position + rightWallOffset;
            RotateColliderToVertical(rightWall);
            ScaleObjectAtLength(rightWall, verticalLengthRatio);
            
            var topWall = Instantiate(wallCollider, transform);
            topWall.name = "topWall";
            var topWallOffset = new Vector2(0, verticalOffset);
            topWall.transform.position = transform.position + topWallOffset.ToVector3();
            RotateColliderToHorizontal1(topWall);
            ScaleObjectAtLength(topWall, horizontalLengthRatio);
            
            var botWall = Instantiate(wallCollider, transform);
            botWall.name = "botWall";
            var botWallOffset = new Vector2(0, -verticalOffset);
            botWall.transform.position = transform.position + botWallOffset.ToVector3();
            RotateColliderToHorizontal1(botWall);
            ScaleObjectAtLength(botWall, horizontalLengthRatio);
        }

        private static void GetColliderDimensionsNormalized(BoxCollider2D collider, out float length, out float thickness)
        {
            GetColliderDimensions(collider, out length, out thickness);

            if (length < thickness)
            {
                // Swap length and thickness if necessary
                (length, thickness) = (thickness, length);
            }
        }

        private static void GetColliderDimensions(BoxCollider2D collider, out float length, out float thickness)
        {
            var size = collider.size;
            length = size.x;
            thickness = size.y;
        }
        
        private static void RotateColliderToHorizontal1(BoxCollider2D collider)
        {
            var rotation = collider.transform.eulerAngles.z;
            var angle = Mathf.DeltaAngle(rotation, 0f);

            GetColliderDimensions(collider, out var length, out var thickness);

            if (thickness < length)
            {
                angle += 90f;
            }

            collider.transform.Rotate(new Vector3(0, 0, angle));
        }
        
        private static void RotateColliderToVertical(BoxCollider2D collider)
        {
            var rotation = collider.transform.eulerAngles.z;
            var angle = Mathf.DeltaAngle(rotation, 90f);

            GetColliderDimensions(collider, out var length, out var thickness);

            if (length < thickness)
            {
                angle += 90f;
            }

            collider.transform.Rotate(new Vector3(0, 0, angle));
        }
        
        private static void ScaleObjectAtLength(BoxCollider2D collider, float scale)
        {
            GetColliderDimensions(collider, out var length, out var thickness);

            Vector3 scaleVector;

            if (length >= thickness)
            {
                scaleVector = new Vector3(scale, 1f, 1f);
            }
            else
            {
                collider.transform.Rotate(new Vector3(0, 0, 90f));
                scaleVector = new Vector3(1f, scale, 1f);
            }

            collider.transform.localScale = Vector3.Scale(collider.transform.localScale, scaleVector);
        }
    }
}