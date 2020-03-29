using System;

namespace Intersect.Client.Framework.Gwen.Control
{

    public class EnumRadioButtonGroup<T> : RadioButtonGroup where T : struct, IConvertible
    {

        public EnumRadioButtonGroup(Base parent) : base(parent)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an enumerated type!");
            }

            this.Text = typeof(T).Name;
            for (var i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
            {
                var name = Enum.GetNames(typeof(T))[i];
                var lrb = this.AddOption(name);
                lrb.UserData = Enum.GetValues(typeof(T)).GetValue(i);
            }
        }

        public T SelectedValue
        {
            get => (T) this.Selected.UserData;
            set
            {
                foreach (var child in Children)
                {
                    if (child is LabeledRadioButton && child.UserData.Equals(value))
                    {
                        (child as LabeledRadioButton).RadioButton.Press();
                    }
                }
            }
        }

    }

}
