using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAABB : MonoBehaviour
{
    void OnDrawGizmos() {

        MeshRenderer mesh = GetComponent<MeshRenderer>();

        Gizmos.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
    }
}
