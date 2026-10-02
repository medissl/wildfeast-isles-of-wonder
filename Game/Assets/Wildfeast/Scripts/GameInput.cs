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
        public static bool Bag => Keyboard.current?.bKey.wasPressedThisFrame == true;
        public static bool Use => Keyboard.current?.spaceKey.wasPressedThisFrame == true || (Mouse.current?.leftButton.wasPressedThisFrame == true && !(UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject() ?? false)) || Gamepad.current?.buttonWest.wasPressedThisFrame == true;
        public static int Slot
        {
            get {var k=Keyboard.current;if(k==null)return -1;var keys=new[]{k.digit1Key,k.digit2Key,k.digit3Key,k.digit4Key,k.digit5Key,k.digit6Key,k.digit7Key,k.digit8Key};for(int i=0;i<8;i++)if(keys[i].wasPressedThisFrame)return i;return -1;}
        }
        public static int Scroll => Mouse.current == null ? 0 : Mouse.current.scroll.ReadValue().y>0 ? -1 : Mouse.current.scroll.ReadValue().y<0 ? 1 : 0;
        public static bool LeftCut => Keyboard.current?.aKey.wasPressedThisFrame == true || Keyboard.current?.leftArrowKey.wasPressedThisFrame == true;
        public static bool RightCut => Keyboard.current?.dKey.wasPressedThisFrame == true || Keyboard.current?.rightArrowKey.wasPressedThisFrame == true;
    }
}
