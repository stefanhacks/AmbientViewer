using UnityEngine;

public enum Deform
{
  Move = KeyCode.Alpha1,
  Scale = KeyCode.Alpha2,
  Rotate = KeyCode.Alpha3,
  Axis = KeyCode.LeftShift,
}

public class InteractionController : MonoBehaviour
{
  // Selection Variables
  private GameObject selectedObject;
  private Vector3 lastPos = Vector3.zero;
  private Plane floorPlane;
  private int furnitureMask;

  // Deformation Variables
  private bool deforming = false;
  private Deform deformType = Deform.Move;

  private float verticalIntensity = 0.1f;
  private Vector3 scaleIntensity = new Vector3(2, 2, 2);
  private Vector3 rotationIntensity = new Vector3(.7f, .7f, .7f);

  private bool lockedHorizontal = false;
  private float lockedRotationTreshold = 30;
  private Vector2 rotationAmount = new Vector2(30, 30);

  private void Start()
  {
    // Sets up a plane to use for movement, Normal is on y.
    this.floorPlane = new Plane(Vector3.up, Vector3.zero);

    // Masks are bits, so we need to shift to get them. 
    // Doing this on start avoids unecessary update() work.
    this.furnitureMask = 1 << LayerMask.NameToLayer("Furniture");
  }

  private void Update()
  {
    this.CheckDeformType();
    this.CheckClick();
  }

  #region Check Inputs
  /// <summary>
  /// Checks the input for deform types.
  /// </summary>
  private void CheckDeformType()
  {
    if (Input.GetKeyUp((KeyCode)Deform.Move))
    {
      deformType = Deform.Move;
      GUI.SetMessage(MessageBox.Console, "[Interaction] Set to move objects.");
    }
    if (Input.GetKeyUp((KeyCode)Deform.Scale))
    {
      deformType = Deform.Scale;
      GUI.SetMessage(MessageBox.Console, "[Interaction] Set to scale objects.");
    }
    if (Input.GetKeyUp((KeyCode)Deform.Rotate))
    {
      deformType = Deform.Rotate;
      GUI.SetMessage(MessageBox.Console, "[Interaction] Set to rotate objects.");
    }
  }

  /// <summary>
  /// Checks for a click with left mouse, selects object if one is hit.
  /// </summary>
  private void CheckClick()
  {
    // If click...
    if (Input.GetMouseButtonDown(0))
    {
      // Stops deforming anything.
      this.deforming = false;

      // Figures out if the click was on a Furniture Obj.
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hitInfo;

      // if (Physics.Raycast(ray, out hitInfo))
      if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, this.furnitureMask))
      {
        // Selects the object if new.
        GameObject target = hitInfo.transform.gameObject;
        if (target != selectedObject)
        {
          this.Select(target);
        }

        // Starts deforming if isn't.
        else
        {
          this.lastPos = Input.mousePosition;
          this.deforming = true;
        }
      }
      // If it doesn't hit, just clear selection.
      else this.ClearSelection();
    }
    // If object is being deformed...
    else if (this.deforming == true)
    {
      // On continuous mouse press, deform.
      if (Input.GetMouseButton(0))
        this.DeformSelection(this.selectedObject);

      // On release, clear console.
      else if (Input.GetMouseButtonUp(0))
        GUI.SetMessage(MessageBox.Console, "");
    }
  }
  #endregion

  #region Treat Selections
  /// <summary>
  /// Sets object as selected.
  /// </summary>
  /// <param name="obj">GameObject to set the selection on.</param>
  private void Select(GameObject obj)
  {
    this.selectedObject = obj;
    AmbientGizmo.SpawnGizmo(obj);
    GUI.SetMessage(MessageBox.Console, "[Interaction] Object selected.");
  }

  /// <summary>
  /// Clears internal selection variables.
  /// </summary>
  private void ClearSelection()
  {
    this.selectedObject = null;
    AmbientGizmo.ClearGizmo();
  }
  #endregion

  #region Object Deformation
  /// <summary>
  /// Treats the deformation selection.
  /// </summary>
  /// <param name="selection">GameObject to deform.</param>
  private void DeformSelection(GameObject selection)
  {
    // Using selection via argument to ensure these methods aren't called without one.
    switch (deformType)
    {
      case Deform.Move:
        this.MoveSelection(selection);
        break;
      case Deform.Scale:
        this.ScaleSelection(selection);
        break;
      case Deform.Rotate:
        this.RotateSelection(selection);
        break;
      default:
        throw new System.Exception("Invalid deform type selected.");
    }

    // Updates Gizmo position.
    AmbientGizmo.SpawnGizmo(selection);
    this.lastPos = Input.mousePosition;
  }

  private void MoveSelection(GameObject selection)
  {
    // With Axis key, shifts object up and down.
    if (Input.GetKey((KeyCode)Deform.Axis))
    {
      Vector3 delta = this.lastPos - Input.mousePosition;
      float vAmount = delta.y * this.verticalIntensity;

      Vector3 newPos = selection.transform.position;
      newPos.y -= vAmount;

      selection.transform.position = newPos;
    }

    // Without, moves it around.
    else
    {
      // Figures where the mouse is regarding Floor Plane.
      Vector3 planePoint = Input.mousePosition;
      planePoint.y -= selection.transform.position.y;

      // Nabs intersection.
      Ray ray = Camera.main.ScreenPointToRay(planePoint);
      float coords;

      // Positions object on plane.
      if (this.floorPlane.Raycast(ray, out coords))
      {
        Vector3 center = selection.GetComponent<Renderer>().bounds.center;
        Vector3 delta = selection.transform.position - center;
        delta.y = 0;

        Vector3 newPos = ray.GetPoint(coords);
        newPos.y = selection.transform.position.y;

        GUI.SetMessage(MessageBox.Console, "[Interaction] Moving Object.");
        selection.transform.position = newPos + delta;
      }
    }
  }

  private void ScaleSelection(GameObject selection)
  {
    Vector3 amount = (Input.mousePosition - this.lastPos) * Time.deltaTime;
    amount.Scale(this.scaleIntensity);

    Vector3 newScale = selection.transform.localScale;

    // With Axis key, scales equaly.
    if (Input.GetKey((KeyCode)Deform.Axis))
    {
      newScale.x += amount.y;
      newScale.y += amount.y;
      newScale.z += amount.y;
    }

    // Without, squishes.
    else
    {
      newScale.x += amount.x;
      newScale.z += amount.y;
    }

    GUI.SetMessage(MessageBox.Console, "[Interaction] Scaling Object.");
    selection.transform.localScale = newScale;
  }

  private void RotateSelection(GameObject selection)
  {
    Vector3 delta = (Input.mousePosition - this.lastPos) * Time.deltaTime;
    Vector3 newRotation = delta.normalized;
    newRotation.Scale(this.rotationIntensity);

    // With Axis key, controlled rotation.
    if (Input.GetKey((KeyCode)Deform.Axis))
    {
      this.rotationAmount += new Vector2(newRotation.x, newRotation.y);
      if (this.rotationAmount.x > this.lockedRotationTreshold) this.lockedHorizontal = true;
      else if (this.rotationAmount.y > this.lockedRotationTreshold) this.lockedHorizontal = false;

      if (this.lockedHorizontal == true) newRotation.y = 0;
      else newRotation.x = 0;
    }

    GUI.SetMessage(MessageBox.Console, "[Interaction] Rotating Object.");
    selection.transform.Rotate(newRotation);
  }
}
#endregion
