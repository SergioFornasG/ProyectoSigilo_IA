using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]   //tag
public class EnemyFOVEditor : Editor
{
    private void OnSceneGUI()
    {
        var fov = (EnemyFOV)target; //ref a EnemyFOV
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.visionRadius);  //posicionObjeto, anguloGiraraAlrededor, dondeEmpiezaAngulo, anguloDondePuedeGirar, radio

        var visionAngleL = fov.AngleDirection(-fov.visionAngle / 2, false);
        var visionAngleR = fov.AngleDirection(fov.visionAngle / 2, false);
        Handles.DrawLine(fov.transform.position, fov.transform.position + visionAngleL * fov.visionRadius); //Desde transform, hasta la mitad izquierda del angulo. El radio para la distancia
        Handles.DrawLine(fov.transform.position, fov.transform.position + visionAngleR * fov.visionRadius); //Lo mismo pero mitad derecha

    }
}
