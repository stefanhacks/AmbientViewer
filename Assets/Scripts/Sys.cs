using UnityEngine;
using System.Net;

public class Sys : MonoBehaviour
{
  WebClient client;
  string dataURL = "https://s3-sa-east-1.amazonaws.com/static-files-prod/unity3d/models.json";

  void Start()
  {
    this.client = new WebClient();
    this.RequestPresets(this.dataURL);
  }

  /// <summary>
  /// Requests and parses JSON from the given url.
  /// Expects JSON object to fit ServerData struct.
  /// </summary>
  /// <param name="url"></param>
  /// <returns>Parsed ServerData object.</returns>
  private ServerData RequestPresets(string url)
  {
    // For the sake of Console Sanity.
    Debug.ClearDeveloperConsole();

    string json = this.client.DownloadString(this.dataURL);
    return JsonUtility.FromJson<ServerData>(json);
  }
}
