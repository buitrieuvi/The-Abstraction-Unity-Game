using System;
using UnityEngine;

[Serializable]
public class LayerController
{
    public LayerMask GroundLayer;
    public LayerMask NPCLayer;

    public bool ContainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }

    public bool IsGroundLayer(int layer)
    {
        return ContainsLayer(GroundLayer, layer);
    }

    public bool IsNPCLayer(int layer)
    {
        return ContainsLayer(NPCLayer, layer);
    }
}