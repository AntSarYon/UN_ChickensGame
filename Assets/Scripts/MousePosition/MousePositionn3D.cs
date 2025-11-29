using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MousePositionn3D : MonoBehaviour
{
    public static MousePositionn3D Instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask floorLayer;

    // -----------------------------------------------------

    void Awake()
    {
        Instance = this;
    }

    // ------------------------------------------------------

    // Update is called once per frame
    public Vector3 GetMouseWorldPosition()
    {
        //Lanzamos un rayo desde la camara, tomando como base la posicion del Mouse
        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        //Si el Rayo topa con el suelo en algun punto... (Almacenamos la informacion en un RaycastHit)
        if (Physics.Raycast(mouseRay, out RaycastHit raycastHit, float.MaxValue, floorLayer))
        {
            //Hacemos que la Posicion de este objeto coincida donde el Ray choca
            return raycastHit.point;
        }
        //Si el Rayo no toca el Suelo
        else
        {
            //Devolvemos el punto de Spawn del Corral en turno
            return YardsManager.instance.currentYard.PosToSpawn;
        }
    }
}
