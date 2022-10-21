using Newtonsoft.Json;

//A system is defined as something that can change the current game state.
public abstract class CardGameSystem
{
    [JsonProperty]
    protected CardGame cardGame;
}
