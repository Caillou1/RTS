using UnityEngine;

namespace Core.Source
{
    [CreateAssetMenu(fileName = "ConstructAbility", menuName = "ConstructAbility", order = 0)]
    public class ConstructCommand : ACommand
    {
        public GameObject Construct;

        public override void Select()
        {
            CameraController.G.StartCapture(Construct);
        }
    }
}