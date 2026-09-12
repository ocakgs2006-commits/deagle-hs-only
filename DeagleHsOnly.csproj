using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace DeagleHsOnly;

[MinimumApiVersion(80)]
public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription => "Only headshots deal damage. All other hits pass through with zero damage (health is restored instantly).";

    // true  -> rule only applies to Deagle hits (other weapons behave normally)
    // false -> rule applies to EVERY weapon (any non-headshot hit anywhere = 0 damage)
    private const bool OnlyRestrictDeagle = true;

    // CS2 hitgroup constant for the head.
    private const int HITGROUP_HEAD = 1;

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt, HookMode.Post);
        Logger.LogInformation("[DeagleHsOnly] Plugin loaded. OnlyRestrictDeagle = {Flag}", OnlyRestrictDeagle);
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var victim = @event.Userid;
        if (victim == null || !victim.IsValid || victim.PlayerPawn?.Value == null)
            return HookResult.Continue;

        var pawn = victim.PlayerPawn.Value;
        if (!pawn.IsValid || pawn.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            return HookResult.Continue;

        bool isHeadshot = @event.Hitgroup == HITGROUP_HEAD;
        bool isDeagle = (@event.Weapon ?? string.Empty).Contains("deagle");

        bool shouldNegateDamage = !isHeadshot && (!OnlyRestrictDeagle || isDeagle);

        if (!shouldNegateDamage)
            return HookResult.Continue;

        int dmgHealth = @event.DmgHealth;
        if (dmgHealth > 0)
        {
            int newHealth = pawn.Health + dmgHealth;
            if (newHealth > 100) newHealth = 100;

            pawn.Health = newHealth;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        }

        return HookResult.Continue;
    }
}
