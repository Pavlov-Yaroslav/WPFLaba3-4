using System.Collections.Generic;
using WpfApp1.Cells;
using WpfApp1.Players;

public class Player
{
    public int Position { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }

    private readonly List<IPlayerEffect> effects = new List<IPlayerEffect>();

    public Player(string name, string color)
    {
        Name = name;
        Color = color;
        Position = 0;
    }

    public void MoveBy(int delta)
    {
        Position += delta;
    }

    public void ApplyCellResult(CellEffectResult result)
    {
        Position += result.PositionDelta;

        foreach (var effect in result.Effects)
            AddEffect(effect);
    }

    public void AddEffect(IPlayerEffect effect)
    {
        effects.Add(effect);
    }

    public bool CanMove()
    {
        bool canMove = true;

        foreach (var effect in effects)
        {
            if (!effect.CanPlayerMove(this))
                canMove = false;

            effect.OnTurnPassed();
        }

        effects.RemoveAll(e => e.IsExpired);
        return canMove;
    }
    public override string ToString()
    {
        return $"{Name}, {Position}, {Color}";
    }
}
