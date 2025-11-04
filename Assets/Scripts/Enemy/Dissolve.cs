using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;
    [SerializeField] private Material _mat;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    private Coroutine _dissolveRoutine;
    private WaitForSeconds _waitSeconds = new WaitForSeconds(1f);
    private WaitForSeconds _destroyWaitSeconds = new WaitForSeconds(8f);

    private float _lerpDissolve;
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start()
    {
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _mat = _skinnedMeshRenderer.material;
    }

    public void StartDissolveRoutine()
    {
        if (_dissolveRoutine == null)
            _dissolveRoutine = StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        yield return _waitSeconds;
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            _lerpDissolve = Mathf.Lerp(0f, 1.1f, elapsedTime / _dissolveTime);
            _mat.SetFloat(_dissolveAmount, _lerpDissolve);
            yield return null;
        }
        _dissolveRoutine = null;
        yield return _destroyWaitSeconds;
        Destroy(this.gameObject);
    }
}
