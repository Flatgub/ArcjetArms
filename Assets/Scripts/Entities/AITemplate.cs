using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiTemplate
{
    int DifficultyScore { get; }
    void ApplyTo(Entity entity);

}
