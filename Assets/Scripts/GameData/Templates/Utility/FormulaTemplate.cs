using System;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class Formulas : TemplateClass<FormulaTemplate>
    {
    }

    [Serializable]
    public class FormulaTemplate
    {
        public string Kind;
        public string Formula;
    }
}