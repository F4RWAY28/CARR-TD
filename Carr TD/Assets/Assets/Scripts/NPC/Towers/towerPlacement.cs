using UnityEngine;
using System.Collections;

public class towerPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    public Camera cam;
    public GameObject towerPrefab;
    public GameObject previewPrefab;
    public int towerCost = 100;
    public float heightOffset = 0f;
    public LayerMask groundMask;
    public float towerRange = 5f;
    public int rangeSegments = 50;
    [Tooltip("Scale of the transparent preview (X, Y, Z)")]
    public Vector3 previewScale = Vector3.one;

    private GameObject previewInstance;
    private LineRenderer rangeRenderer;
    private bool isPlacing = false;
    private bool canPlace = false;
    private Renderer[] previewRenderers;

    private Collider[] overlapResults = new Collider[10];
    public float checkRadius = 0.5f;

    void Update()
    {
        if (!isPlacing) return;

        UpdatePreview();

        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceTower();
        else if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    public void StartPlacement()
    {
        if (gameManager.Instance.money < towerCost || isPlacing) return;

        isPlacing = true;

        previewInstance = Instantiate(previewPrefab);

        // Set the preview scale using the Vector3
        previewInstance.transform.localScale = previewScale;

        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();

        SetPreviewTransparency(0.5f);
        AddOutline(previewInstance, Color.white, 0.05f);

        // Setup range visualization
        rangeRenderer = previewInstance.GetComponent<LineRenderer>();
        if (rangeRenderer == null) rangeRenderer = previewInstance.AddComponent<LineRenderer>();

        rangeRenderer.positionCount = rangeSegments + 1;
        rangeRenderer.startWidth = 0.05f;
        rangeRenderer.endWidth = 0.05f;
        rangeRenderer.useWorldSpace = true;

        Material lrMat = new Material(Shader.Find("Sprites/Default"));
        lrMat.color = Color.cyan;
        rangeRenderer.material = lrMat;

        DrawRangeCircle(previewInstance.transform.position, towerRange);
    }

    void UpdatePreview()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask)) return;

        previewInstance.transform.position = hit.point + Vector3.up * heightOffset;
        canPlace = false;

        // Check placement validity
        int hits = Physics.OverlapSphereNonAlloc(previewInstance.transform.position, checkRadius, overlapResults);
        bool collidingWithPath = false;
        bool onGround = hit.collider.CompareTag("Ground");

        for (int i = 0; i < hits; i++)
        {
            if (overlapResults[i].CompareTag("Enemy path"))
            {
                collidingWithPath = true;
                break;
            }
        }

        if (onGround && !collidingWithPath)
        {
            canPlace = true;
            SetPreviewColor(Color.green);
        }
        else
        {
            SetPreviewColor(Color.red);
        }

        DrawRangeCircle(previewInstance.transform.position, towerRange);
    }

    private void SetPreviewColor(Color color)
    {
        foreach (Renderer rend in previewRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    mat.color = new Color(color.r, color.g, color.b, c.a);
                }
            }
        }
    }

    private void SetPreviewTransparency(float alpha)
    {
        foreach (Renderer rend in previewRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;

                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
            }
        }
    }

    void PlaceTower()
    {
        if (gameManager.Instance.money < towerCost || !canPlace) return;

        GameObject tower = Instantiate(towerPrefab, previewInstance.transform.position, Quaternion.identity);
        gameManager.Instance.LoseMoney(towerCost);

        StartCoroutine(FlashPlacedTower(tower));
        EndPlacement();
    }

    private IEnumerator FlashPlacedTower(GameObject tower)
    {
        Renderer rend = tower.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            Color flash = Color.green * 0.5f + Color.white * 0.5f;
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                rend.material.color = Color.Lerp(flash, original, elapsed / duration);
                yield return null;
            }

            rend.material.color = original;
        }
    }

    void CancelPlacement() => EndPlacement();

    void EndPlacement()
    {
        if (previewInstance != null) Destroy(previewInstance);
        isPlacing = false;
        canPlace = false;
        rangeRenderer = null;
    }

    private void AddOutline(GameObject obj, Color outlineColor, float thickness)
    {
        foreach (Renderer rend in obj.GetComponentsInChildren<Renderer>())
        {
            GameObject outlineObj = Instantiate(rend.gameObject, rend.transform.position, rend.transform.rotation, rend.transform);
            outlineObj.transform.localScale *= (1f + thickness);
            Renderer outlineRend = outlineObj.GetComponent<Renderer>();

            Material outlineMat = new Material(rend.sharedMaterial);
            outlineMat.color = outlineColor;
            outlineRend.material = outlineMat;

            outlineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void DrawRangeCircle(Vector3 center, float radius)
    {
        if (rangeRenderer == null) return;

        float angleStep = 360f / rangeSegments;
        for (int i = 0; i <= rangeSegments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0.01f, Mathf.Sin(angle)) * radius;
            rangeRenderer.SetPosition(i, pos);
        }
    }
}