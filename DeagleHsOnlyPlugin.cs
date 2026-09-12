using CounterStrikeSharp.API.Core;

namespace DeagleHsOnly;

public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "2.1.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription =>
        "Deagle sadece headshot ile hasar verir; body/kol/bacak vurusları tamamen engellenir. Diger silahlar (AWP dahil) etkilenmez.";

    // Konsola detay yazar. Sorun kalmadigindan emin olunca false yapip yeniden derle.
    private const bool DEBUG = true;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnEntityTakeDamagePre>(OnTakeDamagePre);
        System.Console.WriteLine("[DeagleHsOnly] Plugin yuklendi (pre-damage hook aktif) v2.1.0.");
    }

    private HookResult OnTakeDamagePre(CBaseEntity entity, CTakeDamageInfo info)
    {
        try
        {
            // Sadece oyunculara gelen hasarla ilgilen
            if (entity.DesignerName != "player")
                return HookResult.Continue;

            string weaponName = GetAttackerActiveWeapon(info);

            if (DEBUG)
            {
                System.Console.WriteLine(
                    $"[DeagleHsOnly] Hasar -> AktifSilah={weaponName} Hitgroup={info.GetHitGroup()} Damage={info.Damage}");
            }

            // Sadece Deagle icin devreye gir
            if (string.IsNullOrEmpty(weaponName) || !weaponName.Contains("deagle"))
                return HookResult.Continue;

            // Kafadan vurduysa dokunma, normal hasar/olum gecerli olsun
            if (info.GetHitGroup() == HitGroup_t.HITGROUP_HEAD)
                return HookResult.Continue;

            if (DEBUG)
            {
                System.Console.WriteLine("[DeagleHsOnly] Body/leg vurus engellendi (hasar yok).");
            }

            // Body/kol/bacak/boyun -> hasari tamamen engelle
            return HookResult.Handled;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"[DeagleHsOnly] HATA: {ex.Message}");
            return HookResult.Continue;
        }
    }

    private string GetAttackerActiveWeapon(CTakeDamageInfo info)
    {
        try
        {
            var attackerEntity = info.Attacker.Value;
            if (attackerEntity == null || !attackerEntity.IsValid)
                return "";

            var attackerPawn = attackerEntity.As<CCSPlayerPawn>();
            if (attackerPawn == null)
                return "";

            var controller = attackerPawn.Controller.Value as CCSPlayerController;
            var activeWeapon = controller?.PlayerPawn?.Value?.WeaponServices?.ActiveWeapon?.Value;

            return activeWeapon?.DesignerName ?? "";
        }
        catch
        {
            return "";
        }
    }
}
