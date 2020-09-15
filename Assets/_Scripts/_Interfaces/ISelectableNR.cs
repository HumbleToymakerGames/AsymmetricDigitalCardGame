public interface ISelectableNR
{
    bool CanHighlight(bool highlight = true);
    bool CanSelect();

    void Highlighted();
    void Selected();
}
