using UnityEngine;

public enum Move
{
  Forward = KeyCode.W,
  Left = KeyCode.A,
  Backward = KeyCode.S,
  Right = KeyCode.D,
  Down = KeyCode.Q,
  Up = KeyCode.E,
}

public class CameraControl : MonoBehaviour
{
  // Settings
  private KeyCode activateKey = KeyCode.Mouse1;
  private bool moving = false;

  // Rotation Vars
  private float angularSpeed = 0.25f;
  private Vector3 lastPos = Vector3.zero;

  // Position Vars
  private float mainSpeed = 20.0f; //regular speed
  private float minY = 0.5f;

  void Update()
  {
    if (this.CheckActivation())
    {
      this.UpdateRotation();
      this.UpdatePosition();
    }
  }

  /// <summary>
  /// Checks if Camera is being moved and sets last mouse position if so.
  /// </summary>
  /// <returns>True if camera is moving, false if not.</returns>
  private bool CheckActivation()
  {
    if (Input.GetKeyUp(this.activateKey))
    {
      this.moving = false;
      Cursor.visible = true;
      GUI.SetMessage(MessageBox.Console, Messages.CameraRelease);
    }

    if (Input.GetKeyDown(this.activateKey))
    {
      this.moving = true;
      this.lastPos = Input.mousePosition;
      Cursor.visible = false;
      GUI.SetMessage(MessageBox.Console, Messages.CameraMove);
    }

    return this.moving;
  }

  /// <summary>
  /// Updates camera rotation based on mouse movement.
  /// </summary>
  private void UpdateRotation()
  {
    // Save some processing.
    if (Input.mousePosition == this.lastPos) return;

    Vector3 delta = Input.mousePosition - this.lastPos;
    float newX = this.transform.eulerAngles.x - delta.y * angularSpeed;
    float newY = this.transform.eulerAngles.y + delta.x * angularSpeed;

    // So camera doesn't flip over itself.
    if (newX > 90 && newX < 270) newX = transform.eulerAngles.x;

    Vector3 newRotation = new Vector3(newX, newY, 0);
    this.transform.eulerAngles = newRotation;

    this.lastPos = Input.mousePosition;
  }

  /// <summary>
  /// Updates camera position based on keyboard input.
  /// </summary>
  private void UpdatePosition()
  {
    // Keyboard commands
    Vector3 input = new Vector3();

    // Forwards, left, backwards, right.
    if (Input.GetKey((KeyCode)Move.Forward)) input += new Vector3(0, 0, 1);
    if (Input.GetKey((KeyCode)Move.Left)) input += new Vector3(-1, 0, 0);
    if (Input.GetKey((KeyCode)Move.Backward)) input += new Vector3(0, 0, -1);
    if (Input.GetKey((KeyCode)Move.Right)) input += new Vector3(1, 0, 0);

    // Upwards, Downwards.
    if (Input.GetKey((KeyCode)Move.Up)) input += new Vector3(0, 1, 0);
    if (Input.GetKey((KeyCode)Move.Down)) input += new Vector3(0, -1, 0);

    // Once again, saving some processing;
    if (input == Vector3.zero) return;

    // Calcs movement with vertical bounds.
    Vector3 move = input * this.mainSpeed * Time.deltaTime;
    transform.Translate(move);

    if (this.transform.position.y < this.minY)
      this.transform.position = new Vector3(this.transform.position.x, this.minY, this.transform.position.z);
  }
}
