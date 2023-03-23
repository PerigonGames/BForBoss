using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public partial class PlayerMovementBehaviour : IGetPlayerTransform
    {
        Transform IGetPlayerTransform.Value => rootPivot;
    }
}
