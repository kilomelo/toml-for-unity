using UnityEngine;

namespace Samples.Tommy
{
    public class TommyExampleComp : MonoBehaviour
    {
        void Start()
        {
            TommyExample.ParseExample();
            TommyExample.ExceptionHandleExample();
            TommyExample.GenerateTOMLFileExample();
            TommyExample.CollapsedValuesExample();
            TommyExample.ExtensionExample();
        }
    }
}