namespace RaceVoice
{
    public class ComboBoxItem<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}