using UnityEngine;

namespace Assets.Scripts.Core.BaseClasses
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public Vector3 MoveCharacter(float x, float y, int speedModifer = 1)
        {
            return new Vector3(x, y, 0) * (0.025f * speedModifer);
        }
    }
}