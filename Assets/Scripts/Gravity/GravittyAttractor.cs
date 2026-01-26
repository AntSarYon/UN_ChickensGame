using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravittyAttractor : MonoBehaviour
{
    public static GravittyAttractor Instance;

    public float gravityForce = 10.0f;

    // ----------------------------------------

    void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------

    public void Attract(Transform body)
    {
        // Obtenemos la ubicacion del jugador desde el centro del planeta
        Vector3 attractionDirection = (transform.position - body.position).normalized;

        //Le aplicamos una fuerza (Gravedad ficticia) al cuerpo
        body.GetComponent<Rigidbody>().AddForce(attractionDirection * gravityForce);

        // Definimos la rotacion obJetivo...
        Quaternion targetRotation = Quaternion.FromToRotation(
            body.up, // Cuerpo arriba
            -attractionDirection // gravedad hacia arriba(en positivo)
            ) * body.rotation; // A la rotacion obtenida le agregamos la rotacion actual del objeto

        //Hacemos que la rotacion del cuerpo rote hacia la objetivo mediante una interpolacion ESFERICA (SLerp)
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
