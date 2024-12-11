namespace OneDay.Core.Editor.Build
{
    public class UploadSummary
    {
        public bool IsSuccessful { get; }
        public string Output { get; }

        public UploadSummary(bool isSuccessful, string output)
        {
            IsSuccessful = isSuccessful;
            Output = output;
        }
    }
}