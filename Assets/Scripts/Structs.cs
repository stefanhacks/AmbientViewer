[System.Serializable]
public struct ServerData
{
  public Furniture[] models;
}

[System.Serializable]
public struct Furniture
{
  public string name;
  public float[] position;
  public float[] rotation;
  public float[] scale;
}