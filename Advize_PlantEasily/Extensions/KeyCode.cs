using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Valheim.SettingsGui;

namespace Advize_PlantEasily;

public static class KeyCodeExtensions
{

    /// <summary>
    /// Utility extension method mimicking Valheim's internal
    /// behaviour for localizing keycodes. The original logic can be
    /// found in <c>ZInput.GetBoundKeyString(...)</c>, unfortunately it is
    /// intended for use with <c>ZInput.ButtonDef</c> button names and
    /// dispatches the underlying keycodes right away in the same
    /// function, hence why we had to duplicate the code.
    /// </summary>
    public static string ToLocalizableString(this KeyCode keyCode)
    {
        if (keyCode >= KeyCode.JoystickButton0 && keyCode <= KeyCode.JoystickButton9)
        {
            ZInput.ButtonDef buttonDef;
            string buttonName = keyCode.ToString();
            if (!ZInput.instance.m_buttons.TryGetValue(buttonName, out buttonDef)) return "";
            InputBinding inputBinding = buttonDef.ButtonAction.bindings.First();

            string text = ZInput.instance.MapKeyFromPath(inputBinding.effectivePath);
            string controllerPlatform = ZInput.CurrentGlyph == GamepadGlyphs.Playstation ? "ps5" : "xbox";
            return ZInput.s_gamepadSpriteMap[text][controllerPlatform];
        }

        MouseButton mouseButton;
        bool isMouseButton = ZInput.TryKeyCodeToMouseButton(keyCode, out mouseButton);
        if (isMouseButton)
        {
            string mouseString = mouseButton switch
            {
                MouseButton.Left => "$button_mouse0",
                MouseButton.Right => "$button_mouse1",
                MouseButton.Middle => "$button_mouse2",
                MouseButton.Forward => Mouse.current?.forwardButton.displayName ?? "Mouse Forward",
                MouseButton.Back => Mouse.current?.backButton.displayName ?? "Mouse Back",
                _ => null,
            };

            if (mouseString is not null)
            {
                return mouseString;
            }
        }

        Key key = ZInput.KeyCodeToKey(keyCode, logWarning: false);
        return key switch
        {
            Key.Comma => ",",
            Key.Period => ".",
            Key.Space => "$button_space",
            Key.LeftShift => "$button_lshift",
            Key.RightShift => "$button_rshift",
            Key.LeftAlt => "$button_lalt",
            Key.RightAlt => "$button_ralt",
            Key.LeftCtrl => "$button_lctrl",
            Key.RightCtrl => "$button_rctrl",
            Key.Enter => "$button_return",
            Key.NumpadEnter => "$button_return",
            Key.UpArrow => "↑",
            Key.RightArrow => "→",
            Key.DownArrow => "↓",
            Key.LeftArrow => "←",
            Key.None => "$menu_none",
            _ => Keyboard.current?[key].displayName ?? key.ToString(),
        };
    }
}
