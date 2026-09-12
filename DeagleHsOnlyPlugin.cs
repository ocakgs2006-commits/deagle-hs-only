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
    public override string ModuleVersion => "2.1.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription => "Only headshots deal damage. All other hits pass through with zero damage.";

    private const int HITGROUP_HEAD = 1;

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt, HookMode.Post);
        Logger.LogInformation("[DeagleHsOnly] Plugin loaded v2.1.0");
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var victim = @event.Userid;
        if (victim == null || !victim.IsValid || victim.PlayerPawn?.Value == null)
            return HookResult.Continue;

        var pawn = victim.PlayerPawn.Value;
        if (!pawn.IsValid)
            return HookResult.Continue;

        bool isHeadshot = @event.Hitgroup == HITGROUP_HEAD;
        int dmgHealth = @event.DmgHealth;

        Logger.LogInformation("[DeagleHsOnly] hitgroup=" + @event.Hitgroup + " weapon=" + @event.Weapon + " dmgHealth=" + dmgHealth + " healthBefore=" + pawn.Health);

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
