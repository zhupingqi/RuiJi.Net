namespace RuiJi.Net.Core.Utils.Suffix
{
    internal class RuleDefinition
    {
        public readonly string[] Labels;
        public readonly int? Length;

        public RuleDefinition(string[] labels, int length)
        {
            Labels = labels;
            Length = length;
        }
    }
}