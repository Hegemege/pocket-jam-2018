using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardController : AIAgentController
{
    public StockType PreferredStockType;

    protected override void ReachedTarget()
    {
        // TODO: update station lizard count?
    }
}
