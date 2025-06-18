namespace FirstGame;

public enum EquipmentType
{
    Weapon,
    Head,
    Body,
    Arm,
    Feet,
    AccessoryLeft,
    AccessoryRight
}

public class Equipment
{
    public string Name { get; set; }
    public EquipmentType Type { get; set; }
    public int STR { get; set; }
    public int VIT { get; set; }
    public int AGI { get; set; }
    public int INT { get; set; }
    public int LUK { get; set; }

    public Equipment(string name, EquipmentType type, int str = 0, int vit = 0, int agi = 0, int @int = 0, int luk = 0)
    {
        Name = name;
        Type = type;
        STR = str;
        VIT = vit;
        AGI = agi;
        INT = @int;
        LUK = luk;
    }
}
