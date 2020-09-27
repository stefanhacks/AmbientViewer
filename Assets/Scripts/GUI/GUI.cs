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
}

public class GUI : MonoBehaviour
{
  // Parts, some serialized so they are visible on editor but not explicitly public.
  [SerializeField] private TMPro.TextMeshProUGUI consoleBar = null;
  [SerializeField] private TMPro.TextMeshProUGUI shiftBar = null;
  static Dictionary<int, TMPro.TextMeshProUGUI> bars;
  static Dictionary<int, string> text;

  /// <summary>
  /// Readies internal objects for use.
  /// </summary>
  public void Setup()
  {
    bars = new Dictionary<int, TMPro.TextMeshProUGUI>() {
      { (int) MessageBox.Console, consoleBar },
      { (int) MessageBox.Shift, shiftBar },
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
    };
  }

  /// <summary>
  /// Given a MessageBox enum value, targets a text box and sets it message.
  /// </summary>
  /// <param name="target">MessageBox enum value of the text box to change.</param>
  /// <param name="content">Message content to populate box with.</param>
  public static void SetMessage(MessageBox target, Messages content)
  {
    bars[(int)target].text = text[(int)content];
  }
}
