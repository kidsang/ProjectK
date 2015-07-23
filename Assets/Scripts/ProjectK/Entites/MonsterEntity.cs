using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class MonsterEntity : SceneEntity
    {
        public float MoveSpeed { get; private set; } // 移动速度，单位米/秒

        private Vector2 startLocation;
        private Vector2 endLocation;

        private List<Vector2> waypoints;
        private int nextWaypointIndex = -1;

        public override void Init(ResourceLoader loader, EntitySetting template)
        {
            base.Init(loader, template);

            MonsterEntitySetting setting = template as MonsterEntitySetting;
            MoveSpeed = setting.MoveSpeed;
        }

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
                nextWaypointIndex = 1;
            }

            Vector3 position = gameObject.transform.position;
            if (nextWaypointIndex < waypoints.Count)
            {
                Vector3 waypoint = MapUtils.LocationToPosition(waypoints[nextWaypointIndex]);
                Vector3 direction = waypoint - position;
                float move = MoveSpeed * scene.DeltaTime;
                if (direction.sqrMagnitude > move * move)
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
