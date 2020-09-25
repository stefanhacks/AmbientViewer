using UnityEngine;

public static class AmbientGizmo
{
  private static GameObject gizmo = null;
  private static string materialPath = "Gizmo/Mat";

  /// <summary>
  /// Spawns the Gizmo Object on top of an object.
  /// In effect, the object is instantiated only once.
  /// </summary>
  /// <param name="at">Target GameObject.</param>
  public static void SpawnGizmo(GameObject at)
  {
    // This way, object is created only once - then reused, saving processing.
    if (gizmo == null) MakeBox();

    // Center position of render.
    Vector3 position = at.GetComponent<Renderer>().bounds.center;
    gizmo.SetActive(true);
    gizmo.transform.position = position;
  }

  /// <summary>
  /// Hides the Gizmo Object.
  /// </summary>
  public static void ClearGizmo()
  {
    if (gizmo != null) gizmo.SetActive(false);
  }

  /// <summary>
  /// Makes the Gizmo Object.
  /// </summary>
  private static void MakeBox()
  {
    // Making the object dynamically for the sake of it.
    gizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    gizmo.layer = LayerMask.NameToLayer("AmbientGizmo");

    Material boxMat = Resources.Load<Material>(materialPath);
    gizmo.GetComponent<Renderer>().material = boxMat;
  }
}