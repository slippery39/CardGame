using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolveController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.0125f;
    public bool alive = false;

    public Animator animator;
    public float refreshRate = 0.025f;
    private Material[] skinnedMaterials;
    // Start is called before the first frame update
    void Start()
    {
        if (skinnedMesh != null)
        {
            skinnedMaterials = skinnedMesh.materials;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        alive = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }

        if (skinnedMaterials.Length > 0)
        {
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                 


                    yield return new WaitForSeconds(refreshRate);
                }
            }
        }

        animator.SetBool("Die", false);
        foreach (var material in skinnedMaterials)
        {
            material.SetFloat("_DissolveAmount", 0);
            VFXGraph.gameObject.SetActive(false);            
        }
        
    }


}
