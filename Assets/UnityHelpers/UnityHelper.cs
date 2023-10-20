using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Common helper methods
/// </summary>
public static class UnityHelper
{
    
    /// <summary>
    /// Finds the first object hit from a raycast based on the mouse position.
    /// </summary>
    /// <returns></returns>
    public static RaycastHit CastRay()
    {
        Vector3 screenMousePositionFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);

        Vector3 screenMousePositionNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 worldMousePointerFar = Camera.main.ScreenToWorldPoint(screenMousePositionFar);
        Vector3 worldMousePointerNear = Camera.main.ScreenToWorldPoint(screenMousePositionNear);

        RaycastHit hit;

        Physics.Raycast(worldMousePointerNear, worldMousePointerFar - worldMousePointerNear, out hit);

        return hit;
    }

    public static RaycastHit CastRay(Camera camera)
    {
        Vector3 screenMousePositionFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            camera.farClipPlane);

        Vector3 screenMousePositionNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            camera.nearClipPlane);

        Vector3 worldMousePointerFar = camera.ScreenToWorldPoint(screenMousePositionFar);
        Vector3 worldMousePointerNear = camera.ScreenToWorldPoint(screenMousePositionNear);

        RaycastHit hit;

        Physics.Raycast(worldMousePointerNear, worldMousePointerFar - worldMousePointerNear, out hit);

        return hit;
    }


    /// <summary>
    /// Returns all objects hit from a raycast from the mouse position.
    /// </summary>
    /// <returns></returns>
    public static RaycastHit[] CastRayMulti()
    {
        Vector3 screenMousePositionFar = new Vector3(
    Input.mousePosition.x,
    Input.mousePosition.y,
    Camera.main.farClipPlane);

        Vector3 screenMousePositionNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 worldMousePointerFar = Camera.main.ScreenToWorldPoint(screenMousePositionFar);
        Vector3 worldMousePointerNear = Camera.main.ScreenToWorldPoint(screenMousePositionNear);

        RaycastHit[] hits;

        hits = Physics.RaycastAll(worldMousePointerNear, worldMousePointerFar - worldMousePointerNear);

        return hits;
    }

    public static Color ToHDR(this Color color, float intensity)
    {
        float factor = Mathf.Pow(2, intensity);
        return new Color(color.r * factor, color.g * factor, color.b * factor);
    }
    public static Color ToHDR(this Color color, int intensity)
    {
        float factor = Mathf.Pow(2, intensity);
        return new Color(color.r * factor, color.g * factor, color.b * factor);
    }

    public static Color SetAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}

