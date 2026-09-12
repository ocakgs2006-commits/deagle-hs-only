using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace DeagleHsOnly;

public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription =>
        "Deagle sadece headshot ile can goturur; body shot hasari etkisiz hale gelir. Diger silahlar (AWP dahil) etkilenmez.";

    // CS2/Source 2 hitgroup degerleri:
    // 0 = Generic, 1 = Head, 2 = Chest, 3 = Stomach, 4 = LeftArm,
    // 5 = RightArm, 6 = LeftLeg, 7 = RightLeg, 8 = Neck, 10 = Gear
    private const int HITGROUP_HEAD = 1;

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        Console.WriteLine("[DeagleHsOnly] Plugin yuklendi.");
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        // Sadece Deagle vurusları icin devreye gir
        if (@event.Weapon != "deagle")
            return HookResult.Continue;

        // Kafadan vurduysa dokunma, normal hasar/olum gecerli olsun
        if (@event.Hitgroup == HITGROUP_HEAD)
            return HookResult.Continue;

        var victim = @event.Userid;
        if (victim == null || !victim.IsValid)
            return HookResult.Continue;

        var pawn = victim.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        // Vurustan hemen sonraki can + alinan hasar = vurulmadan onceki can.
        // Body shot'ta bu hasari geri vererek etkisiz hale getiriyoruz.
        int restoredHealth = @event.Health + @event.DmgHealth;
        if (restoredHealth > 100)
            restoredHealth = 100;

        if (pawn.Health > 0) // oyuncu hala hayattaysa geri ver
        {
            pawn.Health = restoredHealth;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        }

        return HookResult.Continue;
    }
}
