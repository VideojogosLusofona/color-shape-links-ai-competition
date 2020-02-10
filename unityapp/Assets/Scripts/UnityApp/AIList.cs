using UnityEngine;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    public class AIList : PropertyAttribute
    {
        public string[] AIs => AIManager.Instance.AIs;
    }
}
