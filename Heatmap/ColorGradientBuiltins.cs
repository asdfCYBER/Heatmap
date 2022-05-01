using UnityEngine;

namespace Heatmap
{
    public partial class ColorGradient
    {
        // This file contains several built-in ColorGradient definitions

        #region Basic colormaps
        // These use predefined UnityEngine.Color values

        public static ColorGradient GreenRed { get; } = new ColorGradient(
            name: "green-red",
            editable: false,
            Color.green,
            Color.red
        );

        public static ColorGradient BlueRed { get; } = new ColorGradient(
            name: "blue-red",
            editable: false,
            Color.blue,
            Color.red
        );

        #endregion

        #region Perceptually Uniform Sequential colormaps
        // Generated from Matplotlib colormaps with Tools/colorgradient_generator.py

        public static ColorGradient Viridis { get; } = new ColorGradient(
            name: "viridis",
            editable: false,
            new Color(0.267004f, 0.004874f, 0.329415f),
            new Color(0.275191f, 0.194905f, 0.496005f),
            new Color(0.212395f, 0.359683f, 0.551710f),
            new Color(0.153364f, 0.497000f, 0.557724f),
            new Color(0.122312f, 0.633153f, 0.530398f),
            new Color(0.288921f, 0.758394f, 0.428426f),
            new Color(0.626579f, 0.854645f, 0.223353f),
            new Color(0.993248f, 0.906157f, 0.143936f)
        );

        public static ColorGradient Cividis { get; } = new ColorGradient(
            name: "cividis",
            editable: false,
            new Color(0.000000f, 0.135112f, 0.304751f),
            new Color(0.130669f, 0.231458f, 0.432840f),
            new Color(0.298421f, 0.332247f, 0.423973f),
            new Color(0.425120f, 0.431334f, 0.447692f),
            new Color(0.555393f, 0.537807f, 0.471147f),
            new Color(0.695985f, 0.648334f, 0.440072f),
            new Color(0.849223f, 0.771947f, 0.359729f),
            new Color(0.995737f, 0.909344f, 0.217772f)
        );

        public static ColorGradient Plasma { get; } = new ColorGradient(
            name: "plasma",
            editable: false,
            new Color(0.050383f, 0.029803f, 0.527975f),
            new Color(0.325150f, 0.006915f, 0.639512f),
            new Color(0.546157f, 0.038954f, 0.647010f),
            new Color(0.723444f, 0.196158f, 0.538981f),
            new Color(0.859750f, 0.360588f, 0.406917f),
            new Color(0.955470f, 0.533093f, 0.285490f),
            new Color(0.994495f, 0.740880f, 0.166335f),
            new Color(0.940015f, 0.975158f, 0.131326f)
        );

        public static ColorGradient Inferno { get; } = new ColorGradient(
            name: "inferno",
            editable: false,
            new Color(0.001462f, 0.000466f, 0.013866f),
            new Color(0.155850f, 0.044559f, 0.325338f),
            new Color(0.397674f, 0.083257f, 0.433183f),
            new Color(0.621685f, 0.164184f, 0.388781f),
            new Color(0.832299f, 0.283913f, 0.257383f),
            new Color(0.961293f, 0.488716f, 0.084289f),
            new Color(0.981173f, 0.759135f, 0.156863f),
            new Color(0.988362f, 0.998364f, 0.644924f)
        );

        public static ColorGradient Magma { get; } = new ColorGradient(
            name: "magma",
            editable: false,
            new Color(0.001462f, 0.000466f, 0.013866f),
            new Color(0.135053f, 0.068391f, 0.315000f),
            new Color(0.372116f, 0.092816f, 0.499053f),
            new Color(0.594508f, 0.175701f, 0.501241f),
            new Color(0.828886f, 0.262229f, 0.430644f),
            new Color(0.973381f, 0.461520f, 0.361965f),
            new Color(0.997341f, 0.733545f, 0.505167f),
            new Color(0.987053f, 0.991438f, 0.749504f)
        );

        #endregion
    }
}
