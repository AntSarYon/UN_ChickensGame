using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickeableObject : Interactable
{
    public bool isPickeable;

    private Transform defaultParent;

    private Rigidbody mRb;
    private Collider mCollider;

    // ---------------------------------------

    void Awake()
    {
        isPickeable = true;
        interactionMessage = "Agarrar";

        //Almacenamos referencia al Parent inicial del Pickeable
        defaultParent = transform.parent;

        mRb = GetComponent<Rigidbody>();
        mCollider = GetComponent<Collider>();
    }

    // ----------------------------------------------------

    public void Pick(Transform holdingZone)
    {
        //Desactivamos flag de "Se puede agarrar"
        isPickeable = false;

        //Desactivamos su Coision
        mCollider.enabled = false;

        //Le desactivamos la gravedad
        mRb.useGravity = false;

        //Lo marcamos como Kinemático para que las Físicas no le afecten.
        mRb.isKinematic = true;

        // Fijamos el Pickeabe en la zona de Hold (Lo hacems hijo)
        transform.position = holdingZone.position;
        transform.SetParent(holdingZone);

        SetInteractioonMessage("Soltar");

    }

    // ----------------------------------------------------

    public void Drop()
    {
        //Desactivamos flag de "Se puede agarrar"
        isPickeable = true;

        //Desactivamos su Coision
        mCollider.enabled = true;

        //Le desactivamos la gravedad
        mRb.useGravity = true;

        //Lo marcamos como Kinemático para que las Físicas no le afecten.
        mRb.isKinematic = false;

        SetInteractioonMessage("Agarrar");

        //Devolvemos a Parent original de este bjeto (Si es que lo tiene)
        if (defaultParent) transform.SetParent(defaultParent);
        else transform.SetParent(null);
    }

    // ----------------------------------------------------

    public void Throw(Vector3 throwDirection)
    {
        //Soltamos el Objeto
        Drop();

        //Reproducimos soindo de lanzamiento
        GameSoundsController.Instance.PlayThrowSound();

        //Le aplicamos fuerza en la direccion recibida
        mRb.AddForce(throwDirection * 75, ForceMode.Impulse);
    }

    // -----------------------------------------------------

    public override void Interact(Transform holdingZone, InteractionController interactionController = null)
    {
        //Llamamos a la funcion para Recoger
        Pick(holdingZone);
    }

    // -----------------------------------------------------------------
}
