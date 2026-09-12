using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace DeagleHsOnly;

[MinimumApiVersion(80)]
public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "3.0.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription => "Only headshots deal damage. Everything else is blocked before it is ever applied.";

    // CS2 hitgroup constant for the head.
    private const int HITGROUP_HEAD = 1;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnPlayerTakeDamagePre>(OnPlayerTakeDamagePre);
        Logger.LogInformation("[DeagleHsOnly] Plugin loaded (pre-damage hook, headshot-only).");
    }

    private HookResult OnPlayerTakeDamagePre(CCSPlayerPawn player, CTakeDamageInfo info)
    {
        if (info.Hitgroup != HITGROUP_HEAD)
        {
            // Blocks the hit entirely before any damage is applied.
            // Bullet effectively passes straight through - zero damage, no health flicker.
            return HookResult.Handled;
        }

        return HookResult.Continue;
    }
}
