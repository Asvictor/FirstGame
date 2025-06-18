using System;

namespace FirstGame;

public class Skill
{
    public string Name { get; set; }
    public int UnlockLevel { get; set; }
    public int BasePower { get; set; }
    public float SkillMultiplier { get; set; }
    public float IntMultiplier { get; set; }
    public float Cooldown { get; set; } // in seconds
    public float CooldownTimer { get; set; } // runtime only

    public Skill(string name, int unlockLevel, int basePower, float skillMultiplier, float intMultiplier, float cooldown = 2f)
    {
        Name = name;
        UnlockLevel = unlockLevel;
        BasePower = basePower;
        SkillMultiplier = skillMultiplier;
        IntMultiplier = intMultiplier;
        Cooldown = cooldown;
        CooldownTimer = 0f;
    }

    public int CalculateDamage(int playerAtk, int playerInt)
    {
        // (BasePower * SkillMultiplier) * (1 + IntMultiplier/100)
        return (int)((BasePower * SkillMultiplier) * (1 + (IntMultiplier / 100f)) + playerAtk + playerInt * IntMultiplier);
    }
}
