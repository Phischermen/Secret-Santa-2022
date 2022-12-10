using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(Arena))]
public class ArenaEditor : UnityEditor.Editor
{
    private BoxBoundsHandle _arenaBoxBoundsHandle = new BoxBoundsHandle();
    private List<BoxBoundsHandle> _obstacleBoxBoundsHandles = new List<BoxBoundsHandle>();

    // Draw bounding box controls for the arena
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    static void RenderGizmo(Arena arena, GizmoType gizmoType)
    {
        // Draw all the bounding boxes.
        DrawBox(arena.arenaBounds, Color.green);
        foreach (var arenaArenaObstacle in arena.arenaObstacles)
        {
            DrawBox(arenaArenaObstacle, Color.red);
        }
        void DrawBox(Bounds bounds, Color color)
        {
            var position = bounds.center - bounds.extents;
            var rectangle = new Rect(position, bounds.extents * 2);
            var faceColor = new Color(color.r, color.g, color.b, color.a * 0.1f);
            Handles.DrawSolidRectangleWithOutline(rectangle, faceColor, color);
        }
    }

    private void OnSceneGUI()
    {
        Arena arena = (Arena)target;
        // Ensure there are exactly the same amount of obstacle handles for obstacles.
        if (_obstacleBoxBoundsHandles.Count > arena.arenaObstacles.Count)
        {
            _obstacleBoxBoundsHandles.RemoveRange(arena.arenaObstacles.Count, _obstacleBoxBoundsHandles.Count - arena.arenaObstacles.Count);
        }
        else if (_obstacleBoxBoundsHandles.Count < arena.arenaObstacles.Count)
        {
            for (int i = _obstacleBoxBoundsHandles.Count; i < arena.arenaObstacles.Count; i++)
            {
                _obstacleBoxBoundsHandles.Add(new BoxBoundsHandle());
            }
        }
        
        ProcessHandle(_arenaBoxBoundsHandle, ref arena.arenaBounds);
        for (var i = 0; i < arena.arenaObstacles.Count; i++)
        {
            var obstacle = arena.arenaObstacles[i];
            ProcessHandle(_obstacleBoxBoundsHandles[i], ref obstacle);
            arena.arenaObstacles[i] = obstacle;
        }

        void ProcessHandle(BoxBoundsHandle handle, ref Bounds bounds)
        {
            // Copy the bounds to the handle.
            handle.center = bounds.center;
            handle.size = bounds.size;
            EditorGUI.BeginChangeCheck();
            handle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // Copy the handle's updated data back to the target object.
                Bounds newBounds = new Bounds();
                newBounds.center = handle.center;
                newBounds.size = handle.size;
                bounds = newBounds;
            }
        }
        
    }
}