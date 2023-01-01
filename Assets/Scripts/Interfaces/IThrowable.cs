using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable
{
    /// <summary>
    /// commence le mouvement de l'objet à lancer
    /// </summary>
    /// <param name="xThrower">la position en X du lanceur, pour savoir vers ou lancer l'objet</param>
    /// <param name="throwerSpeed">la vitesse du lanceur, qui sera transmise à l'objet</param>
    void getThrown(float xThrower, float throwerSpeed);
}
