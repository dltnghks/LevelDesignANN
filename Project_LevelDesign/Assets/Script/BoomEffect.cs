using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyEffect());
    }
    private IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
