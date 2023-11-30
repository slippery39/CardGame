public interface IManaFilter
{
    bool Check(CardInstance cardToCheck);
    string RulesTextString(bool plural = false);
}




