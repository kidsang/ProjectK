using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class HeroEntity : SceneEntity
    {
        public float MoveSpeed { get; private set; } // 移动速度，单位米/秒

        public override void Activate(Scene scene, float time)
        {
            base.Activate(scene, time);


        }
    }
}
