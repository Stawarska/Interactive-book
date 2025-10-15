namespace Book
{
    public interface IPageContent
    {
        public string FieldName { get; }

        #if UNITY_EDITOR
        void DrawEditor();
        #endif
    }
}