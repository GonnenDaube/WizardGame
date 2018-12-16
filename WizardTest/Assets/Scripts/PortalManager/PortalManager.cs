using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{

    public GameObject worldGenerator;
    public GameObject transition;

    private string world_target;
    private string portal_target;

    public void SetLink(string world_target, string portal_target)
    {
        this.world_target = world_target;
        this.portal_target = portal_target;
    }

    public void Travel()
    {
        transition.GetComponent<Animator>().SetBool("isTraveling", true);
        StartCoroutine(RegenerateWorld());
    }

    private IEnumerator RegenerateWorld()
    {
        yield return new WaitForSeconds(0.25f);
        WorldGenerator generator = worldGenerator.GetComponent<WorldGenerator>();
        if (!generator.IsRegenerating())
        {
            generator.world_id = world_target;
            generator.RegenerateWorld(portal_target);
        }
    }
}
