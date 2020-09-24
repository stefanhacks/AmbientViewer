using UnityEngine;
using System.Collections.Generic;

public enum MessageBox
{
  Console = 0
}

public class GUI : MonoBehaviour
{
  // Parts, some serialized so they are visible on editor but not explicitly public.
  [SerializeField] private TMPro.TextMeshProUGUI consoleBar = null;
  static Dictionary<int, TMPro.TextMeshProUGUI> bars;

  /// <summary>
  /// Readies internal objects for use.
  /// </summary>
  public void Setup()
  {
    bars = new Dictionary<int, TMPro.TextMeshProUGUI>() {
      { (int)MessageBox.Console, consoleBar },
    };
  }

  /// <summary>
  /// Given a MessageBox enum value, targets a text box and sets it message.
  /// </summary>
  /// <param name="target">MessageBox enum value of the text box to change.</param>
  /// <param name="message">Message to set.</param>
  public static void SetMessage(MessageBox target, string message)
  {
    bars[(int)target].text = message;
  }
}
