using UnityEngine;

public class BackgroundRenderer : MonoBehaviour
{
    Renderer r;
    Vector2 offSet = new Vector2(0, 0);
    public float speed = 2;
    void Awake()
    {
        r = GetComponent<Renderer>();
    }
    // Update is called once per frame
   void Update()
    {
        Material mat = r.sharedMaterial;
        offSet.x += speed * Time.deltaTime;
        mat.mainTextureOffset = offSet;
    }
}
