﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackableStatus
{
    void GainStack(IStackableStatus other);

}
