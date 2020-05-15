using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A simple utility class for handling object picking.
    /// </summary>
    public class ObjectPicker
    {
        private readonly Dictionary<string, int> pickerWindows = new Dictionary<string, int>();
        private readonly Dictionary<string, Object> pickerObjects = new Dictionary<string, Object>();

        /// <summary>
        /// Shows a new object picker dialog.
        /// </summary>
        /// <param name="name">The event name, used for later accesss.</param>
        /// <param name="obj">The currently selected object.</param>
        /// <param name="scene">Whether or not to allow scene objects.</param>
        /// <typeparam name="T">The type of object to pick for.</typeparam>
        /// <remarks>
        /// This action does nothing is an object picker is already open.
        /// </remarks>
        public void ShowPickerWindow<T>(string name, Object obj, bool scene)
        where T : Object
        {
            if (pickerWindows.ContainsKey(name))
                return;

            int pickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
            EditorGUIUtility.ShowObjectPicker<T>(obj, scene, "", pickerWindow);

            pickerWindows[name] = pickerWindow;
            pickerObjects[name] = null;
        }

        /// <summary>
        /// Retrieves the selected picker object, if available.
        /// </summary>
        /// <param name="name">The event name.</param>
        /// <returns>
        /// The newly selected object, or null if a selection hasn't been made, or
        /// if the picker window isn't open.
        /// </returns>
        public Object PickerObject(string name)
        {
            if (!pickerWindows.ContainsKey(name))
                return null;

            int pickerWindow = pickerWindows[name];

            if (Event.current.commandName == "ObjectSelectorUpdated" &&
                EditorGUIUtility.GetObjectPickerControlID() == pickerWindow)
                pickerObjects[name] = EditorGUIUtility.GetObjectPickerObject();

            if (Event.current.commandName == "ObjectSelectorClosed" &&
                EditorGUIUtility.GetObjectPickerControlID() == pickerWindow)
            {
                pickerWindows.Remove(name);

                Object obj = null;
                if (pickerObjects.ContainsKey(name))
                {
                    obj = pickerObjects[name];
                    pickerObjects.Remove(name);
                }

                return obj;
            }

            return null;
        }
    }
}
