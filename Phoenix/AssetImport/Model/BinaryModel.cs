using Phoenix.Rendering.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.AssetImport.Model
{
    public class BinaryModel
    {
        public List<BinaryModelPart> Parts { get; private set; }

        public BinaryModel(List<BinaryModelPart> parts)
        {
            Parts = parts;
        }

    }
}
