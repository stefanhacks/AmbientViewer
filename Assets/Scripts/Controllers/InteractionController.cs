﻿using UnityEngine;

public enum Deform
{
  Move = KeyCode.Alpha1,
  Scale = KeyCode.Alpha2,
  Rotate = KeyCode.Alpha3,
  Clone = KeyCode.Alpha4,
  Delete = KeyCode.Alpha5,
  Axis = KeyCode.LeftShift,
}

public class InteractionController : MonoBehaviour
{
  // Load Control
  public bool readingInputs = false;

  // Selection Variables
  private GameObject selectedObject;
  private Vector3 lastPos = Vector3.zero;
  private Plane floorPlane;
  private int furnitureMask;

  // Deformation Variables
  private bool deforming = false;
  private Deform deformType = Deform.Move;

  private Transform furnitureRoot = null;
  public Transform FurnitureRoot { set { this.furnitureRoot = value; } }

  private float verticalIntensity = 0.1f;
  private float rotationIntensity = 100;
  private Vector3 scaleIntensity = new Vector3(2, 2, 2);

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
    if (!this.readingInputs) return;

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
      GUI.SetMessage(Messages.Move);
      GUI.SetShiftMessage(ShiftMessages.Move);
    }
    else if (Input.GetKeyUp((KeyCode)Deform.Scale))
    {
      deformType = Deform.Scale;
      GUI.SetMessage(Messages.Scale);
      GUI.SetShiftMessage(ShiftMessages.Scale);
    }
    else if (Input.GetKeyUp((KeyCode)Deform.Rotate))
    {
      deformType = Deform.Rotate;
      GUI.SetMessage(Messages.Rotate);
      GUI.SetShiftMessage(ShiftMessages.Rotate);
    }
    else if (Input.GetKeyUp((KeyCode)Deform.Clone))
    {
      deformType = Deform.Clone;
      GUI.SetShiftMessage(ShiftMessages.Clear);
      if (this.selectedObject != null) this.CloneSelection(this.selectedObject);
      else GUI.SetMessage(Messages.SelectForClone);
    }
    else if (Input.GetKeyUp((KeyCode)Deform.Delete))
    {
      deformType = Deform.Delete;
      GUI.SetShiftMessage(ShiftMessages.Clear);
      if (this.selectedObject != null) this.DeleteSelection(this.selectedObject);
      else GUI.SetMessage(Messages.SelectForDelete);
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
        GUI.SetMessage(Messages.Clear);
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
    GUI.SetMessage(Messages.Selected);
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
        break;
    }

    // Updates Gizmo position.
    AmbientGizmo.SpawnGizmo(selection);
    this.lastPos = Input.mousePosition;
  }

  /// <summary>
  /// Moves selected object, considering mouse interaction.
  /// </summary>
  /// <param name="selection">GameObject to move.</param>
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
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      float coords;

      // Nabs intersection.
      if (this.floorPlane.Raycast(ray, out coords))
      {
        // Positions object on plane.
        Vector3 center = selection.GetComponent<Renderer>().bounds.center;
        Vector3 delta = selection.transform.position - center;
        delta.y = 0;

        Vector3 newPos = ray.GetPoint(coords);
        newPos.y = selection.transform.position.y;

        GUI.SetMessage(Messages.Moving);
        selection.transform.position = newPos + delta;
      }
    }
  }

  /// <summary>
  /// Scales selected object, considering mouse interaction.
  /// </summary>
  /// <param name="selection">GameObject to scale.</param>
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

    GUI.SetMessage(Messages.Scaling);
    selection.transform.localScale = newScale;
  }

  /// <summary>
  /// Rotates selected object, considering mouse interaction.
  /// </summary>
  /// <param name="selection">GameObject to rotate.</param>
  private void RotateSelection(GameObject selection)
  {
    float aroundX = Input.GetAxis("Mouse X") * this.rotationIntensity * Mathf.Deg2Rad;
    float aroundY = Input.GetAxis("Mouse Y") * this.rotationIntensity * Mathf.Deg2Rad;

    // Lock around an axis.
    if (Input.GetKey((KeyCode)Deform.Axis))
    {
      aroundX = Mathf.Abs(aroundX) > Mathf.Abs(aroundY) ? aroundX : 0;
      aroundY = Mathf.Abs(aroundY) > Mathf.Abs(aroundX) ? aroundY : 0;
    }

    selection.transform.Rotate(Vector3.up, -aroundX);
    selection.transform.Rotate(Vector3.right, aroundY);

    GUI.SetMessage(Messages.Rotating);
  }
  #endregion

  #region Extra Operations
  /// <summary>
  /// Clones the selected object and updates selection pointer to it.
  /// </summary>
  /// <param name="selection"></param>
  private void CloneSelection(GameObject selection)
  {
    // Offsetting it a bit so it looks not as confusing.
    Vector3 oldPos = selection.transform.position;
    Vector3 newPos = new Vector3(oldPos.x + 0.75f, oldPos.y, oldPos.z + 0.5f);

    GameObject clone = Object.Instantiate<GameObject>(selection);
    clone.transform.parent = this.furnitureRoot;
    clone.transform.position = newPos;

    // Update selection pointer and override console message.
    this.Select(clone);
    GUI.SetMessage(Messages.Clone);
  }

  /// <summary>
  /// Destroys the selected object and clears selection pointers.
  /// </summary>
  /// <param name="selection"></param>
  private void DeleteSelection(GameObject selection)
  {
    Destroy(selection);
    this.ClearSelection();
    GUI.SetMessage(Messages.Delete);
  }
  #endregion
}
