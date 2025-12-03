using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Rendering
{
    public struct CommonUBO
    {
        public Matrix4x4 View;
        public Matrix4x4 Projection;
        public float Time;
        public float DeltaTime;

        public CommonUBO(Matrix4x4 view, Matrix4x4 proj, float time, float deltaTime)
        {
            View = view;
            Projection = proj;
            Time = time;
            DeltaTime = deltaTime;
        }
    }
}
