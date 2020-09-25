using UnityEngine;

public class InteractionController : MonoBehaviour
{
  private GameObject selectedObject;

  private void Update()
  {
    this.CheckClick();
  }

  /// <summary>
  /// Checks for a click with left mouse, selects object if one is hit.
  /// </summary>
  private void CheckClick()
  {
    if (Input.GetMouseButton(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

      RaycastHit hitInfo;
      if (Physics.Raycast(ray, out hitInfo)) this.Select(hitInfo.transform.gameObject);
      else this.ClearSelection();
    }
  }

  /// <summary>
  /// Sets object as selected.
  /// </summary>
  /// <param name="obj">GameObject to set the selection on.</param>
  private void Select(GameObject obj)
  {
    this.selectedObject = obj;
    AmbientGizmo.SpawnGizmo(obj);
  }

  /// <summary>
  /// Clears internal selection variables.
  /// </summary>
  private void ClearSelection()
  {
    this.selectedObject = null;
  }
}
