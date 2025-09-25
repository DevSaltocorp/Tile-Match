using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    void Awake()
    {
        SetOrientation();
    }

    void SetOrientation()
    {
        // Calculate physical screen size in inches
        float dpi = Screen.dpi;
        float widthInches = Screen.width / dpi;
        float heightInches = Screen.height / dpi;
        float diagonalInches = Mathf.Sqrt(widthInches * widthInches + heightInches * heightInches);

        // If diagonal is greater than ~7 inches → treat as tablet
        if (diagonalInches >= 7.0f)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Debug.Log("Tablet detected → Landscape mode");
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Debug.Log("Phone detected → Portrait mode");
        }
    }
}
