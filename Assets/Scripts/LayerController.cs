using System;
using UnityEngine;

[Serializable]
public class LayerController
{
    public LayerMask GroundLayer;
    public LayerMask InteracLayer;

    public bool ContainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }

    public bool IsGroundLayer(int layer)
    {
        return ContainsLayer(GroundLayer, layer);
    }

    public bool IsInteracLayer(int layer)
    {
        return ContainsLayer(InteracLayer, layer);
    }
}