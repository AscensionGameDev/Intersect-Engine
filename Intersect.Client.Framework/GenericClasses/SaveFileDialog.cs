namespace Intersect.Client.Framework.GenericClasses
{

    public class SaveFileDialog
    {
        public string Title { get; set; }

        public string InitialDirectory { get; set; }

        public string DefaultExt { get; set; }

        public string Filter { get; set; }

        public bool CheckPathExists { get; set; }

        public bool Multiselect { get; set; }

        public string FileName { get; set; }

        public bool OverwritePrompt { get; set; }

        public DialogResult ShowDialog()
        {
            return DialogResult.Ok;
        }

    }

}
