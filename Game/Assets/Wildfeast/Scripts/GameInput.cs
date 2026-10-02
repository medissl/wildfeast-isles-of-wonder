using UnityEngine;
using UnityEngine.InputSystem;

namespace Wildfeast
{
    public static class GameInput
    {
        public static Vector2 Move
        {
            get
            {
                var k = Keyboard.current; Vector2 v = Vector2.zero;
                if (k != null) { if (k.aKey.isPressed || k.leftArrowKey.isPressed) v.x--; if (k.dKey.isPressed || k.rightArrowKey.isPressed) v.x++; if (k.sKey.isPressed || k.downArrowKey.isPressed) v.y--; if (k.wKey.isPressed || k.upArrowKey.isPressed) v.y++; }
                if (Gamepad.current != null) v += Gamepad.current.leftStick.ReadValue() + Gamepad.current.dpad.ReadValue();
                return Vector2.ClampMagnitude(v, 1);
            }
        }
        public static bool Interact => Keyboard.current?.eKey.wasPressedThisFrame == true || Gamepad.current?.buttonSouth.wasPressedThisFrame == true;
        public static bool Press => Keyboard.current?.spaceKey.wasPressedThisFrame == true || Gamepad.current?.buttonSouth.wasPressedThisFrame == true;
        public static bool Hold => Keyboard.current?.spaceKey.isPressed == true || Gamepad.current?.buttonSouth.isPressed == true;
        public static bool Back => Keyboard.current?.escapeKey.wasPressedThisFrame == true || Gamepad.current?.buttonEast.wasPressedThisFrame == true;
        public static bool Journal => Keyboard.current?.tabKey.wasPressedThisFrame == true;
    }
}
