using UnityEngine;
using System.Collections.Generic;

public enum MessageBox
{
  Console = 0,
  Shift = 1
}

public enum Messages
{
  Clear = 0,
  Move = 1,
  Scale = 2,
  Rotate = 3,
  Moving = 4,
  Scaling = 5,
  Rotating = 6,
  Selected = 7,
  CameraMove = 8,
  CameraRelease = 9,
  Ready = 10,
  Clone = 11,
  Delete = 12,
  SelectForClone = 13,
  SelectForDelete = 14,
}

public enum ShiftMessages
{
  Clear = 0,
  Move = 1,
  Scale = 2,
  Rotate = 3,
}

public class GUI : MonoBehaviour
{
  // Parts, some serialized so they are visible on editor but not explicitly public.
  [SerializeField] private TMPro.TextMeshProUGUI consoleText = null;
  [SerializeField] private TMPro.TextMeshProUGUI shiftText = null;
  static Dictionary<int, TMPro.TextMeshProUGUI> bars;
  static Dictionary<int, string> text;
  static Dictionary<int, string> shift;

  /// <summary>
  /// Readies internal objects for use.
  /// </summary>
  public void Setup()
  {
    bars = new Dictionary<int, TMPro.TextMeshProUGUI>() {
      { (int) MessageBox.Console, consoleText },
      { (int) MessageBox.Shift, shiftText },
    };

    text = new Dictionary<int, string>() {
      { (int) Messages.Clear, "" },
      { (int) Messages.Move, "[Interaction] Set to move objects." },
      { (int) Messages.Scale, "[Interaction] Set to scale objects." },
      { (int) Messages.Rotate, "[Interaction] Set to rotate objects." },
      { (int) Messages.Moving, "[Interaction] Moving Object." },
      { (int) Messages.Scaling, "[Interaction] Scaling Object." },
      { (int) Messages.Rotating, "[Interaction] Rotating Object." },
      { (int) Messages.Selected, "[Interaction] Object selected." },
      { (int) Messages.CameraMove,  "[Camera] Moving." },
      { (int) Messages.CameraRelease, "[Camera] Released." },
      { (int) Messages.Ready, "Ready." },
      { (int) Messages.Clone, "[Interaction] Object was cloned." },
      { (int) Messages.Delete, "[Interaction] Object was deleted." },
      { (int) Messages.SelectForClone, "[Interaction] Select an object before trying to clone." },
      { (int) Messages.SelectForDelete, "[Interaction] Select an object before trying to delete." },
    };

    shift = new Dictionary<int, string>() {
      { (int) ShiftMessages.Clear, "" },
      { (int) ShiftMessages.Move, "Vertical" },
      { (int) ShiftMessages.Scale, "Uniform" },
      { (int) ShiftMessages.Rotate, "Via Axis" },
    };

    bars[(int)MessageBox.Shift].text = shift[(int)ShiftMessages.Move];
  }

  /// <summary>
  /// Given a Message enum content, populates console bar with it.
  /// </summary>
  /// <param name="content">Message content to populate box with.</param>
  public static void SetMessage(Messages content)
  {
    bars[(int)MessageBox.Console].text = text[(int)content];
  }

  /// <summary>
  /// Given a ShiftMessage enum content, populates shift bar with it.
  /// </summary>
  /// <param name="content"></param>
  public static void SetShiftMessage(ShiftMessages content)
  {
    bars[(int)MessageBox.Shift].text = shift[(int)content];
  }
}
