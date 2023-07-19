//Set VFX particle mesh from existing animation character.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SetBoids : MonoBehaviour
{
    Mesh mesh;

    [SerializeField]
    VisualEffect visualEffect;

    // Update is called once per frame
    void Update()
    {

        mesh = AvatarMeshNow.avatarMesh;

        if(mesh != null)
        {
            visualEffect.SetMesh("Mesh", mesh);
        }

    }
}