using Unity.Mathematics;
using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for the <see cref="Camera"/> class to enhance functionality and simplify common operations.
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Calculates the viewport extents of a camera, including an optional margin.
        /// </summary>
        /// <param name="camera">The camera for which the viewport extents are calculated.</param>
        /// <param name="viewportMargin">An optional margin to add to the viewport extents. Defaults to a margin of 0.2f for both axes if not provided.</param>
        /// <returns>A <see cref="Vector2"/> representing the viewport extents with the applied margin.</returns>
        public static Vector2 GetViewportExtentsWithMargin(this Camera camera, Vector2? viewportMargin = null)
        {
            var margin = viewportMargin ?? new Vector2(0.2f, 0.2f);

            Vector2 result;
            var halfFieldOfView = camera.fieldOfView * 0.5f * math.TORADIANS;
            result.y = camera.nearClipPlane * math.tan(halfFieldOfView);
            result.x = result.y * camera.aspect + margin.x;
            result.y += margin.y;
            return result;
        }
    }
}