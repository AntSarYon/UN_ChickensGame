using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalCharacter : MonoBehaviour
{
    private Rigidbody mRb;

    void Awake()
    {
        mRb = GetComponent<Rigidbody>();
    }

    // -----------------------------------------------
    void Start()
    {
        //Configuramos el Rigid body para restringir su rotacion, y que no use la gravedad
        mRb.useGravity = false;
        mRb.constraints = RigidbodyConstraints.FreezeRotation;
        // La funcion de Attraccion del Atractor sera lo que controle estas propiedades
    }

    void Update()
    {
        GravittyAttractor.Instance.Attract(transform);
    }
}
