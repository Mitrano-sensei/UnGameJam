using System;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
public class AttackGA : GameAction
{
    [SerializeField, SerializeReference, SR] private ProjectileSettings projectileSettings;
    
    public ProjectileSettings ProjectileSettings => projectileSettings;
}