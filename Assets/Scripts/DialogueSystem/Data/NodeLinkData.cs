using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    public class NodeLinkData
    {
        [field: FormerlySerializedAs("BaseNodeGuid")]
        [field: SerializeField]
        public string BaseNodeGuid { get; set; }

        [field: FormerlySerializedAs("PortName")]
        [field: SerializeField]
        public string PortName { get; set; }

        [field: FormerlySerializedAs("TargetNodeGuid")]
        [field: SerializeField]
        public string TargetNodeGuid { get; set; }
    }
}