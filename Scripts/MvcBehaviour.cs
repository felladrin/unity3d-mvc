using UnityEngine;

public class MvcBehaviour : MonoBehaviour
{
    Application app;

    protected Application App
    {
        get
        {
            if (app == null)
                app = Object.FindObjectOfType<Application>();

            return app;
        }
    }
}
