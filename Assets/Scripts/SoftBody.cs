using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SoftBody : MonoBehaviour
{
    [Header("Deformation Settings")]
    [SerializeField] private float deformationRadius = 0.5f;
    [SerializeField] private float deformationForce = 0.2f;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private Mesh _runtimeMesh;
    private Vector3[] _baseVertices;
    private Vector3[] _modifiedVertices;

    private const float MaxDisplacementFactor = 0.2f;

    private void Awake()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        _runtimeMesh = Instantiate(skinnedMeshRenderer.sharedMesh);
        skinnedMeshRenderer.sharedMesh = _runtimeMesh;

        _baseVertices = _runtimeMesh.vertices;
        _modifiedVertices = new Vector3[_baseVertices.Length];
        _baseVertices.CopyTo(_modifiedVertices, 0);
    }

    public void AddForce(Vector3 point)
    {
        DeformMesh(point);
    }

    private void DeformMesh(Vector3 point)
    {
        for (int i = 0; i < _baseVertices.Length; i++)
        {
            var worldVertex = transform.TransformPoint(_baseVertices[i]);
            var distance = Vector3.Distance(worldVertex, point);
            if (distance > deformationRadius)
                continue;

            var falloff = 1f - distance / deformationRadius;
            var displacement = Mathf.Min(deformationForce * falloff, deformationRadius * MaxDisplacementFactor);
            var direction = (worldVertex - point).normalized;
            var displacedVertex = worldVertex + direction * displacement;
            _modifiedVertices[i] = transform.InverseTransformPoint(displacedVertex);
        }

        _runtimeMesh.vertices = _modifiedVertices;
        _runtimeMesh.RecalculateNormals();
        _runtimeMesh.RecalculateBounds();
    }

    public void ResetMesh()
    {
        _runtimeMesh.vertices = _baseVertices;
        _runtimeMesh.RecalculateNormals();
        _runtimeMesh.RecalculateBounds();
    }
}