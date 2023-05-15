using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialGlowVehicle : MonoBehaviour
{
    public GameObject[] lightObject;
    private float lightDelayFastGlow = 0.15f;
    private float lightDelaySlowGlow = 0.025f;
    public bool fastGlow = false;
    public bool slowGlow = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Glow());
    }

    // glow material
    IEnumerator Glow()
    {
        float minIntensity = 0f;
        float maxIntensity = 60f;
        Color emissionColor = lightObject[0].GetComponent<Renderer>().material.GetColor("_EmissionColor");

        while (fastGlow)
        {
            yield return new WaitForSeconds(lightDelayFastGlow);
            for (int j = 0; j < lightObject.Length; j++)
            {
                lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * maxIntensity);
            }
            yield return new WaitForSeconds(lightDelayFastGlow);
            for (int j = 0; j < lightObject.Length; j++)
            {
                lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * minIntensity);
            }
        }

        while (slowGlow)
        {
            for (float i = maxIntensity; i >= minIntensity; i--)
            {
                yield return new WaitForSeconds(lightDelaySlowGlow);
                for (int j = 0; j < lightObject.Length; j++)
                {
                    lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * i);
                }
            }
            for (float i = minIntensity; i <= maxIntensity; i++)
            {
                yield return new WaitForSeconds(lightDelaySlowGlow);
                for (int j = 0; j < lightObject.Length; j++)
                {
                    lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * i);
                }
            }
        }
    }
}




