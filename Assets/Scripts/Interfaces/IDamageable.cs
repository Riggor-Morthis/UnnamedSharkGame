using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Comment le personnage perd des points de vie
    /// </summary>
    /// <param name="damages">Le nombre de points de vie � perdre</param>
    void TakeDamage(int damages);
}
