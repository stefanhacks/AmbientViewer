using System.Collections.Generic;
using UnityEngine;

public static class Factory
{

  /// <summary>
  /// Map containing references to the resource folder for JSON keys.
  /// </summary>
  /// <typeparam name="string">Model name.</typeparam>
  /// <typeparam name="string">Path to FBX asset.</typeparam>
  /// <returns></returns>
  public static Dictionary<string, string> modelMap = new Dictionary<string, string>() {
    {"modelo01", "Furniture_ges1/glass_table/little_glass_table"},
    {"modelo02", "Furniture_ges1/glass_table/glass_table"},
    {"modelo03", "Furniture_ges1/sek/sek4"},
    {"modelo04", "Furniture_ges1/tumba_fur/tumba_fur"},
    {"modelo05", "Furniture_ges1/bed1/bed1"},

  };

  /// <summary>
  /// Instantiates GameObject from Furniture data.
  /// </summary>
  /// <param name="data">Furniture data</param>
  /// <returns></returns>
  public static GameObject Furniture(Furniture data)
  {
    // Gets Resource Path
    string resPath;
    try
    {
      resPath = modelMap[data.name];
    }
    catch
    {
      // Minimal error handling.
      throw new System.Exception("FBX not found. Check Factory modelMap.");
    }

    // Loads Mesh into GameObject.
    GameObject mesh = Resources.Load<GameObject>(resPath);

    // Parses data.
    Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
    Vector3 rotation = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);
    Vector3 scale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);

    // Instantiates object.
    GameObject obj = Object.Instantiate(mesh, position, Quaternion.Euler(rotation));

    // Point of discussion: FBX's scale may not always be set to an absolute value, free assets'
    // used in this demo certainly aren't, but rather a relative one, so scaling the converted
    // Value to the Server Given one was the chosen approach. Should this inconsistency be
    // aknowledged and worked around accordingly, such as if server sent data being already 
    // parsed to all FBX patterns, this may be replaced with the direct approach:
    // obj.transform.localScale = scale;

    // Left as is to treat for the free asset scale discrepancy.
    obj.transform.localScale = Vector3.Scale(scale, obj.transform.localScale);

    return new GameObject();
  }
}
