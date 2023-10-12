using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace BetterTargetPlugin
{
    internal class GameObjectList
    {
        public uint objectId;
        public GameObject? gameObject;
        public float distance;

    }
}
