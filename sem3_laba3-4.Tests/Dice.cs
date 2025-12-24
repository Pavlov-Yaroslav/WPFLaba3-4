using System;
using Xunit;

namespace WpfApp1.Tests
{
    public class Dice
    {
        public int Edge { get; private set; }
        private readonly Random _rand = new Random();

        public void Roll()
        {
            Edge = _rand.Next(1, 7);
        }
    }
    public class DiceTests
    {
        [Fact]
        public void Roll_EdgeIsBetween1And6()
        {
            var dice = new Dice();

            for (int i = 0; i < 100; i++)
            {
                dice.Roll();
                Assert.InRange(dice.Edge, 1, 7);
            }
        }

        [Fact]
        public void Roll_EdgeChangesOverMultipleRolls()
        {
            var dice = new Dice();
            var results = new HashSet<int>();

            for (int i = 0; i < 50; i++)
            {
                dice.Roll();
                results.Add(dice.Edge);
            }

            Assert.True(results.Count > 1, "Dice.Roll() не генерирует разные значения");
        }
    }
}
