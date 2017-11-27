using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Blending
{
    public enum BlendMode
    {
        /// <summary>
        /// No blending. Draw B on image.
        /// </summary>
        None,
        /// <summary>
        /// Blends image A and B with the alpha of image B
        /// </summary>
        Over,
        /// <summary>
        /// Blends image A and B with the alpha of image B but only when B is non-zero. When B is zero, alpha of A is assumed to be 1.0.
        /// </summary>
        OverWhereNonZero,
    }
}
