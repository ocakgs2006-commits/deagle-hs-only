using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace DeagleHsOnly;

[MinimumApiVersion(80)]
public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "3.1.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription => "Deagle: only headshots deal damage. Reserve ammo never runs out (clip still empties/reloads normally).";

    private const int HITGROUP_HEAD = 1;
    private const int FixedReserveAmmo = 999;

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt, HookMode.Post);
        RegisterListener<Listeners.OnTick>(OnTick);
        Logger.LogInformation("[DeagleHsOnly] Plugin loaded v3.1.0 (infinite reserve ammo)");
    }

    private void OnTick()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || !player.PawnIsAlive)
                continue;

            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid)
                continue;

            var weaponServices = pawn.WeaponServices;
            if (weaponServices == null)
                continue;

            var activeWeapon = weaponServices.ActiveWeapon.Value;
            if (activeWeapon == null || !activeWeapon.IsValid)
                continue;

            if (activeWeapon.ReserveAmmo[0] < FixedReserveAmmo)
            {
                activeWeapon.ReserveAmmo[0] = FixedReserveAmmo;
                Utilities.SetStateChanged(activeWeapon, "CBasePlayerWeapon", "m_pReserveAmmo");
            }
        }
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var victim = @event.Userid;
        if (victim == null || !victim.IsValid || victim.PlayerPawn?.Value == null)
            return HookResult.Continue;

        var pawn = victim.PlayerPawn.Value;
        if (!pawn.IsValid)
            return HookResult.Continue;

        string weapon = @event.Weapon ?? string.Empty;
        bool isDeagle = weapon.Contains("deagle");

        if (!isDeagle)
            return HookResult.Continue;

        bool isHeadshot = @event.Hitgroup == HITGROUP_HEAD;
        int dmgHealth = @event.DmgHealth;

        Logger.LogInformation("[DeagleHsOnly] hitgroup=" + @event.Hitgroup + " weapon=" + weapon + " dmgHealth=" + dmgHealth + " healthBefore=" + pawn.Health);

        if (!isHeadshot && dmgHealth > 0)
        {
            int newHealth = pawn.Health + dmgHealth;
            if (newHealth > 100) newHealth = 100;

            pawn.Health = newHealth;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

            Logger.LogInformation("[DeagleHsOnly] restored health to " + newHealth);
        }

        return HookResult.Continue;
    }
}
