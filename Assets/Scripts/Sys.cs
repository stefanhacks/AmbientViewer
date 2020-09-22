using UnityEngine;
using System.Net;

public class Sys : MonoBehaviour
{
  // Web Client
  WebClient client;
  string dataURL = "https://s3-sa-east-1.amazonaws.com/static-files-prod/unity3d/models.json";

  // Aux objects
  public Transform furnitureRoot;

  void Start()
  {
    if (this.furnitureRoot == null)
      throw new System.Exception("No transform provided to be used as root for Furniture Objects.");

    this.client = new WebClient();
    ServerData data = this.RequestPresets(this.dataURL);
    this.MakeObjects(data);
  }

  /// <summary>
  /// Requests and parses JSON from the given url.
  /// Expects JSON object to fit ServerData struct.
  /// </summary>
  /// <param name="url">Url to fetch JSON from.</param>
  /// <returns>Parsed ServerData object.</returns>
  private ServerData RequestPresets(string url)
  {
    // For the sake of Console Sanity.
    Debug.ClearDeveloperConsole();

    string json = this.client.DownloadString(this.dataURL);
    return JsonUtility.FromJson<ServerData>(json);
  }

  /// <summary>
  /// Given ServerData, plots an object for every datapoint.
  /// </summary>
  /// <param name="data">Parsed ServerData object.</param>
  private void MakeObjects(ServerData data)
  {
    foreach (var model in data.models)
    {
      GameObject box = Factory.Furniture(model);
      box.transform.parent = this.furnitureRoot;
    }
  }
}
