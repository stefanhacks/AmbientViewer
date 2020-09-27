using UnityEngine;
using System.Net;
using System.Threading.Tasks;

[RequireComponent(typeof(InteractionController))]
public class Sys : MonoBehaviour
{
  // Web Client
  private WebClient client;
  private string dataURL = "https://s3-sa-east-1.amazonaws.com/static-files-prod/unity3d/models.json";

  // Prefab Paths
  private string guiPath = "GUI/GUI";

  // Aux objects
  public Transform furnitureRoot;

  async void Start()
  {
    // For the sake of Console Sanity.
    Debug.ClearDeveloperConsole();

    if (this.furnitureRoot == null)
      throw new System.Exception("No transform provided to be used as root for Furniture Objects.");

    // Readying loading tasks.
    Task<GameObject> gui = this.LoadGUI();
    Task<ServerData> web = this.RequestPresets(this.dataURL);

    // Await for tasks to end, then boot.
    await Task.WhenAll(new Task[] { gui, web });
    this.SetToLoad(gui.Result, web.Result);
  }

  #region Loading Tasks
  /// <summary>
  /// Asynchronously loads GUI.
  /// </summary>
  /// <returns>GUI as GameObject</returns>
  private async Task<GameObject> LoadGUI()
  {
    Object prefab = await Resources.LoadAsync<GameObject>(this.guiPath);
    return prefab as GameObject;
  }

  /// <summary>
  /// Asynchronously requests and parses JSON from the given
  /// url. Expects JSON object to fit ServerData struct.
  /// </summary>
  /// <param name="url">Url to fetch JSON from.</param>
  /// <returns>Parsed ServerData object.</returns>
  private async Task<ServerData> RequestPresets(string url)
  {
    this.client = new WebClient();
    string json = await this.client.DownloadStringTaskAsync(this.dataURL);
    return JsonUtility.FromJson<ServerData>(json);
  }
  #endregion

  /// <summary>
  /// Instantiates the GUI prefab and, given 
  /// ServerData, plots an object for every datapoint.
  /// </summary>
  /// <param name="gui">GUI GameObject.</param>
  /// <param name="data">Parsed ServerData object.</param>
  private void SetToLoad(GameObject gui, ServerData data)
  {
    this.GetComponent<InteractionController>().FurnitureRoot = this.furnitureRoot;

    GameObject uGUI = Instantiate(gui);
    foreach (var model in data.models)
    {
      GameObject box = Factory.Furniture(model);
      box.transform.parent = this.furnitureRoot;
    }

    GUI gm = uGUI.GetComponent<GUI>();
    gm.Setup();
    GUI.SetMessage(Messages.Ready);

  }
}
