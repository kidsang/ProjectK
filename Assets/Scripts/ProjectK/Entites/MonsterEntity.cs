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

        private MapPath path;
        private int nextWaypointIndex = 1;
        private int nextPositionIndex;
        private List<Vector3> wayPositions;

        public override void Init(ResourceLoader loader, EntitySetting template)
        {
            base.Init(loader, template);

            MonsterEntitySetting setting = template as MonsterEntitySetting;
            MoveSpeed = setting.MoveSpeed;
        }

        public void SetPath(MapPath path)
        {
            this.path = path;
        }

        public override void Activate(Scene scene)
        {
            base.Activate(scene);

            if (wayPositions == null)
                path.FindPathPosition(Location, nextWaypointIndex, out wayPositions, out nextPositionIndex);

            Vector3 position = gameObject.transform.position;
            if (nextPositionIndex < wayPositions.Count)
            {
                Vector3 waypoint = MapUtils.LocationToPosition(wayPositions[nextPositionIndex]);
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
                    nextPositionIndex += 1;
                }

                gameObject.transform.position = position;
            }
        }
    }
}
