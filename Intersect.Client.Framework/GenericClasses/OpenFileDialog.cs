namespace Intersect.Client.Framework.GenericClasses
{

    public enum DialogResult
    {

        Ok = 0,

    }

    public partial class OpenFileDialog
    {
        public string Title { get; set; }

        public string InitialDirectory { get; set; }

        public string DefaultExt { get; set; }

        public string Filter { get; set; }

        public bool CheckPathExists { get; set; }

        public bool Multiselect { get; set; }

        public string FileName { get; set; }

        public DialogResult ShowDialog()
        {
            return DialogResult.Ok;
        }

    }

}
