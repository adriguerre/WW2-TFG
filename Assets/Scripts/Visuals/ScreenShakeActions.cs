using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ScreenShakeActions : MonoBehaviour
{

    void Start()
    {
        ShootAction.onAnyShoot += ShootAction_OnAnyShoot;
        SniperShootAction.onAnyShoot += SniperShoot_OnAnyShoot;
        SupressionAction.onAnySupressionShoot += SupressionAction_OnAnySupressionShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        SwordAction.onAnySwordHit += SwordAction_OnAnySwordHit;
        RocketProjectile.onAnyRocketExploded += RocketProjectile_OnAnyRocketExploded;
        TNTObject.onAnyTNTExploded += TNT_OnANyTNTExploded;
    }

    private void TNT_OnANyTNTExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(4f);
    }

    private void SniperShoot_OnAnyShoot(object sender, SniperShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(1f);
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
    private void SupressionAction_OnAnySupressionShoot(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(3f);
    }


    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void RocketProjectile_OnAnyRocketExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
