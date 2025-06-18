namespace FirstGame;

public enum Gender
{
    Male,
    Female
}

public class Player
{
    private const int MaxStatValue = 999;

    public string Name { get; private set; }
    public Gender Gender { get; private set; }
    public int Level { get; private set; } = 1;
    public int MaxLevel { get; } = 999;
    public int Experience { get; private set; } = 0;
    public int ExperienceToNextLevel => 100 * Level * (1 + (Level % 10)); // Linear growth
    public int StatPoints { get; private set; } = 10;

    // Stats
    public int STR { get; private set; } = 1;
    public int VIT { get; private set; } = 1;
    public int AGI { get; private set; } = 1;
    public int INT { get; private set; } = 1;
    public int LUK { get; private set; } = 1;

    public Equipment Weapon { get; private set; }
    public Equipment Head { get; private set; }
    public Equipment Body { get; private set; }
    public Equipment Arm { get; private set; }
    public Equipment Feet { get; private set; }
    public Equipment AccessoryLeft { get; private set; }
    public Equipment AccessoryRight { get; private set; }

    // Total stats (base + all equipment)
    public int TotalSTR => STR + (Weapon?.STR ?? 0) + (Head?.STR ?? 0) + (Body?.STR ?? 0) + (Arm?.STR ?? 0) + (Feet?.STR ?? 0) + (AccessoryLeft?.STR ?? 0) + (AccessoryRight?.STR ?? 0);
    public int TotalVIT => VIT + (Weapon?.VIT ?? 0) + (Head?.VIT ?? 0) + (Body?.VIT ?? 0) + (Arm?.VIT ?? 0) + (Feet?.VIT ?? 0) + (AccessoryLeft?.VIT ?? 0) + (AccessoryRight?.VIT ?? 0);
    public int TotalAGI => AGI + (Weapon?.AGI ?? 0) + (Head?.AGI ?? 0) + (Body?.AGI ?? 0) + (Arm?.AGI ?? 0) + (Feet?.AGI ?? 0) + (AccessoryLeft?.AGI ?? 0) + (AccessoryRight?.AGI ?? 0);
    public int TotalINT => INT + (Weapon?.INT ?? 0) + (Head?.INT ?? 0) + (Body?.INT ?? 0) + (Arm?.INT ?? 0) + (Feet?.INT ?? 0) + (AccessoryLeft?.INT ?? 0) + (AccessoryRight?.INT ?? 0);
    public int TotalLUK => LUK + (Weapon?.LUK ?? 0) + (Head?.LUK ?? 0) + (Body?.LUK ?? 0) + (Arm?.LUK ?? 0) + (Feet?.LUK ?? 0) + (AccessoryLeft?.LUK ?? 0) + (AccessoryRight?.LUK ?? 0);

    public Player(string name, Gender gender)
    {
        Name = name;
        Gender = gender;
    }

    public void AddExperience(int amount)
    {
        Experience += amount;
        while (Level < MaxLevel && Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        StatPoints += 5; // Example: 5 points per level
    }

    public bool AllocateStat(string stat)
    {
        if (StatPoints <= 0) return false;
        var statKey = stat.ToUpper();
        var success = statKey switch
        {
            "STR" when STR < MaxStatValue => STR++ >= 0,
            "VIT" when VIT < MaxStatValue => VIT++ >= 0,
            "AGI" when AGI < MaxStatValue => AGI++ >= 0,
            "INT" when INT < MaxStatValue => INT++ >= 0,
            "LUK" when LUK < MaxStatValue => LUK++ >= 0,
            _ => false
        };
        if (!success) return false;
        StatPoints--;
        return true;
    }

    public void Equip(Equipment equipment)
    {
        if (equipment == null) return;
        switch (equipment.Type)
        {
            case EquipmentType.Weapon:
                Weapon = equipment;
                break;
            case EquipmentType.Head:
                Head = equipment;
                break;
            case EquipmentType.Body:
                Body = equipment;
                break;
            case EquipmentType.Arm:
                Arm = equipment;
                break;
            case EquipmentType.Feet:
                Feet = equipment;
                break;
            case EquipmentType.AccessoryLeft:
                AccessoryLeft = equipment;
                break;
            case EquipmentType.AccessoryRight:
                AccessoryRight = equipment;
                break;
        }
    }

    public void Unequip(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                Weapon = null;
                break;
            case EquipmentType.Head:
                Head = null;
                break;
            case EquipmentType.Body:
                Body = null;
                break;
            case EquipmentType.Arm:
                Arm = null;
                break;
            case EquipmentType.Feet:
                Feet = null;
                break;
            case EquipmentType.AccessoryLeft:
                AccessoryLeft = null;
                break;
            case EquipmentType.AccessoryRight:
                AccessoryRight = null;
                break;
        }
    }
}
