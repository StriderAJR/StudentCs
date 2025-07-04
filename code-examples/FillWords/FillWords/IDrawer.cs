namespace FillWords
{
    public interface IDrawer
    {
        void Draw(FillWords game, int hoverX, int hoverY);
        void DrawMessage(string message);
    }
}