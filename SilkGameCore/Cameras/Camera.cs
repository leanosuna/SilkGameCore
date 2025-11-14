using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SilkGameCore.Cameras
{
    public abstract class Camera
    {
        public Matrix4x4 View { get; set; }
        public Matrix4x4 Projection { get; set; }
    }
}
