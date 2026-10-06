using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private Transform target;

    #endregion

    #region Private Variables

    private Vector3 offset;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (target == null)
        {
            return;
        }

        offset = transform.position - target.position;
    }

    /// <summary>
    /// Moves the camera to follow the target with the specified offset.
    /// LateUpdate is used to ensure that the camera follows the target after all other updates have been processed.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        transform.position = target.position + offset;
    }

    #endregion
}
