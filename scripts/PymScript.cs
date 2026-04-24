using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PymScript : MonoBehaviour
{

    public Material growthMaterial;
    public float offset = 5f;
    public float endTime = 1000f;

    Renderer[] _renderers;
    Material[][] _originalMaterials;
    Material[] _growthInstances;
    private bool _growth = false;

    static readonly int SizeID = Shader.PropertyToID("_Growth_Scale");
    static readonly int TimeID = Shader.PropertyToID("_TimeBase");

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        //Yes, ideally I'm stacking more materials on this object, but I know this logic won't work if I want to combine shaders
        _originalMaterials = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].sharedMaterials;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !_growth)
        {
            StartGrowth();
            endTime = Time.time + offset;
        }

        if (_growth && _growthInstances != null)
        {
            foreach (var mat in _growthInstances)
            {
                if (mat == null) continue;

                float current = mat.GetFloat(SizeID);
                if (current == 1) 
                    mat.SetFloat(TimeID, 0f);
            }

            if (GrowthFinished())
            {
                gameObject.SetActive(false);
            }
        }

    }
    public void StartGrowth()
    {
        _growth = true;

        _growthInstances = new Material[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            var inst = Instantiate(growthMaterial);
            inst.name = _renderers[i].name + "_growth";
            _growthInstances[i] = inst;

            var mats = new System.Collections.Generic.List<Material>(_originalMaterials[i]);
            mats.Add(inst);
            if(mats.Count > 0) mats.RemoveAt(0);
            _renderers[i].materials = mats.ToArray();

        }
    }

    private bool GrowthFinished()
    {
        return Time.time >= endTime;
    }
}
