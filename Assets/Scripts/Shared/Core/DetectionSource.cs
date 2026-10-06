using UnityEngine;

public class DetectionSource : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private DetectionType detectionType;

    #endregion

    #region Public Properties

    public DetectionType Type => detectionType;

    #endregion
}
