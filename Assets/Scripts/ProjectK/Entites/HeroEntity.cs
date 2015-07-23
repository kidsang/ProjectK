using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class HeroEntity : SceneEntity
    {
        public float MoveSpeed { get; private set; } // 移动速度，单位米/秒

        private Vector2 startLocation;
        private Vector2 endLocation;

        private List<Vector2> waypoints;
        private int nextWaypointIndex = -1;

        public void SetStartEnd(Vector2 startLocation, Vector2 endLocation)
        {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
        }

        public override void Activate(Scene scene)
        {
            base.Activate(scene);

            if (waypoints == null)
            {
                waypoints = new List<Vector2>();
                scene.Map.CalculatePath(startLocation, endLocation, waypoints);
                nextWaypointIndex = -1;
            }

            Vector3 position = gameObject.transform.position;
            if (nextWaypointIndex == -1)
            {
                for (int i = 0; ; i++)
                {
                    Vector2 waypoint = waypoints[i];
                    if (waypoint.x == position.x && waypoint.y == position.y)
                    {
                        nextWaypointIndex = i + 1;
                        break;
                    }
                }
            }

            if (nextWaypointIndex < waypoints.Count)
            {
                Vector3 waypoint = waypoints[nextWaypointIndex];
                Vector3 direction = waypoint - position;
                float move = MoveSpeed * scene.DeltaTime;
                if (direction.sqrMagnitude > move)
                {
                    direction.Normalize();
                    position += direction * move;
                }
                else
                {
                    position = waypoint;
                    nextWaypointIndex += 1;
                }

                gameObject.transform.position = position;
            }
        }
    }
}
