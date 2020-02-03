using UnityEngine;

#pragma warning disable CS0414

namespace WraithavenGames.Bones3.Utility
{
    [ExecuteInEditMode]
    public class EditorCloneCallback : MonoBehaviour
    {
        [HideInInspector, SerializeField] private int instanceId = 0;

        private void Awake()
        {
            #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    int id = GetInstanceID();
                    if (instanceId != id)
                    {
                        if (instanceId == 0)
                            instanceId = id;
                        else
                        {
                            instanceId = id;
                            foreach (ICloneCallbackReceiver v in GetComponents<ICloneCallbackReceiver>())
                                v.OnObjectCloned();
                        }
                    }
                }
            #endif
        }
    }
}
