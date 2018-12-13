using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class WorldGenerator : MonoBehaviour
{
    public HttpRequest request;
    public GameObject _layer;
    public GameObject _portal;
    public GameObject _sprite;
    public GameObject player;
    public GameObject title;
    public GameObject transition;
    public string world_id;
    private WorldData.World world;
    private string start_portal;

    private Dictionary<string, SpriteLoadData> sprites;
    private BinaryFormatter formatter;

    private string cachePath;

    void Start()
    {
        formatter = new BinaryFormatter();
        cachePath = Application.persistentDataPath + "/Cache/";
        if (!Directory.Exists(cachePath + "/Cache/Sprites"))
        {
            Directory.CreateDirectory(cachePath + "/Sprites");
        }
        sprites = new Dictionary<string, SpriteLoadData>();
        RegenerateWorld("");
    }

    public void RegenerateWorld(string start_portal)
    {
        GameObject layer;
        for (int i = 0; i < 6; i++)
        {
            layer = GameObject.Find("Layer" + i);
            if (layer != null)
                GameObject.Destroy(layer);
        }
        this.start_portal = start_portal;
        request.Request("WorldBuilder/_GetWorld?id=" + world_id, BuildWorld);
    }

    private IEnumerator BuildWorld(WWW req)
    {
        yield return req;
        WorldData.World world = JsonConvert.DeserializeObject<WorldData.World>(req.text);
        GenerateWorld(world);
    }

    private void GenerateWorld(WorldData.World world)
    {
        WorldData.Layer l;
        WorldData.Layer objectLayer;
        Portal portal;
        GameObject layer;
        GameObject portalObj;
        MeshFilter meshFilter;
        PolygonCollider2D polygonCollider;
        MeshRenderer meshRenderer;
        Mesh mesh;
        List<Vector2> line;
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uvs;
        List<string> sprite_ids = new List<string>();
        SpriteData data;
        FileStream fileStream;
        for (int j = 0; j < world.layers.Count; j++)
        {
            l = world.layers[j];
            layer = (GameObject)Instantiate(_layer);
            mesh = new Mesh();
            vertices = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();
            line = WorldData.Layer.GetCurvedLine(l.x, l.y);
            foreach (Vector2 point in line)
            {
                vertices.Add(new Vector3(point.x, point.y, 0.0f));
            }
            triangles = WorldData.Layer.GenerateTriangles(line);
            foreach (Vector2 vec in vertices)
            {
                uvs.Add(new Vector2(vec.x / l.size, vec.y / 100));
            }

            foreach (WorldData.Sprite sprite in l.sprites)
            {
                if (File.Exists(cachePath + "/Sprites/" + sprite.id + ".bin"))
                {
                    if (!sprites.ContainsKey(sprite.id))
                    {
                        fileStream = new FileStream(cachePath + "/Sprites/" + sprite.id + ".bin", FileMode.Open);
                        data = (SpriteData)formatter.Deserialize(fileStream);
                        AddSprite(sprite.id, data);
                    }
                }
                else if (!sprite_ids.Contains(sprite.id))
                    sprite_ids.Add(sprite.id);
            }

            //init mesh
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateBounds();
            meshFilter = layer.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            if (j == 4)
            {
                polygonCollider = layer.GetComponent<PolygonCollider2D>();
                List<Vector2> vert2 = new List<Vector2>();
                foreach (Vector3 vert in vertices)
                {
                    vert2.Add(new Vector2(vert.x, vert.y - 1.0f));
                }
                polygonCollider.points = vert2.ToArray();
            }
            meshRenderer = layer.GetComponent<MeshRenderer>();
            meshRenderer.material.SetColor("_Color", new Color(l.color[0] / 255.0f, l.color[1] / 255.0f, l.color[2] / 255.0f));
            layer.name = "Layer" + j;
            layer.layer = LayerMask.NameToLayer(layer.name.ToLower());
            layer.transform.position = new Vector3(-(16.0f / 9.0f) * 5.0f, 0.0f, world.layers.Count - j);
            objectLayer = layer.GetComponent<WorldData.Layer>();
            objectLayer.Copy(l);
        }

        layer = GameObject.Find("Layer4");
        player.GetComponent<Rigidbody2D>().isKinematic = false;

        foreach (Portal p in world.layers[4].portals)
        {
            portalObj = (GameObject)Instantiate(_portal);
            portal = portalObj.GetComponent<Portal>();
            portal.x = p.x;
            portal.y = p.y;
            portal.name = p.name;
            portal.link = p.link;
            portal.title = title;
            portalObj.layer = layer.layer;
            portalObj.transform.position = new Vector3(world.layers[4].Ratio * (p.x / 10.0f - 5.0f), (100 - p.y) / 10.0f - 5.0f, 0.0f);
            portalObj.transform.parent = layer.transform;
        }
        this.world = world;
        Movement movement = player.GetComponent<Movement>();
        movement.SetPlayerPosition();
        if (sprite_ids.Count > 0)
        {
            string ids = JsonConvert.SerializeObject(sprite_ids);
            request.Request("SpriteBuilder/_GetSpriteSources", ids, AddSprites);
        }
        else
        {
            if (start_portal != "")
                request.Request("Worlds/_GetPortal?id=" + start_portal, SetInitialPosition);
            AddSpriteObjectsToWorld(new Dictionary<string, Tuple<string, bool, string>>());
        }
    }

    private IEnumerator AddSprites(WWW req)
    {
        yield return req;
        Dictionary<string, Tuple<string, bool, string>> sprite_sources = JsonConvert.DeserializeObject<Dictionary<string, Tuple<string, bool, string>>>(req.text);
        if (start_portal != "")
            request.Request("Worlds/_GetPortal?id=" + start_portal, SetInitialPosition);
        AddSpriteObjectsToWorld(sprite_sources);
    }

    private IEnumerator SetInitialPosition(WWW req)
    {
        yield return req;
        Vector2 pos = JsonConvert.DeserializeObject<Vector2>(req.text);
        Movement movement = player.GetComponent<Movement>();
        WorldData.Layer layer = GameObject.Find("Layer4").GetComponent<WorldData.Layer>();
        pos = new Vector2(world.layers[4].Ratio * (pos.x / 10.0f - 5.0f), (100 - pos.y) / 10.0f - 5.0f);
        movement.SetPlayerPosition(pos);
    }

    private void AddSpriteObjectsToWorld(Dictionary<string, Tuple<string, bool, string>> sources)
    {
        WorldData.Layer layer;
        GameObject worldLayer;
        foreach (var kvp in sources)
        {
            AddSprite(kvp);
        }
        for (int i = 0; i < world.layers.Count; i++)
        {
            layer = world.layers[i];
            worldLayer = GameObject.Find("Layer" + i);
            foreach (WorldData.Sprite s in layer.sprites)
            {
                if (sprites[s.id].scriptable)
                {
                    InstantiateScriptable(s, worldLayer, layer, i);
                }
                else
                {
                    InstantiateSprite(s, worldLayer, layer, i);
                }
            }
        }
        if (GameObject.FindWithTag("Task") != null)
            GameObject.FindWithTag("Task").GetComponent<Task>().ReloadScriptables();
        transition.GetComponent<Animator>().SetBool("isTraveling", false);
    }

    private void InstantiateSprite(WorldData.Sprite s, GameObject worldLayer, WorldData.Layer layer, int i)
    {
        Vector3 pos = new Vector3(layer.Ratio * (s.x / 10.0f - 5.0f), (100 - s.y) / 10.0f - 5.0f, world.layers.Count - i - (s.zIndex + 1) / 1000.0f);
        float scale = layer.Ratio * s.size / 10.0f;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -s.rotation);
        GameObject worldSprite = (GameObject)Instantiate(_sprite);
        SpriteRenderer spriteRenderer = worldSprite.GetComponent<SpriteRenderer>();
        Sprite sprite = sprites[s.id].image;
        spriteRenderer.sprite = sprite;
        worldSprite.transform.parent = worldLayer.transform;
        worldSprite.transform.position = pos;
        worldSprite.transform.localScale *= scale;
        worldSprite.transform.rotation = rotation;
        worldSprite.layer = worldLayer.layer;
    }

    private void InstantiateScriptable(WorldData.Sprite s, GameObject worldLayer, WorldData.Layer layer, int i)
    {
        Vector3 pos = new Vector3(layer.Ratio * (s.x / 10.0f - 5.0f), (100 - s.y) / 10.0f - 5.0f, world.layers.Count - i - (s.zIndex + 1) / 1000.0f);
        float scale = layer.Ratio * s.size / 10.0f;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -s.rotation);
        string pathname = "Scriptables/" + sprites[s.id].name;
        GameObject scriptable = (GameObject)Instantiate((GameObject)Resources.Load(pathname));
        UpdateLayer(scriptable, LayerMask.NameToLayer(worldLayer.name.ToLower()));
        scriptable.transform.parent = worldLayer.transform;
        scriptable.transform.position = pos;
        scriptable.transform.localScale *= scale;
        scriptable.transform.rotation = rotation;
    }

    private void UpdateLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            UpdateLayer(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    private void AddSprite(KeyValuePair<string, Tuple<string, bool, string>> kvp)
    {
        string source;
        source = kvp.Value.Item1;
        byte[] data = System.Convert.FromBase64String(source.Substring(source.IndexOf("base64,") + "base64,".Length));
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(data);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), texture.width);
        sprites.Add(kvp.Key, new SpriteLoadData(sprite, kvp.Value.Item2, kvp.Value.Item3));
        //if reached here, then should save file to cache
        FileStream fileStream = File.Create(cachePath + "/Sprites/" + kvp.Key + ".bin");
        SpriteData sData = new SpriteData(data, kvp.Value.Item2, kvp.Value.Item3);
        formatter.Serialize(fileStream, sData);
        fileStream.Close();
    }

    private void AddSprite(string id, SpriteData data)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(data.image);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), texture.width);
        sprites.Add(id, new SpriteLoadData(sprite, data.scriptable, data.name));
    }
}
