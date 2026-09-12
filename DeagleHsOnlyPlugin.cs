using CounterStrikeSharp.API.Core;

namespace DeagleHsOnly;

public class DeagleHsOnlyPlugin : BasePlugin
{
    public override string ModuleName => "Deagle HS Only";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "Custom";
    public override string ModuleDescription =>
        "Deagle sadece headshot ile hasar verir; body/kol/bacak vurusları tamamen engellenir (hasar yok, yavaslama/tagging efekti yok). Diger silahlar (AWP dahil) etkilenmez.";

    // Konsola detay yazar. Sorun kalmadigindan emin olunca false yapip yeniden derle.
    private const bool DEBUG = true;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnEntityTakeDamagePre>(OnTakeDamagePre);
        System.Console.WriteLine("[DeagleHsOnly] Plugin yuklendi (pre-damage hook aktif).");
    }

    private HookResult OnTakeDamagePre(CBaseEntity entity, CTakeDamageInfo info)
    {
        try
        {
            // Sadece oyunculara gelen hasarla ilgilen
            if (entity.DesignerName != "player")
                return HookResult.Continue;

            // Hasari veren silahi bul (Inflictor = hasara sebep olan entity, genelde silah)
            var inflictor = info.Inflictor.Value;
            string weaponName = inflictor?.DesignerName ?? "";

            if (DEBUG)
            {
                System.Console.WriteLine(
                    $"[DeagleHsOnly] Hasar -> Weapon={weaponName} Hitgroup={info.GetHitGroup()} Damage={info.Damage}");
            }

            // Sadece Deagle icin devreye gir
            if (!weaponName.Contains("deagle"))
                return HookResult.Continue;

            // Kafadan vurduysa dokunma, normal hasar/olum gecerli olsun
            if (info.GetHitGroup() == HitGroup_t.HITGROUP_HEAD)
                return HookResult.Continue;

            // Body/kol/bacak/boyun -> hasari tamamen engelle (Handled = tum hasar surecini iptal eder)
            if (DEBUG)
            {
                System.Console.WriteLine("[DeagleHsOnly] Body/leg vurus engellendi (hasar yok).");
            }

            return HookResult.Handled;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"[DeagleHsOnly] HATA: {ex.Message}");
            return HookResult.Continue;
        }
    }
}
