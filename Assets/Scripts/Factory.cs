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
    {"modelo01", "Furniture_ges1/bed1/bed1"},
    {"modelo02", "Furniture_ges1/printer/printer"},
    {"modelo03", "Furniture_ges1/speaker/speaker"},
    {"modelo04", "Furniture_ges1/glass_table/glass_table"},
    {"modelo05", "Furniture_ges1/tv/tv1"},

  };

  /// <summary>
  /// Instantiates GameObject from Furniture data.
  /// </summary>
  /// <param name="data">Furniture data</param>
  /// <returns></returns>
  public static GameObject Furniture(Furniture data)
  {
    // Replace with actual 3D model.
    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    obj.name = data.name;
    // GameObject obj = new GameObject(data.name);

    // Sets data.
    Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
    Vector3 rotation = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);
    Vector3 scale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);

    obj.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
    obj.transform.localScale = scale;

    return new GameObject();
  }
}
