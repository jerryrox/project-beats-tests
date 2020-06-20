using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBGame.Tests
{
    /// <summary>
    /// An object representing a keybinding action for use in manual testing.
    /// </summary>
    public class TestKeyBinding {

        private KeyCode keyCode;
        private Action action;
        private string description;

        public TestKeyBinding(KeyCode keyCode, Action action, string description)
        {
            this.keyCode = keyCode;
            this.action = action;
            this.description = description;
        }

        /// <summary>
        /// Returns the displayed usage description for the bound key.
        /// </summary>
        public string GetUsage() => $"[KeyCode({keyCode})]: {description}";

        /// <summary>
        /// Checks whether the bound key is pressed and if true, execute the associated action.
        /// </summary>
        public void CheckInput()
        {
            if(action != null && Input.GetKeyDown(keyCode))
                action.Invoke();
        }
    }
}