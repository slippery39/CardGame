public enum SimResult
{
    Win,
    Loss,
    Draw
}
public class SimResultData
{
    public string Deck1 { get; set; }
    public string Deck2 { get; set; }
    //Result from the perspective of deck 1
    public SimResult Result { get; set; }
}